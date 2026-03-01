using System;
using DNExtensions.Utilities.SerializableSelector;
using FishingVillage.Player;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.GameActions
{
    [Serializable]
    [MovedFrom("")]
    [SerializableSelectorName("Enable Backpack", "Inventory")]
    public class EnableBackpackAction : GameAction
    {
        [SerializeField] private bool enabled = true;

        public override string ActionName => $"Sets HasBackpack to {enabled}";

        public override void Execute()
        {
            PlayerInventory.Instance?.SetHasBackpack(enabled);
        }
    }
}