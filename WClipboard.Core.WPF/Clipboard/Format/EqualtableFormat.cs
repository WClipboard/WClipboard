using WClipboard.Core.Clipboard.Format;

namespace WClipboard.Core.WPF.Clipboard.Format
{
    public abstract class EqualtableFormat
    {
        public ClipboardFormat Format { get; }

        protected EqualtableFormat(ClipboardFormat format)
        {
            Format = format;
        }
    }
}
