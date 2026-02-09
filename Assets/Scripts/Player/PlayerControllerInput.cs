using System;
using DNExtensions.InputSystem;
using DNExtensions.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingVillage.Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerControllerInput : InputReaderBase
{
    [SerializeField, ReadOnly] private Vector2 moveInput;
    
    private InputActionMap _playerActionMap;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _interactAction;
    private InputAction _useAction;
    private InputAction _cycleItemsAction;
    
    public Vector2 MoveInput => moveInput;
    
    public event Action<InputAction.CallbackContext> OnJumpAction;
    public event Action<InputAction.CallbackContext> OnInteractAction;
    public event Action<InputAction.CallbackContext> OnUseAction;
    public event Action<InputAction.CallbackContext> OnCycleItemsAction;

    private void Awake()
    {
        _playerActionMap = PlayerInput.actions.FindActionMap("Player");

        if (_playerActionMap == null)
        {
            Debug.LogError("Player Action Map not found. Please check the action maps in the Player Input component.");
            return;
        }

        _moveAction = _playerActionMap.FindAction("Move");
        _jumpAction = _playerActionMap.FindAction("Jump");
        _interactAction = _playerActionMap.FindAction("Interact");
        _useAction = _playerActionMap.FindAction("Use");
        _cycleItemsAction = _playerActionMap.FindAction("CycleItems");
        
        if (_moveAction == null) Debug.LogError("Move action not found in Player Action Map.");
        if (_jumpAction == null) Debug.LogError("Jump action not found in Player Action Map.");
        if (_interactAction == null) Debug.LogError("Interact action not found in Player Action Map.");
        if (_useAction == null) Debug.LogError("Use action not found in Player Action Map.");
        if (_cycleItemsAction == null) Debug.LogError("Cycle Items action not found in Player Action Map.");
        
        _playerActionMap.Enable();
    }

    private void OnEnable()
    {
        SubscribeToAction(_moveAction, OnMove);
        SubscribeToAction(_jumpAction, OnJump);
        SubscribeToAction(_interactAction, OnInteract);
        SubscribeToAction(_useAction, OnUse);
        SubscribeToAction(_cycleItemsAction, OnCycleItems);
    }

    private void OnDisable()
    {
        UnsubscribeFromAction(_moveAction, OnMove);
        UnsubscribeFromAction(_jumpAction, OnJump);
        UnsubscribeFromAction(_interactAction, OnInteract);
        UnsubscribeFromAction(_useAction, OnUse);
        UnsubscribeFromAction(_cycleItemsAction, OnCycleItems);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    
    private void OnInteract(InputAction.CallbackContext context)
    {
        
        OnInteractAction?.Invoke(context);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        
        OnJumpAction?.Invoke(context);
    }
    
    private void OnUse(InputAction.CallbackContext context)
    {
        OnUseAction?.Invoke(context);
    }
    
    private void OnCycleItems(InputAction.CallbackContext context)
    {
        
        OnCycleItemsAction?.Invoke(context);
    }
    }
}