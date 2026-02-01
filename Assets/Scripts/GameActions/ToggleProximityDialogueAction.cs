using System;
using DNExtensions.Utilities.PrefabSelector;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
[SerializableSelectorName("Toggle Proximity Dialogue", "NPC")]
public class ToggleProximityDialogueAction : GameAction
{
    [SerializeField, PrefabSelector("Assets/Prefabs/Npcs")]  private NPC npc;
    [SerializeField] private bool allowProximityDialogue;
    
    public override string ActionName => npc ? $"Toggle {npc.Name} proximity dialogue" : $"Toggle NPC proximity dialogue (No NPC was set)";
    
    public override void Execute()
    {
        if (npc)
        {
            var sceneNpc = FindNpcInScene(npc.InteractableID);
            if (sceneNpc)
            {
                sceneNpc.EnableProximityDialogue(allowProximityDialogue);
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