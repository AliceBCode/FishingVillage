
using FishingVillage.Interactable;
using UnityEngine;

namespace FishingVillage.RopeSystem
{
    public class RopePoint : MonoBehaviour
    {
        public bool isAnchor;
        public Vector3 StartPosition { get; private set; }
        public Collider Collider { get; private set; }
        public ConstrainableRopePath ParentPath { get; private set; }

        private void Awake()
        {
            Collider = GetComponent<Collider>();
            ParentPath = GetComponentInParent<ConstrainableRopePath>();
            StartPosition = transform.localPosition;
        }
        
    }
}