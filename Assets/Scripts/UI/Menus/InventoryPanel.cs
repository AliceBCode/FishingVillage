
using System.Collections.Generic;
using DNExtensions.ObjectPooling;
using TMPro;
using UnityEngine;

namespace FishingVillage.UI.Menus
{


    public class InventoryPanel : MonoBehaviour
    {
        [SerializeField] private InventoryPanelItem panelItemPrefab;
        [SerializeField] private Transform container;
        [SerializeField] private TextMeshProUGUI selectedItemTextName;
        [SerializeField] private TextMeshProUGUI selectedItemTextDescription;
    
        private readonly List<InventoryPanelItem> _itemSlots = new List<InventoryPanelItem>();

        private void Awake()
        {
            GameEvents.OnInventoryChanged += OnInventoryChanged;
            GameEvents.OnInventoryItemSelected += OnInventoryItemSelected;
        }

        private void OnDestroy()
        {
            GameEvents.OnInventoryChanged -= OnInventoryChanged;
            GameEvents.OnInventoryItemSelected -= OnInventoryItemSelected;
        }

        private void OnInventoryItemSelected(SOItem item)
        {
            if (!item) return;

            selectedItemTextDescription.text = item.Description;
            selectedItemTextName.text = item.Name;
        }

        private void OnInventoryChanged(PlayerInventory inventory)
        {
            if (!inventory) return;
            
            ClearItems();
            
            foreach (var item in inventory.AllItems)
            {
                GameObject slotGo = ObjectPooler.GetObjectFromPool(panelItemPrefab.gameObject);
                slotGo.transform.SetParent(container, false);
                InventoryPanelItem slot = slotGo.GetComponent<InventoryPanelItem>();
                
                slot.Setup(item);
                _itemSlots.Add(slot);
            }
            
        }

        private void ClearItems()
        {
            foreach (var slot in _itemSlots)
            {
                if (slot) ObjectPooler.ReturnObjectToPool(slot.gameObject);
            }
            
            _itemSlots.Clear();
            
            selectedItemTextDescription.text = string.Empty;
            selectedItemTextName.text = string.Empty;
        }
    }
}