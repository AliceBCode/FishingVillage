using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;



[Serializable]
[SerializableSelectorName("Complete Dialogue Sequence")]
public class CompleteDialogueObjective : MissionObjective
{
    [SerializeField] private NPC npcReference;
    
    private string _targetID;
    
    public override string Name => "Complete Dialogue";
    public override string Description => npcReference 
        ? $"Talk with {npcReference.name}" 
        : "Talk with (no NPC was set!)";
    
    public override void Initialize()
    {
        if (!npcReference)
        {
            Debug.LogError("No NPC reference set in dialogue objective!");
            return;
        }
        
        _targetID = npcReference.InteractableID;
        
        if (string.IsNullOrEmpty(_targetID))
        {
            Debug.LogError($"NPC prefab {npcReference.name} has no ID set!");
            return;
        }
        
        GameEvents.OnDialogueSequenceCompleted += OnDialogueCompleted;
    }
    
    public override void Cleanup()
    {
        GameEvents.OnDialogueSequenceCompleted -= OnDialogueCompleted;
    }
    
    public override bool Evaluate()
    {
        return false;
    }
    
    private void OnDialogueCompleted(NPC npc)
    {
        
        if (npc && npc.InteractableID == _targetID)
        {
            SetMet();
        }
    }
}