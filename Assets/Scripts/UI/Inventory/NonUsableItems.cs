namespace UI
{
    using System.Collections.Generic;
    using UnityEngine;

    public class NonUsableItemsUI : MonoBehaviour
    {
        [SerializeField] private InventoryItem itemPrefab;
        [SerializeField] private Transform container;
    
        private readonly List<InventoryItem> _itemSlots = new List<InventoryItem>();

        private void OnEnable()
        {
            GameEvents.OnInventoryChanged += OnInventoryChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnInventoryChanged -= OnInventoryChanged;
        }

        private void OnInventoryChanged(PlayerInventory inventory)
        {
            if (!inventory) return;
            
            ClearItems();
            
            foreach (var item in inventory.NonUsableItems)
            {
                var slot = Instantiate(itemPrefab, container);
                slot.Image.sprite = item.Icon;
                slot.Text.text = item.Name;
                _itemSlots.Add(slot);
            }
        }

        private void ClearItems()
        {
            foreach (var slot in _itemSlots)
            {
                if (slot) Destroy(slot.gameObject);
            }
            _itemSlots.Clear();
        }
    }
}