using System;

namespace WClipboard.Core.Clipboard.Trigger
{
    public sealed class ClipboardTriggerType
    {
        public string Name { get; }
        public object IconSource { get; }
        public ClipboardTriggerSourceType Source { get; }
        public int Priority { get; }

        public ClipboardTriggerType(string name, object iconSource, ClipboardTriggerSourceType source, int priority = 0)
        {
            if (priority < 0)
                throw new ArgumentOutOfRangeException(nameof(priority), $"The minimal {nameof(priority)} is 0");
            if (source != ClipboardTriggerSourceType.Extern && priority != 0)
                throw new ArgumentException($"The {nameof(priority)} must be 0 if {nameof(source)} is not {nameof(ClipboardTriggerSourceType.Extern)}", nameof(priority));

            Name = name ?? throw new ArgumentNullException(nameof(name));
            IconSource = iconSource ?? throw new ArgumentNullException(nameof(iconSource));
            Priority = priority;
            Source = source;
        }
    }
}
