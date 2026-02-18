using System;
using DNExtensions.Systems.AudioLibrary;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;

namespace FishingVillage.GameActions
{
    [Serializable]
    [SerializableSelectorName("Play Audio", "Audio")]
    public class PlayAudioAction : GameAction
    {
        [SerializeField] private string audioID;

        public override string ActionName => $"Play ID: {audioID}";

        public override void Execute()
        {
            AudioManager.Instance?.Play(audioID);
        }
    }
}