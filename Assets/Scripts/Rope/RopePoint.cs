using UnityEngine;

namespace FishingVillage.RopeSystem
{
    public class RopePoint : MonoBehaviour
    {
        public bool isAnchor;

        public Vector3 StartPosition { get; private set; }
        


        private void Awake()
        {
            StartPosition = transform.localPosition;
        }
    }
}