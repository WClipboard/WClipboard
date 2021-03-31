using System.Collections;
using System.Collections.Generic;

namespace WClipboard.Core.Utilities.Collections
{
    public interface IReadOnlyKeyedCollection<TKey, T> : IEnumerable<T>, IReadOnlyCollection<T>, IEnumerable, IReadOnlyDictionary<TKey, T>
    {
        TKey GetKey(T item);
        bool Contains(T item);
    }

    public interface IKeyedCollection<TKey, T> : IReadOnlyKeyedCollection<TKey, T>, ICollection<T>, ICollection 
    {
        bool Remove(TKey key);
    }
}
