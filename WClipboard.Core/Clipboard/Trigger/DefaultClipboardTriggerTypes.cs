using System;

namespace WClipboard.Core.Clipboard.Trigger.Defaults
{
    public static class DefaultClipboardTriggerTypes
    {
        public static ClipboardTriggerType OS { get; } = new OSClipboardTriggerType(nameof(OS), "WindowsIcon");
        public static ClipboardTriggerType Copy { get;  } = new MergableClipboardTriggerType(nameof(Copy), "CopyIcon", TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5));
        public static ClipboardTriggerType Cut { get; } = new MergableClipboardTriggerType(nameof(Cut), "CutIcon", TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5));
        public static ClipboardTriggerType Paste { get; } = new ReferenceClipboardTriggerType(nameof(Paste), "PasteIcon");
    }
}
