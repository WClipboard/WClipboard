using Microsoft.Extensions.DependencyInjection;
using WClipboard.Core.DI;
using WClipboard.Windows.Notifications;

namespace WClipboard.Windows.DI
{
    public class StartupWindows : IStartup
    {
        public void ConfigureServices(IServiceCollection services, IStartupContext context)
        {
            services.AddSingleton<IGlobalKeyboardListener, GlobalKeyboardListener>();
            services.AddSingleton<INotificationsManager, NotificationsManager>();
            services.AddSingleton<IStartMenuShortcutManager, StartMenuShortcutManager>();
            services.AddSingleton<IHiddenWindowMessages, HiddenWindowMessages>();
            services.AddSingleton<ITaskbarIcon, TaskbarIcon>();
            services.AddSingleton<IClipboardViewer, ClipboardViewer>();
        }
    }
}
