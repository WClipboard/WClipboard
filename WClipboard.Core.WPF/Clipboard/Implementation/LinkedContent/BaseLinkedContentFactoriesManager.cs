using System;
using System.Threading.Tasks;

namespace WClipboard.Core.WPF.Clipboard.Implementation.LinkedContent
{
    public abstract class BaseLinkedContentFactoriesManager<TImplementation> : ILinkedContentFactoriesManager where TImplementation : ClipboardImplementation
    {
        public Type ForType => typeof(TImplementation);

        public Task ProvideAsync(ClipboardImplementation implementation, IClipboardObjectManager clipboardObjectManager)
        {
            return ProvideAsync((TImplementation)implementation, clipboardObjectManager);
        }

        protected abstract Task ProvideAsync(TImplementation implementation, IClipboardObjectManager clipboardObjectManager);
    }
}
