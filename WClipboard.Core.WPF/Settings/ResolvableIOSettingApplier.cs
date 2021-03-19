using WClipboard.Core.Settings;

namespace WClipboard.Core.WPF.Settings
{
    public class ResolvableIOSettingApplier : ISettingApplier
    {
        public SettingChangeMode ChangeMode { get; }

        public SettingChangeEffect ChangeEffect { get; }

        public ResolvableIOSettingApplier(SettingChangeMode changeMode, SettingChangeEffect changeEffect)
        {
            ChangeMode = changeMode;
            ChangeEffect = changeEffect;
        }

        public void Apply(SettingViewModel setting)
        {
            ((IResolveableIOSetting)setting.Model).SetResolvedValue(setting.Value);
        }

        public object? GetCurrentValue(SettingViewModel setting)
        {
            return ((IResolveableIOSetting)setting.Model).GetResolvedValue();
        }
    }

    public class ResolvableIOSettingApplier<TValue> : ResolvableIOSettingApplier, ISettingApplier<TValue>
    {
        public ResolvableIOSettingApplier(SettingChangeMode changeMode, SettingChangeEffect changeEffect) : base(changeMode, changeEffect)
        {
        }

        public void Apply(SettingViewModel<TValue> setting) => base.Apply(setting);

        public TValue GetCurrentValue(SettingViewModel<TValue> setting)
        {
            return (TValue)base.GetCurrentValue(setting)!;
        }
    }
}
