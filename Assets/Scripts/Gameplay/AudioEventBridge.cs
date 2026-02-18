using DNExtensions.Systems.AudioLibrary;
using FishingVillage.Player;
using UnityEngine;

namespace FishingVillage.Gameplay
{
    public class AudioEventBridge : MonoBehaviour
    {
        [SerializeField] private string playerWalkSoundId = "Walk";
        [SerializeField] private string playerJumpSoundId = "Jump";
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
            AudioManager.Instance?.PlayAtPosition(playerWalkSoundId, PlayerController.Instance.transform);
        }
        private void PlayJumpSound()
        {
            AudioManager.Instance?.PlayAtPosition(playerJumpSoundId, PlayerController.Instance.transform);
        }
        private void PlaySignalSound(string id) => AudioManager.Instance?.Play(id);
    }
}