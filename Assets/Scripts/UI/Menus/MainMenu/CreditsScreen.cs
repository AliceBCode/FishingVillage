
using DNExtensions.Systems.MenuSystem;
using DNExtensions.Utilities.AutoGet;
using DNExtensions.Utilities.CustomFields;
using UnityEngine;
using UnityEngine.UI;
using Screen = DNExtensions.Systems.MenuSystem.Screen;

namespace FishingVillage.UI.Menus
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Screen))]
    public class CreditsScreen : MonoBehaviour
    {
        
        [SerializeField] private Screen mainMenuScreen;
        [SerializeField] private Button backButton;
        
        [SerializeField, HideInInspector, AutoGetScene] private MenuManager menuManager;
        [SerializeField, HideInInspector, AutoGetSelf] private Screen screen;

        private void Awake()
        {
            backButton?.onClick.AddListener(OnBackClicked);

        }
        
        private void OnBackClicked()
        {
            menuManager?.ShowScreen(mainMenuScreen);
        }
    }
}
