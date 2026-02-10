using System;
using DNExtensions.Utilities.CustomFields;
using DNExtensions.Utilities.SerializableSelector;
using FishingVillage.Player;
using UnityEngine;

namespace FishingVillage.GameActions
{
    [Serializable]
    [SerializableSelectorName("Set Locked State", "Player")]
    public class LockPlayerAction : GameAction
    {
        [SerializeField] private OptionalField<AnimatorTriggerField> setAnimation = new OptionalField<AnimatorTriggerField>(new AnimatorTriggerField("Player Animator Controller"));
        
        public override string ActionName => "Lock Player Movement";

        public override void Execute()
        {
            PlayerController.Instance?.SetLocked(setAnimation.Value);
        }
    }
}