using DNExtensions.Shapes;
using DNExtensions.Utilities;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace FishingVillage.UI.Menus
{
    public class InventoryPanelItem : Selectable
    {
        
        [Header("Animation")]
        [SerializeField] private float animationDuration = 0.1f;
        [SerializeField] private Ease animationEase = Ease.OutQuad;
        
        [Header("Icon")]
        [SerializeField] private Image iconImage;
        [SerializeField] private float scaleMultiplier = 1.1f;
        
        [Header("Background")]
        [SerializeField] private SDFRectangle backgroundImage;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = Color.yellow;
        [SerializeField] private float rotationAngle = -20f;

        
        private SOItem _item;
        private Sequence _selectionSequence;
        private Quaternion _startBackgroundRotation;
        private Vector3 _startIconScale;

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
            _startIconScale = iconImage.transform.localScale;
            
            backgroundImage.baseColor = normalColor;
            _startBackgroundRotation = backgroundImage.transform.localRotation;
            
            this.EnableMouseHoverSelection();
        }


        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            AnimateSelected();
            GameEvents.InventoryItemSelected(_item);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            AnimateDeselected();
        }

        private void AnimateSelected()
        {
            if (_selectionSequence.isAlive)
            {
                _selectionSequence.Stop();
            }

            var startRotation = backgroundImage.transform.localRotation.eulerAngles;
            
            backgroundImage.baseColor = selectedColor;

            _selectionSequence = Sequence.Create()
                .Group(Tween.LocalRotation(backgroundImage.transform, startRotation, new Vector3(0, 0, rotationAngle), animationDuration,animationEase))
                .Group(Tween.Scale(iconImage.transform, _startIconScale * scaleMultiplier, animationDuration,animationEase));

        }

        private void AnimateDeselected()
        {
            if (_selectionSequence.isAlive)
            {
                _selectionSequence.Stop();
            }
            
            var startRotation = backgroundImage.transform.localRotation;
            
            backgroundImage.baseColor = normalColor;
            
            _selectionSequence = Sequence.Create()
                    .Group(Tween.LocalRotation(backgroundImage.transform, startRotation,_startBackgroundRotation, animationDuration,animationEase))
                    .Group(Tween.Scale(iconImage.transform,  _startIconScale, animationDuration,animationEase));
        }
    }
}