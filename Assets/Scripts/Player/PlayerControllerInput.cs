using System;
using DNExtensions.Systems.InputSystem;
using DNExtensions.Utilities;
using FishingVillage.Gameplay;
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
        private InputAction _toggleInventoryAction;
        private InputAction _toggleMapAction;
        private InputAction _pauseAction;

        public Vector2 MoveInput => moveInput;

        public event Action<InputAction.CallbackContext> OnJumpAction;
        public event Action<InputAction.CallbackContext> OnInteractAction;
        public event Action<InputAction.CallbackContext> OnUseAction;
        public event Action<InputAction.CallbackContext> OnCycleItemsAction;
        public event Action<InputAction.CallbackContext> OnToggleInventoryAction;
        public event Action<InputAction.CallbackContext> OnToggleMapAction;
        public event Action<InputAction.CallbackContext> OnPauseAction;

        private void Awake()
        {
            _playerActionMap = PlayerInput.actions.FindActionMap("Player");

            if (_playerActionMap == null)
            {
                Debug.LogError("Player Action Map not found.");
                return;
            }

            _moveAction = _playerActionMap.FindAction("Move");
            _jumpAction = _playerActionMap.FindAction("Jump");
            _interactAction = _playerActionMap.FindAction("Interact");
            _useAction = _playerActionMap.FindAction("Use");
            _cycleItemsAction = _playerActionMap.FindAction("CycleItems");
            _toggleInventoryAction = _playerActionMap.FindAction("ToggleInventory");
            _toggleMapAction = _playerActionMap.FindAction("ToggleMap");
            _pauseAction = _playerActionMap.FindAction("TogglePause");

            if (_moveAction == null) Debug.LogError("Move action not found.");
            if (_jumpAction == null) Debug.LogError("Jump action not found.");
            if (_interactAction == null) Debug.LogError("Interact action not found.");
            if (_useAction == null) Debug.LogError("Use action not found.");
            if (_cycleItemsAction == null) Debug.LogError("CycleItems action not found.");
            if (_toggleInventoryAction == null) Debug.LogError("ToggleInventory action not found.");
            if (_toggleMapAction == null) Debug.LogError("ToggleMap action not found.");
            if (_pauseAction == null) Debug.LogError("Pause action not found.");

            _playerActionMap.Enable();
        }

        private void OnEnable()
        {
            SubscribeToAction(_moveAction, OnMove);
            SubscribeToAction(_jumpAction, OnJump);
            SubscribeToAction(_interactAction, OnInteract);
            SubscribeToAction(_useAction, OnUse);
            SubscribeToAction(_cycleItemsAction, OnCycleItems);
            SubscribeToAction(_toggleInventoryAction, OnToggleInventory);
            SubscribeToAction(_toggleMapAction, OnToggleMap);
            SubscribeToAction(_pauseAction, OnPause);
            
            GameEvents.OnMenuOpened += DisableGameplayInput;
            GameEvents.OnMenuClosed += EnableGameplayInput;
        }

        private void OnDisable()
        {
            UnsubscribeFromAction(_moveAction, OnMove);
            UnsubscribeFromAction(_jumpAction, OnJump);
            UnsubscribeFromAction(_interactAction, OnInteract);
            UnsubscribeFromAction(_useAction, OnUse);
            UnsubscribeFromAction(_cycleItemsAction, OnCycleItems);
            UnsubscribeFromAction(_toggleInventoryAction, OnToggleInventory);
            UnsubscribeFromAction(_toggleMapAction, OnToggleMap);
            UnsubscribeFromAction(_pauseAction, OnPause);
            
            GameEvents.OnMenuOpened -= DisableGameplayInput;
            GameEvents.OnMenuClosed -= EnableGameplayInput;
        }
        
        private void EnableGameplayInput()
        {
            _moveAction?.Enable();
            _jumpAction?.Enable();
            _interactAction?.Enable();
            _useAction?.Enable();
            _cycleItemsAction?.Enable();
        }

        private void DisableGameplayInput()
        {
            _moveAction?.Disable();
            _jumpAction?.Disable();
            _interactAction?.Disable();
            _useAction?.Disable();
            _cycleItemsAction?.Disable();
        }

        private void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();
        private void OnJump(InputAction.CallbackContext context) => OnJumpAction?.Invoke(context);
        private void OnInteract(InputAction.CallbackContext context) => OnInteractAction?.Invoke(context);
        private void OnUse(InputAction.CallbackContext context) => OnUseAction?.Invoke(context);
        private void OnCycleItems(InputAction.CallbackContext context) => OnCycleItemsAction?.Invoke(context);
        private void OnToggleInventory(InputAction.CallbackContext context) => OnToggleInventoryAction?.Invoke(context);
        private void OnToggleMap(InputAction.CallbackContext context) => OnToggleMapAction?.Invoke(context);
        private void OnPause(InputAction.CallbackContext context) => OnPauseAction?.Invoke(context);
    }
}