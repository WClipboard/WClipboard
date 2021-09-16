using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WClipboard.App.ViewModels;
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
            Logger.Log(LogLevel.Critical, $"Exception cathed in {nameof(DispatcherUnhandledException)}", e.Exception);

            Shutdown(-2);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            foreach (var informService in DiContainer.SP!.GetServices<IAfterWPFAppStartupListener>())
            {
                informService.AfterWPFAppStartup();
            }

            var overviewWindow = DiContainer.SP!.Create<OverviewWindowViewModel>();
            MainWindow = overviewWindow.Window;

            base.OnStartup(e);
        }
    }
}
