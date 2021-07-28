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

    public class IOSettingApplier<TValue> : IOSettingApplier, ISettingApplier<TValue>
    {
        public IOSettingApplier(SettingChangeMode changeMode, SettingChangeEffect changeEffect) : base(changeMode, changeEffect) { }

        public void Apply(SettingViewModel<TValue> setting)
        {
            ((IIOSetting)setting.Model).Value = setting.Value;
        }

        public TValue GetCurrentValue(SettingViewModel<TValue> setting)
        {
            return ((IIOSetting)setting.Model).GetValue<TValue>();
        }
    }
}
