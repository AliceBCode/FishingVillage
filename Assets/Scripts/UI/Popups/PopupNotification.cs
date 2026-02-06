using System;
using DNExtensions.ObjectPooling;
using DNExtensions.Shapes;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PrimeTween;

namespace FishingVillage.UI.Popup
{
    

    public class PopupNotification : MonoBehaviour, IPoolable
    {
        [Header("References")] 
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private SDFCircle iconBackground;
        [SerializeField] private Image iconImage;
        [SerializeField] private SDFRectangle backgroundImage;

        [Header("Animation")] 
        [SerializeField] private float showDuration = 0.3f;
        [SerializeField] private float hideDuration = 0.2f;

        public RectTransform RectTransform { get; private set; }

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();

            if (!_canvasGroup)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        public void Setup(string message, Color backgroundColor, Sprite icon = null)
        {
            messageText.text = message;

            if (iconBackground)
            {
                iconBackground.gameObject.SetActive(icon);
            }
            
            if (iconImage)
            {
                iconImage.gameObject.SetActive(icon);
                if (icon) iconImage.sprite = icon;
            }

            if (backgroundImage)
            {
                backgroundImage.baseColor = backgroundColor;
            }

            Show();
        }

        private void Show()
        {
            _canvasGroup.alpha = 0f;
            transform.localScale = Vector3.one * 0.8f;

            Tween.Alpha(_canvasGroup, 1f, showDuration, Ease.OutCubic);
            Tween.Scale(transform, Vector3.one, showDuration, Ease.OutBack);
        }

        public void Hide(Action onComplete = null)
        {
            Tween.Alpha(_canvasGroup, 0f, hideDuration, Ease.InCubic)
                .OnComplete(() => onComplete?.Invoke());
        }

        public void OnPoolGet()
        {
            
        }

        public void OnPoolReturn()
        {
            _canvasGroup.alpha = 0f;
            transform.localScale = Vector3.one;
   
        }

        public void OnPoolRecycle()
        {

        }
    }
}