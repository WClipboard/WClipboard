using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace WClipboard.Core.Utilities.Collections
{
    public class ObservableKeyedCollectionFunc<TKey, T> : INotifyPropertyChanged, INotifyCollectionChanged, IList<T>, IReadOnlyList<T>, IKeyedCollection<TKey, T> where TKey : notnull
    {
        private readonly ObservableCollection<T> _base;
        private readonly ConcurrentDictionary<TKey, T> _dict;
        private readonly Func<T, TKey> _keySelector;

        public int Count => _base.Count;
        bool ICollection<T>.IsReadOnly => false;
        bool ICollection.IsSynchronized => ((ICollection)_base).IsSynchronized;
        object ICollection.SyncRoot => ((ICollection)_base).SyncRoot;

        public T this[TKey key] => _dict[key];

        T IList<T>.this[int index]
        {
            get => _base[index];
            set
            {
                var old = _base[index];
                var oldKey = _keySelector(old);
                var newKey = _keySelector(value);

                if (Equals(oldKey, newKey))
                {
                    _dict[oldKey] = value;
                    _base[index] = value;
                }
                else if (_dict.TryAdd(newKey, value))
                {
                    _dict.TryRemove(oldKey, out var _);
                    _base[index] = value;
                }
                else
                {
                    ThrowKeyDuplicate(nameof(value));
                }
            }
        }

        T IReadOnlyList<T>.this[int index] => _base[index];

        public IEnumerable<TKey> Keys => _dict.Keys;

        IEnumerable<T> IReadOnlyDictionary<TKey, T>.Values => _dict.Values;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public ObservableKeyedCollectionFunc(Func<T, TKey> keySelector)
        {
            _base = new ObservableCollection<T>();
            _dict = new ConcurrentDictionary<TKey, T>();
            _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));

            _base.CollectionChanged += Base_CollectionChanged;
            ((INotifyPropertyChanged)_base).PropertyChanged += Base_PropertyChanged;
        }

        public ObservableKeyedCollectionFunc(Func<T, TKey> keySelector, IEnumerable<T> items) : this(keySelector)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var item in items)
            {
                Add(item);
            }
        }

        private void Base_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        private void Base_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(sender, e);
        }

        private static void ThrowKeyDuplicate(string paramName)
        {
            throw new ArgumentException("There is already a item with this key", paramName);
        }

        public bool Add(T item)
        {
            if (_dict.TryAdd(_keySelector(item), item))
            {
                _base.Add(item);
                return true;
            }
            return false;
        }

        void ICollection<T>.Add(T item)
        {
            if (!Add(item))
            {
                ThrowKeyDuplicate(nameof(item));
            }
        }

        public void Clear()
        {
            _dict.Clear();
            _base.Clear();
        }

        public bool Contains(T item)
        {
            return _dict.TryGetValue(_keySelector(item), out var value) && ReferenceEquals(item, value);
        }

        public bool ContainsKey(TKey key)
        {
            return _dict.ContainsKey(key);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _base.CopyTo(array, arrayIndex);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_base).CopyTo(array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _base.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _base.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (_dict.TryAdd(_keySelector(item), item))
            {
                _base.Insert(index, item);
            }
            else
            {
                ThrowKeyDuplicate(nameof(item));
            }
        }

        public bool Remove(TKey key)
        {
            if (_dict.TryRemove(key, out var dictItem))
            {
                _base.Remove(dictItem);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Remove(T item)
        {
            var key = _keySelector(item);
            if (_dict.TryGetValue(key, out var dictItem) && ReferenceEquals(item, dictItem))
            {
                _dict.TryRemove(key, out var _);
                _base.Remove(dictItem);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemoveAt(int index)
        {
            Remove(_base[index]);
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out  T value)
        {
            return _dict.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _base.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, T>> IEnumerable<KeyValuePair<TKey, T>>.GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        public TKey GetKey(T item) => _keySelector(item);
    }
}
