using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using WClipboard.Core.Settings;

namespace WClipboard.Core.WPF.Settings.Defaults
{
    public class TextSettingViewModel : SettingViewModel<string>
    {
        public IEnumerable<ValidationRule> ValidationRules { get; }

        public TextSettingViewModel(ISetting model, ISettingApplier<string> settingsApplier, string description, IEnumerable<ValidationRule>? validationRules = null) : base(model, settingsApplier, description)
        {
            ValidationRules = validationRules?.ToList() ?? Enumerable.Empty<ValidationRule>();
        }
    }
}
