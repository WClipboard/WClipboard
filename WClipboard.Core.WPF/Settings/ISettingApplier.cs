namespace WClipboard.Core.WPF.Settings
{
    public interface ISettingApplier
    {
        SettingChangeMode ChangeMode { get; }
        SettingChangeEffect ChangeEffect { get; }
        object? GetCurrentValue(SettingViewModel setting);

        void Apply(SettingViewModel setting);
    }

    public interface ISettingApplier<TValue> : ISettingApplier
    {
        TValue GetCurrentValue(SettingViewModel<TValue> setting);

        void Apply(SettingViewModel<TValue> setting);
    }
}
