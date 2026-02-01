using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
[SerializableSelectorName("Start Mission", "Mission")]
public class StartMissionAction : GameAction
{
    [SerializeField] private SOMission mission;
    
    public override string ActionName => mission ? $"Start Mission: {mission.Name}" : "Start Mission (No Mission Was Set)";
    
    public override void Execute()
    {
        if (mission)
        {
            mission.StartMission();
        }
    }
}