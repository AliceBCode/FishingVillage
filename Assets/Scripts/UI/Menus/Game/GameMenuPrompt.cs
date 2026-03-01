
using DNExtensions.Systems.Shapes;
using DNExtensions.Systems.Springs;
using DNExtensions.Utilities.AutoGet;
using TMPro;
using UnityEngine;


namespace FishingVillage.UI.Menus
{
    

    public class GameMenuPrompt : MonoBehaviour
    {

        [Header("Settings")] 
        [SerializeField] private Color menuColor = Color.dodgerBlue;

        [Header("References")] 
        [SerializeField] private TextMeshProUGUI menuTextMesh;
        [SerializeField] private TextMeshProUGUI hudTextMesh;
        [SerializeField] private SDFCircle circle;
        [SerializeField, AutoGetSelf] private RectSpring rectSpring;


        private Color _hudColor = Color.clear;

        private void Awake()
        {
            _hudColor = circle.baseColor;
        }

        public void EnablePrompt(bool state, bool animate = true)
        {
            if (animate)
            {
                if (state)
                {
                    rectSpring.AnimateFromOffset();
                }
                else
                {
                    rectSpring.AnimateToOffset();
                }
            }
            else
            {
                if (state)
                {
                    rectSpring.SnapToBase();
                }
                else
                {
                    rectSpring.SnapToOffset();
                }
            }

        }

        public void ShowMenuVisuals()
        {
            circle.baseColor = menuColor;
            menuTextMesh.gameObject.SetActive(true);
            hudTextMesh.gameObject.SetActive(false);
        }

        public void ShowGameplayVisuals()
        {
            circle.baseColor = _hudColor;
            menuTextMesh.gameObject.SetActive(false);
            hudTextMesh.gameObject.SetActive(true);

        }
    }
}