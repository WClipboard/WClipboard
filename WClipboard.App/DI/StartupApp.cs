using Microsoft.Extensions.DependencyInjection;
using WClipboard.App.Settings;
using WClipboard.App.ViewModels.Interactables;
using WClipboard.Core.DI;
using WClipboard.Core.WPF.Extensions;
using WClipboard.Core.WPF.Themes;

namespace WClipboard.App.DI
{
    public sealed class StartupApp : IStartup
    {
        void IStartup.ConfigureServices(IServiceCollection services, IStartupContext context)
        {
            services.AddInteractable<OpenSettingsInteractable>();

            services.AddUISettingsFactory<AppUISettingsFactory>();

            services.AddTheme(
                new Theme("Dark", "Themes/Dark.xaml", context.AppInfo.Name),
                new Theme("Light", "Themes/Light.xaml", context.AppInfo.Name));
        }
    }
}
