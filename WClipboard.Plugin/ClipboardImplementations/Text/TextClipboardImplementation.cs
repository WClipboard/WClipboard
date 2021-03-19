using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.WPF.Clipboard;
using WClipboard.Core.WPF.Clipboard.Implementation;

#nullable enable

namespace WClipboard.Plugin.ClipboardImplementations.Text
{
    public class TextClipboardImplementation : ClipboardImplementation
    {
        public string Source { get; }

        public TextClipboardImplementation(ClipboardFormat format, ClipboardImplementationFactory factory, ClipboardImplementation parent, string source) : base(format, factory, parent)
        {
            Source = source;
        }

        public TextClipboardImplementation(ClipboardFormat format, ClipboardImplementationFactory factory, ClipboardObject clipboardObject, string source) : base(format, factory, clipboardObject)
        {
            Source = source;
        }

        public TextClipboardImplementation(ClipboardObject clipboardObject, TextEquatableFormat source) : base(source.Format, source.Factory, clipboardObject)
        {
            Source = source.Text;
        }

        public override bool IsEqual(EqualtableFormat equaltable)
        {
            if (!(equaltable is TextEquatableFormat textEquatable))
                return false;

            if (Source.Length != textEquatable.Text.Length)
                return false;

            return Source == textEquatable.Text;
        }
    }
}
