using System;
using System.Collections.Generic;
using FishingVillage.Interactable;
using FishingVillage.Missions;
using FishingVillage.Player;
using UnityEngine;

namespace FishingVillage
{
    public static class GameEvents
    {
        public static event Action<PlayerInventory> OnInventoryChanged;
        public static event Action<SOItem> OnInventoryItemSelected;
        public static event Action<PlayerState> OnPlayerStateChanged;
        public static event Action<SOItem> OnItemObtained;
        public static event Action<SOItem> OnItemRemoved;
        public static event Action<SOItem> OnItemUsed;
        public static event Action<SOItem> OnItemEquipped;
        public static event Action OnWalkAction;
        public static event Action OnJumpedAction;


        public static event Action<SOItem, NPC> OnItemGivenToNpc;
        public static event Action<NPC> OnDialogueSequenceCompleted;
        public static event Action<IInteractable> OnInteractedWith;
        public static event Action<string, SOItem> OnItemUsedInTrigger;
        public static event Action<string> OnTriggerEntered;
        public static event Action<string> OnTriggerExited;
        public static event Action<string> OnTimelineSignalReceived;

        public static event Action<SOMission> OnMissionStarted;
        public static event Action<SOMission> OnMissionCompleted;
        
        private static readonly Dictionary<string, HashSet<int>> ActiveTriggers = new Dictionary<string, HashSet<int>>();




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

        public static void PlayerStateChanged(PlayerState state)
        {
            OnPlayerStateChanged?.Invoke(state);
        }

        public static void ItemEquipped(SOItem item)
        {
            OnItemEquipped?.Invoke(item);
        }

        public static void InventoryItemSelected(SOItem item)
        {
            OnInventoryItemSelected?.Invoke(item);
        }

        public static void ItemGivenToNpc(SOItem item, NPC npc)
        {
            OnItemGivenToNpc?.Invoke(item, npc);
        }
        

        public static void DialogueSequenceCompleted(NPC npc)
        {
            OnDialogueSequenceCompleted?.Invoke(npc);
        }

        public static void InteractedWith(IInteractable interactable)
        {
            OnInteractedWith?.Invoke(interactable);
        }

        public static void TriggerEntered(string triggerID, int instanceID)
        {
            if (!ActiveTriggers.ContainsKey(triggerID))
            {
                ActiveTriggers[triggerID] = new HashSet<int>();
            }
            
            ActiveTriggers[triggerID].Add(instanceID);
            OnTriggerEntered?.Invoke(triggerID);
        }
        
        public static void ItemUsedInTrigger(string triggerID, SOItem item)
        {
            OnItemUsedInTrigger?.Invoke(triggerID, item);
        }

        public static void TriggerExited(string triggerID, int instanceID)
        {
            if (!ActiveTriggers.TryGetValue(triggerID, out var trigger)) return;
            
            trigger.Remove(instanceID);
            
            if (ActiveTriggers[triggerID].Count == 0)
            {
                ActiveTriggers.Remove(triggerID);
            }
            
            OnTriggerExited?.Invoke(triggerID);
        }
        
        public static bool IsPlayerInTrigger(string triggerID)
        {
            return ActiveTriggers.ContainsKey(triggerID) && ActiveTriggers[triggerID].Count > 0;
        }
        
        public static void TimelineSignalReceived(string signalID)
        {
            OnTimelineSignalReceived?.Invoke(signalID);
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


    }
}