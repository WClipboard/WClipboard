using WClipboard.Core.Clipboard.Trigger;

namespace WClipboard.Core.WPF.Clipboard.Trigger
{
    public sealed class ResolvedClipboardTrigger
    {
        public ClipboardTrigger Trigger { get; }
        public ClipboardObject? Object { get; }
        public ResolvedClipboardTriggerType ResolvedType { get; }

        internal ResolvedClipboardTrigger(ClipboardTrigger trigger, ClipboardObject? clipboardObject, ResolvedClipboardTriggerType resolvedType)
        {
            Trigger = trigger;
            Object = clipboardObject;
            ResolvedType = resolvedType;
        }
    }
}
