using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using WClipboard.App.Settings;
using WClipboard.Core;
using WClipboard.Windows;

namespace WClipboard.App.Setup
{
    public enum InstalledState
    {
        NotInstalled,
        OlderVersionPresent,
        NewerVersionPresent,
        Installed
    }

    internal class Installer
    {
        private readonly IAppInfo appInfo;

        internal Installer(IAppInfo appInfo)
        {
            this.appInfo = appInfo;
        }

        internal InstalledState GetStatus()
        {
            using (var appRegisteryKey = Registry.CurrentUser.OpenSubKey($@"SOFTWARE\{appInfo.Name}")) {
                if (!(appRegisteryKey?.GetValue("Version") is string versionStr))
                {
                    return InstalledState.NotInstalled;
                }

                var version = Version.Parse(versionStr);

                if (version < appInfo.Version)
                {
                    return InstalledState.OlderVersionPresent;
                }
                else if (version > appInfo.Version)
                {
                    return InstalledState.NewerVersionPresent;
                }
                else
                {
                    return InstalledState.Installed;
                }
            }
        }

        private void CloseOtherProcessInstances()
        {
            var ourProcess = Process.GetCurrentProcess();
            var otherProcesses = Process.GetProcessesByName(ourProcess.ProcessName);

            Parallel.ForEach(otherProcesses, (process) =>
            {
                if (process.Id != ourProcess.Id)
                {
                    process.CloseMainWindow();
                    process.WaitForExit(2000);
                    if (!process.HasExited)
                        process.Kill();
                }
            });
        }

        internal void Install()
        {
            CloseOtherProcessInstances();

            using (var appRegisteryKey = Registry.CurrentUser.CreateSubKey($@"SOFTWARE\{appInfo.Name}"))
            {
                if (appRegisteryKey.GetValue("LicenseAccepted")?.Equals(1) != true)
                {
                    if (MessageBox.Show(@$"{appInfo.Name} - An intelligent, free to use, opensource clipboard manager
Copyright (C) {DateTime.Now.Year}  Wibren Wiersma

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, version 3 of the License.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program.  If not, see <https://www.gnu.org/licenses/>.

By pressing OK you accept the license", "License", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.Cancel) { return; }
                }

                if (!appRegisteryKey.GetValue("WarningAccepted")?.Equals(1) != true)
                {
                    if (MessageBox.Show($@"{appInfo.Name} is currently under development.
Because of that it may happen that WClipboard contains security vulnerabilities.
The task of a clipboard manager is to processes clipboard data, which can be sensitve.

By pressing OK you are informed about and accept; the related security and privacy risks.", "Beta software and risks", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel) { return; }
                }

                appRegisteryKey.SetValue("LicenseAccepted", 1, RegistryValueKind.DWord);
                appRegisteryKey.SetValue("WarningAccepted", 1, RegistryValueKind.DWord);
                appRegisteryKey.SetValue("Version", appInfo.Version.ToString(), RegistryValueKind.String);
            }

            new StartMenuShortcutManager(appInfo).EnsureShortcut();
        }

        internal void Uninstall()
        {
            CloseOtherProcessInstances();

            Registry.CurrentUser.DeleteSubKeyTree($@"SOFTWARE\{appInfo.Name}");

            // Now start removal
            OpenOnStartupSettingsApplier.RemoveStartup(appInfo);

            new StartMenuShortcutManager(appInfo).DeleteShortcut();
        }
    }
}
