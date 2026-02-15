using System;
using DNExtensions.Utilities;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.GameActions
{
    [Serializable]
    [MovedFrom("")]
    [SerializableSelectorName("Start Mission", "Mission")]
    public class StartMissionAction : GameAction
    {
        [SerializeField, SOSelector("Assets/Data")] private Missions.SOMission mission;

        public override string ActionName => mission ? $"Start Mission: {mission.Name}" : "Start Mission (No Mission Was Set)";

        public override void Execute()
        {
            if (mission)
            {
                mission.StartMission();
            }
        }
    }
}