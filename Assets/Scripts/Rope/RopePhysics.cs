using UnityEngine;

public class RopePhysics : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private float targetRadius = 2f;
    [SerializeField] private float targetStrength = 25f;
    
    [Header("Physics")]
    [SerializeField, Min(0)] private float gravity = 0.5f;
    [SerializeField, Range(0,1)] private float damping = 0.95f;
    [SerializeField, Min(0)] private float stiffness = 0.1f;
    [SerializeField, Min(0)] private float massScale = 0.5f;
    [SerializeField, Min(0)] private int iterations = 3;
    
    private RopePoint[] _points;
    
    private void Awake()
    {
        _points = GetComponentsInChildren<RopePoint>();
        CalculateMasses();
        
        foreach (var point in _points)
        {
            point.restPosition = point.transform.position;
            point.currentPosition = point.restPosition;
        }
    }
    
    private void CalculateMasses()
    {
        for (int i = 0; i < _points.Length; i++)
        {
            if (_points[i].isAnchor)
            {
                _points[i].mass = 0f;
            }
            else
            {
                _points[i].mass = GetDistanceToNearestAnchor(i) * massScale;
            }
        }
    }
    
    private void Update()
    {
        ApplyForces();
        Integrate();
        
        for (int i = 0; i < iterations; i++)
        {
            ApplyConstraints();
        }
        
        UpdateTransforms();
    }
    
    private void ApplyForces()
    {
        for (int i = 0; i < _points.Length; i++)
        {
            if (_points[i].isAnchor) continue;
            
            var point = _points[i];
            
            Vector3 gravityForce = Vector3.down * (gravity * point.mass);
            
            Vector3 targetForce = Vector3.zero;
            if (target)
            {
                float distance = Vector3.Distance(target.position, point.currentPosition);
                if (distance < targetRadius)
                {
                    float influence = 1f - (distance / targetRadius);
                    targetForce = Vector3.down * (targetStrength * influence);
                }
            }
            
            Vector3 springForce = (point.restPosition - point.currentPosition) * stiffness;
            
            Vector3 acceleration = (gravityForce + targetForce + springForce) / point.mass;
            
            point.velocity += acceleration * Time.deltaTime;
            point.velocity *= damping;
        }
    }
    
    private void Integrate()
    {
        foreach (var point in _points)
        {
            if (point.isAnchor) continue;
            point.currentPosition += point.velocity * Time.deltaTime;
        }
    }
    
    private void ApplyConstraints()
    {
        for (int i = 0; i < _points.Length - 1; i++)
        {
            var p1 = _points[i];
            var p2 = _points[i + 1];
            
            Vector3 delta = p2.currentPosition - p1.currentPosition;
            float currentLength = delta.magnitude;
            float restLength = Vector3.Distance(p1.restPosition, p2.restPosition);
            
            if (currentLength == 0f) continue;
            
            float correction = (currentLength - restLength) / currentLength;
            Vector3 correctionVector = delta * (correction * 0.5f);
            
            if (!p1.isAnchor) p1.currentPosition += correctionVector;
            if (!p2.isAnchor) p2.currentPosition -= correctionVector;
        }
    }
    
    private void UpdateTransforms()
    {
        foreach (var point in _points)
        {
            point.transform.position = point.currentPosition;
        }
    }
    
    private int GetDistanceToNearestAnchor(int index)
    {
        int minDistance = int.MaxValue;
        
        for (int i = 0; i < _points.Length; i++)
        {
            if (_points[i].isAnchor)
            {
                minDistance = Mathf.Min(minDistance, Mathf.Abs(i - index));
            }
        }
        
        return minDistance;
    }
}