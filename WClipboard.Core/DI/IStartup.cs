using Microsoft.Extensions.DependencyInjection;
using System;

namespace WClipboard.Core.DI
{
    public interface IStartup
    {
        void ConfigureServices(IServiceCollection services, IStartupContext context);
    }
}
