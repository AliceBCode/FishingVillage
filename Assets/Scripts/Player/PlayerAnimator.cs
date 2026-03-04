using DNExtensions.Utilities;
using DNExtensions.Utilities.AutoGet;
using FishingVillage.Gameplay;
using PrimeTween;
using UnityEngine;

namespace FishingVillage.Player
{
    [RequireComponent(typeof(PlayerControllerInput))]
    public class PlayerAnimator : MonoBehaviour
    {
        [Header("Jump")] 
        [SerializeField] private float jumpDuration = 0.1f;

        [Header("Change Direction")]
        [SerializeField] private float directionThreshold = 0.1f;
        [SerializeField] private float directionDuration = 0.15f;
        [SerializeField] private Ease directionEase = Ease.InOutCubic;

        [Header("References")] 
        [SerializeField] private Transform modelTransform;
        
        
        [SerializeField, AutoGetSelf, HideInInspector] private Animator animator;
        private bool _facingLeft;
        private bool _facingUp;
        private bool _facingDown;
        private PlayerController _controller;
        private Sequence _rotationTween;

        private void Awake()
        {
            _controller = GetComponent<PlayerController>();
            modelTransform.eulerAngles = new Vector3(0f, 180f, 0f);
        }

        private void OnEnable()
        {
            GameEvents.OnJumpedAction += PlayJumpedActionAnimation;
            GameEvents.OnItemObtained += OnItemObtainedAnimation;
        }

        private void OnDisable()
        {
            GameEvents.OnJumpedAction -= PlayJumpedActionAnimation;
            GameEvents.OnItemObtained -= OnItemObtainedAnimation;
        }

        private void Update()
        {
            HandleHorizontalViewDirection();
            HandleVerticalViewDirection();
        }
        
        private void OnItemObtainedAnimation(SOItem item)
        {
            TriggerAnimation("Pickup");
        }
        
        private void PlayJumpedActionAnimation()
        {
            Tween.PunchScale(modelTransform, Vector3.one * 1.1f, jumpDuration, 1);
        }

        private void HandleVerticalViewDirection()
        {
            float currentYInput = _controller.velocity.z;
            bool shouldUpdate = false;

            if (currentYInput == 0f && (_facingUp || _facingDown))
            {
                _facingUp = false;
                _facingDown = false;
                shouldUpdate = true;
            } 
            else if (currentYInput > directionThreshold && !_facingUp)
            {
                _facingUp = true;
                _facingDown = false;
                shouldUpdate = true;
            }
            else if (currentYInput < -directionThreshold && !_facingDown)
            {
                _facingDown = true;
                _facingUp = false;
                shouldUpdate = true;
            }

            if (shouldUpdate)
            {
                AnimateRotation(false);
            }
        }

        private void HandleHorizontalViewDirection()
        {
            float currentXInput = _controller.velocity.x;

            if (currentXInput < 0 && !_facingLeft)
            {
                _facingLeft = true;
                AnimateRotation(true);
            }
            else if (currentXInput > 0 && _facingLeft)
            {
                _facingLeft = false;
                AnimateRotation(true);
            }
        } 

        private void AnimateRotation(bool withPunchScale)
        {
            if (_rotationTween.isAlive)
            {
                _rotationTween.Complete();
            }

            var targetRotation = GetTargetRotation();
            _rotationTween = Sequence.Create();
            
            if (withPunchScale)
            {
                _rotationTween.Group(Tween.LocalRotation(modelTransform, Quaternion.Euler(targetRotation), directionDuration, directionEase));
                _rotationTween.Group(Tween.PunchScale(modelTransform, Vector3.one * 1.1f, directionDuration * 1.5f, 1));
                
            }
            else
            {
                _rotationTween.Group(Tween.LocalRotation(modelTransform, Quaternion.Euler(targetRotation), directionDuration * 0.5f, directionEase));
            }
        }

        private Vector3 GetTargetRotation()
        {
            float horizontalAngle = _facingLeft ? 0f : 180f;
            float verticalAngle = _facingUp ? -30f : (_facingDown ? 30f : 0f);
            float angleMultiplier = _facingLeft ? -1f : 1f;
            
            return new Vector3(0f, horizontalAngle + (verticalAngle * angleMultiplier), 0f);
        }
        
        public void TriggerAnimation(string animTrigger)
        {
            if (string.IsNullOrEmpty(animTrigger)) return;
            
            animator.SetTrigger(animTrigger);
        }


    }
}