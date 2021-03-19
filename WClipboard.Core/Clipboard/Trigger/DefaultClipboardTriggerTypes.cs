namespace WClipboard.Core.Clipboard.Trigger.Defaults
{
    public static class DefaultClipboardTriggerTypes
    {
        public static ClipboardTriggerType OS { get; } = new ClipboardTriggerType(nameof(OS), "SettingsIcon", ClipboardTriggerSourceType.Intern, 0);
        public static ClipboardTriggerType Copy { get;  } = new ClipboardTriggerType(nameof(Copy), "CopyIcon", ClipboardTriggerSourceType.Extern, 2000);
        public static ClipboardTriggerType Cut { get; } = new ClipboardTriggerType(nameof(Cut), "CutIcon", ClipboardTriggerSourceType.Extern, 2000);
        public static ClipboardTriggerType Paste { get; } = new ClipboardTriggerType(nameof(Paste), "PasteIcon", ClipboardTriggerSourceType.Extern, 1000);
    }
}
