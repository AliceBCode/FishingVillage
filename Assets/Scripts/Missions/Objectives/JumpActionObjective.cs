using System;
using UnityEngine;

[Serializable]
[AddTypeMenu("Jump action")]
public class JumpActionObjective : MissionObjective
{
    public override string Name => "Jump Action";
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