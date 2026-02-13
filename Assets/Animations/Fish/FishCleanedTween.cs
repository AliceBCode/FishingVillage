using DNExtensions.Utilities.Button;
using PrimeTween;
using UnityEngine;

namespace FishingVillage.Gameplay
{
    public class FishCleanedTween : MonoBehaviour
    {
        [SerializeField] private float duration = 1f;
        [SerializeField] private float moveUpDistance = 3f;
        [SerializeField] private float moveAwayDistance = 2f;
        [SerializeField] private float moveSideDistance = 1.5f;
        [SerializeField] private int rotationLoops = 2;
        [SerializeField] private Transform fish;

        private Sequence _sequence;

        private void OnDisable()
        {
            if (_sequence.isAlive)
            {
                _sequence.Stop();
            }
        }

        [Button]
        public void PlayCleanedAnimation()
        {
            if (_sequence.isAlive)
            {
                _sequence.Stop();
            }
            
            Vector3 startPos = fish.position;
            float randomSide = Random.value > 0.5f ? 1 : -1;
            Vector3 targetPos = startPos + Vector3.up * moveUpDistance + fish.forward * moveAwayDistance + fish.right * moveSideDistance * randomSide;

            _sequence = Sequence.Create()
                .Group(Tween.Position(fish, targetPos, duration, Ease.OutBack))
                .Group(Tween.LocalEulerAngles(fish, fish.eulerAngles, new Vector3(0f, 0f, 360f * rotationLoops * randomSide), duration, Ease.InOutSine))
                .Group(Tween.Scale(fish, 0f, duration * 0.5f, Ease.InBack, startDelay: duration * 0.2f))
                .OnComplete(() => gameObject.SetActive(false));
        }
    }
}