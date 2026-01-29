using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;


[Serializable]
[SerializableSelectorName("Walk action")]
public class WalkActionObjective : MissionObjective
{
    public override string Name => "Walk Action";
    public override string Description => $"Walk";
    
    public override void Initialize()
    {
        GameEvents.OnWalkAction += OnWalkAction;
    }
    
    public override void Cleanup()
    {
        GameEvents.OnWalkAction -= OnWalkAction;
    }
    
    public override bool Evaluate()
    {
        return false;
    }
    
    private void OnWalkAction()
    {
        SetMet();
    }
}
