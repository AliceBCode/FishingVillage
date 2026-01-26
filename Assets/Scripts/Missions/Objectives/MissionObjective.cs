using System;
using UnityEngine;

[Serializable]
public abstract class MissionObjective
{
    public static event Action<MissionObjective> OnObjectiveMet;
    
    [SerializeField] private bool isHidden;
    [SerializeField] private bool requiresPreviousObjective;
    
    public bool IsHidden => isHidden;
    public bool RequiresPreviousObjective => requiresPreviousObjective;
    
    public bool Met { get; protected set; }
    public bool IsActive { get; private set; } = true;


    
    public abstract string Name { get; }
    public abstract string Description { get; }
    
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
}