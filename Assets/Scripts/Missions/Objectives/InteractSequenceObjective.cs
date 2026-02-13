using System;
using DNExtensions.Utilities.SerializableSelector;
using DNExtensions.Utilities.SerializedInterface;
using FishingVillage.Interactable;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.Missions.Objectives
{
    [Serializable]
    [MovedFrom("")]
    [SerializableSelectorName("Interact Sequence", "Interactable")]
    public class InteractSequenceObjective : MissionObjective
    {
        [SerializeField] private InterfaceReference<IInteractable, MonoBehaviour>[] requiredSequence;
    
        private string[] _targetIDs;
        private int _currentIndex;
        
        protected override string Description => $"Interact In Order ({_currentIndex}/{requiredSequence.Length})";
        
        public override void Initialize()
        {
            _currentIndex = 0;
        
            _targetIDs = new string[requiredSequence.Length];
            for (int i = 0; i < requiredSequence.Length; i++)
            {
                if (!requiredSequence[i].IsNull)
                {
                    _targetIDs[i] = GetInteractableID(requiredSequence[i].UnderlyingValue);
                }
                else
                {
                    Debug.LogError($"Sequence objective has null reference at index {i}!");
                }
            }
        
            GameEvents.OnInteractedWith += OnInteraction;
        }

        public override void Cleanup()
        {
            GameEvents.OnInteractedWith -= OnInteraction;
        }
        
        public override bool Evaluate()
        {
            return _currentIndex >= requiredSequence.Length;
        }
        
        private void OnInteraction(IInteractable interactable)
        {
            if (_currentIndex >= _targetIDs.Length || interactable is not MonoBehaviour mb) return;

            bool isPartOfSequence = Array.Exists(_targetIDs, id => MatchesID(mb, id));
            if (!isPartOfSequence) return;

            if (MatchesID(mb, _targetIDs[_currentIndex]))
            {
                _currentIndex++;

                if (Evaluate())
                {
                    SetMet();
                }
            }
            else
            {
                _currentIndex = 0;
            }
        }
    }
}