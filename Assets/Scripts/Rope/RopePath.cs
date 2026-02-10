using UnityEngine;

namespace FishingVillage.Rope
{
    public class RopePath : MonoBehaviour
    {
        [Header("Curve Settings")]
        [SerializeField] private float targetWeight = 2f;
        [SerializeField] private float restingSag = 1f;
        [SerializeField] private Transform target;
        
        [Header("Spring Settings")]
        [SerializeField] private bool useSpringOnRelease = true;
        [SerializeField] private float springStrength = 100f;
        [SerializeField] private float springDamping = 8f;

        private RopePoint[] _points;
        private Vector3[] _velocities;
        private Vector3[] _lastPositions;
        
        private void Awake()
        {
            _points = GetComponentsInChildren<RopePoint>();
            
            if (_points.Length > 0)
            {
                _velocities = new Vector3[_points.Length];
                _lastPositions = new Vector3[_points.Length];
                
                for (int i = 0; i < _points.Length; i++)
                {
                    _lastPositions[i] = _points[i].transform.position;
                }
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }


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
            else
            {
                return CalculateRestingPosition(t, start, end);
            }
        }

        private void Update()
        {
            if (_points == null || _points.Length < 2) return;

            Vector3 startAnchor = _points[0].transform.position;
            Vector3 endAnchor = _points[^1].transform.position;
            
            float currentWeightT = target ? GetClosestT(target.position) : 0f;

            for (int i = 1; i < _points.Length - 1; i++)
            {
                Vector3 currentPos = _points[i].transform.position;
                Vector3 targetPos;
                
                float pointT = i / (float)(_points.Length - 1);

                if (target)
                {
                    targetPos = CalculatePointOnTargetCurve(pointT, startAnchor, endAnchor, currentWeightT);
                }
                else
                {
                    targetPos = CalculateRestingPosition(pointT, startAnchor, endAnchor);
                }
                
                if (target)
                {
                    _points[i].transform.position = targetPos;
                    
                    if (Time.deltaTime > 0)
                    {
                        _velocities[i] = (targetPos - _lastPositions[i]) / Time.deltaTime;
                    }
                }
                else
                {
                    if (useSpringOnRelease)
                    {
                        UpdateSpring(i, currentPos, targetPos);
                    }
                    else
                    {
                        _points[i].transform.position = targetPos;
                        _velocities[i] = Vector3.zero;
                    }
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
            Vector3 straightPos = Vector3.Lerp(start, end, t);
            
            float sagCurve = Mathf.Sin(t * Mathf.PI);
            straightPos.y -= sagCurve * restingSag;

            return straightPos;
        }

        private Vector3 CalculatePointOnTargetCurve(float pointT, Vector3 start, Vector3 end, float weightT)
        {
            Vector3 sagPosition = Vector3.Lerp(start, end, weightT);
            sagPosition.y -= targetWeight;

            Vector3 pos;
            float smoothY;

            if (pointT < weightT)
            {
                float localT = pointT / weightT;
                if (weightT <= 0.001f) localT = 0; 

                pos = Vector3.Lerp(start, sagPosition, localT);
                float curve = 1f - Mathf.Pow(1f - localT, 2f);
                smoothY = Mathf.Lerp(start.y, sagPosition.y, curve);
            }
            else
            {
                float denominator = (1f - weightT);
                float localT = (denominator <= 0.001f) ? 1 : (pointT - weightT) / denominator;

                pos = Vector3.Lerp(sagPosition, end, localT);
                float curve = Mathf.Pow(localT, 2f);
                smoothY = Mathf.Lerp(sagPosition.y, end.y, curve);
            }

            pos.y = smoothY;
            return pos;
        }
    }
}