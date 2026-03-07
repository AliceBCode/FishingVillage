using System;
using DNExtensions.Systems.AudioTrack;
using DNExtensions.Utilities.CustomFields;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;

namespace FishingVillage.GameActions
{
    [Serializable]
    [SerializableSelectorName("Stop Track", "Audio")]
    public class StopTrackAction : GameAction
    {
        [SerializeField, AudioTrackID] private string audioID;
        [SerializeField] private OptionalField<float> overrideFade = new OptionalField<float>(1f, false);

        public override string ActionName => $"Stops Track: {audioID}";

        public override void Execute()
        {
            if (overrideFade)
            {
                AudioTrack.Stop(audioID, overrideFade.Value);
            }
            else
            {
                AudioTrack.Stop(audioID);
            }

        }
    }
}