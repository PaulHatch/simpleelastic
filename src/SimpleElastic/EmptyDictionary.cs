using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleElastic
{
    /// <summary>
    /// Empty read-only dictionary which is completely immutable. Used as result for
    /// cases where no values were present.
    /// </summary>
    internal class EmptyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        private EmptyDictionary() { }
        public static IReadOnlyDictionary<TKey, TValue> Instance { get; } = new EmptyDictionary<TKey, TValue>();
        public TValue this[TKey key] => throw new KeyNotFoundException();
        public IEnumerable<TKey> Keys => Array.Empty<TKey>();
        public IEnumerable<TValue> Values => Array.Empty<TValue>();
        public int Count => 0;
        public bool ContainsKey(TKey key)
        {
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Array.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)Array.Empty<KeyValuePair<TKey, TValue>>())
                .GetEnumerator();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            return false;
        }
    }
}