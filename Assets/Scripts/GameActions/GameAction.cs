using System;
using FishingVillage.Interactable;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.GameActions
{
    [Serializable]
    [MovedFrom("")]
    public abstract class GameAction
    {
        public abstract string ActionName { get; }
        public abstract void Execute();


        protected NPC FindNpcInScene(string id)
        {
            var allNpCs = UnityEngine.Object.FindObjectsByType<NPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var npc in allNpCs)
            {
                if (npc.InteractableID == id)
                {
                    return npc;
                }
            }
            return null;
        }
    }
}