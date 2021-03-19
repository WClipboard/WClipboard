using System.Collections.Specialized;
using System.Windows.Input;
using WClipboard.Core.WPF.Models;
using WClipboard.Core.WPF.ViewModels;
using System.Threading.Tasks;
using System;
using WClipboard.Core.DI;
using WClipboard.Core.WPF.Clipboard.ViewModel;

namespace WClipboard.Plugin.Pinned
{
    public class PinnedInteractable : Interactable<ClipboardObjectViewModel>
    {
        public PinnedInteractable() : base("PinIcon", new PinnedInteractableAction())
        { }

        public override InteractableState CreateState(ClipboardObjectViewModel viewModel)
        {
            return new PinnedInteractableState(viewModel, this);
        }

        private class PinnedInteractableAction : AsyncInteractableAction<ClipboardObjectViewModel>
        {
            private readonly Lazy<PinnedManager> pinnedManager = DiContainer.SP.GetLazy<PinnedManager>();

            public PinnedInteractableAction() : base("Pin", new KeyGesture(Key.P, ModifierKeys.Shift | ModifierKeys.Alt))
            { }

            protected override async Task ExecuteAsync(ClipboardObjectViewModel parameter)
            {
                if (PinnedManager.IsPinned(parameter.Model))
                {
                    pinnedManager.Value.UnPin(parameter.Model);
                }
                else
                {
                    await pinnedManager.Value.Pin(parameter.Model);
                }
            }
        }

        private class PinnedInteractableState : ToggleableInteractableState
        {
            private readonly ClipboardObjectViewModel clipboardObjectViewModel;

            public PinnedInteractableState(ClipboardObjectViewModel clipboardObjectViewModel, PinnedInteractable pinnedInteractable) : base(pinnedInteractable)
            {
                this.clipboardObjectViewModel = clipboardObjectViewModel;
                clipboardObjectViewModel.Model.Properties.CollectionChanged += UpdateToggled;

                UpdateToggled();
            }

            private void UpdateToggled(object sender = null, NotifyCollectionChangedEventArgs e = null)
            {
                Toggled = PinnedManager.IsPinned(clipboardObjectViewModel.Model);
            }
        }
    }
}
