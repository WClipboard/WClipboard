using System.Collections.Generic;
using System.Linq;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.Settings;
using WClipboard.Core.WPF.Clipboard.Format;
using WClipboard.Core.WPF.Settings.Local;

namespace WClipboard.Core.WPF.Clipboard.Filter
{
    public class OwnerProgramClipboardFilter : IClipboardFilter
    {
        private readonly IIOSetting programFilterSetting;

        public OwnerProgramClipboardFilter(IIOSettingsManager settingsManager)
        {
            programFilterSetting = settingsManager.GetSetting(SettingConsts.OwnerProgramClipboardFilterKey);
        }

        public bool ShouldFilter(ClipboardTrigger clipboardTrigger, IEnumerable<EqualtableFormat> equaltableFormats)
        {
            if(programFilterSetting.Value is List<string> paths)
            {
                if(clipboardTrigger.DataSourceProgram?.Path != null && paths.Any(p => string.Equals(p, clipboardTrigger.DataSourceProgram.Path, System.StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
