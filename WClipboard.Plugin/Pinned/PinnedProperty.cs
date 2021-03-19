using WClipboard.Core.WPF.Clipboard;

namespace WClipboard.Plugin.Pinned
{
    public class PinnedProperty : ClipboardObjectProperty
    {
        public static PinnedProperty Instance { get; } = new PinnedProperty();
        private PinnedProperty() : base(true)
        { }
    }
}
