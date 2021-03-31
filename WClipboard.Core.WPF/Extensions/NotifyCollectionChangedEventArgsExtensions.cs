using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using WClipboard.Core.Extensions;

namespace WClipboard.Core.WPF.Extensions
{
    public static class NotifyCollectionChangedEventArgsExtensions
    {
        public static CollectionDifferences<T> GetDifferences<T>(this NotifyCollectionChangedEventArgs e)
        {
            return e.Action switch
            {
                NotifyCollectionChangedAction.Add => new CollectionDifferences<T>(added: e.NewItems?.Cast<T>()),
                NotifyCollectionChangedAction.Remove => new CollectionDifferences<T>(removed: e.OldItems?.Cast<T>()),
                NotifyCollectionChangedAction.Move => new CollectionDifferences<T>(),
                NotifyCollectionChangedAction.Replace => new CollectionDifferences<T>(added: e.NewItems?.Cast<T>(), removed: e.OldItems?.Cast<T>()),
                _ => throw new NotImplementedException($"{nameof(GetDifferences)} does not work with {e.Action}"),
            };
        }

        public static CollectionDifferences<object> GetDifferences(this NotifyCollectionChangedEventArgs e) => GetDifferences<object>(e);
    }

    public class CollectionDifferences<T>
    {
        public IReadOnlyCollection<T> Added { get;}
        public IReadOnlyCollection<T> Removed { get; }

        public CollectionDifferences(IReadOnlyCollection<T>? added = null, IReadOnlyCollection<T>? removed = null)
        {
            Added = added ?? Array.Empty<T>();
            Removed = removed ?? Array.Empty<T>();
        }
    }
}
