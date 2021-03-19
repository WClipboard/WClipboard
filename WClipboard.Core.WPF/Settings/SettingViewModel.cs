using WClipboard.Core.Settings;
using WClipboard.Core.WPF.ViewModels;

namespace WClipboard.Core.WPF.Settings
{
    public abstract class SettingViewModel : BaseViewModel<ISetting>
    {
        private object? _value;

        public object? Value
        {
            get => _value;
            set => SetProperty(ref _value, value, nameof(Value), nameof(IsChanged)).OnChanged(OnValueChanged);
        }

        public object? OriginalValue { get; }

        public bool IsChanged => OriginalValue != _value;

        private bool _isApplied = true;
        public bool IsApplied { 
            get => _isApplied;
            set => SetProperty(ref _isApplied, value);
        }

        public ISettingApplier Applier { get; }

        public string Description { get; }

        private MessageBarViewModel? _messageBar;

        public MessageBarViewModel? MessageBar {
            get => _messageBar;
            set => SetProperty(ref _messageBar, value);
        }

        protected SettingViewModel(ISetting model, ISettingApplier settingsApplier, string description) : base(model)
        {
            Applier = settingsApplier;
            Description = description;
            OriginalValue = Applier.GetCurrentValue(this);
            _value = OriginalValue;
        }

        protected virtual void OnValueChanged(object? oldValue)
        {
            if (Applier.ChangeMode == SettingChangeMode.Direct)
            {
                Applier.Apply(this);
            } 
            else if(Applier.ChangeMode == SettingChangeMode.OnSave)
            {
                IsApplied = false;
            }
        }
            
        public virtual void Save()
        {
            if(Applier.ChangeMode == SettingChangeMode.OnSave && !IsApplied)
            {
                Applier.Apply(this);
                IsApplied = true;
            }
        }

        public void Restore() => Value = OriginalValue;
    }

    public abstract class SettingViewModel<TValue> : SettingViewModel 
    {
        public new TValue Value
        {
            get => (TValue)base.Value!;
            set => base.Value = value;
        }

        public new ISettingApplier<TValue> Applier => (ISettingApplier<TValue>)base.Applier;

        protected SettingViewModel(ISetting model, ISettingApplier<TValue> settingsApplier, string description) : base(model, settingsApplier, description)
        {
        }
    }

    public abstract class SettingViewModel<TValue, TModel> : SettingViewModel<TValue> where TModel : ISetting
    {
        public new TModel Model => (TModel)base.Model;

        protected SettingViewModel(TModel model, ISettingApplier<TValue> settingsApplier, string description) : base(model, settingsApplier, description)
        {
        }
    }
}
