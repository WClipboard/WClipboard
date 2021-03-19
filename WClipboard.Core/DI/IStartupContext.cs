using WClipboard.Core.IO;
using WClipboard.Core.Settings;

namespace WClipboard.Core.DI
{
    public interface IStartupContext
    {
        IAppInfo AppInfo {get;}
        IAppDataManager AppDataManager { get; }
        IIOSettingsManager IOSettingsManager { get; }
    }
}
