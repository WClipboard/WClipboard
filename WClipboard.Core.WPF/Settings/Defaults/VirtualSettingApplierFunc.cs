using System;

namespace WClipboard.Core.WPF.Settings.Defaults
{
    public class VirtualSettingApplierFunc : VirtualSettingApplier
    {
        private readonly Action<SettingViewModel> _apply;

        private readonly object _originalValue;

        public VirtualSettingApplierFunc(SettingChangeMode changeMode, SettingChangeEffect changeEffect, object originalValue, Action<SettingViewModel> apply) : base(changeMode, changeEffect)
        {
            _apply = apply;
            _originalValue = originalValue;
        }

        public override void Apply(SettingViewModel setting) => _apply(setting);

        public override object GetCurrentValue(SettingViewModel setting) => _originalValue;
    }

    public class VirtualSettingApplierFunc<TValue> : VirtualSettingApplier<TValue>
    {
        private readonly Action<SettingViewModel<TValue>> _apply;

        private readonly TValue _originalValue;

        public VirtualSettingApplierFunc(SettingChangeMode changeMode, SettingChangeEffect changeEffect, TValue originalValue, Action<SettingViewModel<TValue>> apply) : base(changeMode, changeEffect)
        {
            _apply = apply;
            _originalValue = originalValue;
        }

        public override void Apply(SettingViewModel<TValue> setting) => _apply(setting);

        public override TValue GetCurrentValue(SettingViewModel<TValue> setting) => _originalValue;
    }
}
