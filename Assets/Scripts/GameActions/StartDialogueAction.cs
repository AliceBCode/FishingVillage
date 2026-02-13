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
    [SerializableSelectorName("Start Dialogue", "NPC")]
    public class StartDialogueAction : GameAction
    {
        [SerializeField, PrefabSelector("Assets/Prefabs/Npcs", LockToFilter = true)] private NPC npc;
        [SerializeField] private SODialogueSequence dialogue;

        public override string ActionName => npc ? $"Start Dialogue with {npc.Name}" : "Start Dialogue (No NPC was set)";

        public override void Execute()
        {
            if (npc && dialogue)
            {
                var sceneNpc = FindNpcInScene(npc);
                if (sceneNpc)
                {
                    sceneNpc.StartDialogueSequence(dialogue);
                }
                else
                {
                    Debug.LogWarning($"Could not find NPC with ID {npc} in scene!");
                }
            }
        }

    }
}