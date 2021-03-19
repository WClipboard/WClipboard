using WClipboard.Core.Utilities;

namespace WClipboard.Core.WPF.Clipboard.Metadata
{
    public abstract class ClipboardObjectMetadata : BindableBase
    {
        public object IconSource { get; }
        public string Name { get; }

        protected ClipboardObjectMetadata(object iconSource, string name)
        {
            IconSource = iconSource;
            Name = name;
        }
    }
}
