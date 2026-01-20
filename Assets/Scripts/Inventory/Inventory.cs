using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    [SerializeField] private int maxSlots = 10;
    [SerializeField] private List<SOItem> items;
    
    public event Action<SOItem> OnItemAdded;
    public event Action<SOItem> OnItemRemoved;
    public event Action<Inventory> OnInventoryChanged;
    
    public IReadOnlyList<SOItem> Items => items;
    public int Count => items.Count;
    public int MaxSlots => maxSlots;
    public bool IsFull => items.Count >= maxSlots;
    public bool IsEmpty => items.Count == 0;

    public Inventory()
    {
        items = new List<SOItem>();
    }

    public Inventory(int maxSlots)
    {
        this.maxSlots = maxSlots;
        items = new List<SOItem>();
    }


    
    public bool TryAddItem(SOItem item)
    {
        if (!item)
        {
            Debug.LogWarning("Attempted to add null item to inventory.");
            return false;
        }
        
        if (IsFull)
        {
            Debug.LogWarning($"Inventory is full. Cannot add {item.name}.");
            return false;
        }
        
        items.Add(item);
        OnItemAdded?.Invoke(item);
        OnInventoryChanged?.Invoke(this);
        return true;
    }
    
    public bool TryRemoveItem(SOItem item)
    {
        if (!item)
        {
            Debug.LogWarning("Attempted to remove null item from inventory.");
            return false;
        }
        
        if (!items.Remove(item))
        {
            Debug.LogWarning($"Item {item.name} not found in inventory.");
            return false;
        }
        
        OnItemRemoved?.Invoke(item);
        OnInventoryChanged?.Invoke(this);
        return true;
    }
    
    public bool HasItem(SOItem item)
    {
        return items.Contains(item);
    }
    
    public void Clear()
    {
        items.Clear();
        OnInventoryChanged?.Invoke(this);
    }
    
    public SOItem GetItemAtIndex(int index)
    {
        if (index < 0 || index >= items.Count)
        {
            Debug.LogWarning($"Index {index} out of range.");
            return null;
        }
        
        return items[index];
    }

    public int GetItemIndex(SOItem item)
    {
        if (!item) return -1;
        
        return items.IndexOf(item);
    }
}