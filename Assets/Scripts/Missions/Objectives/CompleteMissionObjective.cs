using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.Missions.Objectives
{
    [Serializable]
    [MovedFrom("")]
    [SerializableSelectorName("Complete Mission", "Mission")]
    public class CompleteMissionObjective : MissionObjective
    {
        [SerializeField] private SOMission mission;
        
        protected override string Description => $"Complete Mission {mission.Name}";

        public override void Initialize()
        {
            GameEvents.OnMissionCompleted += OnMissionCompleted;
        }

        public override void Cleanup()
        {
            GameEvents.OnMissionCompleted -= OnMissionCompleted;
        }

        private void OnMissionCompleted(SOMission completedMission)
        {
            if (completedMission == mission)
            {
                SetMet();
            }

        }

        public override bool Evaluate()
        {
            return false;
        }
        
    }
}
