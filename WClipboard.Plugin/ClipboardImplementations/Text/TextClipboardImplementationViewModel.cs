using WClipboard.Core.WPF.Clipboard.Implementation.ViewModel;
using WClipboard.Core.WPF.Clipboard.ViewModel;

namespace WClipboard.Plugin.ClipboardImplementations.Text
{
    public class TextClipboardImplementationViewModel : ClipboardImplementationViewModel<TextClipboardImplementation>
    {
        public TextClipboardImplementationViewModel(TextClipboardImplementation implementation, ClipboardObjectViewModel clipboardObject) : base(implementation, clipboardObject)
        {}
    }
}
