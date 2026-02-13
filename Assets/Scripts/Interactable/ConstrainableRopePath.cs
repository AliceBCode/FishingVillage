using DNExtensions.Utilities;
using DNExtensions.Utilities.AutoGet;
using FishingVillage.Gameplay;
using FishingVillage.Player;
using FishingVillage.RopeSystem;
using FishingVillage.UI;
using UnityEngine;

namespace FishingVillage.Interactable
{
    [RequireComponent(typeof(Rope))]
    public class ConstrainableRopePath : MonoBehaviour, IInteractable, IConstrainablePath
    {
        [Header("References")]
        [SerializeField] private Vector3 offset = Vector3.zero;
        [SerializeField, MinMaxRange(0,1)] private RangedFloat tRange = new (0f, 1f);
        [SerializeField, ReadOnly] private bool isConstrained;

        private InteractableVisuals _visuals;
        [SerializeField, AutoGetSelf, HideInInspector] private Rope rope;
        
        private void Awake()
        {
            _visuals = GetComponent<InteractableVisuals>();
        }
        
        public Vector3 GetPositionAt(float t)
        {
            t = tRange.Clamp(t);
            
            if (rope)
            {
                return rope.GetPointAt(t).Add(offset);
            }
            return Vector3.zero.Add(offset);
        }

        public float GetClosestT(Vector3 position)
        {
            if (rope)
            {
                return tRange.Clamp(rope.GetClosestT(position));
            }
            
            return tRange.Clamp(0f);
        }
        
        public void Release()
        {
            isConstrained = false;
            rope?.SetTarget(null);
        }

        public bool CanInteract()
        {
            return !isConstrained;
        }

        public void Interact()
        {
            if (!CanInteract()) return;
            
            isConstrained = true;
            
            rope?.SetTarget(PlayerController.Instance?.transform);
            PlayerController.Instance?.AttachToPath(this);
        }

        public void ShowInteract()
        {
            if (CanInteract())
            {
                _visuals?.Show();
            }
        }

        public void HideInteract()
        {
            _visuals?.Hide();
        }
    }
}