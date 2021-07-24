using System.Collections.Generic;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.WPF.Clipboard.Format;

namespace WClipboard.Core.WPF.Clipboard.Filter
{
    public interface IClipboardFilter
    {
        bool ShouldFilter(ClipboardTrigger clipboardTrigger, IEnumerable<EqualtableFormat> equaltableFormats);
    }
}
