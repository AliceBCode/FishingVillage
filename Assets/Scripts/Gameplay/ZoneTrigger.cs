using System;
using DNExtensions;
using DNExtensions.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ZoneTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string triggerID;
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private LayerMask triggerLayer;
    [SerializeField] private Collider col;
    
    [Space]
    [SerializeField] private UnityEvent onTriggered;
    
    [SerializeField, ReadOnly] private bool hasTriggered;

    

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered && triggerOnce) return;
        
        if (((1 << other.gameObject.layer) & triggerLayer) == 0) return;
        
        if (other.TryGetComponent(out PlayerController player))
        {
            hasTriggered = true;
            
            if (!string.IsNullOrEmpty(triggerID))
            {
                GameEvents.TriggerEntered(triggerID);
            }
            
            onTriggered?.Invoke();
        }
    }

    public void Reset()
    {
        hasTriggered = false;
    }

    private void OnDrawGizmos()
    {
        if (!col) return;   
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(col.bounds.size.x, col.bounds.size.y, col.bounds.size.z));
        
        Handles.Label(transform.position + new Vector3(0,col.bounds.size.y + 0.05f,0), $"Zone Trigger({triggerID})");
    }
}