using System.Threading.Tasks;
using WClipboard.Core.Clipboard.Trigger;

namespace WClipboard.Core.WPF.Clipboard
{
    internal class ClipboardTriggerScheduler
    {
        private ClipboardTrigger? _lastOSTrigger;
        private ClipboardTrigger? _currentMergableTrigger;

        private readonly ClipboardClonerThread clipboardClonerThread;

        public ClipboardTriggerScheduler(ClipboardClonerThread clipboardClonerThread)
        {
            this.clipboardClonerThread = clipboardClonerThread;
        }

        public async Task Schedule(ClipboardTrigger trigger)
        {
            if (trigger.Type is OSClipboardTriggerType)
            {
                lock (this)
                {
                    if (_currentMergableTrigger != null)
                    {
                        if (trigger.When >= _currentMergableTrigger.When && trigger.When <= _currentMergableTrigger.When + ((MergableClipboardTriggerType)_currentMergableTrigger.Type).MergeTimeout)
                        {
                            trigger.Merge(_currentMergableTrigger);
                            _currentMergableTrigger = null;
                        }
                    }

                    _lastOSTrigger = trigger;
                }
            }
            else if (trigger.Type is MergableClipboardTriggerType mergable)
            {
                lock (this)
                {
                    if (_currentMergableTrigger != null)
                    {
                        clipboardClonerThread.ProcessClipboardTrigger(_currentMergableTrigger).ConfigureAwait(false);
                        _currentMergableTrigger = null;
                    }

                    if (_lastOSTrigger != null)
                    {
                        if (_lastOSTrigger.When >= trigger.When - mergable.MergeBefore && _lastOSTrigger.When <= trigger.When)
                        {
                            _lastOSTrigger.Merge(trigger);
                            _lastOSTrigger = null;
                            //exit method, trigger is already merged
                            return;
                        }
                    }

                    _currentMergableTrigger = trigger;
                }

                await Task.Delay(trigger.When + mergable.MergeTimeout - System.DateTime.Now).ConfigureAwait(false);

                lock (this)
                {
                    if (trigger == _currentMergableTrigger)
                    {
                        _currentMergableTrigger = null;
                        //exit lock and resolve
                    } 
                    else
                    {
                        //exit method, trigger is already merged
                        return;
                    }
                }
            }

            await clipboardClonerThread.ProcessClipboardTrigger(trigger).ConfigureAwait(false);
        }
    }
}
