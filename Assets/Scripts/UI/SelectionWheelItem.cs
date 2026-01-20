namespace UI
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class SelectionWheelItem : MonoBehaviour
    {
        [SerializeField] private Image image;

        private RectTransform _rectTransform;
        public RectTransform RectTransform => _rectTransform ??= GetComponent<RectTransform>();
        public Image Image => image;

        public void SetAlpha(float alpha)
        {
            Color c = image.color;
            c.a = alpha;
            image.color = c;
        }
    }
}