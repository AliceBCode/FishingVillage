using System;
using DNExtensions.Utilities.PrefabSelector;
using DNExtensions.Utilities.SerializableSelector;
using FishingVillage.Dialogue;
using FishingVillage.Interactable;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.GameActions
{
    [Serializable]
    [MovedFrom("")]
    [SerializableSelectorName("Set Farewell Lines", "NPC")]
    public class SetFarewellLines : GameAction
    {
        [SerializeField, PrefabSelector("Assets/Prefabs/Npcs", LockToFilter = true)] private NPC npc;
        [SerializeField] private SODialogueLines farewellLines;

        public override string ActionName => npc ? $"Set {npc.Name} farewell lines" : $"Set NPC farewell lines (No NPC was set)";

        public override void Execute()
        {
            if (!npc || !farewellLines) return;

            var sceneNpc = FindNpcInScene(npc);
            if (sceneNpc)
            {
                sceneNpc.SetFarewellLines(farewellLines);
            }
            else
            {
                Debug.LogWarning($"Could not find NPC with ID {npc} in scene!");
            }
        }


    }
}