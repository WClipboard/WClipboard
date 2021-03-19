using WClipboard.Core.WPF.Clipboard.ViewModel;
using WClipboard.Core.WPF.Utilities;
using WClipboard.Core.WPF.ViewModels;

namespace WClipboard.Core.WPF.Clipboard.Implementation.ViewModel
{
    public abstract class ClipboardImplementationViewModelFactory : ViewModelFactory<ClipboardImplementation>
    {
        public sealed override BaseViewModel<ClipboardImplementation>? Create(ClipboardImplementation implementation, object? parent) => parent is ClipboardObjectViewModel clipboardObject ? Create(implementation, clipboardObject) : null;

        public abstract ClipboardImplementationViewModel Create(ClipboardImplementation implementation, ClipboardObjectViewModel clipboardObject);
    }
}
