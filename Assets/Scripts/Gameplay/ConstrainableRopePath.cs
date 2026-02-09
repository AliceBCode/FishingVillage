using DNExtensions.Utilities;
using DNExtensions.Utilities.AutoGet;
using DNExtensions.Utilities.RangedValues;
using FishingVillage.Interactable;
using FishingVillage.Player;
using FishingVillage.Rope;
using FishingVillage.UI;
using UnityEngine;

namespace FishingVillage.Gameplay
{
    public class ConstrainableRopePath : MonoBehaviour, IInteractable, IConstrainablePath
    {
        [Header("References")]
        [SerializeField] private Vector3 offset = Vector3.zero;
        [SerializeField, MinMaxRange(0,1)] private RangedFloat tRange = new (0f, 1f);
        [SerializeField, AutoGetSelf] private RopePath ropePath;
        [SerializeField, ReadOnly] private bool isConstrained;

        public Vector3 GetPositionAt(float t)
        {
            t = tRange.Clamp(t);
            
            if (ropePath)
            {
                return ropePath.GetPointAt(t).Add(offset);
            }
            return Vector3.zero.Add(offset);
        }

        public float GetClosestT(Vector3 position)
        {
            if (ropePath)
            {
                return tRange.Clamp(ropePath.GetClosestT(position));
            }
            
            return tRange.Clamp(0f);
        }
        
                
        public void Release()
        {
            isConstrained = false;
            ropePath?.SetTarget(null);
        }

        public bool CanInteract()
        {
            return !isConstrained;
        }

        public void Interact()
        {
            if (!CanInteract()) return;
            
            isConstrained = true;
            
            ropePath?.SetTarget(PlayerController.Instance?.transform);
            PlayerController.Instance?.AttachToPath(this);
        }

        public void ShowInteract()
        {
            if (!CanInteract()) return;
            
            InteractPrompt.Instance?.Show(transform.position + Vector3.up);
        }

        public void HideInteract()
        {
            InteractPrompt.Instance?.Hide(true);
        }
    }
}