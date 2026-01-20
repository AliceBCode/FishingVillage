namespace UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class InventoryItem : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI text;
    
        public Image Image => image;
        public TextMeshProUGUI Text => text;
    }
}