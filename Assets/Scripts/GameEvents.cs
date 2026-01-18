using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<SOItem> OnItemObtained;
    public static event Action<SOItem> OnItemRemoved;
    public static event Action<SOItem, NPC> OnItemGivenToNpc;
    public static event Action<NPC> OnNpcTalkedTo;
    public static event Action<string> OnTriggerEntered;
    public static event Action<SOMission> OnMissionStarted;
    public static event Action<SOMission> OnMissionCompleted;
    
    
    
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
        OnItemGivenToNpc?.Invoke(item, npc);
    }
    
    public static void NpcTalkedTo(NPC npc)
    {
        OnNpcTalkedTo?.Invoke(npc);
    }
    
    public static void TriggerEntered(string triggerID)
    {
        OnTriggerEntered?.Invoke(triggerID);
    }
    
    public static void MissionStarted(SOMission mission)
    {
        OnMissionStarted?.Invoke(mission);
    }
    
    public static void MissionCompleted(SOMission mission)
    {
        OnMissionCompleted?.Invoke(mission);
    }
}