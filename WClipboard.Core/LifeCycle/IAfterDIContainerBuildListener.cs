using WClipboard.Core.DI;

namespace WClipboard.Core.LifeCycle
{
    /// <summary>
    /// The <see cref="DiContainer.Build"/> will invoke <see cref="AfterDIContainerBuild()"/> on all singleton registered classes, if injected with <see cref="DI.Extensions.AddSingletonWithAutoInject"/>, after building.
    /// </summary>
    [AutoInject]
    public interface IAfterDIContainerBuildListener
    {
        void AfterDIContainerBuild();
    }
}
