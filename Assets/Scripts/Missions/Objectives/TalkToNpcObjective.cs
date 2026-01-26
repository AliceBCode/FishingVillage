using System;
using UnityEngine;


[Serializable]
[AddTypeMenu("Talk to NPC")]
public class TalkToNpcObjective : MissionObjective
{
    [SerializeField] private NPC npcReference;
    
    private string targetID;
    
    public override string Name => "Talk To NPC";
    public override string Description => $"Talk to {(npcReference ? npcReference.name : "Unknown")}";
    
    public override void Initialize()
    {
        if (!npcReference)
        {
            Debug.LogError("No NPC prefab reference set in objective!");
            return;
        }
        
        targetID = npcReference.InteractableID;
        
        if (string.IsNullOrEmpty(targetID))
        {
            Debug.LogError($"NPC prefab {npcReference.name} has no ID set!");
            return;
        }
        
        GameEvents.OnNpcTalkedTo += OnNPCTalkedTo;
    }
    
    public override void Cleanup()
    {
        GameEvents.OnNpcTalkedTo -= OnNPCTalkedTo;
    }
    
    public override bool Evaluate()
    {
        return false;
    }
    
    private void OnNPCTalkedTo(NPC npc)
    {
        if (npc && npc.InteractableID == targetID)
        {
            SetMet();
        }
    }
}