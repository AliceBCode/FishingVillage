using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
[AddTypeMenu("Start Mission")]
public class StartMissionAction : GameAction
{
    [SerializeField] private SOMission mission;
    
    public override string ActionName => mission ? $"Start Mission: {mission.Name}" : "Start Mission (No mission was set)";
    
    public override void Execute()
    {
        if (mission)
        {
            mission.StartMission();
        }
    }
}