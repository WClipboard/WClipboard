using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WClipboard.Core.WPF.Clipboard.Implementation.ViewModel;
using WClipboard.Core.WPF.Extensions;
using WClipboard.Core.WPF.Models;

using SysClipboard = System.Windows.Clipboard;

namespace WClipboard.Core.WPF.Defaults.Interactables
{
    public class CopyFormatInteractable : Interactable<ClipboardImplementationViewModel>
    {
        public CopyFormatInteractable() : base("CopyIcon", new CopyFormatInteractableAction())
        {
        }

        private class CopyFormatInteractableAction : AsyncInteractableAction<ClipboardImplementationViewModel>
        {
            public CopyFormatInteractableAction() : base("Copy format", new KeyGesture(Key.C, ModifierKeys.Alt)){}

            protected override async Task ExecuteAsync(ClipboardImplementationViewModel parameter)
            {
                var dataObject = new DataObject();
                DataObjectExtensions.SetLinkedWClipboardId(dataObject, parameter.Model.ClipboardObject.Id);

                await parameter.Model.Factory.WriteToDataObject(parameter.Model, dataObject);

                SysClipboard.SetDataObject(dataObject, false);
            }
        }
    }
}
