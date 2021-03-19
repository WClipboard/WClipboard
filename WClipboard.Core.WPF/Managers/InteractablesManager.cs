using System.Collections.Generic;
using System.Linq;
using WClipboard.Core.WPF.Models;
using WClipboard.Core.WPF.ViewModels;
using WClipboard.Core.Extensions;

#nullable enable

namespace WClipboard.Core.WPF.Managers
{
    public interface IInteractablesManager
    {
        IEnumerable<InteractableState> CreateStates(object viewModel);
        void AssignStates(IHasAssignableInteractables target);
    }

    public class InteractablesManager : IInteractablesManager
    {
        private readonly IEnumerable<Interactable> interactables;

        public InteractablesManager(IEnumerable<Interactable> interactables)
        {
            this.interactables = interactables;
        }

        public void AssignStates(IHasAssignableInteractables target) => 
            target.Interactables.AddRange(interactables.Select(i => i.CreateState(target)).NotNull());

        public IEnumerable<InteractableState> CreateStates(object viewModel)
        {
            return interactables.Select(i => i.CreateState(viewModel)).NotNull();
        }
    }
}
