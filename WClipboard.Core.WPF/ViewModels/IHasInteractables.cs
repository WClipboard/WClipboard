using System.Collections.Generic;
using WClipboard.Core.WPF.Utilities;

namespace WClipboard.Core.WPF.ViewModels
{
    /// <summary>
    /// Generic check to define Interactables property checked in the UI such as in the ListBoxItemStyle (to allow the keybindings in the upper tree (the ListBoxItem))
    /// </summary>
    public interface IHasInteractables
    {
        IReadOnlyList<InteractableState> Interactables { get; }
    }

    /// <summary>
    /// Generic check to define Interactables property checked in the UI such as in the ListBoxItemStyle (to allow the keybindings in the upper tree (the ListBoxItem)). Assignable enables the auto assignment of registered Interactables inside the InteractablesManager
    /// </summary>
    public interface IHasAssignableInteractables : IHasInteractables
    {
        new ConcurrentBindableList<InteractableState> Interactables { get; }
    }
}
