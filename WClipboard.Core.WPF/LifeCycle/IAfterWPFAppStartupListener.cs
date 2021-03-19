using WClipboard.Core.DI;

namespace WClipboard.Core.WPF.LifeCycle
{
    [AutoInject]
    public interface IAfterWPFAppStartupListener
    {
        void AfterWPFAppStartup();
    }
}
