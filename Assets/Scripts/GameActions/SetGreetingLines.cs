using System;
using DNExtensions.Utilities.PrefabSelector;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.GameActions
{
    [Serializable]
    [MovedFrom("")]
    [SerializableSelectorName("Set Greeting Lines", "NPC")]
    public class SetGreetingLines : GameAction
    {
        [SerializeField, PrefabSelector("Assets/Prefabs/Npcs")] private Interactable.NPC npc;
        [SerializeField] private Dialogue.SODialogueLines greetingLines;

        public override string ActionName => npc ? $"Set {npc.Name} greeting lines" : $"Set NPC greeting lines (No NPC was set)";

        public override void Execute()
        {
            if (!npc || !greetingLines) return;

            var sceneNpc = FindNpcInScene(npc.InteractableID);
            if (sceneNpc)
            {
                sceneNpc.SetGreetingLines(greetingLines);
            }
            else
            {
                Debug.LogWarning($"Could not find NPC with ID {npc.InteractableID} in scene!");
            }
        }


    }
}