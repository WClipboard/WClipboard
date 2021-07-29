using Microsoft.Toolkit.Uwp.Notifications;
using System.Threading.Tasks;
using System.Windows.Threading;
using WClipboard.Core;
using Windows.UI.Notifications;

namespace WClipboard.Windows.Notifications
{
    public interface INotificationsManager
    {
        Task ShowNotification(INotification notification);
        Task ShowNotification(ToastContentBuilder toastContentBuilder, CustomizeToast customizeToast);
    }

    public class NotificationsManager : INotificationsManager
    {
        private readonly ToastNotifier toastNotifier;
        private readonly Dispatcher dispatcher;

        public NotificationsManager(IStartMenuShortcutManager shortcutManager, IAppInfo appInfo)
        {
            shortcutManager.EnsureShortcut();
            toastNotifier = ToastNotificationManager.CreateToastNotifier(appInfo.Name);
            dispatcher = Dispatcher.CurrentDispatcher;
        }

        public Task ShowNotification(INotification notification)
        {
            return dispatcher.InvokeAsync(() => toastNotifier.Show(notification.CreateNotification())).Task;
        }

        public Task ShowNotification(ToastContentBuilder toastContentBuilder, CustomizeToast customizeToast)
        {
            return dispatcher.InvokeAsync(() => toastContentBuilder.Show(customizeToast)).Task;
        }
    }
}
