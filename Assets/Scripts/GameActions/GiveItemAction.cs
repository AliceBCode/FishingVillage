using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;
using UnityEngine.Events;
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
            if (item && Player.PlayerInventory.Instance)
            {
                Player.PlayerInventory.Instance.TryAddItem(item);
            }
        }
    }
}