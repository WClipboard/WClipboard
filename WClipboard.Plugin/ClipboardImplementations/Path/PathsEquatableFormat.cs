using System;
using System.Collections.ObjectModel;
using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.WPF.Clipboard.Implementation;

namespace WClipboard.Plugin.ClipboardImplementations.Path
{
    public class PathsEquatableFormat : EqualtableFormat
    {
        public ReadOnlyCollection<string> Paths { get; }

        public PathsEquatableFormat(ClipboardImplementationFactory factory, Type implementationType, ClipboardFormat format, string[] paths) : base(factory, implementationType, format)
        {
            Paths = new ReadOnlyCollection<string>(paths);
        }
    }
}
