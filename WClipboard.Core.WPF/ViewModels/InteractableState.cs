using System.Linq;
using WClipboard.Core.Utilities;
using WClipboard.Core.WPF.Models;

#nullable enable

namespace WClipboard.Core.WPF.ViewModels
{
    public class InteractableState : BindableBase
    {
        private bool enabled = true;
        private string? disabledReason = null;
        private bool visible = true;

        public bool Enabled { 
            get => enabled; 
            private set => SetProperty(ref enabled, value); 
        }

        public string? DisabledReason { 
            get => disabledReason;
            private set => SetProperty(ref disabledReason, value);
        }

        public bool Visible { 
            get => visible; 
            protected set => SetProperty(ref visible, value); 
        }

        public string Tooltip => GetTooltip();

        public Interactable Interactable { get; }

        public InteractableState(Interactable interactable)
        {
            Interactable = interactable;
        }

        protected void Disable(string reason)
        {
            Enabled = false;
            DisabledReason = reason;
        }

        protected void Enable()
        {
            Enabled = true;
            DisabledReason = null;
        }

        protected virtual string GetTooltip()
        {
            return string.Join("\n", Interactable.Actions.Select(ia =>  ia.GetTooltip()));
        }
    }
}
