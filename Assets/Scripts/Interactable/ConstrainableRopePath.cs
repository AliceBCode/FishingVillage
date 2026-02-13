using System;
using DNExtensions.Utilities;
using DNExtensions.Utilities.AutoGet;
using FishingVillage.Gameplay;
using FishingVillage.Player;
using FishingVillage.RopeSystem;
using UnityEngine;

namespace FishingVillage.Interactable
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rope))]
    public class ConstrainableRopePath : MonoBehaviour, IInteractable, IConstrainablePath
    {
        [Header("References")]
        [SerializeField] private Vector3 offset = Vector3.zero;
        [SerializeField, MinMaxRange(0,1)] private RangedFloat tRange = new (0f, 1f);
        [SerializeField, ReadOnly] private bool isConstrained;

        private InteractableVisuals _visuals;
        private bool _isShowingPrompt;
        [SerializeField, AutoGetSelf, HideInInspector] private Rope rope;
        
        private void Awake()
        {
            if (!rope)
            {
                enabled = false;
                return;
            }
            _visuals = GetComponent<InteractableVisuals>();

        }
        

        private void Update()
        {
            if (_isShowingPrompt && PlayerController.Instance)
            {
                Vector3 closestPoint = GetClosestRopePointPosition(PlayerController.Instance.transform.position);
                bool playerIsAbove = PlayerController.Instance.transform.position.y > closestPoint.y;
                Vector3 adjustedOffset = playerIsAbove ? Vector3.down : Vector3.up;
        
                _visuals?.UpdatePromptPosition(closestPoint + adjustedOffset);
            }
        }
        
        private Vector3 GetClosestRopePointPosition(Vector3 position)
        {
            if (rope.Points.Length == 0) return transform.position;

            int startIndex = Mathf.RoundToInt(tRange.minValue * (rope.Points.Length - 1));
            int endIndex = Mathf.RoundToInt(tRange.maxValue * (rope.Points.Length - 1));

            Vector3 closest = rope.Points[startIndex].transform.position;
            float minDist = float.MaxValue;

            for (int i = startIndex; i < endIndex; i++)
            {
                Vector3 a = rope.Points[i].transform.position;
                Vector3 b = rope.Points[i + 1].transform.position;
                Vector3 ab = b - a;
                float t = Mathf.Clamp01(Vector3.Dot(position - a, ab) / ab.sqrMagnitude);
                Vector3 projected = a + t * ab;
        
                float dist = Vector3.Distance(position, projected);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = projected;
                }
            }

            return closest;
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
            GameEvents.InteractedWith(this);
        }

        public void ShowInteract()
        {
            if (!CanInteract()) return;

            _isShowingPrompt = true;

            if (PlayerController.Instance)
            {
                Vector3 closestPoint = GetClosestRopePointPosition(PlayerController.Instance.transform.position);
                bool playerIsAbove = PlayerController.Instance.transform.position.y > closestPoint.y;
                Vector3 adjustedOffset = playerIsAbove ? Vector3.down : Vector3.up;
        
                _visuals?.Show(closestPoint + adjustedOffset);
            }
            else
            {
                _visuals?.Show();
            }
        }

        public void HideInteract()
        {
            _isShowingPrompt = false;
            _visuals?.Hide();
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

        private void OnDrawGizmosSelected()
        {
            if (rope)
            {
                Vector3 startPoint = GetPositionAt(tRange.minValue);
                Vector3 endPoint = GetPositionAt(tRange.maxValue);
                
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(startPoint, 0.1f);
                Gizmos.DrawSphere(endPoint, 0.1f);
                Gizmos.DrawLine(startPoint, endPoint);
            }
        }
    }
}