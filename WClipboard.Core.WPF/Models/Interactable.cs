#nullable enable

using System;
using System.Collections.Generic;
using WClipboard.Core.WPF.ViewModels;

namespace WClipboard.Core.WPF.Models
{
    public class Interactable
    {
        public object IconSource { get; }
        public IReadOnlyCollection<InteractableAction> Actions { get; }

        public Interactable(object iconSource, params InteractableAction[] actions)
        {
            IconSource = iconSource;
            Actions = actions;

            if(actions.Length == 0)
            {
                throw new InvalidOperationException($"It is required to have atleast one {nameof(InteractableAction)} in an {nameof(Interactable)}");
            }
        }

        public virtual InteractableState? CreateState(object viewModel)
        {
            return new InteractableState(this);
        }
    }

    public class Interactable<TViewModel> : Interactable where TViewModel : notnull
    {
        public Interactable(object iconSource, params InteractableAction<TViewModel>[] actions) : base(iconSource, actions)
        {
        }

        public override InteractableState? CreateState(object viewModel) => viewModel is TViewModel vm ? CreateState(vm) : null;
        public virtual InteractableState? CreateState(TViewModel viewModel)
        {
            return base.CreateState(viewModel);
        }
    }
}
