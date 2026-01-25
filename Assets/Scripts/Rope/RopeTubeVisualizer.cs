using DNExtensions.Button;
using UnityEngine;
using DNExtensions.TubeRenderer;

[ExecuteInEditMode]
[RequireComponent(typeof(TubeRenderer))]
public class RopeTubeVisualizer : MonoBehaviour
{
    [SerializeField] private bool autoUpdate = true;
    
    private TubeRenderer tubeRenderer;

    private void Awake()
    {
        if (!tubeRenderer)
        {
            tubeRenderer = GetComponent<TubeRenderer>();
        }
    }

    private void Update()
    {
        if (autoUpdate && tubeRenderer)
        {
            UpdateVisualization();
        }
    }

    [Button]
    public void UpdateVisualization()
    {
        var points = GetComponentsInChildren<RopePoint>();
        if (points.Length == 0) return;

        Vector3[] positions = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            positions[i] = transform.InverseTransformPoint(points[i].transform.position);
        }

        tubeRenderer.SetPositions(positions);
    }
}