using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.Utilities;
using WClipboard.Core.WPF.Clipboard.Trigger;
using SysClipboard = System.Windows.Clipboard;

namespace WClipboard.Core.WPF.Clipboard
{
    internal class ClipboardClonerThread : IDisposable
    {
        private readonly IClipboardObjectsManager _clipboardObjectsManager;
        private readonly ILogger<ClipboardClonerThread> _logger;

        private readonly BlockingCollection<ClipboardTriggerQueueItem> _triggerQueue;
        private bool _disposedValue;

        public ClipboardClonerThread(IClipboardObjectsManager clipboardObjectsManager, ILogger<ClipboardClonerThread> logger)
        {
            _logger = logger;
            _clipboardObjectsManager = clipboardObjectsManager;
            _triggerQueue = new BlockingCollection<ClipboardTriggerQueueItem>();

            // Uses own thread to dequeue synchronic (no racing conditions) and for acces of Clipboard (STA thread, instead of MainThread)
            var thread = new Thread(Run);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Name = nameof(ClipboardObjectsManager) + nameof(Thread);
            thread.Start();
        }

        private void Run()
        {
            foreach (var queueItem in _triggerQueue.GetConsumingEnumerable())
            {
                try
                {
                    queueItem.Task.SetResult(_clipboardObjectsManager.ProcessClipboardTrigger(queueItem.Trigger, SysClipboard.GetDataObject()));
                }
                catch(Exception ex)
                {
                    _logger.Log(LogLevel.Info, "An exception occured while processing clipboard trigger", ex);
                    queueItem.Task.SetException(ex);
                }
            }
        }

        public Task<ResolvedClipboardTrigger> ProcessClipboardTrigger(ClipboardTrigger trigger)
        {
            var queueItem = new ClipboardTriggerQueueItem(trigger);
            _triggerQueue.Add(queueItem);
            return queueItem.Task.Task;
        }

        private class ClipboardTriggerQueueItem
        {
            public ClipboardTrigger Trigger { get; }
            public TaskCompletionSource<ResolvedClipboardTrigger> Task { get; }

            public ClipboardTriggerQueueItem(ClipboardTrigger trigger)
            {
                Trigger = trigger;
                Task = new TaskCompletionSource<ResolvedClipboardTrigger>();
            }
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    //This ends the thread since the foreach loop will be breaked by this line
                    _triggerQueue.CompleteAdding();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
