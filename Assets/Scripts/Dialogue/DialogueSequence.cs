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

        public string GetNextLine()
        {
            if (IsComplete) return string.Empty;

            string line = _dialogueSequence.GetLine(_currentIndex);
            _currentIndex++;
            return line;
        }

        public void Reset() => _currentIndex = 0;
    }
}