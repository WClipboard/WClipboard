using System;
using WClipboard.Core.Settings;

namespace WClipboard.Core.WPF.Settings.Defaults
{
    public class FuncIOSettingsApplier<TUI, TIO> : ISettingApplier<TUI>
    {
        public SettingChangeMode ChangeMode { get; }

        public SettingChangeEffect ChangeEffect { get; }

        private readonly TUI _originalValue;

        private readonly Func<TUI, TIO> funcConverter;

        public FuncIOSettingsApplier(SettingChangeMode changeMode, SettingChangeEffect changeEffect, Func<TUI, TIO> funcConverter, TUI originalValue)
        {
            ChangeMode = changeMode;
            ChangeEffect = changeEffect;
            _originalValue = originalValue;
            this.funcConverter = funcConverter;
        }

        public void Apply(SettingViewModel<TUI> setting)
        {
            ((IIOSetting)setting.Model).Value = funcConverter(setting.Value);
        }

        void ISettingApplier.Apply(SettingViewModel setting) => Apply((SettingViewModel<TUI>)setting);

        public TUI GetCurrentValue(SettingViewModel<TUI> setting)
        {
            return _originalValue;
        }

        object? ISettingApplier.GetCurrentValue(SettingViewModel setting) => GetCurrentValue((SettingViewModel<TUI>)setting);
    }
}
