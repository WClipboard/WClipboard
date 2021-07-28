using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Threading;
using WClipboard.Core.Utilities.Collections;

namespace WClipboard.Core.WPF.Utilities
{
    public class ConcurrentBindableList<T> : ConcurrentObservableList<T>
    {
        public ConcurrentBindableList()
        {
        }

        public ConcurrentBindableList(IEnumerable<T> items) : base(items)
        {
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            var notifyCollectionChangedEventHandler = GetCollectionChangedEventHandler();

            if (notifyCollectionChangedEventHandler == null)
                return;

            foreach (NotifyCollectionChangedEventHandler handler in notifyCollectionChangedEventHandler.GetInvocationList())
            {
                if (handler.Target is DispatcherObject dispatcherObject && !dispatcherObject.CheckAccess())
                {
                    readWriteLock.EnterReadForThread(dispatcherObject.Dispatcher.Thread);
                    dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, InvokeHandlerAsyncDelegateValue, this, handler, args);
                }
                else
                {
                    handler(this, args);
                }
            }
        }

        private delegate void InvokeHandlerAsyncDelegate(ConcurrentBindableList<T> me, NotifyCollectionChangedEventHandler handler, NotifyCollectionChangedEventArgs args);
        private readonly static InvokeHandlerAsyncDelegate InvokeHandlerAsyncDelegateValue = new InvokeHandlerAsyncDelegate(InvokeHandlerAsync);
        private static void InvokeHandlerAsync(ConcurrentBindableList<T> me, NotifyCollectionChangedEventHandler handler, NotifyCollectionChangedEventArgs args)
        {
            handler(me, args);
            me.readWriteLock.ExitRead();
        }
    }
}
