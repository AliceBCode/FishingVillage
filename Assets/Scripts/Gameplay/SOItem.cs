using DNExtensions.Utilities;
using DNExtensions.Utilities.SerializableSelector;
using FishingVillage.GameActions;
using UnityEngine;

namespace FishingVillage.Gameplay
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Item")]
    public class SOItem : ScriptableObject
    {
        [Header("Information")]
        [SerializeField] private new string name;
        [SerializeField, TextArea] private string description;
        [SerializeField, Preview] private Sprite icon;
        
        [Header("Usage")]
        [SerializeField] private bool usable;
        [SerializeReference, SerializableSelector] private GameAction[] actionsOnUse;


        public string Name => name;
        public string Description => description;
        public Sprite Icon => icon;
        public bool Usable => usable;


        public void Use()
        {
            if (!usable) return;

            foreach (var action in actionsOnUse)
            {
                action?.Execute();
            }

            GameEvents.ItemUsed(this);
        }
    }
}