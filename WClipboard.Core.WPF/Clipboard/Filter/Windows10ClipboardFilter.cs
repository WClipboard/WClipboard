using System.Collections.Generic;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.Settings;
using WClipboard.Core.WPF.Clipboard.Format;
using WClipboard.Core.WPF.Settings.Local;

namespace WClipboard.Core.WPF.Clipboard.Filter
{
    public class Windows10ClipboardFilter : IClipboardFilter
    {
        private readonly IIOSetting historySetting;
        private readonly IIOSetting cloudSetting;

        public Windows10ClipboardFilter(IIOSettingsManager iOSettingsManager)
        {
            historySetting = iOSettingsManager.GetSetting(SettingConsts.Windows10HistoryFilterKey);
            cloudSetting = iOSettingsManager.GetSetting(SettingConsts.Windows10CloudFilterKey);
        }

        public bool ShouldFilter(ClipboardTrigger clipboardTrigger, IEnumerable<EqualtableFormat> equaltableFormats)
        {
            if (clipboardTrigger.AdditionalInfo.TryGetValue<Windows10HistoryInfo>(out var historyInfo) && !historyInfo.Allowed && historySetting.GetValue<bool>())
            {
                return true;
            }
            if (clipboardTrigger.AdditionalInfo.TryGetValue<Windows10CloudInfo>(out var cloudInfo) && !cloudInfo.Allowed && cloudSetting.GetValue<bool>())
            {
                return true;
            }

            return false;
        }
    }
}
