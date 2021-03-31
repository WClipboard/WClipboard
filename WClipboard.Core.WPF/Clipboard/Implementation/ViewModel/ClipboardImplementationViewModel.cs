using System.Collections.Specialized;
using System.Linq;
using WClipboard.Core.Extensions;
using WClipboard.Core.WPF.Clipboard.ViewModel;
using WClipboard.Core.WPF.Extensions;
using WClipboard.Core.WPF.Utilities;
using WClipboard.Core.WPF.ViewModels;

namespace WClipboard.Core.WPF.Clipboard.Implementation.ViewModel
{
    public abstract class ClipboardImplementationViewModel : BaseViewModelWithInteractables<ClipboardImplementation>
    {
        public ClipboardObjectViewModel ClipboardObject { get; }

        public BindableObservableCollection<ClipboardImplementationViewModel>? LinkedImplementations { get; }
        public BindableObservableCollection<BaseViewModel>? LinkedContent { get; }

        protected ClipboardImplementationViewModel(ClipboardImplementation implementation, ClipboardObjectViewModel clipboardObject) : base(implementation, new BindableObservableCollection<InteractableState>(clipboardObject.SynchronizationContext))
        {
            ClipboardObject = clipboardObject;

            if (implementation.Parent is null)
            {
                LinkedImplementations = new BindableObservableCollection<ClipboardImplementationViewModel>(implementation.LinkedImplementations!.Select(li => clipboardObject._clipboardObjectManager.CreateViewModel(li, clipboardObject)).NotNull(), clipboardObject.SynchronizationContext);
                implementation.LinkedImplementations!.CollectionChanged += LinkedImplementations_CollectionChanged; // We know that it is not null since Parent is null

                LinkedContent = new BindableObservableCollection<BaseViewModel>(implementation.LinkedContent!.Select(lc => clipboardObject._clipboardObjectManager.CreateViewModel(lc, this)).NotNull(), clipboardObject.SynchronizationContext);
                implementation.LinkedContent!.CollectionChanged += LinkedContent_CollectionChanged; // We know that it is not null since Parent is null
            }
        }

        private void LinkedImplementations_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            var differences = e.GetDifferences<ClipboardImplementation>();
#pragma warning disable CS8604 // Possible null reference argument. This is the case because event handler is only used when not null
            LinkedImplementations.AddRange(differences.Added.Select(li => ClipboardObject._clipboardObjectManager.CreateViewModel(li, ClipboardObject)).NotNull());
            LinkedImplementations.RemoveAll(civm => differences.Removed.Contains(civm.Model));
#pragma warning restore CS8604 // Possible null reference argument.
        }

        private void LinkedContent_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            var differences = e.GetDifferences();
#pragma warning disable CS8604 // Possible null reference argument. This is the case because event handler is only used when not null
            LinkedContent.AddRange(differences.Added.Select(lc => ClipboardObject._clipboardObjectManager.CreateViewModel(lc, this)).NotNull());
            LinkedContent.RemoveAll(lcvm => differences.Removed.Contains(lcvm.Model));
#pragma warning restore CS8604 // Possible null reference argument.
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
