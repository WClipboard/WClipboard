﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Windows;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.Extensions;
using WClipboard.Core.WPF.Clipboard.Implementation;
using WClipboard.Core.WPF.Clipboard.Implementation.ViewModel;
using WClipboard.Core.WPF.Clipboard.Metadata;
using WClipboard.Core.WPF.Clipboard.Trigger.ViewModel;
using WClipboard.Core.WPF.Extensions;
using WClipboard.Core.WPF.Managers;
using WClipboard.Core.WPF.Utilities;
using WClipboard.Core.WPF.ViewModels;

#nullable enable

namespace WClipboard.Core.WPF.Clipboard.ViewModel
{
    public class ClipboardObjectViewModel : BaseViewModelWithInteractables<ClipboardObject>, IDragSource
    {
        private bool _showMetadata = false;

        protected internal readonly IClipboardObjectManager _clipboardObjectManager;

        public SynchronizationContext SynchronizationContext { get; }

        public BindableObservableCollection<ClipboardImplementationViewModel> Implementations { get; }
        public BindableObservableCollection<ClipboardObjectMetadata> Metadata { get; }
        public BindableObservableCollection<ClipboardTriggerViewModel> Triggers { get; }

        public ClipboardTriggerViewModel MainTrigger { get; }

        public IClipboardObjectsListener Listener { get; }

        public bool ShowMetadata
        {
            get => _showMetadata;
            set => SetProperty(ref _showMetadata, value).OnChanged(OnShowMetadataChanged);
        }

        public ClipboardObjectViewModel(ClipboardObject @object, IClipboardObjectsListener listener, IClipboardObjectManager clipboardObjectManager, IInteractablesManager interactablesManager, SynchronizationContext synchronizationContext) : base(@object, new BindableObservableCollection<InteractableState>(synchronizationContext))
        {
            _clipboardObjectManager = clipboardObjectManager;

            Listener = listener;
            SynchronizationContext = synchronizationContext;

            MainTrigger = new ClipboardTriggerViewModel(Model.MainTrigger);

            Implementations = new BindableObservableCollection<ClipboardImplementationViewModel>(synchronizationContext);
            Metadata = new BindableObservableCollection<ClipboardObjectMetadata>(synchronizationContext);
            Triggers = new BindableObservableCollection<ClipboardTriggerViewModel>(synchronizationContext);

            Implementations.AddRange(Create(Model.Implementations));
            Triggers.AddRange(Create(Model.Triggers));

            Model.Implementations.CollectionChanged += UpdateImplementations;
            Model.Triggers.CollectionChanged += UpdateTriggers;

            interactablesManager.AssignStates(this);
        }

        private void OnShowMetadataChanged()
        {
            if(ShowMetadata && Metadata.Count == 0)
            {
                _clipboardObjectManager.AddMetadata(this);
            }
        }

        private IEnumerable<ClipboardImplementationViewModel> Create(IEnumerable<ClipboardImplementation> implementations)
        {
            return implementations.Select(i => _clipboardObjectManager.CreateViewModel(i, this)).OfType<ClipboardImplementationViewModel>();
        }

        private IEnumerable<ClipboardTriggerViewModel> Create(IEnumerable<ClipboardTrigger> triggers)
        {
            return triggers.Select(t => new ClipboardTriggerViewModel(t));
        }

        private void UpdateImplementations(object sender, NotifyCollectionChangedEventArgs e)
        {
            var differences = e.GetDifferences<ClipboardImplementation>();

            if(differences.Removed.Count > 0)
                Implementations.RemoveAll(i => differences.Removed.Contains(i.Model));
            if (differences.Added.Count > 0)
                Implementations.AddRange(Create(differences.Added));
        }

        private void UpdateTriggers(object sender, NotifyCollectionChangedEventArgs e)
        {
            var differences = e.GetDifferences<ClipboardTrigger>();

            if (differences.Removed.Count > 0)
                Triggers.RemoveAll(i => differences.Removed.Contains(i.Model));
            if (differences.Added.Count > 0)
                Triggers.AddRange(Create(differences.Added));
        }

        object IDragSource.GetDragData(DependencyObject dragSource, out DragDropEffects effects)
        {
            effects = DragDropEffects.Copy;
            return Model.GetDataObject().GetAwaiter().GetResult();
        }
    }
}
