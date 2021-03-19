using System.Windows;
using System.Windows.Input;
using WClipboard.Core.WPF.Extensions;
using WClipboard.Core.WPF.Models;

namespace WClipboard.Plugin.ClipboardImplementations.Path.Interactables
{
    public class CopyPathPartInteractable : Interactable<MultiPathViewModel>
    {
        public CopyPathPartInteractable() : base("CopyIcon", new CopyPathPartInteractableAction())
        {
        }

        private class CopyPathPartInteractableAction : InteractableAction<MultiPathViewModel>
        {
            public CopyPathPartInteractableAction() : base("Copy file/directory", new KeyGesture(Key.C, ModifierKeys.Control)) { }

            protected override void Execute(MultiPathViewModel parameter)
            {
                var dataObject = new DataObject();

                //TODO add link
                //DataObjectExtensions.SetLinkedWClipboardId(dataObject, parameter.Implementation.ClipboardObject.Id);

                dataObject.SetFileDropList(parameter.Main.FullPath);
                dataObject.SetShellFileIDList(parameter.Main.FullPath);

                Clipboard.SetDataObject(dataObject, false);
            }
        }
        
    }
}
