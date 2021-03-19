using Microsoft.Extensions.DependencyInjection;
using WClipboard.Core.DI;

namespace WClipboard.Notifications.DI
{
    public class StartupNotifications : IStartup
    {
        public void ConfigureServices(IServiceCollection services, IStartupContext context)
        {
            services.AddSingletonWithAutoInject<INotificationsManager, NotificationsManager>();
        }
    }
}
