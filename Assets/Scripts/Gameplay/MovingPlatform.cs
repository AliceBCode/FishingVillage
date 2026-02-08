using DNExtensions.Utilities.Button;
using UnityEngine;

namespace FishingVillage.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    [SelectionBase]
    public class MovingPlatform : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool active;
    [SerializeField] private Vector3 positionOffset = Vector3.forward;
    [SerializeField] private float waitTimeAtPositions = 1f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private AnimationCurve moveSpeedCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private Vector3 _pathStart;
    private float _pathLength;
    private float _waitTimer;
    private bool _movingToPositionTwo = true;
    private Rigidbody _rb;
    private Vector3 _velocity;

    public Vector3 Velocity => _velocity;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        _startPosition = transform.position;
        _pathStart = _startPosition;
        _targetPosition = _startPosition + positionOffset;
        _pathLength = Vector3.Distance(_pathStart, _targetPosition);
    }
    
    private void FixedUpdate()
    {
        if (!active)
        {
            _velocity = Vector3.zero;
            return;
        }

        if (_waitTimer > 0f)
        {
            _waitTimer -= Time.fixedDeltaTime;
            _velocity = Vector3.zero;
            return;
        }

        Vector3 direction = (_targetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, _targetPosition);
        
        float distanceTraveled = _pathLength - distance;
        float t = Mathf.Clamp01(distanceTraveled / _pathLength);
        float speedMultiplier = moveSpeedCurve.Evaluate(t);
        
        float actualStep = moveSpeed * speedMultiplier * Time.fixedDeltaTime;
        
        if (distance <= actualStep)
        {
            _rb.MovePosition(_targetPosition);
            _velocity = Vector3.zero;
            _waitTimer = waitTimeAtPositions;
            _movingToPositionTwo = !_movingToPositionTwo;
            
            _pathStart = _targetPosition;
            _targetPosition = _movingToPositionTwo ? _startPosition + positionOffset : _startPosition;
            _pathLength = Vector3.Distance(_pathStart, _targetPosition);
        }
        else
        {
            _velocity = direction * (moveSpeed * speedMultiplier);
            Vector3 newPosition = transform.position + _velocity * Time.fixedDeltaTime;
            _rb.MovePosition(newPosition);
        }
    }

    public void SetActive(bool isActive)
    {
        active = isActive;
    }

    [Button]
    public void ToggleActive() => SetActive(!active);
    
    [Button]
    public void ResetPlatform()
    {
        transform.position = _startPosition;
        _targetPosition = _startPosition + positionOffset;
        _movingToPositionTwo = true;
        _waitTimer = 0f;
        _velocity = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Vector3 start = Application.isPlaying ? _startPosition : transform.position;
        Vector3 end = start + positionOffset;
        
        Renderer rend = GetComponentInChildren<Renderer>();
        MeshFilter mf = GetComponent<MeshFilter>();
        var size = rend ? rend.bounds.size : Vector3.one;
    
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(start, end);


        if (mf)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireMesh(mf.mesh, start, Quaternion.identity, size);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireMesh(mf.mesh, end, Quaternion.identity, size);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(start, size);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(end, size);
        }



    }
    }
}