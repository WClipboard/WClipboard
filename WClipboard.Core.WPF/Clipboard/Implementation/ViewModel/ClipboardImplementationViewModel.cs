using WClipboard.Core.WPF.Clipboard.ViewModel;
using WClipboard.Core.WPF.Utilities;
using WClipboard.Core.WPF.ViewModels;

namespace WClipboard.Core.WPF.Clipboard.Implementation.ViewModel
{
    public abstract class ClipboardImplementationViewModel : BaseViewModelWithInteractables<ClipboardImplementation>
    {
        public ClipboardObjectViewModel ClipboardObject { get; }

        protected ClipboardImplementationViewModel(ClipboardImplementation implementation, ClipboardObjectViewModel clipboardObject) : base(implementation, new ConcurrentBindableList<InteractableState>())
        {
            ClipboardObject = clipboardObject;
        }
    }

    public abstract class ClipboardImplementationViewModel<TImplementation> : ClipboardImplementationViewModel, IViewModel<TImplementation>
        where TImplementation : ClipboardImplementation
    {
        public new TImplementation Model => (TImplementation)base.Model;

        protected ClipboardImplementationViewModel(TImplementation implementation, ClipboardObjectViewModel clipboardObject) : base(implementation, clipboardObject)
        {}
    }
}
