using System.Collections;
using DNExtensions.Utilities.AutoGet;
using FishingVillage.Player;
using Unity.Cinemachine;
using UnityEngine;

namespace FishingVillage.Gameplay
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera followCamera;
        [SerializeField] private CinemachineCamera pickupCamera;
        [SerializeField, AutoGetScene] private PlayerController player;
        
        private const int ActiveCameraPriority = 10;
        
        private void OnEnable()
        {
            GameEvents.OnItemObtained += OnItemObtained;
        }

        private void OnDisable()
        {
            GameEvents.OnItemObtained -= OnItemObtained;
        }

        private void Start()
        {
            pickupCamera.Follow = player.transform;
            followCamera.Follow = player.transform;
            SetActiveCamera(followCamera);
        }
        
        private void OnItemObtained(SOItem item)
        {
            SetActiveCamera(pickupCamera);
            StartCoroutine(SetActiveCameraDelayed(followCamera, 1f));
        }
        
        private void SetActiveCamera(CinemachineCamera cam)
        {
            followCamera.Priority = 0;
            pickupCamera.Priority = 0;
            
            cam.Priority = ActiveCameraPriority;
        }
        
        private IEnumerator SetActiveCameraDelayed(CinemachineCamera cam, float delay)
        {
            yield return new WaitForSeconds(delay);
            SetActiveCamera(cam);
        }

        public void ActivateFollowCamera(float delay = 0f)
        {
            if (delay <= 0f)
            {
                SetActiveCamera(followCamera);
                return;
            }
            StartCoroutine(SetActiveCameraDelayed(followCamera, delay));
        }
    }
}
