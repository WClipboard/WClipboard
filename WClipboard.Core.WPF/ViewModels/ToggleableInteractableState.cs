#nullable enable

using WClipboard.Core.WPF.Models;

namespace WClipboard.Core.WPF.ViewModels
{
    public abstract class ToggleableInteractableState : InteractableState
    {
        private bool toggled = false;

        public bool Toggled
        {
            get => toggled;
            protected set
            {
                SetProperty(ref toggled, value);
            }
        }

        protected ToggleableInteractableState(Interactable interactable) : base(interactable)
        {
        }
    }
}
