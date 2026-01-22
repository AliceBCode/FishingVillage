using DNExtensions;
using DNExtensions.Button;
using UnityEngine;

[RequireComponent(typeof(Rope))]
public class RopeCreator : MonoBehaviour
{
    [Header("Generation Settings")]
    [SerializeField, Tooltip("The Rope component to configure")]
    private Rope rope;
    [SerializeField, Tooltip("Number of rope segments to create")]
    [Min(3)] private int pointsAmount = 10;
    [SerializeField, Tooltip("Distance between each rope point")]
    private float pointSpacing = 1f;
    [SerializeField, Tooltip("Direction points are created along")]
    private Vector3 direction = Vector3.right;
    [SerializeField, Tooltip("Automatically mark first and last points as anchors")]
    private bool autoSetEndsAsAnchors = true;
    
    [Header("Point Settings")]
    [SerializeField, Tooltip("Optional prefab to use for rope points. If null, creates primitives")]
    private GameObject pointPrefab;
    [SerializeField, Tooltip("Radius of each rope point sphere")]
    [DisableIf("HasPrefab")] private float pointRadius = 0.2f;
    
    private bool HasPrefab => pointPrefab;

    private void OnValidate()
    {
        if (!rope) rope = GetComponent<Rope>();
        pointSpacing = Mathf.Max(0.01f, pointSpacing);
        pointRadius = Mathf.Max(0.01f, pointRadius);
    }

    [Button(ButtonPlayMode.OnlyWhenNotPlaying)]
    public void CreatePoints()
    {
        if (rope == null)
        {
            Debug.LogError("Rope reference is missing!");
            return;
        }

        ClearPoints();
        
        Vector3 normalizedDirection = direction.normalized;
        
        for (int i = 0; i < pointsAmount; i++)
        {
            GameObject pointObject;
            
            if (pointPrefab != null)
            {
                pointObject = Instantiate(pointPrefab, transform);
            }
            else
            {
                pointObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                pointObject.transform.SetParent(transform);
                pointObject.transform.localScale = Vector3.one * pointRadius * 2f;
            }
            
            pointObject.name = $"RopePoint_{i}";
            
            Vector3 localPosition = normalizedDirection * (i * pointSpacing);
            pointObject.transform.localPosition = localPosition;
            
            RopePoint ropePoint = pointObject.GetComponent<RopePoint>();
            if (!ropePoint)
            {
                ropePoint = pointObject.AddComponent<RopePoint>();
            }
            
            if (autoSetEndsAsAnchors && (i == 0 || i == pointsAmount - 1))
            {
                ropePoint.isAnchor = true;
            }
            else
            {
                ropePoint.isAnchor = false;
            }
        }
        
        Debug.Log($"Created {pointsAmount} rope points");
    }

    [Button(ButtonPlayMode.OnlyWhenNotPlaying)]
    public void ClearPoints()
    {
        RopePoint[] existingPoints = GetComponentsInChildren<RopePoint>();
        
        for (int i = existingPoints.Length - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
            {
                Destroy(existingPoints[i].gameObject);
            }
            else
            {
                DestroyImmediate(existingPoints[i].gameObject);
            }
        }
        
        Debug.Log("Cleared all rope points");
    }
}