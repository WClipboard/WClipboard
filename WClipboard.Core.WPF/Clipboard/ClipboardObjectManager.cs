using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.Extensions;
using WClipboard.Core.WPF.Clipboard.Format;
using WClipboard.Core.WPF.Clipboard.Implementation;
using WClipboard.Core.WPF.Clipboard.Implementation.ViewModel;
using WClipboard.Core.WPF.Clipboard.Metadata;
using WClipboard.Core.WPF.Clipboard.ViewModel;
using WClipboard.Core.WPF.Managers;
using WClipboard.Core.WPF.ViewModels;

namespace WClipboard.Core.WPF.Clipboard
{
    public interface IClipboardObjectManager
    {
        Task AddImplementationsAsync(ClipboardObject clipboardObject, IEnumerable<EqualtableFormat> equaltableFormats);
        Task AddImplementationsAsync(ClipboardObject clipboardObject, Stream stream, ClipboardFormat format);
        IAsyncEnumerable<ClipboardImplementation> CreateLinkedImplementationsAsync(DataObject dataObject, ClipboardImplementation implementation);
        ClipboardImplementationViewModel? CreateViewModel(ClipboardImplementation implementation, ClipboardObjectViewModel clipboardObject);
        void AddMetadata(ClipboardObjectViewModel clipboardObject);
    }

    public sealed class ClipboardObjectManager : IClipboardObjectManager
    {
        private readonly List<ClipboardImplementationFactory> _implementationFactories;
        private readonly IViewModelFactoriesManager _viewModelFactoriesManager;
        private readonly List<ClipboardObjectMetadataFactory> _clipboardObjectMetadataFactories;
        private readonly IInteractablesManager _interactablesManager;

        public ClipboardObjectManager(
            IEnumerable<ClipboardImplementationFactory> implementationFactories,
            IViewModelFactoriesManager viewModelFactoriesManager,
            IEnumerable<ClipboardObjectMetadataFactory> clipboardObjectMetadataFactories,
            IInteractablesManager interactablesManager)
        {
            _implementationFactories = new List<ClipboardImplementationFactory>(implementationFactories);
            _viewModelFactoriesManager = viewModelFactoriesManager;
            _clipboardObjectMetadataFactories = new List<ClipboardObjectMetadataFactory>(clipboardObjectMetadataFactories);
            _interactablesManager = interactablesManager;
        }

        public async Task AddImplementationsAsync(ClipboardObject clipboardObject, IEnumerable<EqualtableFormat> equaltableFormats)
        {
            await Task.WhenAll(equaltableFormats.Select(ef => AddImplementationsAsync(clipboardObject, ef))).ConfigureAwait(false);
        }

        private async Task AddImplementationsAsync(ClipboardObject clipboardObject, EqualtableFormat equatableFormat)
        {
            clipboardObject.Implementations.AddRange((await Task.WhenAll(_implementationFactories.Select(f => f.CreateFromEquatable(clipboardObject, equatableFormat))).ConfigureAwait(false)).NotNull());
        }

        public async Task AddImplementationsAsync(ClipboardObject clipboardObject, Stream stream, ClipboardFormat format)
        {
            var implementations = (await Task.WhenAll(_implementationFactories.Select(f => f.Deserialize(clipboardObject, stream, format))).ConfigureAwait(false)).SelectMany(i => i).ToList();
            foreach (var implementation in implementations)
            {
                clipboardObject.Implementations.Add(implementation);
            }
        }

        public ClipboardImplementationViewModel? CreateViewModel(ClipboardImplementation implementation, ClipboardObjectViewModel clipboardObject)
        {
            var returner = _viewModelFactoriesManager.OfType<ClipboardImplementationViewModelFactory>().Select(f => f.Create(implementation, clipboardObject)).FirstOrDefault(i => !(i is null));
            if (!(returner is null))
                _interactablesManager.AssignStates(returner);
            return returner;
        }

        public void AddMetadata(ClipboardObjectViewModel clipboardObject)
        {
            foreach(var factory in _clipboardObjectMetadataFactories)
            {
                clipboardObject.Metadata.AddRange(factory.Create(clipboardObject));
            }
        }

        public async IAsyncEnumerable<ClipboardImplementation> CreateLinkedImplementationsAsync(DataObject dataObject, ClipboardImplementation implementation)
        {
            foreach (var factory in _implementationFactories)
            {
                var result = await factory.CreateLinkedFromDataObject(implementation, dataObject).ConfigureAwait(false);
                if (!(result is null))
                {
                    yield return result;
                }
            }
        }
    }
}
