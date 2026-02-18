using System;
using DNExtensions.Systems.AudioSystem;
using DNExtensions.Utilities;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;
using UnityEngine.Audio;

namespace FishingVillage.GameActions
{
    [Serializable]
    [SerializableSelectorName("Play Audio", "Audio")]
    public class PlayAudioAction : GameAction
    {
        public enum AudioSourceType { LibraryID, DirectResource }

        [SerializeField] private AudioSourceType sourceType = AudioSourceType.LibraryID;
        [SerializeField, ShowIf("sourceType", (int)AudioSourceType.LibraryID)] private string audioID;
        [SerializeField, ShowIf("sourceType", (int)AudioSourceType.DirectResource)] private AudioResource resource;

        public override string ActionName => sourceType == AudioSourceType.LibraryID 
            ? $"Play ID: {audioID}" 
            : $"Play Resource: {(resource ? resource.name : "None")}";

        public override void Execute()
        {
            if (sourceType == AudioSourceType.LibraryID)
            {
                AudioManager.Instance?.PlayFromLibrary(audioID);
            }
            else if (resource)
            {
                AudioManager.Instance?.PlayDirect(resource);
            }
        }
    }
}