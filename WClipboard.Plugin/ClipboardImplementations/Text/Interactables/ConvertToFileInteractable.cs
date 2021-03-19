using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WClipboard.Core.DI;
using WClipboard.Core.IO;
using WClipboard.Core.WPF.Extensions;
using WClipboard.Core.WPF.Models;

namespace WClipboard.Plugin.ClipboardImplementations.Text.Interactables
{
    public class ConvertToFileInteractable : Interactable<TextClipboardImplementationViewModel>
    {
        public ConvertToFileInteractable() : base("ConvertToFileIcon", new ConvertToFileInteractableAction())
        {
        }

        private class ConvertToFileInteractableAction : InteractableAction<TextClipboardImplementationViewModel>
        {
            private readonly Lazy<ITempManager> _tempManager = DiContainer.SP.GetLazy<ITempManager>();

            public ConvertToFileInteractableAction() : base("To file", new KeyGesture(Key.F, ModifierKeys.Alt))
            {
            }
            protected override async void Execute(TextClipboardImplementationViewModel parameter)
            {
                var fileName = _tempManager.Value.GetNewFileName("txt");

                using (var sw = new StreamWriter(fileName, false, Encoding.Unicode))
                {
                    await sw.WriteAsync(parameter.Model.Source).ConfigureAwait(true);
                }
                var dataObject = new DataObject();
                DataObjectExtensions.SetLinkedWClipboardId(dataObject, parameter.Model.ClipboardObject.Id);
                dataObject.SetFileDropList(fileName);
                Clipboard.SetDataObject(dataObject);
            }
        }
    }
}
