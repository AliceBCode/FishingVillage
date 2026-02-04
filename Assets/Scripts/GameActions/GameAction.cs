using System;
using UnityEngine;

[Serializable]
public abstract class GameAction
{
    public abstract string ActionName { get; }
    public abstract void Execute();
    
    
    protected NPC FindNpcInScene(string id)
    {
        var allNpCs = UnityEngine.Object.FindObjectsByType<NPC>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
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