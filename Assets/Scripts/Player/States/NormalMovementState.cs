using FishingVillage.Gameplay;
using UnityEngine;

namespace FishingVillage.Player
{
    public class NormalMovementState : MovementState
    {
        private bool _hitCeiling;
        private MovingPlatform _currentPlatform;
        private Vector3 _platformVelocity;
        private float _coyoteTimer;

        public override PlayerState Type => PlayerState.Normal;

        public NormalMovementState(PlayerController context) : base(context) { }

        public override void Enter()
        {
            ctx.Controller.enabled = true;
            _coyoteTimer = 0; 
        }

        public override void Update()
        {
            if (ctx.isGrounded)
            {
                _coyoteTimer = ctx.coyoteTime;
            }
            else if (_coyoteTimer > 0f)
            {
                _coyoteTimer -= Time.deltaTime;
            }

            if (ctx.Input.MoveInput != Vector2.zero && ctx.isGrounded)
            {
                GameEvents.WalkedAction();
            }
        }

        public override void FixedUpdate()
        {
            CheckForPlatform();
            CheckCollisions();
            HandleGravity();
            HandleJump();
            HandleMovement();
        }

        private void HandleJump()
        {
            if (ctx.jumpBufferTimer > 0 && (ctx.isGrounded || _coyoteTimer > 0))
            {
                Vector3 vel = ctx.velocity;
                vel.y = ctx.jumpForce;
                ctx.velocity = vel;
                
                ctx.jumpBufferTimer = 0;
                _coyoteTimer = 0;
                GameEvents.JumpedAction();
            }
        }
        
        private void HandleMovement()
        {
            Vector3 vel = ctx.velocity;
            vel.x = ctx.Input.MoveInput.x * ctx.moveSpeed;
            vel.z = ctx.Input.MoveInput.y * ctx.moveSpeed;
            ctx.velocity = vel;

            Vector3 finalVelocity = ctx.velocity + _platformVelocity;
            ctx.Controller.Move(finalVelocity * Time.fixedDeltaTime);
        }

        private void HandleGravity()
        {
            Vector3 vel = ctx.velocity;
            
            if (ctx.isGrounded && vel.y < 0)
            {
                vel.y = -2f;
            }
            else
            {
                vel.y -= ctx.gravity;
                if (vel.y < -ctx.maxFallSpeed)
                {
                    vel.y = -ctx.maxFallSpeed;
                }
            }
            ctx.velocity = vel;
        }

        private void CheckCollisions()
        {
            ctx.isGrounded = Physics.CheckSphere(ctx.transform.position + ctx.groundCheckOffset, ctx.groundCheckRadius, ctx.collisionLayer, QueryTriggerInteraction.Ignore);
            
            if (ctx.velocity.y > 0)
            {
                _hitCeiling = Physics.CheckSphere(ctx.transform.position + ctx.ceilingCheckOffset, ctx.ceilingCheckRadius, ctx.collisionLayer, QueryTriggerInteraction.Ignore);
                if (_hitCeiling)
                {
                    Vector3 vel = ctx.velocity;
                    vel.y = 0;
                    ctx.velocity = vel;
                }
            }
        }

        private void CheckForPlatform()
        {
            if (!ctx.isGrounded)
            {
                _currentPlatform = null;
                _platformVelocity = Vector3.zero;
                return;
            }

            var colliders = Physics.OverlapSphere(ctx.transform.position + ctx.groundCheckOffset, ctx.groundCheckRadius, ctx.collisionLayer, QueryTriggerInteraction.Ignore);
            
            foreach (var col in colliders)
            {
                if (col.TryGetComponent(out MovingPlatform platform))
                {
                    _currentPlatform = platform;
                    _platformVelocity = platform.Velocity;
                    return;
                }
            }

            _currentPlatform = null;
            _platformVelocity = Vector3.zero;
        }
    }
}