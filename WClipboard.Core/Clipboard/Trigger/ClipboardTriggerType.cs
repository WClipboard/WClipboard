using System;

namespace WClipboard.Core.Clipboard.Trigger
{
    public class ClipboardTriggerType
    {
        public string Name { get; }
        public object IconSource { get; }

        public ClipboardTriggerType(string name, object iconSource)
        {
            Name = name;
            IconSource = iconSource;
        }
    }

    public class ReferenceClipboardTriggerType : ClipboardTriggerType {
        public ReferenceClipboardTriggerType(string name, object iconSource) : base(name, iconSource) { }
    }
    public class OSClipboardTriggerType : ClipboardTriggerType {
        public OSClipboardTriggerType(string name, object iconSource) : base(name, iconSource) { }
    }
    public class MergableClipboardTriggerType : ClipboardTriggerType {
        public TimeSpan MergeTimeout { get; }
        public TimeSpan MergeBefore { get; }

        public MergableClipboardTriggerType(string name, object iconSource, TimeSpan mergeTimeout, TimeSpan mergeBefore) : base(name, iconSource) {
            if (mergeTimeout <= TimeSpan.Zero)
                throw new ArgumentException($"{nameof(mergeTimeout)} must be larger than 0", nameof(mergeTimeout));

            MergeTimeout = mergeTimeout;
            MergeBefore = mergeBefore;
        }
    }
    public class CustomClipboardTriggerType : ClipboardTriggerType {
        public CustomClipboardTriggerType(string name, object iconSource) : base(name, iconSource) { }
    }
}
