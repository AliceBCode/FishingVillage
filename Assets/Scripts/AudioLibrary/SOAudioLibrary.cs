using System;
using DNExtensions.Utilities;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

namespace DNExtensions.Systems.AudioSystem
{
    [UniqueSO]
    [CreateAssetMenu(fileName = "AudioLibrary", menuName = "Scriptable Objects/Audio Library")]
    public class SOAudioLibrary : ScriptableObject
    {

        [SerializeField] private SOAudioCategory[] audioCategories = Array.Empty<SOAudioCategory>();


        public SOAudioCategory[] AudioCategories => audioCategories;
        
        public Object GetAudioResource(string id)
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
