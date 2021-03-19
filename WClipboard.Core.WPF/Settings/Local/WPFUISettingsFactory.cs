using WClipboard.Core.Settings;
using WClipboard.Core.Settings.Defaults;
using WClipboard.Core.WPF.Settings.Defaults;
using WClipboard.Core.WPF.Themes;

namespace WClipboard.Core.WPF.Settings.Local
{
    public class WPFUISettingsFactory : BaseUISettingsFactory
    {
        public WPFUISettingsFactory() : base(new []
        {
            SettingConsts.ThemeKey
        })
        {
        }

        public override SettingViewModel? Create(ISetting model)
        {
            return model.Key switch
            {
                SettingConsts.ThemeKey => new ComboBoxSettingViewModel<Theme>(model, new ResolvableIOSettingApplier<Theme>(SettingChangeMode.OnSave, SettingChangeEffect.RestartRequired), (IKeyedCollectionSetting<string, Theme>)model, "Application theme"),
                _ => null,
            };
        }
    }
}
