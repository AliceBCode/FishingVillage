using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
[AddTypeMenu("Custom (Unity Events)")]
public class CustomAction : GameAction
{
    [SerializeField] private UnityEvent onExecute;
    
    public override string ActionName => "Custom Action";
    
    public override void Execute()
    {
        onExecute?.Invoke();
    }
}