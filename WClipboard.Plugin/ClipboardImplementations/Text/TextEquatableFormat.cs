using System;
using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.WPF.Clipboard.Implementation;

namespace WClipboard.Plugin.ClipboardImplementations.Text
{
    public class TextEquatableFormat : EqualtableFormat
    {
        public string Text { get; }

        public TextEquatableFormat(ClipboardImplementationFactory factory, Type implementationType, ClipboardFormat format, string text) : base(factory, implementationType, format)
        {
            Text = text;
        }
    }
}
