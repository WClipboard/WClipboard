using System.Collections.Generic;
using WClipboard.Core.WPF.Clipboard.ViewModel;

namespace WClipboard.Core.WPF.Clipboard.Metadata
{
    public abstract class ClipboardObjectMetadataFactory
    {
        public abstract IEnumerable<ClipboardObjectMetadata> Create(ClipboardObjectViewModel clipboardObject);
    }
}
