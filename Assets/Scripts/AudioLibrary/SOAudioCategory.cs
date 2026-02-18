using System;
using UnityEngine;
using UnityEngine.Audio;

namespace DNExtensions.Systems.AudioSystem
{
    [Serializable]
    public struct AudioObjectsMapping
    {
        public string id;
        public UnityEngine.Object audioObject;
    }
    
    
    [CreateAssetMenu(fileName = "AudioCategory", menuName = "Scriptable Objects/Audio Category")]
    public class SOAudioCategory : ScriptableObject
    {
        
        [SerializeField] private string label;
        [SerializeField] private AudioChannel channel;
        [SerializeField] private AudioMixerGroup audioMixerGroup;
        [SerializeField] private AudioObjectsMapping[] audioMappings = Array.Empty<AudioObjectsMapping>();
        
        
        public string Label => label;
        public AudioMixerGroup AudioMixerGroup => audioMixerGroup;
        public AudioChannel Channel => channel;
        public AudioObjectsMapping[] AudioMappings => audioMappings;



        public UnityEngine.Object GetAudioResource(string id)
        {
            if (audioMappings == null || audioMappings.Length == 0) return null;
            
            foreach (var audioResourceMapping in audioMappings)
            {
                if (audioResourceMapping.id == id)
                {
                    return audioResourceMapping.audioObject;
                }
            }
            
            return null;
        }
        
    }
}
