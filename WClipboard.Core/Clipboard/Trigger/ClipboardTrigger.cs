using System;
using System.Collections.Generic;
using WClipboard.Core.Utilities;
using WClipboard.Core.Utilities.Collections;

namespace WClipboard.Core.Clipboard.Trigger
{
    public sealed class ClipboardTrigger : BindableBase
    {
        public DateTime When { get; }
        public WindowInfo? ForegroundWindow { get; }
        public ProgramInfo? ForegroundProgram { get; }
        public ProgramInfo? DataSourceProgram { get; }

        public ObservableObjectTypeCollection AdditionalInfo { get; }

        private ClipboardTriggerType _type;
        public ClipboardTriggerType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public ClipboardTrigger(DateTime when, ClipboardTriggerType type, ProgramInfo? dataSourceProgram, ProgramInfo? foregroundProgram, WindowInfo? foregroundWindow, IEnumerable<object> additionalInfo)
        {
            When = when;
            ForegroundWindow = foregroundWindow;
            ForegroundProgram = foregroundProgram;
            DataSourceProgram = dataSourceProgram;
            _type = type;
            AdditionalInfo = new ObservableObjectTypeCollection(additionalInfo ?? Array.Empty<object>());
        }

        public ClipboardTrigger(DateTime when, ClipboardTriggerType type, ProgramInfo? dataSourceProgram, ProgramInfo? foregroundProgram, WindowInfo? foregroundWindow, params object[] additionalInfo) : this(when, type, dataSourceProgram, foregroundProgram, foregroundWindow, (IEnumerable<object>)additionalInfo) { }
        public ClipboardTrigger(ClipboardTriggerType type, ProgramInfo? dataSourceProgram, ProgramInfo? foregroundProgram, WindowInfo? foregroundWindow, IEnumerable<object> additionalInfo) : this(DateTime.Now, type, dataSourceProgram, foregroundProgram, foregroundWindow, additionalInfo) { }
        public ClipboardTrigger(ClipboardTriggerType type, ProgramInfo? dataSourceProgram, ProgramInfo? foregroundProgram, WindowInfo? foregroundWindow, params object[] additionalInfo) : this(DateTime.Now, type, dataSourceProgram, foregroundProgram, foregroundWindow, additionalInfo) { }

        public void Merge(ClipboardTrigger other)
        {
            Type = other.Type;

            foreach(var ai in other.AdditionalInfo)
            {
                AdditionalInfo.Add(ai);
            }
        }
    }
}
