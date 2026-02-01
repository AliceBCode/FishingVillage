using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;

[Serializable]
[SerializableSelectorName("Jump Action", "Player")]
public class JumpActionObjective : MissionObjective
{
    public override string Description => $"Jump";
    
    public override void Initialize()
    {
        GameEvents.OnJumpedAction += OnJumpAction;
    }
    
    public override void Cleanup()
    {
        GameEvents.OnJumpedAction -= OnJumpAction;
    }
    
    public override bool Evaluate()
    {
        return false;
    }
    
    private void OnJumpAction()
    {
        SetMet();
    }
}