using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.IO;
using WClipboard.Core.Settings.Defaults;
using WClipboard.Core.Utilities;

namespace WClipboard.Core.DI
{
    public sealed class StartupCore : IStartup
    {
        void IStartup.ConfigureServices(IServiceCollection services, IStartupContext context)
        {
            context.IOSettingsManager.AddSerializers(new DefaultIOSettingsSerializer(), new EnumIOSettingsSerializer());

            services.AddSingleton<IClipboardFormatCategoriesManager, ClipboardFormatCategoriesManager>();
            services.AddSingleton<IClipboardFormatsManager, ClipboardFormatsManager>();

            services.AddSingleton<ITempManager, TempManager>();

            services.AddSingleton<HttpClient>();

            services.AddTransient(typeof(ILogger<>), typeof(Logger<>));
        }
    }
}
