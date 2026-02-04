
using DNExtensions.Shapes;
using TMPro;
using UnityEngine;


namespace FishingVillage.UI.Menus
{
    

    public class GameMenuPrompt : MonoBehaviour
    {

        [Header("Settings")] [SerializeField] private Color menuColor = Color.dodgerBlue;

        [Header("References")] [SerializeField]
        private TextMeshProUGUI menuTextMesh;

        [SerializeField] private TextMeshProUGUI hudTextMesh;
        [SerializeField] private SDFCircle circle;


        private Color _hudColor = Color.clear;

        private void Awake()
        {
            _hudColor = circle.baseColor;
        }

        public void ShowMenuVisuals()
        {
            circle.baseColor = menuColor;
            menuTextMesh.gameObject.SetActive(true);
            hudTextMesh.gameObject.SetActive(false);
        }

        public void ShowDefaultVisuals()
        {
            circle.baseColor = _hudColor;
            menuTextMesh.gameObject.SetActive(false);
            hudTextMesh.gameObject.SetActive(true);

        }
    }
}