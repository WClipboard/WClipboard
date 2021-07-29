using WClipboard.Core.DI;
using WClipboard.Core.WPF.ViewModels;

namespace WClipboard.Core.WPF.LifeCycle
{
    [AutoInject]
    public interface IAfterMainWindowLoadedListener
    {
        void AfterMainWindowLoaded(IMainWindowViewModel mainWindow);
    }
}
