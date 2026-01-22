using DNExtensions;
using DNExtensions.SerializedInterface;
using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerControllerInput))]
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
    
    [Header("Interaction Settings")]
    [SerializeField] private bool canInteractWhileAirborne = true;
    [SerializeField] private float interactCheckRange = 3f;
    [SerializeField] private Vector3 interactCheckOffset = Vector3.zero;
    [SerializeField] private LayerMask interactableLayer;
    
    
    [Separator]
    [SerializeField, ReadOnly] private Vector2 moveInput;
    [SerializeField, ReadOnly] private Vector3 velocity;
    [SerializeField, ReadOnly] private bool isGrounded;
    [SerializeField, ReadOnly] private bool jumpInput;
    [SerializeField, ReadOnly] private float jumpBufferTimer;
    [SerializeField, ReadOnly] private float coyoteTimer;
    [SerializeField, ReadOnly] private InterfaceReference<IInteractable> closetInteractable;
    [SerializeField, ReadOnly] private MovingPlatform currentPlatform;
    [SerializeField, ReadOnly] private SOItem equippedItem;
    [SerializeField] private Inventory inventory = new Inventory();

    
    private CharacterController _controller;
    private PlayerControllerInput _input;
    private Vector3 platformVelocity;
    private bool _allowControl = true;
    
    private bool CanInteract => canInteractWhileAirborne || isGrounded;

    
    
    public Inventory Inventory => inventory;
    public Vector2 MoveInput => moveInput;
    public SOItem EquippedItem => equippedItem;
    

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
        
        inventory.OnItemAdded += OnItemAdded;
        inventory.OnItemRemoved += OnItemRemoved;
    }

    private void Start()
    {
        GameEvents.InventoryChanged(Inventory);
    }


    private void OnEnable()
    {
        _input.OnMoveAction += OnMoveAction;
        _input.OnJumpAction += OnJumpAction;
        _input.OnInteractAction += OnInteractAction;
        _input.OnUseAction += OnUseAction;
        _input.OnCycleItemsAction += OnCycleItemsAction;
        CleaningAnimationBehavior.OnStateEntered += CleaningAnimationBehaviorOnOnStateEntered;
        CleaningAnimationBehavior.OnStateExited += CleaningAnimationBehaviorOnOnStateExited;
    }
    

    private void OnDisable()
    {
        _input.OnMoveAction -= OnMoveAction;
        _input.OnJumpAction -= OnJumpAction;
        _input.OnInteractAction -= OnInteractAction;
        _input.OnUseAction -= OnUseAction;
        _input.OnCycleItemsAction -= OnCycleItemsAction;
        CleaningAnimationBehavior.OnStateExited -= CleaningAnimationBehaviorOnOnStateExited;
    }
    
    private void CleaningAnimationBehaviorOnOnStateEntered()
    {
        _allowControl = false;
        moveInput = Vector2.zero;
        velocity = Vector3.zero;
    }
    
    private void CleaningAnimationBehaviorOnOnStateExited()
    {
        _allowControl = true;
    }
    
    private void OnItemRemoved(SOItem item)
    {
        if (equippedItem == item)
        {
            equippedItem = null;
            GameEvents.ItemEquipped(equippedItem);
        }

        GameEvents.ItemRemoved(item);
        GameEvents.InventoryChanged(Inventory);
    }

    private void OnItemAdded(SOItem item)
    {
        GameEvents.ItemObtained(item);
        
        if (!equippedItem && item.Usable)
        {
            equippedItem = item;
            GameEvents.ItemEquipped(equippedItem);
        }
        
        GameEvents.InventoryChanged(Inventory);
    }


    private void OnInteractAction(InputAction.CallbackContext context)
    {
        if (!_allowControl) return;
        
        if (CanInteract && context.performed && closetInteractable.Value != null)
        {
            closetInteractable.Value.Interact();
        }
    }

    private void OnJumpAction(InputAction.CallbackContext context)
    {
        if (!_allowControl) return;
        
        if (context.performed)
        {
            jumpInput = true;
            jumpBufferTimer = jumpBufferTime;
        }
        else if (context.canceled)
        {
            jumpInput = false;
        }
    }

    private void OnMoveAction(InputAction.CallbackContext context)
    {
        if (!_allowControl) return;
        
        moveInput = context.ReadValue<Vector2>();

        if (context.started && isGrounded)
        {
            GameEvents.WalkedAction();
        }
    }
    
    private void OnUseAction(InputAction.CallbackContext context)
    {
        if (!_allowControl) return;
        
        if (context.started)
        {
            equippedItem?.Use();
        }
    }

    private void OnCycleItemsAction(InputAction.CallbackContext context)
    {
        if (!_allowControl) return;
        
        if (!context.performed || Inventory.IsEmpty) return;

        var usableItems = inventory.UsableItems;
        if (usableItems.Count == 0) return;

        int currentIndex = equippedItem ? usableItems.IndexOf(equippedItem) : -1;
        int cycleDir = Mathf.RoundToInt(context.ReadValue<float>());
    
        if (cycleDir == 0) return;
    
        int nextIndex = currentIndex + cycleDir;
    
        if (nextIndex < 0)
        {
            nextIndex = usableItems.Count - 1;
        }
        else if (nextIndex >= usableItems.Count)
        {
            nextIndex = 0;
        }
    
        equippedItem = usableItems[nextIndex];
        GameEvents.ItemEquipped(equippedItem);
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
    }

    private void FixedUpdate()
    {
        CheckCollisions();
        CheckForInteractable();
        CheckForPlatform();
        HandleGravity();
        HandleJump();
        HandleMovement();   
    }

    private void HandleMovement()
    {
        if (!_controller || !_controller.enabled) return;
        
        velocity.x = moveInput.x * moveSpeed;
        velocity.z = moveInput.y * moveSpeed;
    
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
        if ((jumpInput || jumpBufferTimer > 0) && (isGrounded || coyoteTimer > 0))
        {
            velocity.y = jumpForce;
            jumpInput = false;
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
            bool hitCeiling = Physics.CheckSphere(transform.position + ceilingCheckOffset, ceilingCheckRadius, collisionLayer, QueryTriggerInteraction.Ignore);
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

    private void CheckForInteractable()
    {
        var colliders = Physics.OverlapSphere(transform.position + interactCheckOffset, interactCheckRange, interactableLayer, QueryTriggerInteraction.Ignore);
        var closestDistance = float.MaxValue;
        IInteractable closest = null;

        foreach (var col in colliders)
        {
            if (col.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = interactable;
                }
            }
        }
        
        closetInteractable.Value = closest;
    }
    
    public void ForceJump(float force)
    {
        if (!_controller || !_controller.enabled) return;
        
        velocity.y = force;
        jumpInput = false;
    }
    
    
    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
        Gizmos.DrawWireSphere(transform.position + ceilingCheckOffset, ceilingCheckRadius);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + interactCheckOffset, interactCheckRange);
    }
}