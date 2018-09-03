using System.Collections.Generic;

namespace SimpleElastic
{
    /// <summary>
    /// Represents a suggestion.
    /// </summary>
    public class SuggestionResult
    {
        internal SuggestionResult(
            string text, 
            int offset, 
            int length, 
            IEnumerable<SuggestionResultOption> options)
        {
            Text = text;
            Offset = offset;
            Length = length;
            Options = options;
        }

        /// <summary>
        /// Gets the suggestion text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the suggestion offset.
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Gets the suggestion length.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Gets the suggested options.
        /// </summary>
        public IEnumerable<SuggestionResultOption> Options { get; }

    }
}