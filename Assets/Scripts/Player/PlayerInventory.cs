using System;
using System.Collections.Generic;
using System.Linq;
using DNExtensions;
using DNExtensions.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingVillage.Player
{
    [RequireComponent(typeof(PlayerControllerInput))]
    public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;
    
    [Header("Inventory Settings")]
    [SerializeField, ReadOnly] private List<SOItem> allItems = new List<SOItem>();
    [SerializeField, ReadOnly] private SOItem equippedItem;
    
    private PlayerControllerInput _input;
    private PlayerController _playerController;
    
    public List<SOItem> NonUsableItems => allItems.Where(item => !item.Usable).ToList();
    public List<SOItem> UsableItems => allItems.Where(item => item.Usable).ToList();
    public List<SOItem> AllItems => allItems;
    public SOItem EquippedItem => equippedItem;
    public int Count => allItems.Count;
    public bool IsEmpty => allItems.Count == 0;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        _input = GetComponent<PlayerControllerInput>();
        _playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        GameEvents.InventoryChanged(this);
    }

    private void OnEnable()
    {
        _input.OnUseAction += OnUseAction;
        _input.OnCycleItemsAction += OnCycleItemsAction;
    }

    private void OnDisable()
    {
        _input.OnUseAction -= OnUseAction;
        _input.OnCycleItemsAction -= OnCycleItemsAction;
    }

    private void OnUseAction(InputAction.CallbackContext context)
    {
        if (!_playerController.AllowControl) return;
        
        if (context.started)
        {
            equippedItem?.Use();
        }
    }

    private void OnCycleItemsAction(InputAction.CallbackContext context)
    {
        if (!_playerController.AllowControl) return;
        
        if (!context.performed || IsEmpty) return;

        var usableItems = UsableItems;
        if (usableItems.Count == 0) return;

        int currentIndex = equippedItem ? usableItems.IndexOf(equippedItem) : -1;
        int cycleDir = Mathf.RoundToInt(context.ReadValue<float>());
    
        if (cycleDir == 0) return;
    
        int nextIndex = currentIndex + cycleDir;
    
        if (nextIndex < 0)
        {
            nextIndex = usableItems.Count - 1;
        }
        else if (nextIndex >= usableItems.Count)
        {
            nextIndex = 0;
        }
    
        equippedItem = usableItems[nextIndex];
        GameEvents.ItemEquipped(equippedItem);
    }

    public bool TryAddItem(SOItem item)
    {
        if (!item)
        {
            Debug.LogWarning("Attempted to add null item to inventory.");
            return false;
        }
        
        
        allItems.Add(item);
        OnItemAdded(item);
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
        
        OnItemRemoved(item);
        return true;
    }
    
    public bool HasItem(SOItem item)
    {
        return allItems.Contains(item);
    }
    
    public void Clear()
    {
        allItems.Clear();
        GameEvents.InventoryChanged(this);
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

    private void OnItemAdded(SOItem item)
    {
        GameEvents.ItemObtained(item);
        
        if (!equippedItem && item.Usable)
        {
            equippedItem = item;
            GameEvents.ItemEquipped(equippedItem);
        }
        
        GameEvents.InventoryChanged(this);
    }

    private void OnItemRemoved(SOItem item)
    {
        if (equippedItem == item)
        {
            equippedItem = null;
            GameEvents.ItemEquipped(equippedItem);
        }

        GameEvents.ItemRemoved(item);
        GameEvents.InventoryChanged(this);
    }
    }
}