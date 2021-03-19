using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using WClipboard.Core.Settings;
using WClipboard.Core.Settings.Defaults;
using WClipboard.Core.Utilities;
using WClipboard.Core.WPF.Settings.Local;

namespace WClipboard.Core.WPF.Themes
{
    public interface IThemesManager : ICollectionManager<string, Theme>
    {
        Theme GetCurrent();
        Theme GetResolvedCurrent();
    }

    public class ThemesManager : CollectionManager<string, Theme>, IThemesManager
    {
        private const string CurrentAppThemeRegisteryKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string CurrentAppThemeRegisteryValueName = "AppsUseLightTheme";

        private readonly KeyedCollectionSetting<string, Theme, IThemesManager> setting;

        public ThemesManager(IEnumerable<Theme> themes, IIOSettingsManager settingsManager) : base(t => t.Name, themes)
        {
            if (ContainsKey(SettingConsts.ThemeDefaultName))
                throw new InvalidOperationException($"It is not allowed to register a {nameof(Theme)} with {nameof(Theme.Name)} \"{SettingConsts.ThemeDefaultName}\"");

            _collection.Add(new Theme(SettingConsts.ThemeDefaultName, string.Empty, "Windows"));

            setting = (KeyedCollectionSetting<string, Theme, IThemesManager>)settingsManager.GetSetting(SettingConsts.ThemeKey);
        }

        public Theme GetCurrent() => setting.GetResolvedValue();

        public Theme GetResolvedCurrent()
        {
            var current = GetCurrent();
            if (current.Name != SettingConsts.ThemeDefaultName)
                return current;

            if (TryGetValue(GetCurrentWindowsTheme(), out current))
                return current;

            //Still here?! -> use the first
            return this[Keys.First()];
        }

        private static string GetCurrentWindowsTheme()
        {
            return Registry.GetValue(CurrentAppThemeRegisteryKey, CurrentAppThemeRegisteryValueName, null) is int currentAppTheme && currentAppTheme == 0
                ? "Dark"
                : "Light";
        }
    }
}
