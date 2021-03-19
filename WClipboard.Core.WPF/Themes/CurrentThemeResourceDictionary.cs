using System.Windows;
using WClipboard.Core.DI;
using Microsoft.Extensions.DependencyInjection;
using WClipboard.Core.WPF.Utilities;

namespace WClipboard.Core.WPF.Themes
{
    public class CurrentThemeResourceDictionary : ResourceDictionary
    {
        public CurrentThemeResourceDictionary()
        {
            var themesManager = DiContainer.SP?.GetRequiredService<IThemesManager>()!;
            var currentTheme = themesManager.GetResolvedCurrent();
            Source = ResourceDictionaryUtilities.GetSourceUri(currentTheme.ResourceDictionaryLocation);
        }
    }
}
