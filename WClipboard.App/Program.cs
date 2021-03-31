using System;
using System.Linq;
using WClipboard.App.DI;
using WClipboard.App.Models;
using WClipboard.App.Setup;
using WClipboard.Core.DI;
using WClipboard.Core.WPF.DI;
using WClipboard.Notifications.DI;
using WClipboard.Plugin.DI;

namespace WClipboard.App
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var appInfo = new AppInfo(args);
            var installer = new Installer(appInfo);

            if (args.Contains("/uninstall"))
            {
                installer.Uninstall();
                return;
            }
            else if (args.Contains("/install"))
            {
                installer.Install();
                return;
            }
            else
            {
                var installedState = installer.GetStatus();

                if (installedState == InstalledState.NotInstalled || installedState == InstalledState.OlderVersionPresent)
                {
                    installer.Install();
                    return;
                } 
                else if (installedState == InstalledState.NewerVersionPresent)
                {
                    return;
                }
            }


            if (args.Contains("/nodebug")) {
                LaunchApp(appInfo);
            } else {
                ProgramDebugger.Run(appInfo);
            }
        }

        private static void LaunchApp(AppInfo appInfo)
        {
            DiContainer.Setup()
                .Add<StartupCore>()
                .Add<StartupNotifications>()
                .Add<StartupWpf>()
                .Add<StartupPlugin>()
                .Add<StartupApp>()
                .Build(appInfo);

            App.Main();
        }
    }
}
