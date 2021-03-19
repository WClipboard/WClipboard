using System;
using System.Threading.Tasks;

namespace WClipboard.Core.WPF.Clipboard.Implementation.LinkedContent
{
    public interface ILinkedContentFactoriesManager
    {
        Type ForType { get; }
        Task ProvideAsync(ClipboardImplementation implementation, IClipboardObjectManager clipboardObjectManager);
    }
}
