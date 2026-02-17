using System;
using DNExtensions.Utilities;
using UnityEngine;
using UnityEngine.Audio;

namespace FishingVillage.AudioLibrary
{
    [UniqueSO]
    [CreateAssetMenu(fileName = "AudioLibrary", menuName = "Scriptable Objects/Audio Library")]
    public class SOAudioLibrary : ScriptableObject
    {

        [SerializeField] private SOAudioCategory[] audioCategories = Array.Empty<SOAudioCategory>();


        public SOAudioCategory[] AudioCategories => audioCategories;
        
        public AudioResource GetAudioResource(string id)
        {
            foreach (var category in audioCategories)
            {
                if (!category) continue;
                var resource = category.GetAudioResource(id);
                if (resource) return resource;
            }
            return null;
        }
    }



    

    
    
}
