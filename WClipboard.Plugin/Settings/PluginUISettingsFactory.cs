using WClipboard.Core.Settings;
using WClipboard.Core.Settings.Defaults;
using WClipboard.Core.WPF.Settings;
using WClipboard.Core.WPF.Settings.Defaults;
using WClipboard.Plugin.ClipboardImplementations.Bitmap;

namespace WClipboard.Plugin.Settings
{
    public class PluginUISettingsFactory : BaseUISettingsFactory
    {
        public PluginUISettingsFactory() : base(new[] {
            SettingConsts.ToFileBitmapEncoderKey
        })
        {
        }

        public override SettingViewModel Create(ISetting model)
        {
            return model.Key switch
            {
                SettingConsts.ToFileBitmapEncoderKey => new ComboBoxSettingViewModel<BitmapFileOption>(model, 
                        new ResolvableIOSettingApplier<BitmapFileOption>(SettingChangeMode.Direct, SettingChangeEffect.AtOnce),
                    (IKeyedCollectionSetting<string, BitmapFileOption>)model, "Default to file image encoding", null),
                _ => null,
            };
        }
    }
}
