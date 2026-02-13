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


        protected NPC FindNpcInScene(NPC npcPrefab)
        {
            if (!npcPrefab) return null;
            
            if (!npcPrefab.TryGetComponent<IdentifiableInteractable>(out var identifiable))
            {
                Debug.LogError($"NPC {npcPrefab.Name} is missing IdentifiableInteractable component!");
                return null;
            }

            var allNpCs = UnityEngine.Object.FindObjectsByType<NPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var npc in allNpCs)
            {
                if (npc.TryGetComponent<IdentifiableInteractable>(out var sceneIdentifiable) && sceneIdentifiable.ID == identifiable.ID)
                {
                    return npc;
                }
            }
            
            return null;
        }
        
        protected IInteractable FindInteractableInScene(string id)
        {
            var identifiables = UnityEngine.Object.FindObjectsByType<IdentifiableInteractable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var identifiable in identifiables)
            {
                if (identifiable.ID == id && identifiable.TryGetComponent<IInteractable>(out var interactable))
                {
                    return interactable;
                }
            }
            return null;
        }
    }
}