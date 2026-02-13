using System;
using DNExtensions.Utilities;
using DNExtensions.Utilities.SerializableSelector;
using FishingVillage.GameActions;
using UnityEngine;

namespace FishingVillage.Interactable
{
    [Serializable]
    public class NpcInteraction
    {
        [SerializeReference, SerializableSelector] private GameAction[] actions = Array.Empty<GameAction>();
        [SerializeField,ReadOnly] private bool consumed;
        
        public bool IsConsumed => consumed;

        public void Execute()
        {
            if (consumed) return;

            consumed = true;
            
            foreach (var action in actions)
            {
                action?.Execute();
            }
        }

        public void Reset()
        {
            consumed = false;
        }
    }
}