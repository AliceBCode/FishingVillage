using System;
using UnityEngine;

namespace FishingVillage.Player
{
    public enum TransitionMode { MoveTo, JumpTo }

    public class TransitionMovementState : MovementState
    {
        public override PlayerState Type => PlayerState.Locked;

        private Vector3 _target;
        private Action _onComplete;
        private TransitionMode _mode;

        private float _jumpProgress;
        private Vector3 _jumpStart;
        private float _jumpDuration;
        private float _jumpHeight;

        public TransitionMovementState(PlayerController context) : base(context) { }

        public void Set(Vector3 target, TransitionMode mode, Action onComplete = null, float jumpHeight = 3f, float jumpDuration = 0.5f)
        {
            _target = target;
            _onComplete = onComplete;
            _mode = mode;
            _jumpHeight = jumpHeight;
            _jumpDuration = jumpDuration;
        }

        public override void Enter()
        {
            ctx.velocity = Vector3.zero;

            if (_mode == TransitionMode.JumpTo)
            {
                _jumpStart = ctx.transform.position;
                _jumpProgress = 0f;
            }
        }

        public override void FixedUpdate()
        {
            if (_mode == TransitionMode.MoveTo)
            {
                HandleMoveTo();
            }
            else
            {
                HandleJumpTo();
            }
        }

        private void HandleMoveTo()
        {
            Vector3 delta = _target - ctx.transform.position;

            if (delta.magnitude <= 0.1f)
            {
                ctx.Controller.Move(delta);
                _onComplete?.Invoke();
                return;
            }

            ctx.Controller.Move(delta.normalized * (ctx.moveSpeed * Time.fixedDeltaTime));
        }

        private void HandleJumpTo()
        {
            _jumpProgress += Time.fixedDeltaTime / _jumpDuration;
            _jumpProgress = Mathf.Clamp01(_jumpProgress);

            Vector3 pos = Vector3.Lerp(_jumpStart, _target, _jumpProgress);
            pos.y += _jumpHeight * Mathf.Sin(_jumpProgress * Mathf.PI);

            Vector3 delta = pos - ctx.transform.position;
            ctx.Controller.Move(delta);

            if (_jumpProgress >= 1f)
            {
                _onComplete?.Invoke();
            }
        }
    }
}