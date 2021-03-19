using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.IO;
using WClipboard.Core.Settings.Defaults;

namespace WClipboard.Core.DI
{
    public sealed class StartupCore : IStartup
    {
        void IStartup.ConfigureServices(IServiceCollection services, IStartupContext context)
        {
            context.IOSettingsManager.AddSerializers(new DefaultIOSettingsSerializer());

            services.AddSingleton<IClipboardFormatCategoriesManager, ClipboardFormatCategoriesManager>();
            services.AddSingleton<IClipboardFormatsManager, ClipboardFormatsManager>();

            services.AddSingleton<ITempManager, TempManager>();

            services.AddSingleton<HttpClient>();
        }
    }
}
