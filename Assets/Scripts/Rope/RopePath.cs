using UnityEngine;

namespace FishingVillage.Rope
{
    public class RopePath : MonoBehaviour
{
    [SerializeField] private float targetWeight = 2f;
    [SerializeField] private Transform target;
    [SerializeField, Range(0f, 1f)] private float currentT = 0.5f;
    
    private RopePoint[] _points;
    
    private void Awake()
    {
        _points = GetComponentsInChildren<RopePoint>();
    }
    
    private void Update()
    {
        Vector3 start = _points[0].transform.position;
        Vector3 end = _points[^1].transform.position;
        
        if (target)
        {
            currentT = GetTargetT(start, end);
        }
        
        Vector3 playerPos = Vector3.Lerp(start, end, currentT);
        playerPos.y -= targetWeight;
        
        for (int i = 1; i < _points.Length - 1; i++)
        {
            float t = i / (float)(_points.Length - 1);
            
            Vector3 pos;
            float smoothY;
            
            if (t < currentT)
            {
                float localT = t / currentT;
                pos = Vector3.Lerp(start, playerPos, localT);
                
                float curve = 1f - Mathf.Pow(1f - localT, 2f);
                smoothY = Mathf.Lerp(start.y, playerPos.y, curve);
            }
            else
            {
                float localT = (t - currentT) / (1f - currentT);
                pos = Vector3.Lerp(playerPos, end, localT);
                
                float curve = Mathf.Pow(localT, 2f);
                smoothY = Mathf.Lerp(playerPos.y, end.y, curve);
            }
            
            pos.y = smoothY;
            _points[i].transform.position = pos;
        }
    }
    
    private float GetTargetT(Vector3 start, Vector3 end)
    {
        Vector3 startToEnd = end - start;
        Vector3 startToTarget = target.position - start;
        
        float t = Vector3.Dot(startToTarget, startToEnd) / startToEnd.sqrMagnitude;
        return Mathf.Clamp01(t);
    }
    }
}