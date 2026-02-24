using DNExtensions.Systems.AudioLibrary;
using FishingVillage.Player;
using UnityEngine;

namespace FishingVillage.Gameplay
{
    public class AudioEventBridge : MonoBehaviour
    {
        [SerializeField, AudioID] private string playerWalkSoundId = "Walk";
        [SerializeField, AudioID] private string playerJumpSoundId = "Jump";
        private void OnEnable()
        {
            GameEvents.OnWalkAction += PlayWalkSound;
            GameEvents.OnJumpedAction += PlayJumpSound;
            GameEvents.OnTimelineSignalReceived += PlaySignalSound;
        }

        private void OnDisable()
        {
            GameEvents.OnWalkAction -= PlayWalkSound;
            GameEvents.OnJumpedAction -= PlayJumpSound;
            GameEvents.OnTimelineSignalReceived -= PlaySignalSound;
        }

        private void PlayWalkSound()
        {
            AudioLibrary.PlayAtPosition(playerWalkSoundId, PlayerController.Instance.transform);
        }
        private void PlayJumpSound()
        {
            AudioLibrary.PlayAtPosition(playerJumpSoundId, PlayerController.Instance.transform);
        }
        
        private void PlaySignalSound(string id) => AudioLibrary.Play(id);
    }
}