using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<PlayerInventory> OnInventoryChanged;
    public static event Action<SOItem> OnItemObtained;
    public static event Action<SOItem> OnItemRemoved;
    public static event Action<SOItem> OnItemUsed;
    public static event Action<SOItem> OnItemEquipped;
    public static event Action OnWalkAction;
    public static event Action OnJumpedAction;
    public static event Action OnCleaningUtensilsUsed;
    public static event Action OnHornUsed;
    
    
    public static event Action<SOItem, NPC> OnItemGivenToNpc;
    public static event Action<NPC> OnNpcTalkedTo;
    public static event Action<Interactable> OnInteractedWith;
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

    public static void ItemUsed(SOItem item)
    {
        OnItemUsed?.Invoke(item);
    }
    
    public static void InventoryChanged(PlayerInventory inventory)
    {
        OnInventoryChanged?.Invoke(inventory);
    }

    public static void ItemEquipped(SOItem item)
    {
        OnItemEquipped?.Invoke(item);
    }
    
    public static void ItemGivenToNpc(SOItem item, NPC npc)
    {
        OnItemGivenToNpc?.Invoke(item, npc);
    }
    
    public static void NpcTalkedTo(NPC npc)
    {
        OnNpcTalkedTo?.Invoke(npc);
    }

    public static void InteractedWith(Interactable interactable)
    {
        OnInteractedWith?.Invoke(interactable);
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

    public static void WalkedAction()
    {
        OnWalkAction?.Invoke();
    }

    public static void JumpedAction()
    {
        OnJumpedAction?.Invoke();
    }

    public static void UsedCleaningUtensils()
    {
        OnCleaningUtensilsUsed?.Invoke();
    }

    public static void UsedHorn()
    {
        OnHornUsed?.Invoke();
    }

}