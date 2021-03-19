using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;

namespace WClipboard.Core.WPF.Utilities
{
    public class BindableObservableCollection<T> : ObservableCollection<T>
    {
        public BindableObservableCollection(SynchronizationContext synchronizationContext)
        {
            SynchronizationContext = synchronizationContext;
        }

        public BindableObservableCollection(IEnumerable<T> collection, SynchronizationContext synchronizationContext) : base(collection)
        {
            SynchronizationContext = synchronizationContext;
        }

        public BindableObservableCollection(List<T> list, SynchronizationContext synchronizationContext) : base(list)
        {
            SynchronizationContext = synchronizationContext;
        }

        public SynchronizationContext SynchronizationContext { get; }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (SynchronizationContext is null)
                base.OnCollectionChanged(e);
            else
                SynchronizationContext.Post(SynchronizedOnCollectionChangedCall, e);
        }

        private void SynchronizedOnCollectionChangedCall(object? state)
        {
            if(state is NotifyCollectionChangedEventArgs e)
            {
                base.OnCollectionChanged(e);
            }
        }
    }
}
