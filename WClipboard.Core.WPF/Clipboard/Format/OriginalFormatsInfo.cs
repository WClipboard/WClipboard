using System.Collections.Generic;

namespace WClipboard.Core.WPF.Clipboard.Format
{
    public class OriginalFormatsInfo
    {
        public IReadOnlyList<string> Value { get; }

        public OriginalFormatsInfo(string[] value)
        {
            Value = value;
        }
    }
}
