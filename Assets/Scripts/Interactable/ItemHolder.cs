using FishingVillage.Player;
using UnityEngine;
using UnityEngine.Events;

namespace FishingVillage.Interactable
{
    public class ItemHolder : MonoBehaviour, IInteractable
    {
        [SerializeField] private SOItem item;
        [SerializeField] private UnityEvent onItemTaken;
        
        private InteractableVisuals _visuals;
        private Animator _animator;
        private bool _taken;

        private void Awake()
        {
            _visuals = GetComponent<InteractableVisuals>();
            _animator = GetComponent<Animator>();
        }

        public bool CanInteract() => !_taken && item;

        public void Interact()
        {
            if (!CanInteract()) return;
            
            _taken = true;
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