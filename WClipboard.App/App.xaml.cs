using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using WClipboard.Core.DI;
using WClipboard.Core.Utilities;
using WClipboard.Core.WPF.LifeCycle;

namespace WClipboard.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            DiContainer.Dispose();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Log(LogLevel.Critical, $"Exception cathed in {nameof(DispatcherUnhandledException)}");
            Logger.Log(LogLevel.Critical, e.Exception);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            foreach (var informService in DiContainer.SP!.GetServices<IAfterWPFAppStartupListener>())
            {
                informService.AfterWPFAppStartup();
            }

            base.OnStartup(e);
        }
    }
}
