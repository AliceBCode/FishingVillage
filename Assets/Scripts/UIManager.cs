using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
        public static UIManager Instance;

        [SerializeField] private PlayerController player;
        [SerializeField] private TextMeshProUGUI text;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
        }


        private void Start()
        {
            if (player)
            {
                player.Inventory.OnInventoryChanged += UpdateInventoryUI;
                UpdateInventoryUI(player.Inventory);
            }
        }

        private void OnDestroy()
        {
            if (player)
            {
                player.Inventory.OnInventoryChanged -= UpdateInventoryUI;
            }
        }

        private void UpdateInventoryUI(Inventory inventory)
        {
            
            if (!text) return;
            
            text.text = "Inventory:\n";
            foreach (var item in inventory.Items)
            {
                text.text += item.Name + "\n";
            }
        }
}