using Microsoft.Extensions.DependencyInjection;
using WClipboard.Core.DI;

namespace WClipboard.Windows.DI
{
    public class StartupWindows : IStartup
    {
        public void ConfigureServices(IServiceCollection services, IStartupContext context)
        {
            services.AddSingleton<IGlobalKeyboardListener, GlobalKeyboardListener>();
            services.AddSingletonWithAutoInject<INotificationsManager, NotificationsManager>();
            services.AddSingleton<IHiddenWindowMessages, HiddenWindowMessages>();
            services.AddSingleton<ITaskbarIcon, TaskbarIcon>();
            services.AddSingleton<IClipboardViewer, ClipboardViewer>();
        }
    }
}
