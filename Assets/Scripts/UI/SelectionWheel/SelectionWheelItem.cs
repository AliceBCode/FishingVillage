

using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class SelectionWheelItem : MonoBehaviour
{
    [Header("Visual Feedback")]
    [SerializeField] private float usedPunchStrength = 1.5f;
    [SerializeField] private float usedPunchDuration = 0.25f;
    
    public RectTransform RectTransform { get; private set; }
    public Image Image { get; private set; }
    
    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        Image = GetComponent<Image>();
    }
    
    public void AnimateToPosition(Vector2 position, float scale, float alpha, float duration, Ease ease)
    {
        Tween.UIAnchoredPosition(RectTransform, position, duration, ease);
        Tween.Scale(RectTransform, scale, duration, ease);
        Tween.Alpha(Image, alpha, duration, ease);
    }
    
    public void SetPositionImmediate(Vector2 position, float scale, float alpha)
    {
        RectTransform.anchoredPosition = position;
        RectTransform.localScale = Vector3.one * scale;
        SetAlpha(alpha);
    }
    
    public void PlayUsedAnimation()
    {
        Tween.PunchScale(transform, Vector3.one * usedPunchStrength, usedPunchDuration, 1);
    }
    
    public void SetAlpha(float alpha)
    {
        var color = Image.color;
        color.a = alpha;
        Image.color = color;
    }
}