using System.Threading.Tasks;
using System.Windows.Input;
using WClipboard.Core.WPF.Clipboard;
using WClipboard.Core.WPF.Clipboard.ViewModel;
using WClipboard.Core.WPF.Models;
using SysClipboard = System.Windows.Clipboard;

namespace WClipboard.Core.WPF.Defaults.Interactables
{
    public class CopyClipboardObjectInteractable : Interactable<ClipboardObjectViewModel>
    {
        public CopyClipboardObjectInteractable() : base("CopyIcon", new CopyClipboardObjectInteractableAction()) { }

        private class CopyClipboardObjectInteractableAction : AsyncInteractableAction<ClipboardObjectViewModel>
        {
            public CopyClipboardObjectInteractableAction() : base("Copy object", new KeyGesture(Key.C, ModifierKeys.Alt | ModifierKeys.Shift)) { }

            protected override async Task ExecuteAsync(ClipboardObjectViewModel parameter)
            {
                SysClipboard.SetDataObject(await parameter.Model.GetDataObject(), false);
            }
        }
    }
}
