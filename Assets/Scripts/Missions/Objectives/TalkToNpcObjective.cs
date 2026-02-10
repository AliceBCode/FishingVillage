// using System;
// using DNExtensions.Utilities.PrefabSelector;
// using DNExtensions.Utilities.SerializableSelector;
// using UnityEngine;
// using UnityEngine.Scripting.APIUpdating;
//
// namespace FishingVillage.Missions.Objectives
// {
//     [Serializable]
//     [MovedFrom("")]
//     [SerializableSelectorName("Talk To", "NPC")]
//     public class TalkToNpcObjective : MissionObjective
//     {
//         [SerializeField, PrefabSelector("Assets/Prefabs/Npcs")]  private Interactable.NPC npc;
//
//         private string _targetID;
//
//         public override string Description => $"Talk To {(npc ? npc.Name : "(No NPC Was Set)")}";
//
//         public override void Initialize()
//         {
//             if (!npc)
//             {
//                 Debug.LogError("No NPC prefab reference set in objective!");
//                 return;
//             }
//
//             _targetID = npc.InteractableID;
//
//             if (string.IsNullOrEmpty(_targetID))
//             {
//                 Debug.LogError($"NPC prefab {npc.Name} has no ID set!");
//                 return;
//             }
//
//             GameEvents.OnNpcTalkedTo += OnNPCTalkedTo;
//         }
//
//         public override void Cleanup()
//         {
//             GameEvents.OnNpcTalkedTo -= OnNPCTalkedTo;
//         }
//
//         public override bool Evaluate()
//         {
//             return false;
//         }
//
//         private void OnNPCTalkedTo(Interactable.NPC npc)
//         {
//             if (npc && npc.InteractableID == _targetID)
//             {
//                 SetMet();
//             }
//         }
//     }
// }