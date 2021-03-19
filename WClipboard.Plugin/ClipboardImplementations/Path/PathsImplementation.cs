using System.Collections.ObjectModel;
using System.Linq;
using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.WPF.Clipboard;
using WClipboard.Core.WPF.Clipboard.Implementation;

namespace WClipboard.Plugin.ClipboardImplementations.Path
{
    public class PathsImplementation : ClipboardImplementation
    {
        public ReadOnlyCollection<string> Paths { get; }

        public PathsImplementation(ClipboardFormat format, ClipboardImplementationFactory factory, ClipboardImplementation parent, params string[] paths) : base(format, factory, parent)
        {
            Paths = new ReadOnlyCollection<string>(paths);
        }

        public PathsImplementation(ClipboardFormat format, ClipboardImplementationFactory factory, ClipboardObject clipboardObject, params string[] paths) : base(format, factory, clipboardObject)
        {
            Paths = new ReadOnlyCollection<string>(paths);
        }

        public PathsImplementation(ClipboardObject clipboardObject, PathsEquatableFormat source) : base(source.Format, source.Factory, clipboardObject)
        {
            Paths = source.Paths;
        }

        public override bool IsEqual(EqualtableFormat equaltable)
        {
            if (!(equaltable is PathsEquatableFormat pathsEquatable))
                return false;

            if (Paths.Count != pathsEquatable.Paths.Count)
                return false;

            if (Paths.Except(pathsEquatable.Paths).Any())
                return false;

            return true;
        }
    }
}
