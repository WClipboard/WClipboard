using System.Threading.Tasks;
using System.Windows.Input;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.Clipboard.Trigger.Defaults;
using WClipboard.Core.LifeCycle;
using WClipboard.Core.WPF.Clipboard;
using WClipboard.Core.WPF.Listeners;
using WClipboard.Core.WPF.Managers;
using WClipboard.Core.WPF.Models;
using WClipboard.Windows.Helpers;

namespace WClipboard.Core.WPF.Defaults
{
    public class ClipboardKeyListener : IAfterDIContainerBuildListener
    {
        private const int minimalDelayTime = 100;

        private readonly IClipboardObjectsManager _clipboardObjectsManager;
        private readonly IGlobalKeyEventsManager _globalKeyEventsManager;

        public ClipboardKeyListener(IClipboardObjectsManager clipboardObjectsManager, IGlobalKeyEventsManager globalKeyEventsManager)
        {
            _clipboardObjectsManager = clipboardObjectsManager;
            _globalKeyEventsManager = globalKeyEventsManager;
        }

        void IAfterDIContainerBuildListener.AfterDIContainerBuild()
        {
            _globalKeyEventsManager.AddListener(new GlobalKeyUpEventListener(Key.C, ModifierKeys.Control, OnEvent));
            _globalKeyEventsManager.AddListener(new GlobalKeyUpEventListener(Key.V, ModifierKeys.Control, OnEvent));
            _globalKeyEventsManager.AddListener(new GlobalKeyUpEventListener(Key.X, ModifierKeys.Control, OnEvent));
        }

        private async void OnEvent(GlobalKeyUpEventListener sender, long pressedTime)
        {
            var triggerType = sender.Key switch
            {
                Key.C => DefaultClipboardTriggerTypes.Copy,
                Key.V => DefaultClipboardTriggerTypes.Paste,
                Key.X => DefaultClipboardTriggerTypes.Cut,
                _ => null
            };

            if((sender.Key == Key.C || sender.Key == Key.X) && pressedTime < minimalDelayTime)
            {
                await Task.Delay((int)(minimalDelayTime - pressedTime)).ConfigureAwait(false);
            }

            if(!(triggerType is null))
            {
                _ = _clipboardObjectsManager.ProcessClipboardTrigger(new ClipboardTrigger(triggerType, WindowInfoHelper.GetForegroundWindowInfo(), new TimeKeyDownInfo(pressedTime)));
            }
        }
    }
}
