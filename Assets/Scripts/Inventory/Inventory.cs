using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    [SerializeField] private int maxSlots = 10;
    [SerializeField] private List<SOItem> allItems;
    
    public event Action<SOItem> OnItemAdded;
    public event Action<SOItem> OnItemRemoved;
    public event Action<Inventory> OnInventoryChanged;


    public List<SOItem> NonUsableItems => AllItems.Where(item => !item.Usable).ToList();
    public List<SOItem> UsableItems => AllItems.Where(item => item.Usable).ToList();
    public List<SOItem> AllItems => allItems;
    public int Count => allItems.Count;
    public int MaxSlots => maxSlots;
    public bool IsFull => allItems.Count >= maxSlots;
    public bool IsEmpty => allItems.Count == 0;
    
    
    
    
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
        
        allItems.Add(item);
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
        
        if (!allItems.Remove(item))
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
        return allItems.Contains(item);
    }
    
    public void Clear()
    {
        allItems.Clear();
        OnInventoryChanged?.Invoke(this);
    }
    
    public SOItem GetItemAtIndex(int index)
    {
        if (index < 0 || index >= allItems.Count)
        {
            Debug.LogWarning($"Index {index} out of range.");
            return null;
        }
        
        return allItems[index];
    }

    public int GetItemIndex(SOItem item)
    {
        if (!item) return -1;
        
        return allItems.IndexOf(item);
    }
}