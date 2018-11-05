using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleElastic
{
    /// <summary>
    /// Represents a collection of key/value pairs. This type is similar to 
    /// a dictionary or <see cref="Map"/>, but permits multiple values using
    /// the same key.
    /// </summary>
    [JsonConverter(typeof(TableConverter))]
    public class Table : List<KeyValuePair<string, object>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Table"/> class.
        /// </summary>
        public Table()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Table"/> class with
        /// a single row.
        /// </summary>
        /// <param name="label">The label for the row.</param>
        /// <param name="value">The value for the row.</param>
        public Table(string label, object value)
        {
            Add(label, value);
        }


        /// <summary>
        /// Adds a new row to this table.
        /// </summary>
        /// <param name="label">The label of the row to add.</param>
        /// <param name="value">The value of the row to add.</param>
        public void Add(string label, object value)
        {
            Add(new KeyValuePair<string, object>(label, value));
        }

        /// <summary>
        /// Adds a new row to this table if the condition specified is true.
        /// </summary>
        /// <param name="label">The label of the row to add.</param>
        /// <param name="value">The value of the row to add.</param>
        /// <param name="condition">If true this value will be added,
        /// otherwise this call will be ignored.</param>
        public void Add(string label, object value, bool condition)
        {
            if (condition)
            {
                Add(new KeyValuePair<string, object>(label, value));
            }
        }

        /// <summary>
        /// Adds the specified row if the condition specified is true.
        /// </summary>
        /// <param name="label">The label of the row to add.</param>
        /// <param name="value">The value of the row to add.</param>
        /// <param name="condition">
        /// A predicate delegate which will be evaluated using the value provided. If true, this
        /// value will be added, otherwise it will be ignored.
        /// </param>
        public void Add(string label, object value, Predicate<object> condition)
        {
            if (condition(value))
            {
                Add(label, value);
            }
        }
    }
}
