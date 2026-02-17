using System.Collections.Generic;
using DNExtensions.Utilities.AutoGet;
using DNExtensions.Utilities.Inline;
using UnityEngine;
using UnityEngine.Audio;

namespace FishingVillage.AudioLibrary
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        
        [SerializeField, AutoGetAsset(Folders = new [] {"Assets/Data"}), Inline] private SOAudioLibrary library;
        [SerializeField, AutoGetSelf, HideInInspector] private AudioSource sfxSource;


        private readonly Dictionary<string, AudioResource> _audioCache = new();

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeCache();
        }

        private void InitializeCache()
        {
            if (!library) return;

            foreach (var category in library.AudioCategories)
            {
                if (!category) continue;

                foreach (var mapping in category.AudioResources)
                {
                    if (string.IsNullOrEmpty(mapping.id)) continue;
                    
                    if (!_audioCache.TryAdd(mapping.id, mapping.audioResource))
                    {
                        Debug.LogWarning($"Duplicate Audio ID: {mapping.id} in {category.Label}");
                    }
                }
            }
        }

        private void OnEnable()
        {
            GameEvents.OnJumpedAction += HandleJump;
            GameEvents.OnWalkAction += HandleWalk;
            GameEvents.OnTimelineSignalReceived += PlayFromLibrary; 
        }

        private void OnDisable()
        {
            GameEvents.OnJumpedAction -= HandleJump;
            GameEvents.OnWalkAction -= HandleWalk;
            GameEvents.OnTimelineSignalReceived -= PlayFromLibrary;
        }
        
        private void HandleJump() => PlayFromLibrary("Jump");
        private void HandleWalk() => PlayFromLibrary("Walk");

        public void PlayFromLibrary(string id)
        {
            if (!_audioCache.TryGetValue(id, out var resource)) return;
            
            sfxSource.resource = resource;
            sfxSource.Play();
        }
        
        public void PlayDirect(AudioResource resource)
        {
            if (!resource) return;
            
            sfxSource.resource = resource;
            sfxSource.Play();
        }
    }
}