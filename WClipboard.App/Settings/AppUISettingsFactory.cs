using WClipboard.Core;
using WClipboard.Core.Settings;
using WClipboard.Core.WPF.Settings;
using WClipboard.Core.WPF.Settings.Defaults;

namespace WClipboard.App.Settings
{
    public class AppUISettingsFactory : BaseUISettingsFactory
    {
        public const string OpenOnStartup = "WClipboard.OpenOnStartup";

        private readonly IAppInfo appInfo;

        public AppUISettingsFactory(IAppInfo appInfo) : base(new[] { 
            OpenOnStartup
        })
        {
            this.appInfo = appInfo;
        }

        public override SettingViewModel? Create(ISetting model)
        {
            return model.Key switch
            {
                OpenOnStartup => new CheckBoxSettingViewModel(model, new OpenOnStartupSettingsApplier(appInfo), "Open WClipboard when Windows start"),
                _ => null,
            };
        }
    }
}
