using DNExtensions.Utilities.RangedValues;

namespace UI
{
    using System.Collections.Generic;
    using DNExtensions;
    using UnityEngine;
    using PrimeTween;

    public class SelectionWheel : MonoBehaviour
    {

        [Header("Layout Settings")] 
        [SerializeField] private SelectionWheelItem itemPrefab;
        [SerializeField] private float radiusX = 100f;
        [SerializeField] private float radiusY = 50f;
        [SerializeField, MinMaxRange(0f, 1f)] private RangedFloat scaleRange = new RangedFloat(0.6f, 1f);
        [SerializeField, MinMaxRange(0f, 1f)] private RangedFloat alphaRange = new RangedFloat(0.3f, 1f);

        [Header("Animation")] 
        [SerializeField] private float transitionDuration = 0.3f;
        [SerializeField] private Ease transitionEase = Ease.OutCubic;

        private readonly List<SelectionWheelItem> wheelItems = new List<SelectionWheelItem>();
        private List<SOItem> currentUsableItems = new List<SOItem>();
        private int currentIndex = 0;

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
            if (!item || currentUsableItems.Count == 0) return;

            int newIndex = currentUsableItems.IndexOf(item);
            if (newIndex != -1)
            {
                currentIndex = newIndex;
                AnimateToPositions();
            }
        }

        private void OnInventoryChanged(PlayerInventory inventory)
        {
            RebuildWheel(inventory);
        }
        
        private void OnItemUsed(SOItem item)
        {
            wheelItems[currentIndex]?.PlayUsedAnimation();
        }

        private void RebuildWheel(PlayerInventory inventory)
        {
            foreach (var item in wheelItems)
            {
                if (item) Destroy(item.gameObject);
            }

            wheelItems.Clear();

            if (inventory == null || inventory.UsableItems.Count == 0)
            {
                return;
            }

            currentUsableItems = inventory.UsableItems;
            gameObject.SetActive(true);

            currentIndex = 0;
            if (inventory.EquippedItem)
            {
                int equippedIndex = currentUsableItems.IndexOf(inventory.EquippedItem);
                if (equippedIndex != -1) currentIndex = equippedIndex;
            }

            for (int i = 0; i < currentUsableItems.Count; i++)
            {
                var wheelItem = Instantiate(itemPrefab, transform);
                wheelItem.Image.sprite = currentUsableItems[i].Icon;
                wheelItems.Add(wheelItem);
            }

            SetPositionsImmediate();
        }

        private void SetPositionsImmediate()
        {
            for (int i = 0; i < wheelItems.Count; i++)
            {
                var pos = CalculatePosition(i);
                wheelItems[i].RectTransform.anchoredPosition = pos.position;
                wheelItems[i].RectTransform.localScale = Vector3.one * pos.scale;
                wheelItems[i].SetAlpha(pos.alpha);
            }

            var sorted = new List<(SelectionWheelItem item, int siblingIndex)>();
            for (int i = 0; i < wheelItems.Count; i++)
            {
                var pos = CalculatePosition(i);
                sorted.Add((wheelItems[i], pos.siblingIndex));
            }

            sorted.Sort((a, b) => a.siblingIndex.CompareTo(b.siblingIndex));

            for (int i = 0; i < sorted.Count; i++)
            {
                sorted[i].item.transform.SetSiblingIndex(i);
            }
        }



        private void AnimateToPositions()
        {
            for (int i = 0; i < wheelItems.Count; i++)
            {
                var pos = CalculatePosition(i);
                wheelItems[i].AnimateToPosition(pos.position, pos.scale, pos.alpha, transitionDuration, transitionEase);
            }

            var sorted = new List<(SelectionWheelItem item, int siblingIndex)>();
            for (int i = 0; i < wheelItems.Count; i++)
            {
                var pos = CalculatePosition(i);
                sorted.Add((wheelItems[i], pos.siblingIndex));
            }

            sorted.Sort((a, b) => a.siblingIndex.CompareTo(b.siblingIndex));

            for (int i = 0; i < sorted.Count; i++)
            {
                sorted[i].item.transform.SetSiblingIndex(i);
            }
        }



        private (Vector2 position, float scale, int siblingIndex, float alpha) CalculatePosition(int itemIndex)
        {
            int count = wheelItems.Count;

            int offsetFromCenter = itemIndex - currentIndex;

            if (offsetFromCenter > count / 2)
                offsetFromCenter -= count;
            else if (offsetFromCenter < -count / 2)
                offsetFromCenter += count;

            float anglePerItem = 360f / count;
            float angle = offsetFromCenter * anglePerItem;
            float angleRad = angle * Mathf.Deg2Rad;

            float x = Mathf.Sin(angleRad) * radiusX;
            float y = -Mathf.Cos(angleRad) * radiusY;

            float normalizedOffset = Mathf.Abs((float)offsetFromCenter / Mathf.Max(1, count / 2));
            normalizedOffset = Mathf.Clamp01(normalizedOffset);

            float scale = Mathf.Lerp(scaleRange.maxValue, scaleRange.minValue, normalizedOffset);
            float alpha = Mathf.Lerp(alphaRange.maxValue, alphaRange.minValue, normalizedOffset);

            int siblingIndex = Mathf.RoundToInt((1f - normalizedOffset) * 100);

            return (new Vector2(x, y), scale, siblingIndex, alpha);
        }
    }
}