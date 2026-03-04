using DNExtensions.Systems.AudioLibrary;
using FishingVillage.Gameplay;
using UnityEngine;

namespace FishingVillage.Player
{
    public class PlayerAudioEventBridge : MonoBehaviour
    {
        [SerializeField, AudioLibraryID] private string playerWalkSoundId = "Walk";
        [SerializeField, AudioLibraryID] private string playerJumpSoundId = "Jump";
        [SerializeField, AudioLibraryID] private string playerPickupID = "Pickup";
        
        
        private void OnEnable()
        {
            GameEvents.OnWalkAction += PlayWalkSound;
            GameEvents.OnJumpedAction += PlayJumpSound;
            GameEvents.OnItemObtained += PlayPickupSound;
        }

        private void OnDisable()
        {
            GameEvents.OnWalkAction -= PlayWalkSound;
            GameEvents.OnJumpedAction -= PlayJumpSound;
            GameEvents.OnItemObtained -= PlayPickupSound;
        }
        

        private void PlayWalkSound()
        {
            AudioLibrary.PlayAtPosition(playerWalkSoundId, PlayerController.Instance.transform);
        }
        private void PlayJumpSound()
        {
            AudioLibrary.PlayAtPosition(playerJumpSoundId, PlayerController.Instance.transform);
        }
        
        private void PlayPickupSound(SOItem item) 
        {
            AudioLibrary.PlayAtPosition(playerPickupID, PlayerController.Instance.transform);
        }
        
    }
}