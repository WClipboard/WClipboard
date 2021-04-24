using WClipboard.Core;
using WClipboard.Core.Settings;
using WClipboard.Core.WPF.Settings;
using WClipboard.Core.WPF.Settings.Defaults;

namespace WClipboard.App.Settings
{
    public class AppUISettingsFactory : BaseUISettingsFactory
    {
        public const string OpenOnStartup = "WClipboard.OpenOnStartup";
        public const string MinimizeTo = "User Interface.Overview window.MinimizeTo";

        private readonly IAppInfo appInfo;

        public AppUISettingsFactory(IAppInfo appInfo) : base(new[] { 
            OpenOnStartup,
            MinimizeTo,
        })
        {
            this.appInfo = appInfo;
        }

        public override SettingViewModel? Create(ISetting model)
        {
            return model.Key switch
            {
                OpenOnStartup => new CheckBoxSettingViewModel(model, new OpenOnStartupSettingsApplier(appInfo), "Open WClipboard when Windows start"),
                MinimizeTo => new ComboBoxSettingEnumViewModel<MinimizeTo>(model, new IOSettingApplier<MinimizeTo>(SettingChangeMode.Direct, SettingChangeEffect.AtOnce), "Minimize overview window to"),
                _ => null,
            };
        }
    }
}
