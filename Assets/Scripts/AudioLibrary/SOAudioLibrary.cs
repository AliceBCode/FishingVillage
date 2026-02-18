using System;
using DNExtensions.Utilities;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

namespace DNExtensions.Systems.AudioLibrary
{
    /// <summary>
    /// A ScriptableObject that represents a library of audio categories. Each category contains a list of audio mappings that map string IDs to audio objects.
    /// </summary>
    [UniqueSO]
    [CreateAssetMenu(fileName = "AudioLibrary", menuName = "Scriptable Objects/Audio Library")]
    public class SOAudioLibrary : ScriptableObject
    {

        [SerializeField] private SOAudioCategory[] audioCategories = Array.Empty<SOAudioCategory>();
        public SOAudioCategory[] AudioCategories => audioCategories;
        
    }



    

    
    
}
