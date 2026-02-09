using System;
using DNExtensions;
using DNExtensions.Utilities;
using UnityEngine;

namespace FishingVillage.Dialogue
{
    public enum DialogueLinesType
    {
        Random = 0,
        Sequential = 1,

    }

    [CreateAssetMenu(fileName = "New Dialogue Lines", menuName = "Scriptable Objects/Dialogue Lines")]
    public class SODialogueLines : ScriptableObject
    {

        [SerializeField] private ChanceList<string> dialogueLines;


        public int Count => dialogueLines.Count;

        public string GetRandomLine => dialogueLines.GetRandomItem();
    }
}