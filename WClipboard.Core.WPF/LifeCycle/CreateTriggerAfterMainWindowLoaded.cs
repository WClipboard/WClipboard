using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.Clipboard.Trigger.Defaults;
using WClipboard.Core.WPF.Clipboard;

namespace WClipboard.Core.WPF.LifeCycle
{
    internal class CreateTriggerAfterMainWindowLoaded : IAfterMainWindowLoadedListener
    {
        private readonly IClipboardObjectsManager clipboardObjectsManager;

        public CreateTriggerAfterMainWindowLoaded(IClipboardObjectsManager clipboardObjectsManager)
        {
            this.clipboardObjectsManager = clipboardObjectsManager;
        }

        void IAfterMainWindowLoadedListener.AfterMainWindowLoaded() => clipboardObjectsManager.ProcessClipboardTrigger(new ClipboardTrigger(DefaultClipboardTriggerTypes.OS, null));
    }
}
