using System;
using DNExtensions;
using DNExtensions.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;



public class GameMenusInput : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    
    private InputActionMap _uiActionMap;
    private InputAction _toggleInventoryAction;
    private InputAction _toggleMapAction;
    private InputAction _pauseAction;
    
    
    public event Action<InputAction.CallbackContext> OnToggleInventoryAction;
    public event Action<InputAction.CallbackContext> OnToggleMapAction;
    public event Action<InputAction.CallbackContext> OnPauseAction;


    private void Awake()
    {
        _uiActionMap = playerInput.actions.FindActionMap("UI");

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
    
    
    


    /// <summary>
    /// Subscribes a callback method to all phases of an InputAction (started, performed, canceled).
    /// </summary>
    /// <param name="action">The InputAction to subscribe to. If null, no subscription occurs.</param>
    /// <param name="callback">The callback method to invoke for all action phases.</param>
    private void SubscribeToAction(InputAction action, Action<InputAction.CallbackContext> callback)
    {
        if (action == null)
        {
            Debug.LogError($"No action was found!");
            return;
        }

        action.performed += callback;
        action.started += callback;
        action.canceled += callback;
    }

    /// <summary>
    /// Unsubscribes a callback method from all phases of an InputAction (started, performed, canceled).
    /// </summary>
    /// <param name="action">The InputAction to unsubscribe from. If null, no unsubscription occurs.</param>
    /// <param name="callback">The callback method to remove from all action phases.</param>
    private void UnsubscribeFromAction(InputAction action, Action<InputAction.CallbackContext> callback)
    {
        if (action == null)
        {
            Debug.LogError($"No action was found!");
            return;
        }

        action.performed -= callback;
        action.started -= callback;
        action.canceled -= callback;
    }
}