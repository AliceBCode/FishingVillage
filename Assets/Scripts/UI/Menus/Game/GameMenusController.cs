using DNExtensions.Systems.MenuSystem;
using FishingVillage.Gameplay;
using UnityEngine;
using Screen = DNExtensions.Systems.MenuSystem.Screen;

namespace FishingVillage.UI.Menus
{
    [RequireComponent(typeof(MenuManager))]
    public class GameMenusController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Screen mapScreen;
        [SerializeField] private Screen inventoryScreen;
        [SerializeField] private GameMenuPrompt backpackPrompt;
        [SerializeField] private GameMenuPrompt mapPrompt;

        private MenuManager _menuManager;

        private void Awake()
        {
            _menuManager = GetComponent<MenuManager>();
        }


        private void OnEnable()
        {
            GameEvents.OnToggleInventory += OnToggleInventory;
            GameEvents.OnToggleMap += OnToggleMap;
            GameEvents.OnMapEnabled += OnMapEnabled;
            GameEvents.OnBackpackEnabled += OnBackpackEnabled;
        }

        private void OnDisable()
        {
            GameEvents.OnToggleInventory -= OnToggleInventory;
            GameEvents.OnToggleMap -= OnToggleMap;
            GameEvents.OnMapEnabled -= OnMapEnabled;
            GameEvents.OnBackpackEnabled -= OnBackpackEnabled;
        }

        private void OnBackpackEnabled(bool state)
        {
            backpackPrompt.EnablePrompt(state);
        }

        private void OnMapEnabled(bool state)
        {
           mapPrompt.EnablePrompt(state);
        }
        


        private void CloseMenus()
        {
            _menuManager.HideCurrentScreen();
            mapPrompt.ShowGameplayVisuals();
            backpackPrompt.ShowGameplayVisuals();
        }

        private void OnToggleMap()
        {
            if (!mapScreen) return;

            if (mapScreen.isActiveAndEnabled)
            {
                _menuManager.HideCurrentScreen();
                mapPrompt.ShowGameplayVisuals();
                GameEvents.MenuClosed();
            }
            else
            {
                _menuManager.ShowScreen(mapScreen);
                mapPrompt.ShowMenuVisuals();
                backpackPrompt.ShowGameplayVisuals();
                GameEvents.MenuOpened();
            }
        }

        private void OnToggleInventory()
        {
            if (!inventoryScreen) return;

            if (inventoryScreen.isActiveAndEnabled)
            {
                _menuManager.HideCurrentScreen();
                backpackPrompt.ShowGameplayVisuals();
                GameEvents.MenuClosed();
            }
            else
            {
                _menuManager.ShowScreen(inventoryScreen);
                backpackPrompt.ShowMenuVisuals();
                mapPrompt.ShowGameplayVisuals();
                GameEvents.MenuOpened();
            }
        }
    }
}