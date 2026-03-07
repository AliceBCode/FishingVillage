using System;
using DNExtensions.Utilities.SerializableSelector;
using FishingVillage.Player;
using UnityEngine;

namespace FishingVillage.GameActions
{
    [Serializable]
    [SerializableSelectorName("Teleport", "Player")]
    public class TeleportPlayerAction : GameAction
    {
        [SerializeField] private string triggerID = string.Empty;
        
        public override string ActionName => $"Teleports the player to: {triggerID}";

        public override void Execute()
        {
            if (string.IsNullOrEmpty(triggerID)) return;
            
            var trigger = FindTeleportTriggerInScene(triggerID);
            
            if (!trigger) return;
            
            PlayerController.Instance?.TeleportTo(trigger.transform.position);
        }
    }
}