using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;


[Serializable]
[SerializableSelectorName("Interact With", "Interactable")]
public class InteractWithObjective : MissionObjective
{
    [SerializeField] private Interactable interactableReference;
    
    private string _targetID;
    
    protected override string Description => $"Interact With {(interactableReference ? interactableReference.name : "(No Interactable Set)")}";
    
    public override void Initialize()
    {
        if (!interactableReference)
        {
            Debug.LogError("No Interactable prefab reference set in objective!");
            return;
        }
        
        _targetID = interactableReference.InteractableID;
        
        if (string.IsNullOrEmpty(_targetID))
        {
            Debug.LogError($"Interactable prefab {interactableReference.name} has no ID set!");
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
    
    private void OnInteractedWith(Interactable interactable)
    {
        if (interactable && interactable.InteractableID == _targetID)
        {
            SetMet();
        }
    }
}