using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
[SerializableSelectorName("Give Item")]
public class GiveItemAction : GameAction
{
    [SerializeField] private SOItem item;
    
    public override string ActionName => item ? $"Give {item.Name}" : "Give Item (No item was set)";
    
    public override void Execute()
    {
        if (item && PlayerInventory.Instance)
        {
            PlayerInventory.Instance.TryAddItem(item);
        }
    }
}