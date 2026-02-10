using UnityEngine;
using UnityEngine.Audio;

namespace FishingVillage
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Item SFX")]
        [SerializeField] private AudioResource hornSfx;
        [SerializeField] private AudioResource cleaningSfx;
        

        [Header("Settings")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource musicSource;



        private void OnEnable()
        {
            GameEvents.OnItemUsed += OnItemUsed;
        }

        private void OnDisable()
        {
            GameEvents.OnItemUsed -= OnItemUsed;
        }

        private void OnItemUsed(SOItem item)
        {
            if (item && item.UseSfx)
            {
                sfxSource.resource = item.UseSfx;
                sfxSource.Play();
            }
        }
    }
}