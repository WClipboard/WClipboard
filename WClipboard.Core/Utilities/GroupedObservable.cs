using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WClipboard.Core.Utilities
{
    public class GroupedObservable<TKey, TElement> : IGrouping<TKey, TElement>
    {
        public TKey Key { get; }
        public ObservableCollection<TElement> Elements { get; }

        IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() => Elements.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Elements.GetEnumerator();

        public GroupedObservable(IGrouping<TKey, TElement> linqGroup)
        {
            Key = linqGroup.Key;
            Elements = new ObservableCollection<TElement>(linqGroup);
        }

        public GroupedObservable(TKey key, IEnumerable<TElement> elements)
        {
            Key = key;
            Elements = elements as ObservableCollection<TElement> 
                ?? new ObservableCollection<TElement>(elements);
        }
    }
}
