using DNExtensions.Utilities;
using UnityEngine;
using UnityEngine.Audio;

namespace DNExtensions.Systems.AudioSystem
{
    [System.Serializable]
    public struct AudioSettings
    {
        public AudioClip clip;
        public float volume;
        public float pitch;
        public float stereoPan;
        public float spatialBlend;
        public float reverbZoneMix;
        public bool bypassEffects;
        public bool bypassListenerEffects;
        public bool bypassReverbZones;
        public bool loop;
        public bool set3DSettings;
        public float dopplerLevel;
        public float spread;
        public AudioRolloffMode rolloffMode;
        public float minDistance;
        public float maxDistance;
    }

    [CreateAssetMenu(fileName = "New AudioProfile", menuName = "Scriptable Objects/Audio Profile")]
    public class SOAudioProfile : ScriptableObject
    {
        public AudioClip[] clips;
        [MinMaxRange(0f, 1f)] public RangedFloat volume = new RangedFloat(1, 1);
        [MinMaxRange(-3f, 3f)] public RangedFloat pitch = new RangedFloat(1, 1);
        [Range(-1f, 1f)] public float stereoPan;
        [Range(0f, 1f)] public float spatialBlend;
        [Range(0f, 1.1f)] public float reverbZoneMix = 1f;
        public bool bypassEffects;
        public bool bypassListenerEffects;
        public bool bypassReverbZones;
        public bool loop;
        public bool set3DSettings;
        public float dopplerLevel = 1f;
        public float spread;
        public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
        public float minDistance = 1f;
        public float maxDistance = 500f;

        public AudioSettings GetSettings()
        {
            return new AudioSettings
            {
                clip = clips.Length > 0 ? clips[Random.Range(0, clips.Length)] : null,
                volume = volume.RandomValue,
                pitch = pitch.RandomValue,
                stereoPan = stereoPan,
                spatialBlend = spatialBlend,
                reverbZoneMix = reverbZoneMix,
                bypassEffects = bypassEffects,
                bypassListenerEffects = bypassListenerEffects,
                bypassReverbZones = bypassReverbZones,
                loop = loop,
                set3DSettings = set3DSettings,
                dopplerLevel = dopplerLevel,
                spread = spread,
                rolloffMode = rolloffMode,
                minDistance = minDistance,
                maxDistance = maxDistance
            };
        }
    }
}