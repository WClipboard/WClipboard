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
        public const string StartupMinimized = "WClipboard.StartupMinimized";
        public const string CheckUpdatesOnStartUp = "WClipboard.CheckUpdatesOnStartUp";
        public const string CheckForPrereleases = "WClipboard.CheckForPrereleases";

        private readonly IAppInfo appInfo;

        public AppUISettingsFactory(IAppInfo appInfo) : base(new[] { 
            OpenOnStartup,
            StartupMinimized,
            MinimizeTo,
            CheckUpdatesOnStartUp,
            CheckForPrereleases
        })
        {
            this.appInfo = appInfo;
        }

        public override SettingViewModel? Create(ISetting model)
        {
            return model.Key switch
            {
                OpenOnStartup => new CheckBoxSettingViewModel(model, new OpenOnStartupSettingsApplier(appInfo), $"Open {appInfo.Name} when Windows starts"),
                StartupMinimized => new CheckBoxSettingViewModel(model, new IOSettingApplier<bool?>(SettingChangeMode.Direct, SettingChangeEffect.AtOnce), $"Start {appInfo.Name} minimized if started on Windows startup"),
                MinimizeTo => new ComboBoxSettingEnumViewModel<MinimizeTo>(model, new IOSettingApplier<MinimizeTo>(SettingChangeMode.Direct, SettingChangeEffect.AtOnce), "Minimize overview window to"),
                CheckUpdatesOnStartUp => new CheckBoxSettingViewModel(model, new IOSettingApplier<bool?>(SettingChangeMode.Direct, SettingChangeEffect.AtOnce), $"Check for updates when {appInfo.Name} starts"),
                CheckForPrereleases => new CheckBoxSettingViewModel(model, new IOSettingApplier<bool?>(SettingChangeMode.Direct, SettingChangeEffect.AtOnce), "When checking for updates include prereleases"),
                _ => null,
            };
        }
    }
}
