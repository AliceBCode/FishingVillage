using DNExtensions.ObjectPooling;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace FishingVillage.UI.SelectionWheel
{
    

    public class SelectionWheelItem : MonoBehaviour, IPoolable
    {
        [Header("Use")] 
        [SerializeField] private float usedPunchStrength = 1.5f;
        [SerializeField] private float usedPunchDuration = 0.25f;

        [Header("Animation")] 
        [SerializeField] private float transitionDuration = 0.3f;
        [SerializeField] private Ease transitionEase = Ease.OutCubic;

        private RectTransform _rectTransform;
        private Sequence _transitionSequence;

        public Image Image { get; private set; }
        public SlotType CurrentSlotType { get; private set; }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            Image = GetComponent<Image>();
        }

        public void PlayUsedAnimation()
        {
            Tween.PunchScale(transform, Vector3.one * usedPunchStrength, usedPunchDuration, 1);
        }

        public void AnimateToPosition(Vector2 position, float scale, float alpha, SlotType slotType)
        {
            CurrentSlotType = slotType;
            Tween.UIAnchoredPosition(_rectTransform, position, transitionDuration, transitionEase);
            Tween.Scale(_rectTransform, scale, transitionDuration, transitionEase);
            Tween.Alpha(Image, alpha, transitionDuration, transitionEase);
        }

        public void SetPositionImmediate(Vector2 position, float scale, float alpha, SlotType slotType)
        {
            CurrentSlotType = slotType;
            _rectTransform.anchoredPosition = position;
            _rectTransform.localScale = Vector3.one * scale;

            var color = Image.color;
            color.a = alpha;
            Image.color = color;
        }


        public void OnPoolGet()
        {
            
        }

        public void OnPoolReturn()
        {
            if (_transitionSequence.isAlive)
            {
                _transitionSequence.Stop();
            }
            
            _rectTransform.anchoredPosition = Vector2.zero;
            _rectTransform.localScale = Vector3.one;
            Image.color = Color.white;
        }

        public void OnPoolRecycle()
        {
           
        }
    }
}