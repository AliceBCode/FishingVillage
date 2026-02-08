using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.GameActions
{
    [Serializable]
    [MovedFrom("")]
    [SerializableSelectorName("Set State", "Player")]
    public class SetPlayerStateAction : GameAction
    {
        [SerializeField] private Player.PlayerState state = Player.PlayerState.Normal;

        public override string ActionName => $"Sets player state to: {state}";

        public override void Execute()
        {
            if (Player.PlayerController.Instance)
            {
                Player.PlayerController.Instance.SetState(state);
            }
        }
    }
}