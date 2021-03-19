using WClipboard.Core.IO;
using WClipboard.Core.Settings;

namespace WClipboard.Core.DI
{
    internal class StartupContext : IStartupContext
    {
        public IAppInfo AppInfo { get; }

        public IAppDataManager AppDataManager { get; }

        public IIOSettingsManager IOSettingsManager { get; }

        public StartupContext(IAppInfo appInfo, IAppDataManager appDataManager, IIOSettingsManager iOSettingsManager)
        {
            AppInfo = appInfo;
            AppDataManager = appDataManager;
            IOSettingsManager = iOSettingsManager;
        }
    }
}
