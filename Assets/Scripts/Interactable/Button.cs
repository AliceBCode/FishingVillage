using System;
using DNExtensions.Utilities.SerializableSelector;
using FishingVillage.GameActions;
using FishingVillage.Gameplay;
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
        [SerializeReference, SerializableSelector] private GameAction[] actionsOnPress = Array.Empty<GameAction>();
        [SerializeField] private UnityEvent onPressed;
        
        private InteractableVisuals _visuals;
        private bool _pressed;

        private void Awake()
        {
            _visuals = GetComponent<InteractableVisuals>();
        }

        public bool CanInteract()
        {
            return canInteract && (!_pressed || !oneTimeUse);
        }

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
        
        public void SetCanInteract(bool value)
        {
            canInteract = value;
            if(!canInteract) HideInteract();
        }
    }
}