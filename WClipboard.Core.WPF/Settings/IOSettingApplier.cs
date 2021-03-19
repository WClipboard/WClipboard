using WClipboard.Core.Settings;

namespace WClipboard.Core.WPF.Settings
{
    public class IOSettingApplier : ISettingApplier
    {
        public SettingChangeMode ChangeMode { get; }

        public SettingChangeEffect ChangeEffect { get; }

        public IOSettingApplier(SettingChangeMode changeMode, SettingChangeEffect changeEffect)
        {
            ChangeMode = changeMode;
            ChangeEffect = changeEffect;
        }

        public void Apply(SettingViewModel setting)
        {
            ((IIOSetting)setting.Model).Value = setting.Value;
        }

        public object? GetCurrentValue(SettingViewModel setting)
        {
            return ((IIOSetting)setting.Model).Value;
        }
    }
}
