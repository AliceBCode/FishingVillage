using System;
using DNExtensions.Utilities.SerializableSelector;
using FishingVillage.GameActions;
using UnityEngine;
using UnityEngine.Events;

namespace FishingVillage.Interactable
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public class Button : MonoBehaviour, IInteractable
    {
        [SerializeField] private bool canInteract = true;
        [SerializeField] private bool oneTimeUse;
        [SerializeField] private UnityEvent onPressed;
        [SerializeReference, SerializableSelector] 
        private GameAction[] actionsOnPress = Array.Empty<GameAction>();
        
        private InteractableVisuals _visuals;
        private bool _pressed;

        private void Awake()
        {
            _visuals = GetComponent<InteractableVisuals>();
        }

        public bool CanInteract() => canInteract && (!_pressed || !oneTimeUse);

        public void Interact()
        {
            if (!CanInteract()) return;
            
            _pressed = true;
            
            foreach (var action in actionsOnPress) action?.Execute();
                
            onPressed?.Invoke();
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