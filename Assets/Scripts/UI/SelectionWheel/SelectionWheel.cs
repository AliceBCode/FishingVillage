
namespace FishingVillage.UI.SelectionWheel
{
    using System.Collections.Generic;
    using UnityEngine;
    
    
    public class SelectionWheel : MonoBehaviour
    {
        [Header("Fixed Slots Settings")]
        [SerializeField] private float secondarySlotScale = 0.8f;
        [SerializeField] private float currentSlotScale = 1f;

        [Header("References")] 
        [SerializeField] private RectTransform previousSlot;
        [SerializeField] private RectTransform currentSlot;
        [SerializeField] private RectTransform nextSlot;
        [SerializeField] private SelectionWheelItem itemPrefab;
        [SerializeField] private RectTransform itemsHolder;

        private readonly List<SelectionWheelItem> _wheelItems = new List<SelectionWheelItem>();
        private List<SOItem> _currentUsableItems = new List<SOItem>();
        private int _currentIndex;

        private void OnEnable()
        {
            GameEvents.OnItemEquipped += OnItemEquipped;
            GameEvents.OnInventoryChanged += OnInventoryChanged;
            GameEvents.OnItemUsed += OnItemUsed;
        }

        private void OnDisable()
        {
            GameEvents.OnItemEquipped -= OnItemEquipped;
            GameEvents.OnInventoryChanged -= OnInventoryChanged;
            GameEvents.OnItemUsed -= OnItemUsed;
        }

        private void OnItemEquipped(SOItem item)
        {
            if (!item || _currentUsableItems.Count == 0) return;

            int newIndex = _currentUsableItems.IndexOf(item);
            if (newIndex != -1)
            {
                _currentIndex = newIndex;
                AnimateToPositions();
            }
        }

        private void OnInventoryChanged(PlayerInventory inventory)
        {
            RebuildWheel(inventory);
        }
        
        private void OnItemUsed(SOItem item)
        {
            int currentItemIndex = GetItemIndexInSlot(SlotType.Current);
            if (currentItemIndex != -1)
            {
                _wheelItems[currentItemIndex]?.PlayUsedAnimation();
            }
        }

        private void RebuildWheel(PlayerInventory inventory)
        {
            foreach (var item in _wheelItems)
            {
                if (item) Destroy(item.gameObject);
            }
            _wheelItems.Clear();
            

            _currentUsableItems = inventory.UsableItems;
            
            _currentIndex = 0;
            if (inventory.EquippedItem)
            {
                int equippedIndex = _currentUsableItems.IndexOf(inventory.EquippedItem);
                if (equippedIndex != -1) _currentIndex = equippedIndex;
            }
            
            for (int i = 0; i < _currentUsableItems.Count; i++)
            {
                var wheelItem = Instantiate(itemPrefab, itemsHolder);
                wheelItem.Image.sprite = _currentUsableItems[i].Icon;
                _wheelItems.Add(wheelItem);
            }

            SetPositionsImmediate();
        }

        private void SetPositionsImmediate()
        {
            for (int i = 0; i < _wheelItems.Count; i++)
            {
                var layoutData = CalculateLayoutData(i);
                _wheelItems[i].SetPositionImmediate(layoutData.position, layoutData.scale, layoutData.alpha, layoutData.slotType);
            }
        }

        private void AnimateToPositions()
        {
            for (int i = 0; i < _wheelItems.Count; i++)
            {
                var layoutData = CalculateLayoutData(i);
                _wheelItems[i].AnimateToPosition(layoutData.position, layoutData.scale, layoutData.alpha, layoutData.slotType);
            }
        }
        
        private (Vector2 position, float scale, float alpha, SlotType slotType, int siblingIndex) CalculateLayoutData(int itemIndex)
        {
            int itemCount = _wheelItems.Count;

            // Calculate wrapped offset from current index
            int offsetFromCenter = itemIndex - _currentIndex;
            if (offsetFromCenter > itemCount / 2)
                offsetFromCenter -= itemCount;
            else if (offsetFromCenter < -itemCount / 2)
                offsetFromCenter += itemCount;

            // Determine slot type based on item count and offset
            SlotType slotType = DetermineSlotType(offsetFromCenter, itemCount);

            // Calculate position, scale, alpha based on slot type
            switch (slotType)
            {
                case SlotType.Previous:
                    return (previousSlot.anchoredPosition, secondarySlotScale, 1, slotType, 50);

                case SlotType.Current:
                    return (currentSlot.anchoredPosition, currentSlotScale, 1, slotType, 100);

                case SlotType.Next:
                    return (nextSlot.anchoredPosition, secondarySlotScale, 1, slotType, 50);

                case SlotType.Hidden:
                default:
                    return (new Vector2(currentSlot.anchoredPosition.x,currentSlot.anchoredPosition.y + 100f), 0f, 0f, slotType, 0);
            }
        }

        private SlotType DetermineSlotType(int offsetFromCenter, int itemCount)
        {
            switch (itemCount)
            {
                // 1 item: Only current
                case 1:
                    return offsetFromCenter == 0 ? SlotType.Current : SlotType.Hidden;
                // 2 items: Current and next
                case 2:
                    return offsetFromCenter == 0 ? SlotType.Current : SlotType.Next;
            }

            // 3+ items: Previous, current, next (everything else hidden)
            if (offsetFromCenter == 0) return SlotType.Current;
            if (offsetFromCenter == -1) return SlotType.Previous;
            if (offsetFromCenter == 1) return SlotType.Next;
            
            return SlotType.Hidden;
        }

        private int GetItemIndexInSlot(SlotType targetSlotType)
        {
            for (int i = 0; i < _wheelItems.Count; i++)
            {
                if (_wheelItems[i].CurrentSlotType == targetSlotType)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}