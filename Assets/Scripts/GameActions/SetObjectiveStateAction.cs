using System;
using DNExtensions.Utilities;
using DNExtensions.Utilities.SerializableSelector;
using FishingVillage.Missions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.GameActions
{
    [Serializable]
    [SerializableSelectorName("Set Objective State", "Mission")]
    public class SetObjectiveStateAction : GameAction
    {
        [SerializeField, SOSelector("Assets/Data")] private SOMission mission;
        [SerializeField, EnableIf(nameof(mission)), Min(0)] private int objectiveIndex;
        [SerializeField, EnableIf(nameof(mission))] private bool isActive;
        [SerializeField, EnableIf(nameof(mission))] private bool isMet;
        
        private bool HasMission => true;

        public override string ActionName => mission ? $"Set's The State Of: {mission.Name}" : "Set's The State Of: (No Mission Was Set)";

        public override void Execute()
        {
            if (!mission || !MissionManager.Instance) return;

            var objectives = MissionManager.Instance.GetMissionObjectives(mission);
            if (objectives == null || objectiveIndex >= objectives.Length) return;

            var objective = objectives[objectiveIndex];
            objective.SetActive(isActive);
            objective.ForceSetMet(isMet);
            
        }
    }
}