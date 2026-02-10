using UnityEngine;

namespace FishingVillage.Dialogue
{
    public enum DialogueAdvanceMode
    {
        Automatic = 0,
        Manual = 1
    }

    [CreateAssetMenu(fileName = "New Dialogue Sequence", menuName = "Scriptable Objects/Dialogue Sequence")]
    public class SODialogueSequence : ScriptableObject
    {
        [SerializeField] private DialogueAdvanceMode advanceMode = DialogueAdvanceMode.Manual;
        [SerializeField] private float autoAdvanceDelay = 2f;
        [SerializeField] private string[] dialogueLines;


        public int Count => dialogueLines.Length;
        public DialogueAdvanceMode AdvanceMode => advanceMode;
        public float AutoAdvanceDelay => autoAdvanceDelay;

        public string GetLine(int index)
        {
            var line = "";

            if (index < 0 || index >= dialogueLines.Length) return line;

            line = dialogueLines[index];

            return line;
        }
    }
}