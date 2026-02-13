using DNExtensions.Utilities;
using FishingVillage.Gameplay;
using FishingVillage.Interactable;
using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingVillage.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerControllerInput))]
    [RequireComponent(typeof(PlayerAnimator))]
    [SelectionBase]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance;

        [Header("Movement Settings")]
        public float moveSpeed = 10f;
        public float gravity = 1.5f;
        public float maxFallSpeed = 25f;
        public float jumpForce = 15f;
        public float jumpBufferTime = 0.2f;
        public float coyoteTime = 0.1f;

        [Header("Collision Settings")]
        public float ceilingCheckRadius = 0.1f;
        public Vector3 ceilingCheckOffset = Vector3.up;
        public float groundCheckRadius = 0.31f;
        public Vector3 groundCheckOffset = Vector3.down;
        public LayerMask collisionLayer;

        [Separator]
        [ReadOnly] public bool isGrounded;
        [ReadOnly] public float jumpBufferTimer;
        [ReadOnly] public Vector3 velocity;

        
        private PlayerAnimator _animator;
        private MovementState _currentState;
        private NormalMovementState _normalState;
        private ConstrainedMovementState _constrainedState;
        private LockedMovementState _lockedState;

        
        
        public CharacterController Controller { get; private set; }

        public PlayerControllerInput Input { get; private set; }


        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;

            Controller = GetComponent<CharacterController>();
            Input = GetComponent<PlayerControllerInput>();
            _animator = GetComponent<PlayerAnimator>();

            _normalState = new NormalMovementState(this);
            _constrainedState = new ConstrainedMovementState(this);
            _lockedState = new LockedMovementState(this);
        }

        private void Start()
        {
            SwitchState(_normalState);
        }

        private void OnEnable()
        {
            Input.OnJumpAction += OnJumpAction;
            BlockedMovementAnimationBehavior.OnStateExited += BlockedMovementBehaviorExited;
        }

        private void OnDisable()
        {
            Input.OnJumpAction -= OnJumpAction;
            BlockedMovementAnimationBehavior.OnStateExited -= BlockedMovementBehaviorExited;
        }

        private void BlockedMovementBehaviorExited()
        {
            SwitchState(_normalState);
        }

        private void OnJumpAction(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                jumpBufferTimer = jumpBufferTime;
            }
        }

        private void Update()
        {
            if (jumpBufferTimer > 0f) jumpBufferTimer -= Time.deltaTime;
            _currentState?.Update();
        }

        private void FixedUpdate()
        {
            _currentState?.FixedUpdate();
        }
        
        private void SwitchState(MovementState newState, string animTrigger = "")
        {
            if (_currentState == newState && string.IsNullOrEmpty(animTrigger)) return;

            if (_currentState != newState)
            {
                _currentState?.Exit();
                _currentState = newState;
                _currentState.Enter();
                GameEvents.PlayerStateChanged(_currentState.Type);
            }

            if (!string.IsNullOrEmpty(animTrigger))
            {
                _animator.TriggerAnimation(animTrigger);
            }
        }

        public void SetNormal(string animTrigger = "")
        {
            SwitchState(_normalState, animTrigger);
        }

        public void SetLocked(string animTrigger = "")
        {
            SwitchState(_lockedState, animTrigger);
        }

        public void AttachToPath(ConstrainableRopePath ropePath)
        {
            _constrainedState.SetPath(ropePath);
            SwitchState(_constrainedState);
        }
        
        public void ForceJump(float force)
        {
            if (!Controller.enabled) return;
            
            velocity = new Vector3(velocity.x, force, velocity.z);
        }
        

        public bool CanInteract()
        {
            return _currentState.Type is PlayerState.Normal or PlayerState.Constrained;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);

            if (velocity.y > 0)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position + ceilingCheckOffset, ceilingCheckRadius);
            }
        }
    }
}