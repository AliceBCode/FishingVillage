using System;
using DNExtensions.Utilities.SerializableSelector;
using FishingVillage.Player;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.GameActions
{
    [Serializable]
    [MovedFrom("")]
    [SerializableSelectorName("Give Item", "Item")]
    public class GiveItemAction : GameAction
    {
        [SerializeField] private SOItem item;

        public override string ActionName => item ? $"Give {item.Name}" : "Give Item (No item was set)";

        public override void Execute()
        {
            if (item)
            {
                PlayerInventory.Instance?.TryAddItem(item);
            }
        }
    }
}