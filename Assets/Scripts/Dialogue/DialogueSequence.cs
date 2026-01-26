public class DialogueSequence
{
    private readonly SODialogueSequence dialogueSequence;
    private int currentIndex;
    
    public bool IsComplete => currentIndex >= dialogueSequence.Count;
    public DialogueAdvanceMode AdvanceMode => dialogueSequence.AdvanceMode;
    public float AutoAdvanceDelay => dialogueSequence.AutoAdvanceDelay;
    
    public DialogueSequence(SODialogueSequence sequence)
    {
        dialogueSequence = sequence;
        currentIndex = 0;
    }
    
    public string GetNextLine()
    {
        if (IsComplete) return string.Empty;
        
        string line = dialogueSequence.GetLine(currentIndex);
        currentIndex++;
        return line;
    }
    
    public void Reset() => currentIndex = 0;
}