using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
[AddTypeMenu("Start NPC Dialogue")]
public class StartDialogueAction : GameAction
{
    [SerializeField] private NPC npc;
    [SerializeField] private SODialogueSequence dialogue;
    
    public override string ActionName => npc ? $"Start Dialogue with {npc.Name}" : "Start Dialogue (No NPC was set)";
    
    public override void Execute()
    {
        if (npc && dialogue)
        {
            var sceneNpc = FindNpcInScene(npc.InteractableID);
            if (sceneNpc)
            {
                sceneNpc.StartDialogueSequence(dialogue);
            }
            else
            {
                Debug.LogWarning($"Could not find NPC with ID {npc.InteractableID} in scene!");
            }
        }
    }
    
    private NPC FindNpcInScene(string id)
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