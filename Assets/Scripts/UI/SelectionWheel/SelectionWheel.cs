using DNExtensions.Utilities.RangedValues;

namespace UI
{
    using System.Collections.Generic;
    using UnityEngine;

    public class SelectionWheel : MonoBehaviour
    {
        [Header("Overflow Layout Settings")] 
        [SerializeField] private float overflowRadiusX = 100f;
        [SerializeField] private float overflowRadiusY = 50f;
        [SerializeField] private Vector2 overflowCenterOffset = new Vector2(0f, 100f);
        [SerializeField, MinMaxRange(0f, 1f)] private RangedFloat overflowScaleRange = new RangedFloat(0.5f, 0.8f);
        [SerializeField, MinMaxRange(0f, 1f)] private RangedFloat overflowAlphaRange = new RangedFloat(0.3f, 0.7f);

        [Header("Fixed Slot Visual Settings")]
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
            // Find the item at current index and play its animation
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

            UpdateSiblingIndices();
        }

        private void AnimateToPositions()
        {
            for (int i = 0; i < _wheelItems.Count; i++)
            {
                var layoutData = CalculateLayoutData(i);
                _wheelItems[i].AnimateToPosition(layoutData.position, layoutData.scale, layoutData.alpha, layoutData.slotType);
            }

            UpdateSiblingIndices();
        }

        private void UpdateSiblingIndices()
        {
            // Sort items by their sibling index (z-order)
            var sorted = new List<(SelectionWheelItem item, int siblingIndex)>();
            for (int i = 0; i < _wheelItems.Count; i++)
            {
                var layoutData = CalculateLayoutData(i);
                sorted.Add((_wheelItems[i], layoutData.siblingIndex));
            }

            sorted.Sort((a, b) => a.siblingIndex.CompareTo(b.siblingIndex));

            // Apply sibling indices
            for (int i = 0; i < sorted.Count; i++)
            {
                sorted[i].item.transform.SetSiblingIndex(i);
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

                case SlotType.Overflow:
                    return CalculateOverflowPosition(offsetFromCenter, itemCount);

                case SlotType.Hidden:
                default:
                    return (new Vector2(0, -1000f), 0f, 0f, slotType, 0); // Off-screen
            }
        }

        private SlotType DetermineSlotType(int offsetFromCenter, int itemCount)
        {
            // 1 item: Only current
            if (itemCount == 1)
            {
                return offsetFromCenter == 0 ? SlotType.Current : SlotType.Hidden;
            }

            // 2 items: Current and next
            if (itemCount == 2)
            {
                if (offsetFromCenter == 0) return SlotType.Current;
                if (offsetFromCenter == 1) return SlotType.Next;
                return SlotType.Hidden;
            }

            // 3 items: Previous, current, next
            if (itemCount == 3)
            {
                if (offsetFromCenter == 0) return SlotType.Current;
                if (offsetFromCenter == -1) return SlotType.Previous;
                if (offsetFromCenter == 1) return SlotType.Next;
                return SlotType.Hidden;
            }

            // 4+ items: Previous, current, next + overflow
            if (offsetFromCenter == 0) return SlotType.Current;
            if (offsetFromCenter == -1) return SlotType.Previous;
            if (offsetFromCenter == 1) return SlotType.Next;
            
            // Everything else goes to overflow (items at offset -2, 2, -3, 3, etc.)
            return SlotType.Overflow;
        }

        private (Vector2 position, float scale, float alpha, SlotType slotType, int siblingIndex) CalculateOverflowPosition(int offsetFromCenter, int itemCount)
        {
            // Calculate which "slot" in the overflow arc this item occupies
            // Items at offset 2, -2, 3, -3, etc. should be distributed in the semi-circle
            
            // Get all overflow offsets and find this item's index among them
            List<int> overflowOffsets = new List<int>();
            for (int i = 0; i < itemCount; i++)
            {
                int offset = i - _currentIndex;
                if (offset > itemCount / 2) offset -= itemCount;
                else if (offset < -itemCount / 2) offset += itemCount;
                
                if (Mathf.Abs(offset) > 1)
                {
                    overflowOffsets.Add(offset);
                }
            }

            overflowOffsets.Sort();
            int overflowIndex = overflowOffsets.IndexOf(offsetFromCenter);
            int overflowCount = overflowOffsets.Count;

            // Calculate angle for this overflow item
            // Distribute items in a semi-circle above the fixed slots
            float angleRange = 180f; // Semi-circle
            float startAngle = -90f - (angleRange / 2f); // Start from left side
            float anglePerItem = angleRange / Mathf.Max(1, overflowCount - 1);
            float angle = startAngle + (anglePerItem * overflowIndex);
            float angleRad = angle * Mathf.Deg2Rad;

            // Calculate position on arc
            Vector2 arcPosition = new Vector2(
                Mathf.Cos(angleRad) * overflowRadiusX,
                Mathf.Sin(angleRad) * overflowRadiusY
            );

            // Offset from current slot position
            Vector2 finalPosition = currentSlot.anchoredPosition + overflowCenterOffset + arcPosition;

            // Scale and alpha based on distance from center of overflow arc
            float normalizedPosition = (float)overflowIndex / Mathf.Max(1, overflowCount - 1);
            float distanceFromCenter = Mathf.Abs(normalizedPosition - 0.5f) * 2f; // 0 at center, 1 at edges
            
            float scale = Mathf.Lerp(overflowScaleRange.maxValue, overflowScaleRange.minValue, distanceFromCenter);
            float alpha = Mathf.Lerp(overflowAlphaRange.maxValue, overflowAlphaRange.minValue, distanceFromCenter);

            // Sibling index: overflow items render behind fixed slots
            int siblingIndex = 10 + overflowIndex;

            return (finalPosition, scale, alpha, SlotType.Overflow, siblingIndex);
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