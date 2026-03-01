using DNExtensions.Utilities.AutoGet;
using FishingVillage.Gameplay;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace FishingVillage.UI
{
    
    [SelectionBase]
    public class PickupPrompt : MonoBehaviour
    {
        public static PickupPrompt Instance;
        
        [Header("Settings")] 
        [Tooltip("Duration of the fade in/out animation")] 
        [SerializeField] private float showDuration = 0.5f;
        [SerializeField, AutoGetChildren] private Image itemImage;
        [SerializeField, AutoGetSelf] private CanvasGroup canvasGroup;
        
        private RectTransform _rectTransform;
        private Sequence _fadeSequence;

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        
            Instance = this;
            
            canvasGroup.alpha = 0f;
            _rectTransform = canvasGroup.transform as RectTransform;
        }
        

        private void OnDestroy()
        {
            if (_fadeSequence.isAlive)
            {
                _fadeSequence.Stop();
            }
        }
        

        public void Show(Vector3 position, SOItem item)
        {
            if (_fadeSequence.isAlive)
            {
                _fadeSequence.Stop();
            }
            
            itemImage.sprite = item.Icon;
            _rectTransform.position = position;
            
            var fadeInDuration = showDuration * 0.5f;
            var delay = showDuration * 0.2f;
            var fadeOutDuration = showDuration * 0.3f;
            
            _fadeSequence = Sequence.Create();
            _fadeSequence.Group(Tween.Alpha(canvasGroup, 1f, fadeInDuration));
            _fadeSequence.ChainDelay(delay);
            _fadeSequence.Chain(Tween.Alpha(canvasGroup, 0f, fadeOutDuration));
        }
        

        public void UpdatePosition(Vector3 position)
        {
            _rectTransform.position = position;
        }
    }

}