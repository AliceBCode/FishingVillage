using DNExtensions.Utilities;
using DNExtensions.Utilities.SerializableSelector;
using FishingVillage.GameActions;
using UnityEngine;
using UnityEngine.Audio;

namespace FishingVillage
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Item")]
    public class SOItem : ScriptableObject
    {
        [Header("Settings")]
        [SerializeField] private new string name;
        [SerializeField, TextArea] private string description;
        [SerializeField, Preview] private Sprite icon;
        [SerializeField] private bool usable;
        [SerializeField, ShowIf("usable")] private AudioResource useSfx;
        [SerializeReference, SerializableSelector, ShowIf("usable")] private GameAction[] actionsOnUse;


        public string Name => name;
        public string Description => description;
        public Sprite Icon => icon;
        public bool Usable => usable;
        public AudioResource UseSfx => useSfx;


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