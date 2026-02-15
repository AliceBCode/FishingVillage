using System;
using DNExtensions.Utilities;
using DNExtensions.Utilities.CustomFields;
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
        
        [SerializeField, SOSelector("Assets/Data")] private SODialogueSequence dialogue;
        [SerializeField, PrefabSelector("Assets/Prefabs/Npcs", LockToFilter = true)] private NPC npc;
        [SerializeField, Tooltip("Enables proximity dialogue after the sequence finishes")] private bool enableProximityDialogueAfter = true;
        [SerializeField] private NoteField note = new NoteField("Starting a dialogue sequence with an NPC will disable his proximity dialogue.", false);

        public override string ActionName => npc ? $"Start Dialogue with {npc.Name}" : "Start Dialogue (No NPC was set)";

        public override void Execute()
        {
            if (npc && dialogue)
            {
                var sceneNpc = FindNpcInScene(npc);
                if (sceneNpc)
                {
                    sceneNpc.StartDialogueSequence(dialogue, enableProximityDialogueAfter);

                }
                else
                {
                    Debug.LogWarning($"Could not find NPC with ID {npc} in scene!");
                }
            }
        }

        public StartDialogueAction()
        {
        }

        public StartDialogueAction(SODialogueSequence dialogue, NPC npc, bool enableProximityDialogueAfter)
        {
            this.dialogue = dialogue;
            this.npc = npc;
            this.enableProximityDialogueAfter = enableProximityDialogueAfter;
        }
    }
}