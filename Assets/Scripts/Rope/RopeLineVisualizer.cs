using DNExtensions.Button;
using UnityEngine;

[RequireComponent( typeof(LineRenderer))]
public class RopeLineVisualizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("LineRenderer to draw the rope")]
    private LineRenderer lineRenderer;
    
    [Header("Visual Settings")]
    [SerializeField, Tooltip("Width of the rope line")]
    private float ropeWidth = 0.1f;
    [SerializeField, Tooltip("Material for the rope line")]
    private Material ropeMaterial;
    [SerializeField, Tooltip("Update the line every frame")]
    private bool updateEveryFrame = true;
    
    private RopePoint[] _ropePoints;

    private void OnValidate()
    {
        if (!lineRenderer) lineRenderer = GetComponent<LineRenderer>();
        
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = ropeWidth;
            lineRenderer.endWidth = ropeWidth;
            
            if (ropeMaterial != null)
            {
                lineRenderer.material = ropeMaterial;
            }
        }
    }

    private void Awake()
    {
        if (!lineRenderer) lineRenderer = GetComponent<LineRenderer>();
        
        _ropePoints = GetComponentsInChildren<RopePoint>();
        
        SetupLineRenderer();
    }

    private void SetupLineRenderer()
    {
        if (!lineRenderer) return;
        
        lineRenderer.positionCount = _ropePoints.Length;
        lineRenderer.startWidth = ropeWidth;
        lineRenderer.endWidth = ropeWidth;
        
        if (ropeMaterial != null)
        {
            lineRenderer.material = ropeMaterial;
        }
        
        lineRenderer.useWorldSpace = true;
    }

    private void LateUpdate()
    {
        if (updateEveryFrame)
        {
            UpdateLine();
        }
    }

    public void UpdateLine()
    {
        if (!lineRenderer || _ropePoints == null || _ropePoints.Length == 0)
            return;
        
        lineRenderer.positionCount = _ropePoints.Length;
        
        for (int i = 0; i < _ropePoints.Length; i++)
        {
            lineRenderer.SetPosition(i, _ropePoints[i].transform.position);
        }
    }
    
    [Button(ButtonPlayMode.OnlyWhenNotPlaying)]
    public void RefreshRopePoints()
    {
        _ropePoints = GetComponentsInChildren<RopePoint>();
        SetupLineRenderer();
        UpdateLine();
    }
}
