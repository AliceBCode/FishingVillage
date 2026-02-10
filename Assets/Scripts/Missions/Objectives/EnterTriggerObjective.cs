using System;
using DNExtensions.Utilities.SerializableSelector;
using FishingVillage.Missions.Objectives;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.Missions.Objectives
{
    [Serializable]
    [MovedFrom("")]
    [SerializableSelectorName("Enter Area", "Player")]
    public class EnterTriggerObjective : MissionObjective
    {
        [SerializeField] private string triggerID;
        [SerializeField] private string areaDescription = "Area";

        protected override string Description => $"Go To :{areaDescription}";

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
}
