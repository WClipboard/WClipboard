using System;
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
        public ConvertToFileInteractable(IIOSettingsManager settingsManager) : base("ConvertToFileIcon", new ConvertToFileInteractableAction((IResolveableIOSetting)settingsManager.GetSetting(SettingConsts.ToFileBitmapEncoderKey)))
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
        
    }
}
