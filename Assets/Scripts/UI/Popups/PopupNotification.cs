using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PrimeTween;

namespace FishingVillage.UI.Popup
{
    

    public class PopupNotification : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private TextMeshProUGUI messageText;

        [SerializeField] private Image iconImage;
        [SerializeField] private Image backgroundImage;

        [Header("Animation")] [SerializeField] private float showDuration = 0.3f;
        [SerializeField] private float hideDuration = 0.2f;

        public RectTransform RectTransform { get; private set; }

        private CanvasGroup canvasGroup;

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();

            if (!canvasGroup)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        public void Setup(string message, Color backgroundColor, Sprite icon = null)
        {
            messageText.text = message;

            if (iconImage)
            {
                iconImage.gameObject.SetActive(icon);
                if (icon) iconImage.sprite = icon;
            }

            if (backgroundImage)
            {
                backgroundImage.color = backgroundColor;
            }

            Show();
        }

        private void Show()
        {
            canvasGroup.alpha = 0f;
            transform.localScale = Vector3.one * 0.8f;

            Tween.Alpha(canvasGroup, 1f, showDuration, Ease.OutCubic);
            Tween.Scale(transform, Vector3.one, showDuration, Ease.OutBack);
        }

        public void Hide(Action onComplete = null)
        {
            Tween.Alpha(canvasGroup, 0f, hideDuration, Ease.InCubic)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}