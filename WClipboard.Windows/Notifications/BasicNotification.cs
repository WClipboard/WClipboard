using Microsoft.Toolkit.Uwp.Notifications;
using System;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace WClipboard.Windows.Notifications
{
    public class BasicNotification : INotification
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public event EventHandler? OnClick;

        public ToastNotification CreateNotification()
        {
            


            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml($@"
                <toast>
                    <visual>
                        <binding template='ToastGeneric'>
                            <text>{Title}</text>
                            <text>{Message}</text>
                        </binding>
                    </visual>
                    <actions></actions>
                </toast>");

            ToastNotification toast = new ToastNotification(toastXml);
            toast.Activated += Toast_Activated;

            //if (options.ExpirationTime.HasValue)
            //{
            //    toast.ExpirationTime = new DateTimeOffset(DateTime.Now.Add(options.ExpirationTime.Value));
            //}
            return toast;
        }

        private void Toast_Activated(ToastNotification sender, object args)
        {
            OnClick?.Invoke(this, new EventArgs());
        }
    }
}
