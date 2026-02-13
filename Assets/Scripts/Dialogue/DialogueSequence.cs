namespace FishingVillage.Dialogue
{
    public class DialogueSequence
    {
        private readonly SODialogueSequence _dialogueSequence;
        private int _currentIndex;

        public bool IsComplete => _currentIndex >= _dialogueSequence.Count;
        public DialogueAdvanceMode AdvanceMode => _dialogueSequence.AdvanceMode;
        public float AutoAdvanceDelay => _dialogueSequence.AutoAdvanceDelay;

        public DialogueSequence(SODialogueSequence sequence)
        {
            _dialogueSequence = sequence;
            _currentIndex = 0;
        }

        public string GetCurrentLine()
        {
            if (IsComplete) return string.Empty;
            return _dialogueSequence.GetLine(_currentIndex);
        }

        public void Advance()
        {
            if (!IsComplete) _currentIndex++;
        }

        public void Reset() => _currentIndex = 0;
    }
}