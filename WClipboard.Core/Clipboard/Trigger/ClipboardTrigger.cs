using System;
using System.Collections.Generic;
using WClipboard.Core.Utilities;
using WClipboard.Core.Utilities.Collections;

namespace WClipboard.Core.Clipboard.Trigger
{
    public sealed class ClipboardTrigger : BindableBase
    {
        public DateTime When { get; }
        public WindowInfo? WindowInfo { get; }

        public ObservableObjectTypeCollection AdditionalInfo { get; }

        private ClipboardTriggerType _type;
        public ClipboardTriggerType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public ClipboardTrigger(DateTime when, ClipboardTriggerType type, WindowInfo? windowInfo, IEnumerable<object> additionalInfo)
        {
            When = when;
            WindowInfo = windowInfo;
            _type = type;
            AdditionalInfo = new ObservableObjectTypeCollection(additionalInfo ?? Array.Empty<object>());
        }

        public ClipboardTrigger(DateTime when, ClipboardTriggerType type, WindowInfo? windowInfo, params object[] additionalInfo) : this(when, type, windowInfo, (IEnumerable<object>)additionalInfo) { }
        public ClipboardTrigger(ClipboardTriggerType type, WindowInfo? windowInfo, IEnumerable<object> additionalInfo) : this(DateTime.Now, type, windowInfo, additionalInfo) { }
        public ClipboardTrigger(ClipboardTriggerType type, WindowInfo? windowInfo, params object[] additionalInfo) : this(DateTime.Now, type, windowInfo, additionalInfo) { }

        public bool TryMerge(ClipboardTrigger other)
        {
            switch (Type.Source)
            {
                case ClipboardTriggerSourceType.Custom: //Custom cannot be merged
                case ClipboardTriggerSourceType.Extern: //Extern cannot be merged
                    return false;
                //case ClipboardTriggerSourceType.Extern when other.Type.Source != ClipboardTriggerSourceType.Extern: //Extern can only be overwritten by extern with higher priority
                //    return false;
                case ClipboardTriggerSourceType.Intern when other.Type.Source == ClipboardTriggerSourceType.Custom: //Intern can be overwritten by intern with higher priority or by an extern
                    return false;
                default:
                    if (Type.Priority > other.Type.Priority || When.AddMilliseconds(other.Type.Priority) < other.When) //If same type check if higher priority
                        return false;
                    break;
            }

            //All validation passed, we can merge now

            Type = other.Type;

            foreach(var ai in other.AdditionalInfo)
            {
                AdditionalInfo.Add(ai);
            }

            return true;
        }
    }
}
