using System;
using DNExtensions.Utilities.CustomFields;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.Missions.Objectives
{
    [Serializable]
    [MovedFrom("")]
    public abstract class MissionObjective
{
    public static event Action<MissionObjective> OnObjectiveMet;
    
    [SerializeField] private bool isHidden;
    [SerializeField] private bool requiresPreviousObjective;
    [SerializeField] private OptionalField<string> overrideDescription = new OptionalField<string>(false, true);
    
    public bool IsHidden => isHidden;
    public bool RequiresPreviousObjective => requiresPreviousObjective;
    
    public bool Met { get; protected set; }
    public bool IsActive { get; private set; } = true;


    
    protected abstract string Description { get; }
    
    public abstract void Initialize();
    public abstract void Cleanup();
    public abstract bool Evaluate();
    
    protected void SetMet()
    {
        if (Met || !IsActive) return;
        
        Met = true;
        OnObjectiveMet?.Invoke(this);
    }
    
    public void SetActive(bool active)
    {
        IsActive = active;
    }
    
    public string GetDescription()
    {
        if (overrideDescription.isSet)
        {
            return overrideDescription.Value;
        }

        return Description;
    }
    }
}