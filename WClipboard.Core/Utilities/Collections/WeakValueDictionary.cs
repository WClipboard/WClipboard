using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace WClipboard.Core.Utilities.Collections
{
    public class WeakValueDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TValue : class
    {
        private readonly IDictionary<TKey, WeakReference<TValue>> dictionary;

        public TValue this[TKey key] {
            get
            {
                if (dictionary.TryGetValue(key, out var refValue))
                {
                    if (refValue.TryGetTarget(out var value))
                    {
                        return value;
                    } 
                    else
                    {
                        dictionary.Remove(key);
                    }
                }
                throw new KeyNotFoundException();
            }
            set
            {
                dictionary[key] = new WeakReference<TValue>(value);
            }
        }

        public ICollection<TKey> Keys => dictionary.Keys;

        public ICollection<TValue> Values => throw new NotSupportedException();

        public int Count => dictionary.Count;

        public bool IsReadOnly => false;

        public WeakValueDictionary(IDictionary<TKey, WeakReference<TValue>> dictionary)
        {
            this.dictionary = dictionary;
        }

        public WeakValueDictionary(IDictionary<TKey, TValue> source, IDictionary<TKey, WeakReference<TValue>> dictionary)
        {
            this.dictionary = dictionary;
            foreach (var item in source)
            {
                this.dictionary.Add(item.Key, new WeakReference<TValue>(item.Value));
            }
        }
 
        public void Add(TKey key, TValue value)
        {
            if(dictionary.TryGetValue(key, out var refValue) && !refValue.TryGetTarget(out var _))
            {
                refValue.SetTarget(value);
            } else
            {
                dictionary.Add(key, new WeakReference<TValue>(value));
            }
        }
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (dictionary.TryGetValue(item.Key, out var refValue) && !refValue.TryGetTarget(out var _))
            {
                refValue.SetTarget(item.Value);
            }
            else
            {
                dictionary.Add(item.Key, new WeakReference<TValue>(item.Value));
            }
        }
        public void Clear() => dictionary.Clear();
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (dictionary.TryGetValue(item.Key, out var refValue))
            {
                if(refValue.TryGetTarget(out var value))
                {
                    return value == item.Value;
                } 
                else
                {
                    dictionary.Remove(item.Key);
                }
            }
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            if (dictionary.TryGetValue(key, out var refValue))
            {
                if (refValue.TryGetTarget(out var _))
                {
                    return true;
                }
                else
                {
                    dictionary.Remove(key);
                }
            }
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            for(int i = 0; i < array.Length; i++)
            {

            }
        }
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dictionary.Select(kv =>
            {
                kv.Value.TryGetTarget(out var value);
                return new KeyValuePair<TKey, TValue>(kv.Key, value);
            }).Where(kv => kv.Value != null).GetEnumerator();
        }

        public bool Remove(TKey key) => dictionary.Remove(key);
        public bool Remove(KeyValuePair<TKey, TValue> item) => Contains(item) && Remove(item.Key);
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (dictionary.TryGetValue(key, out var refValue))
            {
                if (refValue.TryGetTarget(out value))
                {
                    return true;
                }
                else
                {
                    dictionary.Remove(key);
                }
            }
            value = null;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
