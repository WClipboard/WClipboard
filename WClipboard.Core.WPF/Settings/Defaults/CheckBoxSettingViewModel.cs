using System;
using WClipboard.Core.Settings;

namespace WClipboard.Core.WPF.Settings.Defaults
{
    public class CheckBoxSettingViewModel : SettingViewModel<bool?>
    {
        private readonly Func<bool?, string>? _valueTextProvider;

        private string? _valueText;
        private bool _isThreeState;
        public string? ValueText { get => _valueText; set => SetProperty(ref _valueText, value); }
        public bool IsThreeState { get => _isThreeState; set => SetProperty(ref _isThreeState, value); }

        public CheckBoxSettingViewModel(ISetting model, ISettingApplier<bool?> settingsApplier, string description, Func<bool?, string>? valueTextProvider = null, bool isThreeState = false) : base(model, settingsApplier, description)
        {
            _valueTextProvider = valueTextProvider;
            ValueText = _valueTextProvider?.Invoke(Value);
            IsThreeState = isThreeState;
        }

        protected override void OnValueChanged(object? oldValue)
        {
            base.OnValueChanged(oldValue);

            if(!(_valueTextProvider is null))
            {
                ValueText = _valueTextProvider(Value);
            }
        }
    }
}
