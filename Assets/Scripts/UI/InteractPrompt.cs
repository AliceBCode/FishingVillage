using DNExtensions.Utilities;
using PrimeTween;
using TMPro;
using UnityEngine;

namespace FishingVillage.UI
{
    
    [SelectionBase]
    public class InteractPrompt : MonoBehaviour
    {
        public static InteractPrompt Instance;
        
        [Header("Settings")] 
        [Tooltip("Duration of the fade in/out animation")] 
        [SerializeField] private float fadeDuration = 0.5f;
        [Tooltip("Whether the speech bubble should rotate to face the camera")] 
        [SerializeField] private bool rotateToCamera = true;
        [SerializeField, EnableIf("rotateToCamera")]
        private float rotationSpeed = 25f;
        [Tooltip("Whether the speech bubble's scale should change based on its distance to the camera")]
        [SerializeField] private bool distanceToCameraAffectsScale = true;
        [SerializeField, MinMaxRange(1, 2), EnableIf("distanceToCameraAffectsScale")]
        private RangedFloat minMaxScaleMultiplier = new RangedFloat(1, 1.5f);
        [SerializeField, MinMaxRange(0, 50), EnableIf("distanceToCameraAffectsScale")]
        private RangedFloat minMaxDistance = new RangedFloat(5, 15);

        [Header("References")] 
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI textPrompt;

        private Camera _cam;
        private Vector3 _baseScale;
        private RectTransform _rectTransform;
        private Coroutine _hideCoroutine;
        private Sequence _fadeSequence;

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        
            Instance = this;
            
            Hide(false);
            _cam = Camera.main;
            _rectTransform = canvasGroup.transform as RectTransform;
            if (_rectTransform) _baseScale = _rectTransform.localScale;
        }

        private void OnDestroy()
        {
            if (_fadeSequence.isAlive)
            {
                _fadeSequence.Stop();
            }
        }

        private void Update()
        {
            RotateToCamera();
            ChangeSizeBasedOnDistance();
        }

        private void RotateToCamera()
        {
            if (!_cam || !rotateToCamera) return;

            Vector3 directionToCamera = _rectTransform.position - _cam.transform.position;
            directionToCamera.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
            _rectTransform.rotation =
                Quaternion.Slerp(_rectTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        private void ChangeSizeBasedOnDistance()
        {
            if (!_cam || !distanceToCameraAffectsScale) return;

            float distance = Vector3.Distance(_rectTransform.position, _cam.transform.position);
            float t = Mathf.InverseLerp(minMaxDistance.minValue, minMaxDistance.maxValue, distance);
            float scaleMultiplier = Mathf.Lerp(minMaxScaleMultiplier.minValue, minMaxScaleMultiplier.maxValue, t);
            _rectTransform.localScale = _baseScale * scaleMultiplier;
        }


        public void Show(Vector3 position)
        {
            if (_fadeSequence.isAlive)
            {
                _fadeSequence.Stop();
            }
            
            _rectTransform.position = position;
            

            _fadeSequence = Sequence.Create();
            _fadeSequence.Group(Tween.Alpha(canvasGroup, 1f, fadeDuration));
        }
        
        

        public void Hide(bool animate)
        {
            if (_fadeSequence.isAlive)
            {
                _fadeSequence.Stop();
            }

            if (animate)
            {
                _fadeSequence = Sequence.Create();
                _fadeSequence.Group(Tween.Alpha(canvasGroup, 0f, fadeDuration));
            }
            else
            {
                
                canvasGroup.alpha = 0f;
            }
        }

        public void UpdatePosition(Vector3 position)
        {
            _rectTransform.position = position;
        }
    }

}