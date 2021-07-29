using System.Collections.Generic;
using System.Windows;
using WClipboard.Core.Clipboard.Trigger;

namespace WClipboard.Core.WPF.Clipboard.Format
{
    public interface IFormatsExtractor
    {
        IEnumerable<EqualtableFormat> Extract(ClipboardTrigger trigger, IDataObject dataObject);
    }
}
