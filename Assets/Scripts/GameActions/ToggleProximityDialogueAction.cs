using System;
using DNExtensions.Utilities.PrefabSelector;
using DNExtensions.Utilities.SerializableSelector;
using FishingVillage.Interactable;
using UnityEngine;

namespace FishingVillage.GameActions
{
    [Serializable]
    [SerializableSelectorName("Toggle Proximity Dialogue", "NPC")]
    public class ToggleProximityDialogueAction : GameAction
    {
        [SerializeField, PrefabSelector("Assets/Prefabs/Npcs", LockToFilter = true)] private NPC npc;
        [SerializeField] private bool allowProximityDialogue;

        public override string ActionName => npc ? $"Toggle {npc.Name} proximity dialogue" : $"Toggle NPC proximity dialogue (No NPC was set)";

        public override void Execute()
        {
            if (npc)
            {
                var sceneNpc = FindNpcInScene(npc);
                if (sceneNpc)
                {
                    sceneNpc.EnableProximityDialogue(allowProximityDialogue);
                }
                else
                {
                    Debug.LogWarning($"Could not find NPC with ID {npc} in scene!");
                }
            }
        }

    }
}