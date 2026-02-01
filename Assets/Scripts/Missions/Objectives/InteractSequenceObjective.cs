using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;


[Serializable]
[SerializableSelectorName("Interact Sequence", "Interactable")]
public class InteractSequenceObjective : MissionObjective
{
    [SerializeField] private Interactable[] requiredSequence;
    
    private string[] targetIDs;
    private int currentIndex;
    
    public override string Description => $"Interact In Order ({currentIndex}/{requiredSequence.Length})";
    
    public override void Initialize()
    {
        currentIndex = 0;
    
        targetIDs = new string[requiredSequence.Length];
        for (int i = 0; i < requiredSequence.Length; i++)
        {
            if (requiredSequence[i])
            {
                targetIDs[i] = requiredSequence[i].InteractableID;
            }
            else
            {
                Debug.LogError($"Sequence objective has null reference at index {i}!");
            }
        }
    
        GameEvents.OnInteractedWith += OnInteraction;
    }

    public override void Cleanup()
    {
        GameEvents.OnInteractedWith -= OnInteraction;
    }
    
    public override bool Evaluate()
    {
        return currentIndex >= requiredSequence.Length;
    }
    
    private void OnInteraction(Interactable interactable)
    {
        if (currentIndex >= targetIDs.Length) return;
        
        bool isPartOfSequence = Array.Exists(targetIDs, id => id == interactable.InteractableID);
        if (!isPartOfSequence) return; 
        
        
        if (interactable.InteractableID == targetIDs[currentIndex])
        {
            currentIndex++;
            
            if (Evaluate())
            {
                SetMet();
            }
        }
        else 
        {
            currentIndex = 0;
        }
    }
}
