using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using WClipboard.App.Settings;
using WClipboard.Core;

namespace WClipboard.App
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
            using (var appInstallKey = Registry.LocalMachine.OpenSubKey($@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{appInfo.Name}"))
            {
                if (appInstallKey == null)
                    return InstalledState.NotInstalled;

                var version = Version.Parse(appInstallKey.GetValue("DisplayVersion") as string);

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

            if (MessageBox.Show(@$"{appInfo.Name} - An intelligent, free to use, opensource clipboard manager
Copyright (C) {DateTime.Now.Year}  Wibren Wiersma

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, version 3 of the License.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program.  If not, see <https://www.gnu.org/licenses/>.

By pressing OK you accept the license", "License", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.Cancel) { return; }

            if (MessageBox.Show($@"{appInfo.Name} is currently under development.
Because of that it may happen that WClipboard contains security vulnerabilities.
The task of a clipboard manager is to processes clipboard data, which can be sensitve.

By pressing OK you are informed about and accept; the related security and privacy risks.", "Beta software and risks", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel) { return; }


            using (var appInstallKey = Registry.LocalMachine.CreateSubKey($@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{appInfo.Name}"))
            {
                appInstallKey.SetValue("DisplayName", appInfo.Name);
                appInstallKey.SetValue("DisplayIcon", appInfo.Path);
                appInstallKey.SetValue("DisplayVersion", appInfo.Version.ToString());
                appInstallKey.SetValue("VersionMajor", appInfo.Version.Major);
                appInstallKey.SetValue("VersionMinor", appInfo.Version.Minor);
                appInstallKey.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"));
                appInstallKey.SetValue("InstallLocation", appInfo.GetDirectory() ?? string.Empty);
                appInstallKey.SetValue("UninstallString", appInfo.Path + " /uninstall");

                appInstallKey.SetValue("NoModify", 1);
                appInstallKey.SetValue("NoRepair", 1);
                appInstallKey.SetValue("Publisher", "WClipboard");
                appInstallKey.SetValue("HelpLink", "https://wclipboard.com");
                appInstallKey.SetValue("URLInfoAbout", "https://wclipboard.com");
            }

            Notifications.NotificationsManager.CreateStartMenuShortcut(appInfo);
        }

        internal void Uninstall()
        {
            CloseOtherProcessInstances();

            // Now start removal
            OpenOnStartupSettingsApplier.RemoveStartup(appInfo);

            Notifications.NotificationsManager.DeleteStartMenuShortcut(appInfo);

            Registry.LocalMachine.DeleteSubKeyTree($@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{appInfo.Name}");

            if (MessageBox.Show($"{appInfo.Name} is installed in {appInfo.GetDirectory()}.\nIs it safe to delete the whole directory?\n\n(if not sure, choice no and delete manualy)", "Is it safe to delete the whole installation directory?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // Prepare for delete
                var deleteProgram = new ProcessStartInfo()
                {
                    Arguments = $"/C choice /C Y /N /D Y /T 3 & Del /F /S /Q \"{appInfo.GetDirectory()}\" & rmdir \"{appInfo.GetDirectory()}\" /s /q",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    FileName = "cmd.exe",
                    UseShellExecute = true,
                };

                Process.Start(deleteProgram);
            }
            else
            {
                MessageBox.Show("After pressing OK the directory will be opened. You can delete the files belonging to the program by yourself", "Info: delete it yourself", MessageBoxButton.OK, MessageBoxImage.Information);
                Process.Start(new ProcessStartInfo($"\"{appInfo.GetDirectory()}\"") { UseShellExecute = true });
            }
        }
    }
}
