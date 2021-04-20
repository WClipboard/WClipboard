using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WClipboard.Core.DI;
using WClipboard.Core.IO;
using WClipboard.Core.Settings;
using WClipboard.Core.WPF.Extensions;
using WClipboard.Core.WPF.Models;
using WClipboard.Plugin.Settings;

namespace WClipboard.Plugin.ClipboardImplementations.Bitmap.Interactables
{
    public class ConvertToFileInteractable : Interactable<BitmapImplementationViewModel>
    {
        public ConvertToFileInteractable(IIOSettingsManager settingsManager) : base("ConvertToFileIcon", 
            new ConvertToFileInteractableAction((IResolveableIOSetting)settingsManager.GetSetting(SettingConsts.ToFileBitmapEncoderKey)),
            new SaveToFileInteractableAction((IResolveableIOSetting)settingsManager.GetSetting(SettingConsts.ToFileBitmapEncoderKey)))
        {

        }

        private class ConvertToFileInteractableAction : InteractableAction<BitmapImplementationViewModel>
        {
            private readonly IResolveableIOSetting setting;
            private readonly Lazy<ITempManager> tempManager = DiContainer.SP.GetLazy<ITempManager>();

            public ConvertToFileInteractableAction(IResolveableIOSetting setting) : base("To file", new KeyGesture(Key.F, ModifierKeys.Alt)) {
                this.setting = setting;
            }

            protected override void Execute(BitmapImplementationViewModel parameter)
            {
                var option = (BitmapFileOption)setting.GetResolvedValue();

                var fileName = tempManager.Value.GetNewFileName(option.Extension);
                
                parameter.Model.GetImage().Save(fileName, option.CreateEncoder());

                var dataObject = new DataObject();
                DataObjectExtensions.SetLinkedWClipboardId(dataObject, parameter.Model.ClipboardObject.Id);
                dataObject.SetFileDropList(fileName);
                Clipboard.SetDataObject(dataObject);
            }
        }

        private class SaveToFileInteractableAction : InteractableAction<BitmapImplementationViewModel>
        {
            private static string lastDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            private readonly IResolveableIOSetting setting;
            private readonly Lazy<IBitmapFileOptionsManager> bitmapFileOptions = DiContainer.SP.GetLazy<IBitmapFileOptionsManager>();

            public SaveToFileInteractableAction(IResolveableIOSetting setting) : base("Save as file", new MouseGesture(MouseAction.LeftClick, ModifierKeys.Control), new KeyGesture(Key.F, ModifierKeys.Alt | ModifierKeys.Control))
            {
                this.setting = setting;
            }

            protected override void Execute(BitmapImplementationViewModel parameter)
            {
                var bitmapFileOptions = this.bitmapFileOptions.Value;
                var defaultOption = (BitmapFileOption)setting.GetResolvedValue();

                var dialog = new SaveFileDialog
                {
                    Title = "Save bitmap",
                    Filter = string.Join("|", bitmapFileOptions.Values.Select(bfo => $"{bfo.Name}|*{bfo.Extension}")),
                    OverwritePrompt = true,
                    ValidateNames =  true,
                    CheckPathExists = true,
                    CheckFileExists = false,
                    FilterIndex = bitmapFileOptions.Values.TakeWhile(bfo => bfo != defaultOption).Count() + 1,
                    DefaultExt = defaultOption.Extension,
                    InitialDirectory = lastDirectory,
                    DereferenceLinks = true,
                };
                if (dialog.ShowDialog() == true)
                {
                    var extension = System.IO.Path.GetExtension(dialog.FileName);
                    var encoder = bitmapFileOptions.Values.FirstOrDefault(bfo => bfo.Extension == extension) ?? defaultOption;

                    parameter.Model.GetImage().Save(dialog.FileName, encoder.CreateEncoder());

                    lastDirectory = System.IO.Path.GetDirectoryName(dialog.FileName);
                }
            }
        }
    }
}
