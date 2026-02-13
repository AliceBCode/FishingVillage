using System;
using DNExtensions.Utilities.SerializableSelector;
using DNExtensions.Utilities.SerializedInterface;
using FishingVillage.Interactable;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.Missions.Objectives
{
    [Serializable]
    [MovedFrom("")]
    [SerializableSelectorName("Interact With", "Interactable")]
    public class InteractWithObjective : MissionObjective
    {
        [SerializeField] private InterfaceReference<IInteractable, MonoBehaviour> interactableReference;
    
        private string _targetID;
        
        protected override string Description => $"Interact With {(interactableReference.UnderlyingValue ? interactableReference.UnderlyingValue.name : "(No Interactable Set)")}";
        
        public override void Initialize()
        {
            if (interactableReference.IsNull)
            {
                Debug.LogError("No Interactable reference set in objective!");
                return;
            }
            
            _targetID = GetInteractableID(interactableReference.UnderlyingValue);
            
            if (string.IsNullOrEmpty(_targetID))
            {
                Debug.LogError($"Interactable {interactableReference.UnderlyingValue.name} has no ID set!");
                return;
            }
            
            GameEvents.OnInteractedWith += OnInteractedWith;
        }
        
        public override void Cleanup()
        {
            GameEvents.OnInteractedWith -= OnInteractedWith;
        }
        
        public override bool Evaluate()
        {
            return false;
        }
        
        private void OnInteractedWith(IInteractable interactable)
        {
            if (interactable is MonoBehaviour mb && MatchesID(mb, _targetID))
            {
                SetMet();
            }
        }
    }
}