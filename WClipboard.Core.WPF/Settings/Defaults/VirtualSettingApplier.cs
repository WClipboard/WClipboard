namespace WClipboard.Core.WPF.Settings.Defaults
{
    public abstract class VirtualSettingApplier : ISettingApplier
    {
        public SettingChangeMode ChangeMode { get; }
        public SettingChangeEffect ChangeEffect { get; }

        protected VirtualSettingApplier(SettingChangeMode changeMode, SettingChangeEffect changeEffect)
        {
            ChangeMode = changeMode;
            ChangeEffect = changeEffect;
        }

        public abstract void Apply(SettingViewModel setting);

        public abstract object? GetCurrentValue(SettingViewModel setting);
    }

    public abstract class VirtualSettingApplier<TValue> : VirtualSettingApplier, ISettingApplier<TValue>
    {
        protected VirtualSettingApplier(SettingChangeMode changeMode, SettingChangeEffect changeEffect) : base(changeMode, changeEffect)
        {
        }

        public sealed override void Apply(SettingViewModel setting) => Apply((SettingViewModel<TValue>)setting);

        public abstract void Apply(SettingViewModel<TValue> setting);

        public sealed override object? GetCurrentValue(SettingViewModel setting) => GetCurrentValue((SettingViewModel<TValue>)setting);

        public abstract TValue GetCurrentValue(SettingViewModel<TValue> setting);
    }
}
