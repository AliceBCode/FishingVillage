using DNExtensions;
using DNExtensions.Utilities;
using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerControllerInput))]
[RequireComponent(typeof(PlayerInventory))]
[SelectionBase]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float gravity = 1f;
    [SerializeField] private float maxFallSpeed = 25f;
    [SerializeField] private float jumpForce = 15f;
    [Tooltip("Time window after pressing jump to still perform a jump when landing.")]
    [SerializeField] private float jumpBufferTime = 0.2f;
    [Tooltip("Time window after leaving ground to still perform a jump.")]
    [SerializeField] private float coyoteTime = 0.1f;
    
    [Header("Collision Settings")]
    [SerializeField] private float ceilingCheckRadius = 0.1f;
    [SerializeField] private Vector3 ceilingCheckOffset = Vector3.up;
    [SerializeField] private float groundCheckRadius = 0.31f;
    [SerializeField] private Vector3 groundCheckOffset = Vector3.down;
    [SerializeField] private LayerMask collisionLayer;
    
    [Separator]
    [SerializeField, ReadOnly] private float jumpBufferTimer;
    [SerializeField, ReadOnly] private Vector3 velocity;
    [SerializeField, ReadOnly] private float coyoteTimer;
    [SerializeField, ReadOnly] private bool isGrounded;
    [SerializeField, ReadOnly] private bool hitCeiling;
    [SerializeField, ReadOnly] private MovingPlatform currentPlatform;
    [SerializeField, ReadOnly] private bool allowControl = true;

    private CharacterController _controller;
    private PlayerControllerInput _input;
    private Vector3 platformVelocity;
    
    public bool IsGrounded => isGrounded;
    public bool AllowControl => allowControl;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        PrimeTweenConfig.warnEndValueEqualsCurrent = false;
        
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerControllerInput>();
    }

    private void OnEnable()
    {
        _input.OnJumpAction += OnJumpAction;
        CleaningAnimationBehavior.OnStateEntered += CleaningAnimationBehaviorOnOnStateEntered;
        CleaningAnimationBehavior.OnStateExited += CleaningAnimationBehaviorOnOnStateExited;
    }

    private void OnDisable()
    {
        _input.OnJumpAction -= OnJumpAction;
        CleaningAnimationBehavior.OnStateEntered -= CleaningAnimationBehaviorOnOnStateEntered;
        CleaningAnimationBehavior.OnStateExited -= CleaningAnimationBehaviorOnOnStateExited;
    }
    
    private void CleaningAnimationBehaviorOnOnStateEntered()
    {
        allowControl = false;
        velocity = Vector3.zero;
    }
    
    private void CleaningAnimationBehaviorOnOnStateExited()
    {
        allowControl = true;
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
        if (jumpBufferTimer > 0f)
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else if (coyoteTimer > 0f)
        {
            coyoteTimer -= Time.deltaTime;
        }
        
        if (_input.MoveInput != Vector2.zero && isGrounded)
        {
            GameEvents.WalkedAction();
        }
    }

    private void FixedUpdate()
    {
        CheckCollisions();
        CheckForPlatform();
        HandleGravity();
        HandleJump();
        HandleMovement();   
    }

    private void HandleMovement()
    {
        if (!_controller || !_controller.enabled) return;
        
        velocity.x = _input.MoveInput.x * moveSpeed;
        velocity.z = _input.MoveInput.y * moveSpeed;
    
        Vector3 finalVelocity = velocity + platformVelocity;
        _controller.Move(finalVelocity * Time.fixedDeltaTime);
    }

    private void HandleGravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y -= gravity;
            if (velocity.y < -maxFallSpeed)
            {
                velocity.y = -maxFallSpeed;
            }
        }
    }

    private void HandleJump()
    {
        if ((_input.JumpPressed || jumpBufferTimer > 0) && (isGrounded || coyoteTimer > 0))
        {
            velocity.y = jumpForce;
            jumpBufferTimer = 0;
            coyoteTimer = 0;
            GameEvents.JumpedAction();
        }
    }

    private void CheckCollisions()
    {
        isGrounded = Physics.CheckSphere(transform.position + groundCheckOffset, groundCheckRadius, collisionLayer, QueryTriggerInteraction.Ignore);
        
        if (velocity.y > 0)
        {
            hitCeiling = Physics.CheckSphere(transform.position + ceilingCheckOffset, ceilingCheckRadius, collisionLayer, QueryTriggerInteraction.Ignore);
            if (hitCeiling)
            {
                velocity.y = 0;
            }
        }
    }

    private void CheckForPlatform()
    {
        if (!isGrounded)
        {
            currentPlatform = null;
            platformVelocity = Vector3.zero;
            return;
        }

        var colliders = Physics.OverlapSphere(transform.position + groundCheckOffset, groundCheckRadius, collisionLayer, QueryTriggerInteraction.Ignore);
        
        foreach (var col in colliders)
        {
            if (col.TryGetComponent(out MovingPlatform platform))
            {
                currentPlatform = platform;
                platformVelocity = platform.Velocity;
                return;
            }
        }

        currentPlatform = null;
        platformVelocity = Vector3.zero;
    }
    
    public void ForceJump(float force)
    {
        if (!_controller || !_controller.enabled) return;
        
        velocity.y = force;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);

        if (velocity.y > 0)
        {
            Gizmos.color = hitCeiling ? Color.red : Color.green;
            Gizmos.DrawWireSphere(transform.position + ceilingCheckOffset, ceilingCheckRadius);
        }
    }
}