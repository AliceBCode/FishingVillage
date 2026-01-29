using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;


[Serializable]
[SerializableSelectorName("Give Item to an NPC")]
public class GiveItemToNpcObjective : MissionObjective
{
    [SerializeField] private SOItem requiredItem;
    [SerializeField] private NPC npcReference;
    
    public SOItem RequiredItem => requiredItem;
    public override string Name => "Give Item To NPC";
    public override string Description => $"Give {(requiredItem ? requiredItem.Name : "Unknown Item")} to {(npcReference ? npcReference.Name : "Unknown NPC")}";
    
    private string targetID;
    
    
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
            Debug.LogError($"NPC prefab {npcReference.Name} has no ID set!");
            return;
        }

        
        GameEvents.OnItemGivenToNpc += OnItemGivenToNPC;
    }
    
    public override void Cleanup()
    {
        GameEvents.OnItemGivenToNpc -= OnItemGivenToNPC;
    }
    
    public override bool Evaluate()
    {
        return false;
    }

    public bool IsNpc(NPC npc)
    {
        return npc && npc.InteractableID == targetID;
    }
    
    private void OnItemGivenToNPC(SOItem item, NPC npc)
    {
        if (item == requiredItem && IsNpc(npc))
        {
            SetMet();
        }
    }
}
