using DNExtensions.TubeRenderer;
using UnityEngine;
using DNExtensions.Utilities.Button;

namespace FishingVillage.Rope
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(TubeRenderer))]
    public class RopeTubeVisualizer : MonoBehaviour
{
    [SerializeField] private bool autoUpdate = true;
    
    private TubeRenderer _tubeRenderer;

    private void Awake()
    {
        if (!_tubeRenderer)
        {
            _tubeRenderer = GetComponent<TubeRenderer>();
        }
    }

    private void Update()
    {
        if (autoUpdate && _tubeRenderer)
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

        _tubeRenderer.SetPositions(positions);
    }
    }
}