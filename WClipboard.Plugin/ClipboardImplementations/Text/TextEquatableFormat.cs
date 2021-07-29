using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.WPF.Clipboard.Format;

namespace WClipboard.Plugin.ClipboardImplementations.Text
{
    public class TextEquatableFormat : EqualtableFormat
    {
        public string Text { get; }

        public TextEquatableFormat(ClipboardFormat format, string text) : base(format)
        {
            Text = text;
        }
    }
}
