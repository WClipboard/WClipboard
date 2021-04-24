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
            clipboardObjectsManager.ProcessClipboardTrigger(new ClipboardTrigger(DefaultClipboardTriggerTypes.OS, WindowInfoHelper.GetForegroundWindowInfo()));
        }

        public void AfterDIContainerBuild()
        {
            
        }
    }
}
