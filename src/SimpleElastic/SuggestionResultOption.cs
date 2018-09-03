namespace SimpleElastic
{
    /// <summary>
    /// Represents a suggestion option.
    /// </summary>
    public class SuggestionResultOption
    {
        internal SuggestionResultOption(string text, string highlighted, float score)
        {
            Text = text;
            Highlighted = highlighted;
            Score = score;
        }

        /// <summary>
        /// Gets the text for this suggestion.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets a highlighted version of the suggestion option (if requested).
        /// </summary>
        public string Highlighted { get; }

        /// <summary>
        /// Gets the score for this suggestion.
        /// </summary>
        public float Score { get; }
    }
}