using System;
using DNExtensions.Systems.AudioLibrary;
using DNExtensions.Systems.AudioTrack;
using DNExtensions.Utilities.CustomFields;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;

namespace FishingVillage.GameActions
{
    [Serializable]
    [SerializableSelectorName("Play Track", "Audio")]
    public class PlayTrackAction : GameAction
    {
        [SerializeField, AudioTrackID] private string audioID;
        [SerializeField] private OptionalField<float> overrideFade = new OptionalField<float>(1f, false);

        public override string ActionName => $"Play Track: {audioID}";

        public override void Execute()
        {
            if (overrideFade)
            {
                AudioTrack.Play(audioID, overrideFade.Value);
            }
            else
            {
                AudioTrack.Play(audioID);
            }

        }
    }
}