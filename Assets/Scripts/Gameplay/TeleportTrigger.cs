using DNExtensions.Utilities.AutoGet;
using UnityEditor;
using UnityEngine;

namespace FishingVillage.Gameplay
{
    
    [RequireComponent(typeof(Collider))]
    public class TeleportTrigger : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string triggerID;
        [SerializeField] private bool showGizmo = true;
        [SerializeField, AutoGetSelf, HideInInspector] private Collider col;

        public string TriggerID => triggerID;
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!col || !showGizmo) return;   
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(col.bounds.size.x, col.bounds.size.y, col.bounds.size.z));
            
            Handles.Label(transform.position + new Vector3(0,col.bounds.size.y/1.5f,0), $"Teleport Trigger({triggerID})");
        }
#endif
    }
}