using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public enum SlotType
{
    Previous,
    Current,
    Next,
    Overflow,
    Hidden
}

public class SelectionWheelItem : MonoBehaviour
{
    [Header("Use")]
    [SerializeField] private float usedPunchStrength = 1.5f;
    [SerializeField] private float usedPunchDuration = 0.25f;
    
    [Header("Transition")]
    [SerializeField] private float transitionDuration = 0.3f;
    [SerializeField] private Ease transitionEase = Ease.OutCubic;

    private RectTransform _rectTransform;
    
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
    


}