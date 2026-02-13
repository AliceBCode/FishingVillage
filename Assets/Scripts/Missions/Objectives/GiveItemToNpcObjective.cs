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
        [SerializeField, PrefabSelector("Assets/Prefabs/Npcs", LockToFilter = true)] private NPC npc;

        public SOItem RequiredItem => requiredItem;

        protected override string Description =>
            $"Give {(requiredItem ? requiredItem.Name : "(No Item Set)")} To {(npc ? npc.Name : "(No NPC Set)")}";

        private string _targetID;

        public override void Initialize()
        {
            if (!npc)
            {
                Debug.LogError("No NPC reference set in objective!");
                return;
            }

            _targetID = GetInteractableID(npc);

            if (string.IsNullOrEmpty(_targetID))
            {
                Debug.LogError($"NPC {npc.Name} has no ID set!");
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

        public bool MatchesNpcID(string npcID)
        {
            return !string.IsNullOrEmpty(npcID) && npcID == _targetID;
        }

        private void OnItemGivenToNPC(SOItem item, NPC npc)
        {
            if (item == requiredItem && MatchesID(npc, _targetID))
            {
                SetMet();
            }
        }
    }
}