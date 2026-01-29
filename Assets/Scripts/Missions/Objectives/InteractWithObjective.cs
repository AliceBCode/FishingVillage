using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;


[Serializable]
[SerializableSelectorName("Interact with")]
public class InteractWithObjective : MissionObjective
{
    [SerializeField] private Interactable interactableReference;
    
    private string targetID;
    
    public override string Name => "Interact with";
    public override string Description => $"Interact with {(interactableReference ? interactableReference.name : "Unknown")}";
    
    public override void Initialize()
    {
        if (!interactableReference)
        {
            Debug.LogError("No Interactable prefab reference set in objective!");
            return;
        }
        
        targetID = interactableReference.InteractableID;
        
        if (string.IsNullOrEmpty(targetID))
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
        if (interactable && interactable.InteractableID == targetID)
        {
            SetMet();
        }
    }
}