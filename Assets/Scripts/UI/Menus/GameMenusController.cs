using DNExtensions.Utilities.Shapes;
using DNExtenstions.MenuSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Screen = DNExtenstions.MenuSystem.Screen;





[RequireComponent(typeof(GameMenusInput))]
[RequireComponent(typeof(MenuManager))]
public class GameMenusController : MonoBehaviour
{
    
    [Header("References")]
    [SerializeField] private Screen mapScreen;
    [SerializeField] private Screen inventoryScreen;
    [SerializeField] private GameMenuPrompt inventoryPrompt;
    [SerializeField] private GameMenuPrompt mapPrompt;
    
    
    private GameMenusInput _input;
    private MenuManager _menuManager;


    private void Awake()
    {
        _input = GetComponent<GameMenusInput>();
        _menuManager = GetComponent<MenuManager>();
    }

    private void OnEnable()
    {
        _input.OnToggleInventoryAction += OnToggleInventory;
        _input.OnToggleMapAction += OnToggleMap;
    }

    private void OnDisable()
    {
        _input.OnToggleInventoryAction -= OnToggleInventory;
        _input.OnToggleMapAction -= OnToggleMap;
    }

    private void OnToggleMap(InputAction.CallbackContext context)
    {
        if (!context.performed || !mapScreen) return;
        
        if (mapScreen.isActiveAndEnabled)
        {
            _menuManager.HideCurrentScreen();
            mapPrompt.ShowHud();
        }
        else
        {
            _menuManager.ShowScreen(mapScreen);
            mapPrompt.ShowMenu();
            inventoryPrompt.ShowHud();
        }
    }

    private void OnToggleInventory(InputAction.CallbackContext context)
    {
        if (!context.performed || !inventoryScreen) return;
        
        if (inventoryScreen.isActiveAndEnabled)
        {
            _menuManager.HideCurrentScreen();
            inventoryPrompt.ShowHud();
        }
        else
        {
            _menuManager.ShowScreen(inventoryScreen);
            inventoryPrompt.ShowMenu();
            mapPrompt.ShowHud();
        }
    }
}