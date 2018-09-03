using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleElastic
{
    /// <summary>
    /// When implemented by a search result class, the document
    /// score from a search query will be assigned to <see cref="Score"/>.
    /// </summary>
    public interface IScoreDocument
    {
        /// <summary>
        /// Gets the score for this document.
        /// </summary>
        float? Score { get; set; }
    }
}
