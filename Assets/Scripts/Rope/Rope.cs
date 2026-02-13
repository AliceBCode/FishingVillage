using System;
using DNExtensions.TubeRenderer;
using DNExtensions.Utilities;
using UnityEngine;
using DNExtensions.Utilities.AutoGet;
using DNExtensions.Utilities.Button;

namespace FishingVillage.RopeSystem
{
    
    
    [Serializable]
    public class CreationSettings
    {
        [Min(3)] public int pointsAmount = 24;
        public float pointSpacing = 0.5f;
        public Vector3 direction = Vector3.right;
        public bool autoSetEndsAsAnchors = true;
        public GameObject pointPrefab;
        [DisableIf(nameof(HasPrefab))] public float pointRadius = 0.2f;

        public bool HasPrefab => pointPrefab;

        public void Validate()
        {
            pointSpacing = Mathf.Max(0.01f, pointSpacing);
            pointRadius = Mathf.Max(0.01f, pointRadius);
        }
    }

    [Serializable]
    public class VisualizationSettings
    {
        public bool autoUpdate = true;
        public bool drawGizmos;
    }
    
    
    [ExecuteAlways]
    [RequireComponent(typeof(TubeRenderer))]
    public class Rope : MonoBehaviour
    {
        [Header("Curve Settings")]
        [SerializeField] private float targetWeight = 2f;
        [SerializeField] private float restingSag = 1f;
        [SerializeField] private Transform target;
        
        [Header("Spring Settings")]
        [SerializeField] private bool useSpringOnRelease = true;
        [SerializeField] private float springStrength = 100f;
        [SerializeField] private float springDamping = 8f;

        [Separator]
        [SerializeField] private CreationSettings creation = new();
        [SerializeField] private VisualizationSettings visualization = new();

        [AutoGetSelf, HideInInspector] public TubeRenderer tubeRenderer;
        private RopePoint[] _points;
        private Vector3[] _velocities;
        private Vector3[] _lastPositions;
        
        public RopePoint[] Points => _points;
        


        
        private void OnValidate()
        {
            creation.Validate();
            if (!tubeRenderer) tubeRenderer = this.GetOrAddComponent<TubeRenderer>();
        }

        private void Awake()
        {
            _points = GetComponentsInChildren<RopePoint>();
            InitializeArrays();
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                UpdateCurve();
            }
            
            if (visualization.autoUpdate)
            {
                UpdateVisualization();
            }
        }
        
        

        #region Public API

        public void SetTarget(Transform newTarget) => target = newTarget;

        public float GetClosestT(Vector3 position)
        {
            if (_points == null || _points.Length < 2) return 0f;

            Vector3 start = _points[0].transform.position;
            Vector3 end = _points[^1].transform.position;
            Vector3 vector = end - start;
            Vector3 originToPos = position - start;

            float t = Vector3.Dot(originToPos, vector) / vector.sqrMagnitude;
            return Mathf.Clamp01(t);
        }

        public Vector3 GetPointAt(float t)
        {
            if (_points == null || _points.Length < 2) return Vector3.zero;

            Vector3 start = _points[0].transform.position;
            Vector3 end = _points[^1].transform.position;

            if (target)
            {
                float weightT = GetClosestT(target.position);
                return CalculatePointOnTargetCurve(t, start, end, weightT);
            }

            return CalculateRestingPosition(t, start, end);
        }
        
        #endregion

        #region Curve Simulation

        private void InitializeArrays()
        {
            if (_points.Length == 0) return;

            _velocities = new Vector3[_points.Length];
            _lastPositions = new Vector3[_points.Length];

            for (int i = 0; i < _points.Length; i++)
                _lastPositions[i] = _points[i].transform.position;
        }

        private void UpdateCurve()
        {
            if (_points == null || _points.Length < 2) return;

            Vector3 start = _points[0].transform.position;
            Vector3 end = _points[^1].transform.position;
            float weightT = target ? GetClosestT(target.position) : 0f;

            for (int i = 1; i < _points.Length - 1; i++)
            {
                float pointT = i / (float)(_points.Length - 1);
                Vector3 targetPos = target
                    ? CalculatePointOnTargetCurve(pointT, start, end, weightT)
                    : CalculateRestingPosition(pointT, start, end);

                if (target)
                {
                    _points[i].transform.position = targetPos;
                    if (Time.deltaTime > 0)
                        _velocities[i] = (targetPos - _lastPositions[i]) / Time.deltaTime;
                }
                else if (useSpringOnRelease)
                {
                    UpdateSpring(i, _points[i].transform.position, targetPos);
                }
                else
                {
                    _points[i].transform.position = targetPos;
                    _velocities[i] = Vector3.zero;
                }

                _lastPositions[i] = _points[i].transform.position;
            }
        }

        private void UpdateSpring(int index, Vector3 currentPos, Vector3 targetPos)
        {
            Vector3 displacement = currentPos - targetPos;
            Vector3 force = -springStrength * displacement - springDamping * _velocities[index];
            _velocities[index] += force * Time.deltaTime;
            _points[index].transform.position += _velocities[index] * Time.deltaTime;
        }

        private Vector3 CalculateRestingPosition(float t, Vector3 start, Vector3 end)
        {
            Vector3 pos = Vector3.Lerp(start, end, t);
            pos.y -= Mathf.Sin(t * Mathf.PI) * restingSag;
            return pos;
        }

        private Vector3 CalculatePointOnTargetCurve(float pointT, Vector3 start, Vector3 end, float weightT)
        {
            Vector3 sagPosition = Vector3.Lerp(start, end, weightT);
            sagPosition.y -= targetWeight;

            Vector3 pos;
            float smoothY;

            if (pointT < weightT)
            {
                float localT = weightT <= 0.001f ? 0 : pointT / weightT;
                pos = Vector3.Lerp(start, sagPosition, localT);
                smoothY = Mathf.Lerp(start.y, sagPosition.y, 1f - Mathf.Pow(1f - localT, 2f));
            }
            else
            {
                float denominator = 1f - weightT;
                float localT = denominator <= 0.001f ? 1 : (pointT - weightT) / denominator;
                pos = Vector3.Lerp(sagPosition, end, localT);
                smoothY = Mathf.Lerp(sagPosition.y, end.y, Mathf.Pow(localT, 2f));
            }

            pos.y = smoothY;
            return pos;
        }
        
        #endregion
        
        
        
        #region Point Creation

        [Button(ButtonPlayMode.OnlyWhenNotPlaying)]
        public void CreatePoints()
        {
            ClearPoints();

            Vector3 dir = creation.direction.normalized;

            for (int i = 0; i < creation.pointsAmount; i++)
            {
                GameObject pointObject = creation.pointPrefab 
                    ? Instantiate(creation.pointPrefab, transform)
                    : CreatePrimitivePoint();

                pointObject.name = $"RopePoint_{i}";
                pointObject.transform.localPosition = dir * (i * creation.pointSpacing);

                var ropePoint = pointObject.GetOrAddComponent<RopePoint>();
                ropePoint.isAnchor = creation.autoSetEndsAsAnchors && (i == 0 || i == creation.pointsAmount - 1);
            }

            _points = GetComponentsInChildren<RopePoint>();
            InitializeArrays();
        }

        [Button(ButtonPlayMode.OnlyWhenNotPlaying)]
        public void ClearPoints()
        {
            var existingPoints = GetComponentsInChildren<RopePoint>();
            for (int i = existingPoints.Length - 1; i >= 0; i--)
            {
                if (Application.isPlaying)
                {
                    Destroy(existingPoints[i].gameObject);
                }
                else
                {
                    DestroyImmediate(existingPoints[i].gameObject);
                }
            }
        }

        private GameObject CreatePrimitivePoint()
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.SetParent(transform);
            obj.transform.localScale = Vector3.one * creation.pointRadius * 2f;
            return obj;
        }
        
        #endregion

        #region Visualization

        private void UpdateVisualization()
        {
            if (!tubeRenderer || _points == null || _points.Length == 0) return;

            Vector3[] positions = new Vector3[_points.Length];
            for (int i = 0; i < _points.Length; i++)
            {
                if (!_points[i]) continue;
                positions[i] = transform.InverseTransformPoint(_points[i].transform.position);
            }

            tubeRenderer.SetPositions(positions);
        }
        
        private void OnDrawGizmos()
        {
            if (_points == null || !visualization.drawGizmos) return;

            foreach (var point in _points)
            {
                if (!point) continue;
                Gizmos.color = point.isAnchor ? Color.red : Color.green;
                Gizmos.DrawSphere(point.transform.position, 0.1f);
            }
            
            if (creation.pointsAmount > _points.Length)
            {
                Gizmos.color = Color.yellow;
                Vector3 dir = creation.direction.normalized;
                for (int i = 0; i < creation.pointsAmount; i++)
                {
                    if (i < _points.Length) continue;
                    Vector3 pos = transform.TransformPoint(dir * (i * creation.pointSpacing));
                    Gizmos.DrawSphere(pos, 0.1f);
                }
            }
        }

        
        #endregion
    }
}