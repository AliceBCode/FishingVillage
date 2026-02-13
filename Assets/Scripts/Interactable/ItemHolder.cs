using DNExtensions.Utilities;
using FishingVillage.Player;
using UnityEngine;
using UnityEngine.Events;

namespace FishingVillage.Interactable
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public class ItemHolder : MonoBehaviour, IInteractable
    {
        [SerializeField] private SOItem item;
        [SerializeField] private UnityEvent onItemTaken;
        [SerializeField, ReadOnly] private bool taken;
        
        private InteractableVisuals _visuals;


        private void Awake()
        {
            _visuals = GetComponent<InteractableVisuals>();
        }

        public bool CanInteract() => !taken && item && enabled;

        public void Interact()
        {
            if (!CanInteract()) return;
            
            taken = true;
            PlayerInventory.Instance.TryAddItem(item);
                
            onItemTaken?.Invoke();
            GameEvents.InteractedWith(this);
        }

        public void ShowInteract()
        {
            if (CanInteract()) _visuals?.Show();
        }

        public void HideInteract()
        {
            _visuals?.Hide();
        }
    }
}