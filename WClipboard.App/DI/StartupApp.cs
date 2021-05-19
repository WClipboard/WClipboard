using Microsoft.Extensions.DependencyInjection;
using WClipboard.App.Cursors;
using WClipboard.App.Settings;
using WClipboard.App.Setup;
using WClipboard.App.ViewModels.Interactables;
using WClipboard.Core.DI;
using WClipboard.Core.Settings.Defaults;
using WClipboard.Core.WPF.Extensions;
using WClipboard.Core.WPF.Themes;
using WClipboard.Core.WPF.Utilities;

namespace WClipboard.App.DI
{
    public sealed class StartupApp : IStartup
    {
        void IStartup.ConfigureServices(IServiceCollection services, IStartupContext context)
        {
            context.IOSettingsManager.AddSettings(new EnumSetting<MinimizeTo>(AppUISettingsFactory.MinimizeTo, MinimizeTo.Taskbar));

            services.AddSingleton<ICursorManager, CursorManager>();

            services.AddInteractable<OpenSettingsInteractable>();

            services.AddUISettingsFactory<AppUISettingsFactory>();

            services.AddTheme(
                new Theme("Dark", "Themes/Dark.xaml", context.AppInfo.Name),
                new Theme("Light", "Themes/Light.xaml", context.AppInfo.Name));

            services.AddTransientWithAutoInject<UpdateChecker>();
        }
    }
}
