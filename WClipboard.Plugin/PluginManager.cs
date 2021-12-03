using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using WClipboard.Core.DI;

namespace WClipboard.Plugin
{
    public interface IPluginManager
    {
        IEnumerable<IPlugin> Plugins { get; }

        void TryAddPlugin(string pluginLocation);
    }
    internal class PluginManager : IPluginManager, IStartup
    {
        private readonly List<IPlugin> _plugins;
        private readonly string _pluginDirectory;

        public IEnumerable<IPlugin> Plugins => _plugins;

        internal PluginManager(IStartupContext context)
        {
            _plugins = new List<IPlugin>();

            var appDirectory = Path.GetDirectoryName(context.AppInfo.Path);
            _pluginDirectory = Path.Combine(appDirectory, "Plugins");
            if (!Directory.Exists(_pluginDirectory))
                return;

            foreach (var pluginLocation in Directory.EnumerateFiles(_pluginDirectory, "*.dll", SearchOption.AllDirectories))
            {
                TryLoadPlugin(pluginLocation);
            }

#if DEBUG
            var debugFileName = Path.Combine(_pluginDirectory, "debug.txt");
            if (File.Exists(debugFileName))
            {
                using(StreamReader sr = new StreamReader(debugFileName))
                {
                    string pluginLocation = null;
                    while (!sr.EndOfStream)
                    {
                        pluginLocation = sr.ReadLine();
                        if (!string.IsNullOrWhiteSpace(pluginLocation))
                        {
                            TryLoadPlugin(Path.Combine(appDirectory, pluginLocation));
                        }
                    }
                }
            }
#endif
        }

        private void TryLoadPlugin(string pluginLocation)
        {
            try
            {
                var loadContext = new PluginLoadContext(pluginLocation);
                var assemblyName = Path.GetFileNameWithoutExtension(pluginLocation);
                var pluginAssembly = loadContext.LoadFromAssemblyName(new AssemblyName(assemblyName));
                var pluginMainClassName = $"{assemblyName}.{assemblyName.Split('.', StringSplitOptions.RemoveEmptyEntries).Last()}";
                if(!pluginMainClassName.EndsWith("Plugin"))
                    pluginMainClassName += "Plugin";
                var pluginMainClassType = pluginAssembly.GetType(pluginMainClassName);

                if (pluginMainClassType != null && typeof(IPlugin).IsAssignableFrom(pluginMainClassType) && pluginMainClassType.IsSealed)
                {
                    if (pluginMainClassType.GetConstructor(Array.Empty<Type>()).Invoke(Array.Empty<object>()) is IPlugin plugin)
                    {
                        _plugins.Add(plugin);
                        return;
                    }
                }

                loadContext.Unload();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load plugin {pluginLocation}. {ex.GetType().Name}: {ex.Message}");
            }
        }

        public void TryAddPlugin(string pluginLocation)
        {
            ZipFile.ExtractToDirectory(pluginLocation, _pluginDirectory, true);
        }

        public void ConfigureServices(IServiceCollection services, IStartupContext context)
        {
            foreach (var plugin in _plugins)
            {
                services.AddAutoInject(plugin);

                plugin.ConfigureServices(services, context);
            }
        }
    }
}
