using System;
using DNExtensions.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingVillage.UI.Menus
{
    

    public class GameMenusInput : InputReaderBase
    {

        private InputActionMap _uiActionMap;
        private InputAction _toggleInventoryAction;
        private InputAction _toggleMapAction;
        private InputAction _pauseAction;


        public event Action<InputAction.CallbackContext> OnToggleInventoryAction;
        public event Action<InputAction.CallbackContext> OnToggleMapAction;
        public event Action<InputAction.CallbackContext> OnPauseAction;


        private void Awake()
        {
            _uiActionMap = PlayerInput.actions.FindActionMap("UI");

            if (_uiActionMap == null)
            {
                Debug.LogError("UI Action Map not found. Please check the action maps in the Player Input component.");
                return;
            }

            _toggleInventoryAction = _uiActionMap.FindAction("ToggleInventory");
            _toggleMapAction = _uiActionMap.FindAction("ToggleMap");
            _pauseAction = _uiActionMap.FindAction("Pause");

            if (_toggleInventoryAction == null) Debug.LogError("ToggleInventory action not found in UI Action Map.");
            if (_toggleMapAction == null) Debug.LogError("ToggleMap action not found in UI Action Map.");
            if (_pauseAction == null) Debug.LogError("Pause action not found in UI Action Map.");

            _uiActionMap.Enable();
        }

        private void OnEnable()
        {
            SubscribeToAction(_toggleInventoryAction, OnToggleInventory);
            SubscribeToAction(_toggleMapAction, OnToggleMap);
            SubscribeToAction(_pauseAction, OnPause);

        }

        private void OnDisable()
        {
            UnsubscribeFromAction(_toggleInventoryAction, OnToggleInventory);
            UnsubscribeFromAction(_toggleMapAction, OnToggleMap);
            UnsubscribeFromAction(_pauseAction, OnPause);

        }

        private void OnToggleInventory(InputAction.CallbackContext context)
        {
            OnToggleInventoryAction?.Invoke(context);
        }

        private void OnToggleMap(InputAction.CallbackContext context)
        {
            OnToggleMapAction?.Invoke(context);
        }

        private void OnPause(InputAction.CallbackContext context)
        {
            OnPauseAction?.Invoke(context);
        }

    }
}