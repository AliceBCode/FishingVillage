using System;
using DNExtensions.Systems.MenuSystem;
using DNExtensions.Utilities.AutoGet;
using DNExtensions.Utilities.CustomFields;
using UnityEngine;
using UnityEngine.UI;
using Screen = DNExtensions.Systems.MenuSystem.Screen;

namespace FishingVillage.UI.Menus
{
    public class MainScreen : MonoBehaviour
    {

        [Header("Settings")]
        [SerializeField] private SceneField gameScene;
        [SerializeField] private Screen optionScreen;
        [SerializeField] private Screen creditsScreen;
        
        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;
        
        [SerializeField, HideInInspector, AutoGetScene] private MenuManager menuManager;


        private void Start()
        {
            playButton?.onClick.AddListener(OnPlayClicked);
            optionsButton?.onClick.AddListener(OnOptionsClicked);
            creditsButton?.onClick.AddListener(OnCreditsClicked);
            quitButton?.onClick.AddListener(OnQuitClicked);
        }

        private void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif

        }

        private void OnCreditsClicked()
        {
            menuManager?.ShowScreen(creditsScreen);
        }

        private void OnOptionsClicked()
        {
            menuManager?.ShowScreen(optionScreen);
        }

        private void OnPlayClicked()
        {
            gameScene?.LoadScene();
        }
    }
}
