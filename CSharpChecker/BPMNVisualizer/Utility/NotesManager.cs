namespace BPMNVisualizer.Utility
{
    public static class ElementNotes
    {
        private static readonly Dictionary<string, string> _notes = new();

        public static void SetNote(string activityId, string note)
        {
            if (string.IsNullOrWhiteSpace(note))
            {
                _notes.Remove(activityId);
            }
            else
            {
                _notes[activityId] = note.Trim();
            }
        }

        public static string GetNote(string activityId)
        {
            return _notes.TryGetValue(activityId, out var note) ? note : null;
        }
    }
}