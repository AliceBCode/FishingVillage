namespace UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class InventoryPanelItem : MonoBehaviour
    {
        [SerializeField] private Image image;
    
        public Image Image => image;
    }
}