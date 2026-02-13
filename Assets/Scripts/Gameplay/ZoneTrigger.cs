using DNExtensions.Utilities;
using DNExtensions.Utilities.AutoGet;
using FishingVillage.Player;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace FishingVillage.Gameplay
{
    
    public enum TriggerMode
    {
        OnEnter = 0,
        OnItemUse = 1,
    }
    
    
    [RequireComponent(typeof(Collider))]
    public class ZoneTrigger : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string triggerID;
        [SerializeField] private bool triggerOnce = true;
        [SerializeField] private LayerMask triggerLayer;
        [SerializeField, HideInInspector, AutoGetSelf] private Collider col;
        
        [Space]
        [SerializeField] private TriggerMode triggerMode = TriggerMode.OnEnter;
        [SerializeField, ShowIf("triggerMode", TriggerMode.OnItemUse)] private SOItem requiredItem;
        [SerializeField] private UnityEvent onTriggered;
        
        [SerializeField] private bool showGizmo = true;
        [SerializeField, ReadOnly] private bool hasTriggered;
        [SerializeField, ReadOnly] private bool playerInside;

        
        private void OnEnable()
        {
            if (triggerMode == TriggerMode.OnItemUse)
            {
                GameEvents.OnItemUsed += OnItemUsed;
            }
        }
        
        private void OnDisable()
        {
            if (triggerMode == TriggerMode.OnItemUse)
            {
                GameEvents.OnItemUsed -= OnItemUsed;
            }
    
            if (playerInside && !string.IsNullOrEmpty(triggerID))
            {
                GameEvents.TriggerExited(triggerID, GetInstanceID());
                playerInside = false;
            }
        }

        private void OnItemUsed(SOItem item)
        {
            if (!item || !requiredItem || !playerInside) return;
            
            if (item == requiredItem && (!hasTriggered || !triggerOnce))
            {
                GameEvents.ItemUsedInTrigger(triggerID, item);
                onTriggered?.Invoke();
                hasTriggered = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & triggerLayer) == 0) return;
            
            if (other.TryGetComponent(out PlayerController player))
            {
                if (!string.IsNullOrEmpty(triggerID))
                {
                    GameEvents.TriggerEntered(triggerID, GetInstanceID());
                }

                if (triggerMode == TriggerMode.OnEnter && (!hasTriggered || !triggerOnce))
                {
                    onTriggered?.Invoke();
                    hasTriggered = true;
                }
                
                playerInside = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            
            if (((1 << other.gameObject.layer) & triggerLayer) == 0) return;

            if (other.TryGetComponent(out PlayerController player))
            {
                if (!string.IsNullOrEmpty(triggerID))
                {
                    GameEvents.TriggerExited(triggerID, GetInstanceID());
                }
                
                playerInside = false;
            }
        }

        public void Reset()
        {
            hasTriggered = false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!col || !showGizmo) return;   
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(col.bounds.size.x, col.bounds.size.y, col.bounds.size.z));
            
            Handles.Label(transform.position + new Vector3(0,col.bounds.size.y/1.5f,0), $"Zone Trigger({triggerID})");
        }
#endif
    }
}