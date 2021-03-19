using System.Collections;
using System.Collections.Generic;

namespace WClipboard.Core.Utilities
{
    public interface IAddOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        void Add(TKey key, TValue value);

        new TValue this[TKey key] { get; set; }
    }

    public class AddOnlyDictionaryProxy<TKey, TValue> : IAddOnlyDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _base;

        public AddOnlyDictionaryProxy()
        {
            _base = new Dictionary<TKey, TValue>();
        }

        public AddOnlyDictionaryProxy(IDictionary<TKey, TValue> @base)
        {
            _base = @base;
        }

        public TValue this[TKey key]
        {
            get => _base[key];
            set => _base[key] = value;
        }
        public int Count => _base.Count;

        public ICollection<TKey> Keys => _base.Keys;
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _base.Keys;

        public ICollection<TValue> Values => _base.Values;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _base.Values;

        public void Add(TKey key, TValue value) => _base.Add(key, value);
        public bool ContainsKey(TKey key) => _base.ContainsKey(key);
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _base.GetEnumerator();
        public bool TryGetValue(TKey key, out TValue value) => _base.TryGetValue(key, out value);
        IEnumerator IEnumerable.GetEnumerator() => _base.GetEnumerator();
    }
}
