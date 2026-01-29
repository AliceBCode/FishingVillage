using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;


[Serializable]
[SerializableSelectorName("Enter a trigger area")]
public class EnterTriggerObjective : MissionObjective
{
    [SerializeField] private string triggerID;
    
    public override string Name => "Enter Trigger";
    public override string Description => $"Enter trigger: {triggerID}";
    
    public override void Initialize()
    {
        GameEvents.OnTriggerEntered += OnTriggerEntered;
    }
    
    public override void Cleanup()
    {
        GameEvents.OnTriggerEntered -= OnTriggerEntered;
    }
    
    public override bool Evaluate()
    {
        return false;
    }
    
    private void OnTriggerEntered(string triggeredID)
    {
        if (triggeredID == triggerID)
        {
            SetMet();
        }
    }
}
