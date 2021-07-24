using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.Clipboard.Trigger.Defaults;
using WClipboard.Core.WPF.Clipboard;
using WClipboard.Core.WPF.ViewModels;
using WClipboard.Windows.Helpers;

namespace WClipboard.Core.WPF.LifeCycle
{
    internal class CreateTriggerAfterMainWindowLoaded : IAfterMainWindowLoadedListener
    {
        private readonly IClipboardObjectsManager clipboardObjectsManager;

        public CreateTriggerAfterMainWindowLoaded(IClipboardObjectsManager clipboardObjectsManager)
        {
            this.clipboardObjectsManager = clipboardObjectsManager;
        }

        void IAfterMainWindowLoadedListener.AfterMainWindowLoaded(IMainWindowViewModel mainWindow) {
            var dataSource = WindowInfoHelper.GetClipboardOwnerWindowInfo();
            var foreground = WindowInfoHelper.GetFromWpfWindow(mainWindow.Window);

            clipboardObjectsManager.ProcessClipboardTrigger(new ClipboardTrigger(DefaultClipboardTriggerTypes.OS, dataSource?.Item2, foreground?.Item2, foreground?.Item1));
        }
    }
}
