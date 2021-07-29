using System.Collections.ObjectModel;
using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.WPF.Clipboard.Format;

namespace WClipboard.Plugin.ClipboardImplementations.Path
{
    public class PathsEquatableFormat : EqualtableFormat
    {
        public ReadOnlyCollection<string> Paths { get; }

        public PathsEquatableFormat(ClipboardFormat format, string[] paths) : base(format)
        {
            Paths = new ReadOnlyCollection<string>(paths);
        }
    }
}
