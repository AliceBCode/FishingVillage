using DNExtensions.Shapes;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace FishingVillage.UI.Menus
{
    public class InventoryPanelItem : Selectable, IPointerClickHandler, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private SDFRectangle backgroundImage;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = Color.yellow;
        [SerializeField] private float rotationAngle = 5f;
        [SerializeField] private float animationDuration = 0.2f;
        
        private SOItem _item;

        protected override void Awake()
        {
            base.Awake();
            transition = Transition.None;
        }

        public void Setup(SOItem item)
        {
            _item = item;
            iconImage.sprite = item.Icon;
            iconImage.color = Color.white;
            backgroundImage.baseColor = normalColor;
            backgroundImage.transform.localRotation = Quaternion.identity;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Select();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            Select();
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            GameEvents.InventoryItemSelected(_item);
            AnimateSelected();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            AnimateDeselected();
        }

        private void AnimateSelected()
        {
            backgroundImage.baseColor = selectedColor;
            backgroundImage.transform.localRotation = Quaternion.Euler(0, 0, rotationAngle);

        }

        private void AnimateDeselected()
        {
            backgroundImage.baseColor = normalColor;
            backgroundImage.transform.localRotation = Quaternion.identity;
        }
    }
}