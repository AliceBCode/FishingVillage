

using DNExtensions.MenuSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using Screen = DNExtensions.MenuSystem.Screen;


namespace FishingVillage.UI.Menus
{
    

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
        private InputActionMap _playerActionMap;

        private bool _menuActive;


        private void Awake()
        {
            _input = GetComponent<GameMenusInput>();
            _menuManager = GetComponent<MenuManager>();
            _playerActionMap = FindFirstObjectByType<PlayerInput>().actions.FindActionMap("Player");
        }

        private void OnEnable()
        {
            _input.OnToggleInventoryAction += OnToggleInventory;
            _input.OnToggleMapAction += OnToggleMap;
            _input.OnPauseAction += OnPause;
        }

        private void OnDisable()
        {
            _input.OnToggleInventoryAction -= OnToggleInventory;
            _input.OnToggleMapAction -= OnToggleMap;
            _input.OnPauseAction -= OnPause;
        }

        private void OnPause(InputAction.CallbackContext context)
        {
            if (!context.performed || !_menuActive) return;
            
            CloseMenus();
        }

        private void CloseMenus()
        {
            _menuActive = false;
            _menuManager.HideCurrentScreen();
            mapPrompt.ShowDefaultVisuals();
            inventoryPrompt.ShowDefaultVisuals();
            _playerActionMap.Enable();

        }

        private void OnToggleMap(InputAction.CallbackContext context)
        {
            if (!context.performed || !mapScreen) return;

            if (mapScreen.isActiveAndEnabled)
            {
                _menuActive = false;
                _menuManager.HideCurrentScreen();
                mapPrompt.ShowDefaultVisuals();
                _playerActionMap.Enable();
            }
            else
            {

                _menuActive = true;
                _menuManager.ShowScreen(mapScreen);
                mapPrompt.ShowMenuVisuals();
                inventoryPrompt.ShowDefaultVisuals();
                _playerActionMap.Disable();
            }
        }

        private void OnToggleInventory(InputAction.CallbackContext context)
        {
            if (!context.performed || !inventoryScreen) return;

            if (inventoryScreen.isActiveAndEnabled)
            {

                _menuActive = false;
                _menuManager.HideCurrentScreen();
                inventoryPrompt.ShowDefaultVisuals();
                _playerActionMap.Enable();
            }
            else
            {

                _menuActive = true;
                _menuManager.ShowScreen(inventoryScreen);
                inventoryPrompt.ShowMenuVisuals();
                mapPrompt.ShowDefaultVisuals();
                _playerActionMap.Disable();
            }
        }
    }
}