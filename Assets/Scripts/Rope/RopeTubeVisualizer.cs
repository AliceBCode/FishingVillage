using System;
using System.Collections.Generic;
using DNExtensions.TubeRenderer;
using DNExtensions.Utilities.AutoGet;
using UnityEngine;
using DNExtensions.Utilities.Button;

namespace FishingVillage.RopeSystem
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(TubeRenderer))]
    public class RopeTubeVisualizer : MonoBehaviour
    {
        [SerializeField] private bool autoUpdate = true;
        
        private TubeRenderer _tubeRenderer;
        [SerializeField, AutoGetChildren, HideInInspector] private RopePoint[] ropePoints = Array.Empty<RopePoint>();

        
        private void Awake()
        {
            if (!_tubeRenderer)
            {
                _tubeRenderer = GetComponent<TubeRenderer>();
            }
            
            if (ropePoints.Length == 0)
            {
                ropePoints = GetComponentsInChildren<RopePoint>();
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
        private void UpdateVisualization()
        {
            if (ropePoints.Length == 0) return;

            Vector3[] positions = new Vector3[ropePoints.Length];
            for (int i = 0; i < ropePoints.Length; i++)
            {
                positions[i] = transform.InverseTransformPoint(ropePoints[i].transform.position);
            }

            _tubeRenderer.SetPositions(positions);
        }
        
        
        private void OnDrawGizmos()
        {
            if (ropePoints.Length > 0)
            {
                foreach (RopePoint point in ropePoints)
                {
                    Gizmos.color = !point.isAnchor ? Color.green : Color.red;
                    Gizmos.DrawSphere(point.transform.position, 0.1f);
                }
            }

        }
    }
}