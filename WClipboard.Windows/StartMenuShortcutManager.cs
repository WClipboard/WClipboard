using ShellLinkPlus;
using System;
using System.IO;
using WClipboard.Core;

namespace WClipboard.Windows
{
    public interface IStartMenuShortcutManager
    {
        void DeleteShortcut();
        void EnsureShortcut();
    }

    public class StartMenuShortcutManager : IStartMenuShortcutManager
    {
        private readonly IAppInfo appInfo;
        private readonly string shortcutFile;

        public StartMenuShortcutManager(IAppInfo appInfo)
        {
            this.appInfo = appInfo;
            shortcutFile = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @$"\Programs\{appInfo.Name}.lnk";
        }

        public void DeleteShortcut()
        {
            if (File.Exists(shortcutFile))
            {
                File.Delete(shortcutFile);
            }
        }

        public void EnsureShortcut()
        {
            bool isModified = false;

            using (ShellLink shortcut = new ShellLink())
            {
                if (File.Exists(shortcutFile))
                    shortcut.Load(shortcutFile);

                if (shortcut.TargetPath != appInfo.Path) { shortcut.TargetPath = appInfo.Path; isModified = true; }
                if (shortcut.AppUserModelID != appInfo.Name) { shortcut.AppUserModelID = appInfo.Name; isModified = true; }

                if (isModified)
                {
                    shortcut.Save(shortcutFile);
                }
            }
        }
    }
}
