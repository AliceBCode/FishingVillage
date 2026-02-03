namespace UI
{
    using System.Collections.Generic;
    using UnityEngine;

    public class InventoryPanel : MonoBehaviour
    {
        [SerializeField] private InventoryPanelItem panelItemPrefab;
        [SerializeField] private Transform container;
    
        private readonly List<InventoryPanelItem> _itemSlots = new List<InventoryPanelItem>();

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
                var slot = Instantiate(panelItemPrefab, container);
                slot.Image.sprite = item.Icon;
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