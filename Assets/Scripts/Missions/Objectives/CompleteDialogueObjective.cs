using System;
using DNExtensions.Utilities.PrefabSelector;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;



[Serializable]
[SerializableSelectorName("Complete Dialogue Sequence", "NPC")]
public class CompleteDialogueObjective : MissionObjective
{
    [SerializeField, PrefabSelector("Assets/Prefabs/Npcs")]  private NPC npc;
    
    private string _targetID;
    
    protected override string Description => npc 
        ? $"Talk With {npc.Name}" 
        : "Talk With (No NPC Was Set)";
    
    public override void Initialize()
    {
        if (!npc)
        {
            Debug.LogError("No NPC reference set in dialogue objective!");
            return;
        }
        
        _targetID = npc.InteractableID;
        
        if (string.IsNullOrEmpty(_targetID))
        {
            Debug.LogError($"NPC prefab {npc.name} has no ID set!");
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