using ShellLinkPlus;
using System;
using System.Diagnostics;
using WClipboard.Core.LifeCycle;
using Microsoft.Extensions.DependencyInjection;
using WClipboard.Core;
using System.IO;

namespace WClipboard.Notifications
{
    public interface INotificationsManager
    {

    }

    public class NotificationsManager : INotificationsManager, IAfterDIContainerBuildListener
    {
        public NotificationsManager()
        {
        }

        public static void DeleteStartMenuShortcut(IAppInfo appInfo)
        {
            var shortcut = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @$"\Programs\{appInfo.Name}.lnk";
            if (File.Exists(shortcut)) {
                File.Delete(shortcut);
            }
        }

        public static void CreateStartMenuShortcut(IAppInfo appInfo)
        {
            string shortcutFolder = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\Programs\";
            using (ShellLink shortcut = new ShellLink())
            {
                var currentProcess = Process.GetCurrentProcess();
                shortcut.TargetPath = appInfo.Path;
                shortcut.AppUserModelID = appInfo.Name;

                shortcut.Save($"{shortcutFolder}{appInfo.Name}.lnk");
            }
        }

        public void AfterDIContainerBuild()
        {
            CreateStartMenuShortcut(Core.DI.DiContainer.SP!.GetService<IAppInfo>()!);
        }
    }
}
