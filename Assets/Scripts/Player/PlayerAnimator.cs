using System;
using DNExtensions;
using PrimeTween;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAnimator : MonoBehaviour
{

    [Header("Change Direction Animation")]
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private Ease ease = Ease.InOutCubic;

    [Header("References")] 
    [SerializeField] private Transform gfx;

    [SerializeField, ReadOnly] private bool facingLeft;

    private PlayerController _playerController;
    private Sequence _directionChangeSequence;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        
        gfx.eulerAngles = new Vector3(0f, 180f, 0f);
    }

    private void Update()
    {
        float currentXInput = _playerController.MoveInput.x;

        if (currentXInput < 0 && !facingLeft)
        {
            FlipSprite(true);
            facingLeft = true;
        }
        else if (currentXInput > 0 && facingLeft)
        {
            FlipSprite(false);
            facingLeft = false;
        }
    }

    private void FlipSprite(bool turnLeft)
    {
        if (_directionChangeSequence.isAlive)
        {
            _directionChangeSequence.Stop();
        }

        var startRotation = gfx.eulerAngles;
        var targetRotation = new Vector3(0, turnLeft ? 0 : 180f, 0);

        _directionChangeSequence = Sequence.Create();
        _directionChangeSequence.Group(Tween.LocalEulerAngles(gfx, startRotation,targetRotation, duration,ease));
        _directionChangeSequence.Group(Tween.PunchScale(gfx, Vector3.one*1.1f, duration*1.5f, 1));
    }
}