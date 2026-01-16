using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<SOItem> OnItemObtained;
    public static event Action<SOItem> OnItemRemoved;
    public static event Action<SOItem, NPC> OnItemGivenToNPC;
    public static event Action<NPC> OnNPCTalkedTo;
    public static event Action<string> OnTriggerEntered;
    
    public static void ItemObtained(SOItem item)
    {
        OnItemObtained?.Invoke(item);
    }
    
    public static void ItemRemoved(SOItem item)
    {
        OnItemRemoved?.Invoke(item);
    }
    
    public static void ItemGivenToNpc(SOItem item, NPC npc)
    {
        OnItemGivenToNPC?.Invoke(item, npc);
    }
    
    public static void NpcTalkedTo(NPC npc)
    {
        OnNPCTalkedTo?.Invoke(npc);
    }
    
    public static void TriggerEntered(string triggerID)
    {
        OnTriggerEntered?.Invoke(triggerID);
    }
}