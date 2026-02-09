using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.Missions.Objectives
{
    [Serializable]
    [MovedFrom("")]
    [SerializableSelectorName("Interact Sequence", "Interactable")]
    public class InteractSequenceObjective : MissionObjective
    {
        [SerializeField] private Interactable.Interactable[] requiredSequence;
    
    private string[] _targetIDs;
    private int _currentIndex;
    
    protected override string Description => $"Interact In Order ({_currentIndex}/{requiredSequence.Length})";
    
    public override void Initialize()
    {
        _currentIndex = 0;
    
        _targetIDs = new string[requiredSequence.Length];
        for (int i = 0; i < requiredSequence.Length; i++)
        {
            if (requiredSequence[i])
            {
                _targetIDs[i] = requiredSequence[i].InteractableID;
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
    
    private void OnInteraction(Interactable.Interactable interactable)
    {
        if (_currentIndex >= _targetIDs.Length) return;

        bool isPartOfSequence = Array.Exists(_targetIDs, id => id == interactable.InteractableID);
        if (!isPartOfSequence) return;


        if (interactable.InteractableID == _targetIDs[_currentIndex])
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
