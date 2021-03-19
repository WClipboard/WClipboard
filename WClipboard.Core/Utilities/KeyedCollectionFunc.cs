using System;
using System.Collections;
using System.Collections.Generic;

namespace WClipboard.Core.Utilities
{
    public class KeyedCollectionFunc<TKey, T> : IKeyedCollection<TKey, T>
    {
        private readonly IDictionary<TKey, T> _base;
        private readonly Func<T, TKey> _keySelector;

        public int Count => _base.Count;
        public bool IsReadOnly => false;

        bool ICollection.IsSynchronized => ((ICollection)_base).IsSynchronized;
        object ICollection.SyncRoot => ((ICollection)_base).SyncRoot;

        public IEnumerable<TKey> Keys => _base.Keys;

        IEnumerable<T> IReadOnlyDictionary<TKey, T>.Values => _base.Values;

        public T this[TKey key] => _base[key];

        public KeyedCollectionFunc(Func<T, TKey> keySelector, IDictionary<TKey, T> proxyBase)
        {
            _base = proxyBase;
            _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        }

        public KeyedCollectionFunc(Func<T, TKey> keySelector) : this(keySelector, new Dictionary<TKey, T>()) { }

        public KeyedCollectionFunc(Func<T, TKey> keySelector, IDictionary<TKey, T> proxyBase, IEnumerable<T> items) : this(keySelector, proxyBase)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            foreach (var item in items)
            {
                Add(item);
            }
        }

        public KeyedCollectionFunc(Func<T, TKey> keySelector, IEnumerable<T> items) : this(keySelector, new Dictionary<TKey, T>(), items) { }

        public bool Add(T item)
        {
            var key = _keySelector(item);
            if (_base.ContainsKey(key))
            {
                return false;
            }
            else
            {
                _base.Add(key, item);
                return true;
            }
        }

        void ICollection<T>.Add(T item)
        {
            if (!Add(item))
                throw new ArgumentException("There is already an item with that key", nameof(item));
        }

        public void Clear()
        {
            _base.Clear();
        }

        public bool Contains(T item)
        {
            return _base.TryGetValue(_keySelector(item), out var dictItem) && ReferenceEquals(item, dictItem);
        }

        public bool ContainsKey(TKey key)
        {
            return _base.ContainsKey(key);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _base.Values.CopyTo(array, arrayIndex);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_base.Values).CopyTo(array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _base.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _base.Values.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, T>> IEnumerable<KeyValuePair<TKey, T>>.GetEnumerator()
        {
            return _base.GetEnumerator();
        }

        public bool Remove(T item)
        {
            if (Contains(item))
            {
                return _base.Remove(_keySelector(item));
            }
            else
            {
                return false;
            }
        }

        public bool Remove(TKey key)
        {
            return _base.Remove(key);
        }

        public bool TryGetValue(TKey key, out T value)
        {
            return _base.TryGetValue(key, out value);
        }

        public TKey GetKey(T item)
        {
            return _keySelector(item);
        }
    }
}
