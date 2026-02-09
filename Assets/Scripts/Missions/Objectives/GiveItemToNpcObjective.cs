using System;
using DNExtensions.Utilities.PrefabSelector;
using DNExtensions.Utilities.SerializableSelector;
using FishingVillage.Interactable;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.Missions.Objectives
{
    [Serializable]
    [MovedFrom("")]
    [SerializableSelectorName("Give Item", "NPC")]
    public class GiveItemToNpcObjective : MissionObjective
    {
        [SerializeField] private SOItem requiredItem;

        [SerializeField, PrefabSelector("Assets/Prefabs/Npcs")]
        private NPC npc;

        public SOItem RequiredItem => requiredItem;

        protected override string Description =>
            $"Give {(requiredItem ? requiredItem.Name : "(No Item Set)")} To {(npc ? npc.Name : "(No NPC Set)")}";

        private string _targetID;


        public override void Initialize()
        {
            if (!npc)
            {
                Debug.LogError("No NPC prefab reference set in objective!");
                return;
            }

            _targetID = npc.InteractableID;

            if (string.IsNullOrEmpty(_targetID))
            {
                Debug.LogError($"NPC prefab {npc.Name} has no ID set!");
                return;
            }


            GameEvents.OnItemGivenToNpc += OnItemGivenToNPC;
        }

        public override void Cleanup()
        {
            GameEvents.OnItemGivenToNpc -= OnItemGivenToNPC;
        }

        public override bool Evaluate()
        {
            return false;
        }

        public bool IsNpc(NPC npc)
        {
            return npc && npc.InteractableID == _targetID;
        }

        private void OnItemGivenToNPC(SOItem item, NPC npc)
        {
            if (item == requiredItem && IsNpc(npc))
            {
                SetMet();
            }
        }
    }
}
