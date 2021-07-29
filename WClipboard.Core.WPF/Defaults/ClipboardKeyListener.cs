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

        private void OnEvent(GlobalKeyUpEventListener sender, long pressedTime)
        {
            var triggerType = sender.Key switch
            {
                Key.C => DefaultClipboardTriggerTypes.Copy,
                Key.V => DefaultClipboardTriggerTypes.Paste,
                Key.X => DefaultClipboardTriggerTypes.Cut,
                _ => null
            };

            if(!(triggerType is null))
            {
                var dataSource = WindowInfoHelper.GetClipboardOwnerWindowInfo();
                var foreground = WindowInfoHelper.GetForegroundWindowInfo();
                _ = _clipboardObjectsManager.ProcessClipboardTrigger(new ClipboardTrigger(triggerType, dataSource?.Item2, foreground?.Item2, foreground?.Item1, new TimeKeyDownInfo(pressedTime)));
            }
        }
    }
}
