using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.Extensions;
using WClipboard.Core.WPF.Clipboard.Implementation;
using WClipboard.Core.WPF.Clipboard.Implementation.LinkedContent;
using WClipboard.Core.WPF.Clipboard.Implementation.ViewModel;
using WClipboard.Core.WPF.Clipboard.Metadata;
using WClipboard.Core.WPF.Clipboard.ViewModel;
using WClipboard.Core.WPF.Managers;
using WClipboard.Core.WPF.Utilities;
using WClipboard.Core.WPF.ViewModels;

namespace WClipboard.Core.WPF.Clipboard
{
    public interface IClipboardObjectManager
    {
        IReadOnlyList<EqualtableFormat> GetEqualtableFormats(DataObject dataObject);
        Task AddImplementationsAsync(ClipboardObject clipboardObject, IEnumerable<EqualtableFormat> equaltableFormats);
        Task AddImplementationsAsync(ClipboardObject clipboardObject, Stream stream, ClipboardFormat format);
        ClipboardImplementationViewModel? CreateViewModel(ClipboardImplementation implementation, ClipboardObjectViewModel clipboardObject);
        void AddMetadata(ClipboardObjectViewModel clipboardObject);
        BaseViewModel? CreateViewModel(object linkedContent, ClipboardImplementationViewModel parent);
        void AddLinkedContent(ClipboardImplementation parent, object content);
    }

    public sealed class ClipboardObjectManager : IClipboardObjectManager
    {
        private readonly List<ClipboardImplementationFactory> _implementationFactories;
        private readonly List<IViewModelFactory> _viewModelFactories;
        private readonly List<ClipboardObjectMetadataFactory> _clipboardObjectMetadataFactories;
        private readonly IInteractablesManager _interactablesManager;
        private readonly ILinkedContentFactoriesManagersManager _linkedContentFactoriesManagersManager;

        public ClipboardObjectManager(
            IEnumerable<ClipboardImplementationFactory> implementationFactories,
            IEnumerable<IViewModelFactory> viewModelFactories,
            IEnumerable<ClipboardObjectMetadataFactory> clipboardObjectMetadataFactories,
            IInteractablesManager interactablesManager,
            ILinkedContentFactoriesManagersManager linkedContentFactoriesManagersManager)
        {
            _implementationFactories = new List<ClipboardImplementationFactory>(implementationFactories);
            _viewModelFactories = new List<IViewModelFactory>(viewModelFactories);
            _clipboardObjectMetadataFactories = new List<ClipboardObjectMetadataFactory>(clipboardObjectMetadataFactories);
            _interactablesManager = interactablesManager;
            _linkedContentFactoriesManagersManager = linkedContentFactoriesManagersManager;
        }

        public IReadOnlyList<EqualtableFormat> GetEqualtableFormats(DataObject dataObject)
        {
            var formats = new List<EqualtableFormat>();

            foreach(var factory in _implementationFactories)
            {
                formats.AddRange(factory.CreateEquatables(dataObject) ?? Enumerable.Empty<EqualtableFormat>());
            }

            return formats;
        }

        public async Task AddImplementationsAsync(ClipboardObject clipboardObject, IEnumerable<EqualtableFormat> equaltableFormats)
        {
            await Task.WhenAll(equaltableFormats.Select(ef => AddImplementationAsync(clipboardObject, ef))).ConfigureAwait(false);
            await _linkedContentFactoriesManagersManager.ProvideAsync(clipboardObject.Implementations, this).ConfigureAwait(false);
        }

        private async Task AddImplementationAsync(ClipboardObject clipboardObject, EqualtableFormat equatableFormat)
        {
            clipboardObject.Implementations.Add(await equatableFormat.Factory.CreateFromEquatable(clipboardObject, equatableFormat).ConfigureAwait(false));
        }

        public async Task AddImplementationsAsync(ClipboardObject clipboardObject, Stream stream, ClipboardFormat format)
        {
            var implementations = (await Task.WhenAll(_implementationFactories.Select(f => f.Deserialize(clipboardObject, stream, format))).ConfigureAwait(false)).SelectMany(i => i).ToList();
            foreach (var implementation in implementations)
            {
                clipboardObject.Implementations.Add(implementation);
            }
            await _linkedContentFactoriesManagersManager.ProvideAsync(implementations, this).ConfigureAwait(false);
        }

        public async void AddLinkedContent(ClipboardImplementation parent, object content)
        {
            if (parent.LinkedImplementations is null || parent.LinkedContent is null)
                throw new InvalidOperationException($"Its not possible to add linked content when {nameof(parent.LinkedImplementations)} or {nameof(parent.LinkedContent)} is null");

            if (content is DataObject dataObject)
            {
                foreach (var facotry in _implementationFactories)
                {
                    var result = await facotry.CreateLinkedFromDataObject(parent, dataObject).ConfigureAwait(false);
                    if (!(result is null))
                    {
                        parent.LinkedImplementations.Add(result);
                        return;
                    }
                }
            }
            else if (!(content is null))
            {
                parent.LinkedContent.Add(content);
            }
        }

        public ClipboardImplementationViewModel? CreateViewModel(ClipboardImplementation implementation, ClipboardObjectViewModel clipboardObject)
        {
            var returner = _viewModelFactories.OfType<ClipboardImplementationViewModelFactory>().Select(f => f.Create(implementation, clipboardObject)).FirstOrDefault(i => !(i is null));
            if (!(returner is null))
                _interactablesManager.AssignStates(returner);
            return returner;
        }

        public BaseViewModel? CreateViewModel(object linkedContent, ClipboardImplementationViewModel parent)
        {
            return _viewModelFactories.Select(f => f.Create(linkedContent, parent)).FirstOrDefault(i => !(i is null));
        }

        public void AddMetadata(ClipboardObjectViewModel clipboardObject)
        {
            foreach(var factory in _clipboardObjectMetadataFactories)
            {
                clipboardObject.Metadata.AddRange(factory.Create(clipboardObject));
            }
        }
    }
}
