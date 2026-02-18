using System.Collections.Generic;
using DNExtensions.Utilities.AutoGet;
using DNExtensions.Utilities.Inline;
using FishingVillage;
using UnityEngine;
using UnityEngine.Audio;

namespace DNExtensions.Systems.AudioSystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        
        public static AudioManager Instance;
        
        [SerializeField, AutoGetAsset(Folders = new [] {"Assets/Data"}), Inline] private SOAudioLibrary library;
        [SerializeField] private AudioSource directSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource musicSource; 
        [SerializeField] private AudioSource ambientSource;


        private readonly Dictionary<string, AudioData> _audioCache = new();
        
        private struct AudioData
        {
            public Object audioObject;
            public AudioMixerGroup group;
            public AudioChannel channel;
        }

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

                foreach (var mapping in category.AudioMappings)
                {
                    if (string.IsNullOrEmpty(mapping.id)) continue;
                    
                    _audioCache[mapping.id] = new AudioData
                    {
                        audioObject = mapping.audioObject,
                        group = category.AudioMixerGroup,
                        channel = category.Channel
                        
                    };
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
        
        private void ApplySettings(AudioSource source, AudioSettings settings)
        {
            source.clip = settings.clip;
            source.volume = settings.volume;
            source.pitch = settings.pitch;
            source.panStereo = settings.stereoPan;
            source.spatialBlend = settings.spatialBlend;
            source.reverbZoneMix = settings.reverbZoneMix;
            source.bypassEffects = settings.bypassEffects;
            source.bypassListenerEffects = settings.bypassListenerEffects;
            source.bypassReverbZones = settings.bypassReverbZones;
            source.loop = settings.loop;

            if (settings.set3DSettings)
            {
                source.dopplerLevel = settings.dopplerLevel;
                source.spread = settings.spread;
                source.rolloffMode = settings.rolloffMode;
                source.minDistance = settings.minDistance;
                source.maxDistance = settings.maxDistance;
            }
        }
        
        
        private void HandleLoopingAudio(AudioSource source, AudioData data)
        {
            if (data.audioObject is AudioClip clip && source.clip == clip && source.isPlaying) return;
            
            source.outputAudioMixerGroup = data.group;
            
            if (data.audioObject is SOAudioProfile profile)
            {
                AudioSettings settings = profile.GetSettings();
                ApplySettings(source, settings);
                source.resource = settings.clip;
            }
            else if (data.audioObject is AudioClip directClip)
            {
                source.loop = true;
                source.resource = directClip;
            }
            
            source.Play();
        }
        
        private void HandleOveShotAudio(AudioSource source, AudioData data)
        {
            
            source.outputAudioMixerGroup = data.group;
            
            if (data.audioObject is SOAudioProfile profile)
            {
                AudioSettings settings = profile.GetSettings();
                ApplySettings(source, settings);
                source.PlayOneShot(settings.clip, settings.volume);
            }
            else if (data.audioObject is AudioClip clip)
            {
                source.PlayOneShot(clip);
            }
        }
        

        public void PlayFromLibrary(string id)
        {
            if (!_audioCache.TryGetValue(id, out var data)) return;
            
            switch (data.channel)
            {
                case AudioChannel.Music:
                    HandleLoopingAudio(musicSource, data);
                    break;
                case AudioChannel.Ambience:
                    HandleLoopingAudio(ambientSource, data);
                    break;
                default: 
                    HandleOveShotAudio(sfxSource, data);
                    break;
            }
        }
        
        public void PlayDirect(AudioResource resource)
        {
            if (!resource) return;
            
            directSource.resource = resource;
            directSource.Play();
        }
        
    }
}