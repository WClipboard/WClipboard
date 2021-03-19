using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WClipboard.Core.Utilities;

namespace WClipboard.Core.WPF.Clipboard.Implementation.LinkedContent
{
    public interface ILinkedContentFactoriesManagersManager : ICollectionManager<Type, ILinkedContentFactoriesManager>
    {
        Task ProvideAsync(IEnumerable<ClipboardImplementation> implementations, IClipboardObjectManager clipboardObjectManager);
    }

    public class LinkedContentFactoriesManagersManager : CollectionManager<Type, ILinkedContentFactoriesManager>, ILinkedContentFactoriesManagersManager
    {
        public LinkedContentFactoriesManagersManager(IEnumerable<ILinkedContentFactoriesManager> managers) : base(m => m.ForType, managers)
        {
        }

        public Task ProvideAsync(IEnumerable<ClipboardImplementation> implementations, IClipboardObjectManager clipboardObjectManager)
        {
            return Task.WhenAll(implementations.Select(i => TryGetValue(i.GetType(), out var manager) ? manager.ProvideAsync(i, clipboardObjectManager) : Task.CompletedTask));
        }
    }
}
