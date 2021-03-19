using System.Collections.Generic;
using WClipboard.Core.Settings;

namespace WClipboard.Core.WPF.Settings
{
    public abstract class BaseUISettingsFactory
    {
        public IReadOnlyList<string> SettingKeys { get; }

        protected BaseUISettingsFactory(IReadOnlyList<string> settingKeys)
        {
            SettingKeys = settingKeys;
        }

        public abstract SettingViewModel? Create(ISetting model);
    }
}
