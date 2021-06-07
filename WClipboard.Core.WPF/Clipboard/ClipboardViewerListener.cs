using System;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.Clipboard.Trigger.Defaults;
using WClipboard.Core.LifeCycle;
using WClipboard.Windows;
using WClipboard.Windows.Helpers;

namespace WClipboard.Core.WPF.Clipboard
{
    public class ClipboardViewerListener : IAfterDIContainerBuildListener
    {
        private readonly IClipboardObjectsManager clipboardObjectsManager;

        public ClipboardViewerListener(IClipboardViewer clipboardViewer, IClipboardObjectsManager clipboardObjectsManager)
        {
            this.clipboardObjectsManager = clipboardObjectsManager;
            clipboardViewer.ClipboardChanged += ClipboardViewer_ClipboardChanged;
        }

        private void ClipboardViewer_ClipboardChanged(object? sender, EventArgs e)
        {
            var info = WindowInfoHelper.GetForegroundOrClipboardOwnerInfo();
            var _ = clipboardObjectsManager.ProcessClipboardTrigger(new ClipboardTrigger(DefaultClipboardTriggerTypes.OS, info?.Item2, info?.Item1));
        }

        public void AfterDIContainerBuild()
        {
            
        }
    }
}
