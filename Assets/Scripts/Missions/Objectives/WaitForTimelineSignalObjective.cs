using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.Missions.Objectives
{
    [Serializable]
    [MovedFrom("")]
    [SerializableSelectorName("Wait For Timeline Signal", "Cutscene")]
    public class WaitForTimelineSignalObjective : MissionObjective
    {
        [SerializeField] private string signalID;
        [SerializeField] private string cutsceneDescription = "Cutscene";

        protected override string Description => $"Watch {cutsceneDescription}";

        public override void Initialize()
        {
            GameEvents.OnTimelineSignalReceived += OnSignalReceived;
        }

        public override void Cleanup()
        {
            GameEvents.OnTimelineSignalReceived -= OnSignalReceived;
        }

        public override bool Evaluate()
        {
            return false;
        }

        private void OnSignalReceived(string receivedSignalID)
        {
            if (receivedSignalID == signalID)
            {
                SetMet();
            }
        }
    }
}