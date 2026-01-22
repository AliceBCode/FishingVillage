using UnityEngine;

public class RopePoint : MonoBehaviour
{
    public bool isAnchor;

    private bool _isInitialized;
    
    public Vector3 StartPosition { get; private set; }
    

    private void Awake()
    {
        StartPosition = transform.localPosition;
        _isInitialized = true;
    }

    public void UpdatePosition(Vector3 position)
    {
        if (isAnchor || !_isInitialized) return;
        
        transform.localPosition = position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = !isAnchor ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}