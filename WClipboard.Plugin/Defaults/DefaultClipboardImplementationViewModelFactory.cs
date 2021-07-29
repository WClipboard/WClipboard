using WClipboard.Core.WPF.Clipboard.Implementation;
using WClipboard.Core.WPF.Clipboard.Implementation.ViewModel;
using WClipboard.Core.WPF.Clipboard.ViewModel;
using WClipboard.Plugin.ClipboardImplementations.Bitmap;
using WClipboard.Plugin.ClipboardImplementations.Path;
using WClipboard.Plugin.ClipboardImplementations.Text;

namespace WClipboard.Plugin.Defaults
{
    internal class DefaultClipboardImplementationViewModelFactory : ClipboardImplementationViewModelFactory
    {
        public override ClipboardImplementationViewModel Create(ClipboardImplementation implementation, ClipboardObjectViewModel clipboardObject)
        {
            if (implementation is TextClipboardImplementation textImplementation)
            {
                if (implementation.Parent is null)
                {
                    return new TextClipboardImplementationViewModel(textImplementation, clipboardObject);
                }
                else
                {
                    return new LinkedTextClipboardImplementationViewModel(textImplementation, clipboardObject);
                }
            } 
            else if (implementation is PathsImplementation pathsImplementation)
            {
                if (pathsImplementation.Paths.Count == 1)
                {
                    return new SinglePathViewModel(pathsImplementation, clipboardObject);
                }
                else
                {
                    return new MultiPathsViewModel(pathsImplementation, clipboardObject);
                }
            }
            else if (implementation is BitmapImplementation bitmapImplementation)
            {
                return new BitmapImplementationViewModel(bitmapImplementation, clipboardObject);
            }
            return null;
        }
    }
}
