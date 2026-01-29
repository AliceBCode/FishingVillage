using System;
using DNExtensions;
using DNExtensions.Utilities;
using PrimeTween;
using UnityEngine;

[RequireComponent(typeof(PlayerControllerInput))]
public class PlayerAnimator : MonoBehaviour
{
    private static readonly int Cleaning = Animator.StringToHash("Cleaning");

    [Header("Jump Animation")] 
    [SerializeField] private float jumpDuration = 0.1f;

    [Header("Change Direction Animation")] 
    [SerializeField] private float directionDuration = 0.15f;
    [SerializeField] private Ease directionEase = Ease.InOutCubic;

    [Header("References")] 
    [SerializeField] private Transform modelTransform;
    [SerializeField] private Animator animator;
    [SerializeField, ReadOnly] private bool facingLeft;
    [SerializeField, ReadOnly] private bool facingUp;
    [SerializeField, ReadOnly] private bool facingDown;

    private PlayerControllerInput _input;
    private Sequence _rotationTween;

    private void Awake()
    {
        _input = GetComponent<PlayerControllerInput>();
        modelTransform.eulerAngles = new Vector3(0f, 180f, 0f);
    }

    private void OnEnable()
    {
        GameEvents.OnJumpedAction += PlayJumpedActionAnimation;
        GameEvents.OnCleaningUtensilsUsed += PlayCleaningAnimation;
    }

    private void OnDisable()
    {
        GameEvents.OnJumpedAction -= PlayJumpedActionAnimation;
        GameEvents.OnCleaningUtensilsUsed -= PlayCleaningAnimation;
    }

    private void Update()
    {
        HandleHorizontalViewDirection();
        HandleVerticalViewDirection();
    }

    private void PlayCleaningAnimation()
    {
        animator.SetTrigger(Cleaning);
    }
    
    private void PlayJumpedActionAnimation()
    {
        Tween.PunchScale(modelTransform, Vector3.one * 1.1f, jumpDuration, 1);
    }

    private void HandleVerticalViewDirection()
    {
        float currentYInput = _input.MoveInput.y;
        bool shouldUpdate = false;

        if (currentYInput == 0f && (facingUp || facingDown))
        {
            facingUp = false;
            facingDown = false;
            shouldUpdate = true;
        } 
        else if (currentYInput > 0f && !facingUp)
        {
            facingUp = true;
            facingDown = false;
            shouldUpdate = true;
        }
        else if (currentYInput < 0f && !facingDown)
        {
            facingDown = true;
            facingUp = false;
            shouldUpdate = true;
        }

        if (shouldUpdate)
        {
            AnimateRotation(false);
        }
    }

    private void HandleHorizontalViewDirection()
    {
        float currentXInput = _input.MoveInput.x;

        if (currentXInput < 0 && !facingLeft)
        {
            facingLeft = true;
            AnimateRotation(true);
        }
        else if (currentXInput > 0 && facingLeft)
        {
            facingLeft = false;
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
        float horizontalAngle = facingLeft ? 0f : 180f;
        float verticalAngle = facingUp ? -30f : (facingDown ? 30f : 0f);
        float angleMultiplier = facingLeft ? -1f : 1f;
        
        return new Vector3(0f, horizontalAngle + (verticalAngle * angleMultiplier), 0f);
    }
}