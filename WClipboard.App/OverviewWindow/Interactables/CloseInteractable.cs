using System.Windows.Input;
using WClipboard.Core.WPF.Clipboard.ViewModel;
using WClipboard.Core.WPF.Models;

namespace WClipboard.App.OverviewWindow.Interactables
{
    public class CloseInteractable : Interactable<ClipboardObjectViewModel>
    {
        public CloseInteractable() : base("CloseIcon", new CloseInteractableAction())
        {
            
        }

        private class CloseInteractableAction : InteractableAction<ClipboardObjectViewModel>
        {
            public CloseInteractableAction() : base("Close", new KeyGesture(Key.Delete)) { }

            protected override void Execute(ClipboardObjectViewModel parameter)
            {
                if (parameter.Listener is OverviewWindowViewModel overviewWindow)
                {
                    overviewWindow.Remove(parameter);
                }
            }
        }
    }
}
