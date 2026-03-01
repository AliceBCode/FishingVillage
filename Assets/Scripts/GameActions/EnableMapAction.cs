using System;
using DNExtensions.Utilities.SerializableSelector;
using FishingVillage.Player;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.GameActions
{
    [Serializable]
    [MovedFrom("")]
    [SerializableSelectorName("Enable Map", "Inventory")]
    public class EnableMapAction : GameAction
    {
        [SerializeField] private bool enabled = true;

        public override string ActionName => $"Sets HasMap to {enabled}";

        public override void Execute()
        {
            PlayerInventory.Instance?.SetHasMap(enabled);
        }
    }
}