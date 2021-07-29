using Windows.UI.Notifications;

namespace WClipboard.Windows.Notifications
{
    public interface INotification
    {
        ToastNotification CreateNotification();
    }
}
