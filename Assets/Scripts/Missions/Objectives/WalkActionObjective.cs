using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine.Scripting.APIUpdating;


namespace FishingVillage.Missions.Objectives
{
    [Serializable]
    [MovedFrom("")]
    [SerializableSelectorName("Walk Action", "Player")]
    public class WalkActionObjective : MissionObjective
    {
        protected override string Description => $"Walk";

        public override void Initialize()
        {
            GameEvents.OnWalkAction += OnWalkAction;
        }

        public override void Cleanup()
        {
            GameEvents.OnWalkAction -= OnWalkAction;
        }

        public override bool Evaluate()
        {
            return false;
        }

        private void OnWalkAction()
        {
            SetMet();
        }
    }
}
