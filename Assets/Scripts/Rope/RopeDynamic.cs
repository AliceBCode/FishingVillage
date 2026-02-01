using DNExtensions;
using DNExtensions.Utilities;
using UnityEngine;

public class RopeDynamic : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;

    [Header("Sag Settings")]
    [SerializeField, Tooltip("Segments near anchors have reduced sag")]
    private int anchorInfluenceRange = 1;
    [SerializeField, Tooltip("Maximum downward sag at target position")]
    private float maxSagAmount = 5f;
    [SerializeField, Tooltip("How far from target the sag affects")]
    private float sagInfluenceRadius = 2f;
    [SerializeField, Tooltip("How far sag propagates from target along rope")]
    private int sagPropagationRange = 3;
    [SerializeField, Range(0f, 1f), Tooltip("How much neighbors affect each other")]
    private float neighborSagFalloff = 0.7f;
    
    [Header("Return Settings")]
    [SerializeField, Tooltip("Use spring physics for return motion")]
    private bool useSpring;
    [SerializeField, Tooltip("How fast rope returns to rest position (non-spring mode)")]
    [DisableIf("useSpring")] private float returnSpeed = 5f;
    [SerializeField, Tooltip("Spring stiffness (higher = faster return)")]
    [EnableIf("useSpring")] private float springStrength = 10f;
    [SerializeField, Tooltip("Spring damping (higher = less bouncy)")]
    [EnableIf("useSpring")] private float springDamping = 2f;
    
    private RopePoint[] _ropePoints;
    private float[] _targetSags;
    private float[] _currentSags;
    private float[] _sagVelocities;
    private float[] _anchorProximityFactors;

    private void Awake()
    {
        _ropePoints = GetComponentsInChildren<RopePoint>();
        _targetSags = new float[_ropePoints.Length];
        _currentSags = new float[_ropePoints.Length];
        _sagVelocities = new float[_ropePoints.Length];
        _anchorProximityFactors = new float[_ropePoints.Length];
        
        CalculateAnchorProximityFactors();
    }

    private void Update()
    {
        if (!target) return;
        
        for (int i = 0; i < _ropePoints.Length; i++)
        {
            if (_ropePoints[i].isAnchor)
            {
                _targetSags[i] = 0f;
                continue;
            }
            
            float distanceToPlayer = Vector3.Distance(target.position, _ropePoints[i].transform.position);
            
            if (distanceToPlayer < sagInfluenceRadius)
            {
                float influence = 1f - (distanceToPlayer / sagInfluenceRadius);
                influence = Mathf.SmoothStep(0f, 1f, influence);
                
                float effectiveSagAmount = maxSagAmount * _anchorProximityFactors[i];
                _targetSags[i] = influence * effectiveSagAmount;
            }
            else
            {
                _targetSags[i] = 0f;
            }
        }
        
        for (int iteration = 0; iteration < sagPropagationRange; iteration++)
        {
            PropagateConstraints();
        }
        
        for (int i = 0; i < _ropePoints.Length; i++)
        {
            if (_ropePoints[i].isAnchor) continue;
            
            if (useSpring)
            {
                UpdateSpring(i);
            }
            else
            {
                _currentSags[i] = Mathf.Lerp(_currentSags[i], _targetSags[i], Time.deltaTime * returnSpeed);
            }
            
            Vector3 newPosition = _ropePoints[i].StartPosition;
            newPosition.y -= _currentSags[i];
            
            _ropePoints[i].UpdatePosition(newPosition);
        }
    }
    
    private void UpdateSpring(int index)
    {
        float displacement = _targetSags[index] - _currentSags[index];
        
        float springForce = displacement * springStrength;
        float dampingForce = -_sagVelocities[index] * springDamping;
        
        float acceleration = springForce + dampingForce;
        
        _sagVelocities[index] += acceleration * Time.deltaTime;
        _currentSags[index] += _sagVelocities[index] * Time.deltaTime;
    }
    
    private void PropagateConstraints()
    {
        for (int i = 0; i < _ropePoints.Length; i++)
        {
            if (_ropePoints[i].isAnchor) continue;
            
            float maxNeighborSag = 0f;
            
            if (i > 0 && !_ropePoints[i - 1].isAnchor)
            {
                maxNeighborSag = Mathf.Max(maxNeighborSag, _targetSags[i - 1] * neighborSagFalloff);
            }
            
            if (i < _ropePoints.Length - 1 && !_ropePoints[i + 1].isAnchor)
            {
                maxNeighborSag = Mathf.Max(maxNeighborSag, _targetSags[i + 1] * neighborSagFalloff);
            }
            
            _targetSags[i] = Mathf.Max(_targetSags[i], maxNeighborSag);
        }
    }
    
    private void CalculateAnchorProximityFactors()
    {
        for (int i = 0; i < _ropePoints.Length; i++)
        {
            if (_ropePoints[i].isAnchor)
            {
                _anchorProximityFactors[i] = 0f;
                continue;
            }
            
            int distanceToNearestAnchor = FindDistanceToNearestAnchor(i);
            
            if (distanceToNearestAnchor >= anchorInfluenceRange)
            {
                _anchorProximityFactors[i] = 1f;
            }
            else
            {
                float t = (float)distanceToNearestAnchor / anchorInfluenceRange;
                _anchorProximityFactors[i] = Mathf.SmoothStep(0f, 1f, t);
            }
        }
    }
    
    private int FindDistanceToNearestAnchor(int pointIndex)
    {
        int minDistance = int.MaxValue;
        
        for (int i = 0; i < _ropePoints.Length; i++)
        {
            if (_ropePoints[i].isAnchor)
            {
                int distance = Mathf.Abs(i - pointIndex);
                minDistance = Mathf.Min(minDistance, distance);
            }
        }
        
        return minDistance;
    }
}