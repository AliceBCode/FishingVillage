using System;
using DNExtensions.Utilities;
using DNExtensions.Utilities.CustomFields;
using FishingVillage.Gameplay;
using FishingVillage.Interactable;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.Missions.Objectives
{
    [Serializable]
    [MovedFrom("")]
    public abstract class MissionObjective
    {
        [Tooltip("If true, this objective will only become active after the previous objective is completed.")]
        [SerializeField] private bool requiresPreviousObjective;
        [Tooltip("If true, this objective will not be shown in the journal or a notifications.")]
        [SerializeField] private bool isHidden;
        [Tooltip("If set, this description will be used instead of the default one.")]
        [SerializeField, HideIf("isHidden")] private OptionalField<string> overrideDescription = new OptionalField<string>(false, true);
        
        public bool IsHidden => isHidden;
        public bool RequiresPreviousObjective => requiresPreviousObjective;
        
        public bool Met { get; protected set; }
        public bool IsActive { get; private set; }

        protected abstract string Description { get; }
        
        
        /// <summary>
        /// Called when the mission is started.
        /// </summary>
        public abstract void Initialize();
        
        /// <summary>
        /// Called when the mission is completed.
        /// </summary>
        public abstract void Cleanup();
        
        /// <summary>
        /// Evaluates the objective's completion condition.
        /// </summary>
        public abstract bool Evaluate();
        
        
        /// <summary>
        /// Called when the objective becomes active. Override to perform any setup or initialization needed when the objective is activated.
        /// </summary>
        protected virtual void OnActivate()
        {
            
        }
        
        
        
        protected void SetMet()
        {
            if (Met || !IsActive) return;
            
            Met = true;
            GameEvents.ObjectiveMet(this);
        }
        
        protected void SetProgressed()
        {
            GameEvents.ObjectiveProgressed(this);
        }

        
        public void SetActive(bool active)
        {
            IsActive = active;
            
            if (active)
            {
                OnActivate();
                GameEvents.ObjectiveActivated(this);
            }
            else
            {
                SetProgressed();
            }
        }
        
        public void ForceSetMet(bool active)
        {
            Met = active;
            SetProgressed();
        }
        
        public string GetDescription()
        {
            if (overrideDescription.isSet)
            {
                return overrideDescription.Value;
            }

            return Description;
        }

        protected string GetInteractableID(MonoBehaviour interactable)
        {
            if (!interactable) return null;
            
            if (interactable.TryGetComponent<IdentifiableInteractable>(out var identifiable))
            {
                return identifiable.ID;
            }
            
            Debug.LogError($"Interactable {interactable.name} is missing IdentifiableInteractable component!");
            return null;
        }

        protected bool MatchesID(MonoBehaviour interactable, string targetID)
        {
            if (!interactable || string.IsNullOrEmpty(targetID)) return false;
            
            if (interactable.TryGetComponent<IdentifiableInteractable>(out var identifiable))
            {
                return identifiable.ID == targetID;
            }
            
            return false;
        }
        
    }
}