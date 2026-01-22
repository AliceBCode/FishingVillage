using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Item SFX")]
    [SerializeField] private AudioResource hornSfx;
    [SerializeField] private AudioClip cleaningSfx;
    
    [Header("Player SFX")]
    [SerializeField] private AudioClip jumpSfx;
    [SerializeField] private AudioClip walkSfx;
    
    [Header("Settings")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;



    private void OnEnable()
    {
        GameEvents.OnHornUsed += PlayHornSfx;
        GameEvents.OnCleaningUtensilsUsed += PlayCleaningSfx;
        GameEvents.OnJumpedAction += PlayJumpSfx;
        GameEvents.OnWalkAction += PlayWalkSfx;
    }
    
    private void OnDisable()
    {
        GameEvents.OnHornUsed -= PlayHornSfx;
        GameEvents.OnCleaningUtensilsUsed -= PlayCleaningSfx;
        GameEvents.OnJumpedAction -= PlayJumpSfx;
        GameEvents.OnWalkAction -= PlayWalkSfx;
    }
    
    private void PlayHornSfx()
    {
        if (!hornSfx) return;
        sfxSource.resource = hornSfx;
        sfxSource.Play();
        
    }
    private void PlayCleaningSfx()
    {
        if (!cleaningSfx) return;
        sfxSource.PlayOneShot(cleaningSfx);
    }
    private void PlayJumpSfx()
    {
        if (!jumpSfx) return;
        sfxSource.PlayOneShot(jumpSfx);
    }
    private void PlayWalkSfx()
    {
        if (!walkSfx) return;
        sfxSource.PlayOneShot(walkSfx);
    }
}