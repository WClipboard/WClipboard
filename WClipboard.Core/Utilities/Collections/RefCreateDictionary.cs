using System;
using System.Collections.Generic;

namespace WClipboard.Core.Utilities.Collections
{
    public class RefCreateDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TKey : notnull
    {
        private readonly Func<TKey, TValue> _createFunc;

        public RefCreateDictionary(Func<TKey, TValue> createFunc)
        {
            _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
        }

        public new TValue this[TKey key]
        {
            get
            {
                if(!TryGetValue(key, out var value))
                {
                    value = _createFunc(key);
                    Add(key, value);
                }
                return value;
            }
            set
            {
                base[key] = value;
            }
        }
    }

    public static class RefCreateDictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
        {
            if (!dictionary.TryGetValue(key, out var value))
            {
                value = new TValue();
                dictionary.Add(key, value);
            }
            return value;
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> createFunc)
        {
            if (!dictionary.TryGetValue(key, out var value))
            {
                value = createFunc();
                dictionary.Add(key, value);
            }
            return value;
        }
    }
}
