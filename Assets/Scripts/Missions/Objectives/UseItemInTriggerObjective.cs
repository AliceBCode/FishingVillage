using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.Missions.Objectives
{
    [Serializable]
    [MovedFrom("")]
    [SerializableSelectorName("Use Item In Trigger Area", "Item")]
    public class UseItemInTriggerObjective : MissionObjective
    {
        [SerializeField] private SOItem item;
        [SerializeField] private int requiredUsagesCount = 1;
        [SerializeField] private string triggerID;
        [SerializeField] private string areaDescription = "Area";

        private int _currentCount;

        protected override string Description
        {
            get
            {
                if (!item)
                {
                    return $"Use: (No Item Selected) In: {areaDescription}";
                }

                if (!item.Usable)
                {
                    return $"Use {item.Name} (Item Is Not Usable) In: {areaDescription}";
                }

                if (requiredUsagesCount > 1)
                {
                    return $"Use {item.Name} In {areaDescription} ({_currentCount}/{requiredUsagesCount})";
                }
            
                return $"Use {item.Name} In {areaDescription}";
            }
        }

        private bool _inTriggerArea;
        
        public override void Initialize()
        {
            _currentCount = 0;
            GameEvents.OnItemUsedInTrigger += OnItemUsedInTrigger;
        }
        
        public override void Cleanup()
        {
            GameEvents.OnItemUsedInTrigger -= OnItemUsedInTrigger;
        }

        public override bool Evaluate()
        {
            return _currentCount >= requiredUsagesCount;
        }
        
        private void OnItemUsedInTrigger(string triggeredID, SOItem usedItem)
        {
            if (usedItem != item || triggeredID != triggerID) return;

            _currentCount++;

            if (Evaluate())
            {
                SetMet();
            }
        }
    }
}