using System;
using DNExtensions.Utilities;
using DNExtensions.Utilities.CustomFields;
using DNExtensions.Utilities.SerializableSelector;
using FishingVillage.Dialogue;
using FishingVillage.GameActions;
using FishingVillage.Interactable;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.Missions.Objectives
{
    [Serializable]
    [MovedFrom("")]
    [SerializableSelectorName("Complete Dialogue Sequence", "NPC")]
    public class CompleteDialogueObjective : MissionObjective
    {
        [SerializeField, PrefabSelector("Assets/Prefabs/Npcs", LockToFilter = true)] private NPC npc;
        [SerializeField, Tooltip("If active will also start the required sequence for the npc, also disables his proximity dialogue")] 
        private OptionalField<SODialogueSequence> alsoStartDialogue = new OptionalField<SODialogueSequence>(false);
        [SerializeField, ShowIf("alsoStartDialogue.isSet"), Tooltip("Enables proximity dialogue after the sequence finishes")] private bool enableProximityDialogueAfter = true;
        [SerializeField, ShowIf("alsoStartDialogue.isSet")] private NoteField note = new NoteField("Starting a dialogue sequence with an NPC will disable his proximity dialogue.", false);
        
        private string _targetID;

        protected override string Description => npc
            ? $"Talk With {npc.Name}"
            : "Talk With (No NPC Was Set)";

        public override void Initialize()
        {
            if (!npc)
            {
                Debug.LogError("No NPC reference set in dialogue objective!");
                return;
            }

            _targetID = GetInteractableID(npc);

            if (string.IsNullOrEmpty(_targetID))
            {
                Debug.LogError($"NPC {npc.Name} has no ID set!");
                return;
            }
            

            GameEvents.OnDialogueSequenceCompleted += OnDialogueCompleted;
        }

        public override void Cleanup()
        {
            GameEvents.OnDialogueSequenceCompleted -= OnDialogueCompleted;
        }

        public override bool Evaluate()
        {
            return false;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            
            if (npc && alsoStartDialogue.Value)
            {
                var startDialogue = new StartDialogueAction(alsoStartDialogue.Value, npc, enableProximityDialogueAfter);
                startDialogue.Execute();
            }
        }

        private void OnDialogueCompleted(NPC npc)
        {
            if (MatchesID(npc, _targetID))
            {
                SetMet();
            }
        }
    }
}