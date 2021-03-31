using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WClipboard.Core.Utilities.Collections;

namespace WClipboard.Core.Utilities
{
    public interface ICollectionManager<TKey, T> : IReadOnlyKeyedCollection<TKey, T>
    {
    }

    public abstract class CollectionManager<TKey, T> : ICollectionManager<TKey, T> where TKey : notnull
    {
        protected readonly KeyedCollectionFunc<TKey, T> _collection;

        public int Count => _collection.Count;
        public T this[TKey key] => _collection[key];
        public IEnumerable<TKey> Keys => _collection.Keys;
        IEnumerable<T>  IReadOnlyDictionary<TKey, T>.Values => _collection;

        protected CollectionManager(Func<T, TKey> keySelector, IEnumerable<T> items)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            _collection = new KeyedCollectionFunc<TKey, T>(keySelector);

            foreach (var item in items)
            {
                if (!_collection.Add(item))
                {
                    throw new ArgumentException($"A {typeof(T).Name} with key {keySelector(item)} is already added", nameof(items));
                }
            }
        }

        public bool ContainsKey(TKey key)
        {
            return _collection.ContainsKey(key);
        }

        public bool Contains(T item)
        {
            return _collection.Contains(item);
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out T value)
        {
            return _collection.TryGetValue(key, out value);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, T>> IEnumerable<KeyValuePair<TKey, T>>.GetEnumerator()
        {
            return ((IReadOnlyDictionary<TKey, T>)_collection).GetEnumerator();
        }

        public TKey GetKey(T item) => _collection.GetKey(item);
    }
}
