using System;
using DNExtensions.Utilities.CustomFields;
using DNExtensions.Utilities.SerializableSelector;
using FishingVillage.Player;
using UnityEngine;

namespace FishingVillage.GameActions
{
    [Serializable]
    [SerializableSelectorName("Set Locked State", "Player")]
    public class LockPlayerStateAction : GameAction
    {
        [SerializeField] private OptionalField<AnimatorParameterField> setAnimation = new OptionalField<AnimatorParameterField>(new AnimatorParameterField(AnimatorParameterType.Trigger, AnimatorSource.Asset));
        
        public override string ActionName => "Set Player State to Locked";

        public override void Execute()
        {
            PlayerController.Instance?.SetLocked(setAnimation.Value);
        }
    }
}