using System;
using DNExtensions.Utilities.CustomFields;
using DNExtensions.Utilities.SerializableSelector;
using FishingVillage.Player;
using UnityEngine;

namespace FishingVillage.GameActions
{
    [Serializable]
    [SerializableSelectorName("Set Normal State", "Player")]
    public class SetNormalPlayerStateAction : GameAction
    {
        [SerializeField] private OptionalField<AnimatorTriggerField> setAnimation = new OptionalField<AnimatorTriggerField>(new AnimatorTriggerField("Player Animator Controller"));
        
        public override string ActionName => "Set Player Normal";

        public override void Execute()
        {
            if (PlayerController.Instance)
            {
                PlayerController.Instance.SetNormal(setAnimation.Value);
            }
        }
    }
}