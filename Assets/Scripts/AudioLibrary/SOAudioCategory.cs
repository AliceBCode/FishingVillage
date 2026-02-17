using System;
using UnityEngine;
using UnityEngine.Audio;

namespace FishingVillage.AudioLibrary
{
    [CreateAssetMenu(fileName = "AudioCategory", menuName = "Scriptable Objects/Audio Category")]
    public class SOAudioCategory : ScriptableObject
    {
        
        [SerializeField] private string label;
        [SerializeField] private AudioResourceMapping[] audioResources = Array.Empty<AudioResourceMapping>();
        
        public AudioResourceMapping[] AudioResources => audioResources;
        public string Label => label;


        public AudioResource GetAudioResource(string id)
        {
            if (audioResources == null || audioResources.Length == 0) return null;
            
            foreach (var audioResourceMapping in audioResources)
            {
                if (audioResourceMapping.id == id)
                {
                    return audioResourceMapping.audioResource;
                }
            }
            
            return null;
        }
        
    }
}
