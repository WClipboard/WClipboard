using Lamar;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using WClipboard.Core.IO;
using WClipboard.Core.LifeCycle;
using WClipboard.Core.Settings;

namespace WClipboard.Core.DI
{
    public class DiContainer
    {
        public static IServiceProvider? SP => _container;

        private static Container? _container;
        private static DiContainer? _setup;

        public static DiContainer Setup()
        {
            if (_container != null)
                throw new InvalidOperationException("Container has already bin setup");
            if (_setup != null)
                throw new InvalidOperationException("Setup is already in progress");
            _setup = new DiContainer();
            return _setup;
        }

        public static void Dispose()
        {
            if (_container != null)
            {
                _container.Dispose();
                _container = null;
            }
        }

        private List<IStartup>? startups;

        private DiContainer()
        {
            startups = new List<IStartup>();
        }

        public DiContainer Add<TStartup>() where TStartup : IStartup, new()
        {
            return Add(new TStartup());
        }

        public DiContainer Add(IStartup startup)
        {
            if (startups == null)
            {
                throw new InvalidOperationException("Cannot add to an already build container");
            }

            startups.Add(startup);
            return this;
        }

        public void Build(IAppInfo appInfo)
        {
            if (startups == null)
            {
                throw new InvalidOperationException("Cannot build an already build container");
            }

            var serviceRegistry = new ServiceRegistry();

            var appDataManager = new AppDataManager(appInfo);
            var settingManager = new IOSettingsManager(appDataManager);

            serviceRegistry.AddSingleton(appInfo);
            serviceRegistry.AddSingleton<IAppDataManager>(appDataManager);
            serviceRegistry.AddSingletonWithAutoInject<IIOSettingsManager, IOSettingsManager>(settingManager);

            var startupContext = new StartupContext(appInfo, appDataManager, settingManager);

            foreach (var startup in startups)
            {
                serviceRegistry.AddAutoInject(startup);
                startup.ConfigureServices(serviceRegistry, startupContext);
            }

            _container = new Container(serviceRegistry);
            _setup = null;

            startups.Clear();
            startups = null;

            foreach(var informService in _container.GetServices<IAfterDIContainerBuildListener>())
            {
                informService.AfterDIContainerBuild();
            }
        }
    }
}
