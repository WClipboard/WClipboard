using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Data;
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
                InvokeNotifyCollectionChangedEventHandler(args, handler);
            }
        }

        private void InvokeNotifyCollectionChangedEventHandler(NotifyCollectionChangedEventArgs args, NotifyCollectionChangedEventHandler handler)
        {
            if (handler.Target is CollectionView && ((args.NewItems?.Count ?? 0) > 1 || (args.OldItems?.Count ?? 0) > 1)) //Fix ranges not supported
            {
                if(args.Action == NotifyCollectionChangedAction.Replace && args.NewItems != null && args.OldItems != null)
                {
                    for(var currentIndex = 0; currentIndex < args.NewItems.Count; currentIndex++)
                    {
                        InvokeNotifyCollectionChangedEventHandler(new NotifyCollectionChangedEventArgs(args.Action, args.NewItems[currentIndex], args.OldItems[currentIndex], args.NewStartingIndex + currentIndex), handler);
                    }
                }

                if((args.NewItems?.Count ?? 0) > 1) { 
                    var newStartingIndex = args.NewStartingIndex;

                    foreach(var newItem in args.NewItems!)
                    {
                        InvokeNotifyCollectionChangedEventHandler(new NotifyCollectionChangedEventArgs(args.Action, newItem, newStartingIndex), handler);
                        newStartingIndex += 1;
                    }
                }

                if ((args.OldItems?.Count ?? 0) > 1) {
                    var oldStartingIndex = args.OldStartingIndex;

                    foreach(var oldItem in args.OldItems!)
                    {
                        InvokeNotifyCollectionChangedEventHandler(new NotifyCollectionChangedEventArgs(args.Action, oldItem, oldStartingIndex), handler);
                        oldStartingIndex += 1;
                    }
                }

                return;
            }

            if (handler.Target is DispatcherObject dispatcherObject && !dispatcherObject.CheckAccess()) //Fix threading access issues
            {
                readWriteLock.EnterReadForThread(dispatcherObject.Dispatcher.Thread);
                dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, InvokeHandlerAsyncDelegateValue, this, handler, args);
            }
            else
            {
                handler(this, args);
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
