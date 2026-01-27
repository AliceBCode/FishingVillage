using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
[AddTypeMenu("Give Item")]
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