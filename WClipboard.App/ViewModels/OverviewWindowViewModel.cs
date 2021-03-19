﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using WClipboard.App.ViewModels.Interactables;
using WClipboard.App.Windows;
using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.DI;
using WClipboard.Core.Extensions;
using WClipboard.Core.Utilities;
using WClipboard.Core.WPF.Clipboard;
using WClipboard.Core.WPF.Clipboard.Trigger;
using WClipboard.Core.WPF.Clipboard.ViewModel;
using WClipboard.Core.WPF.LifeCycle;
using WClipboard.Core.WPF.Managers;
using WClipboard.Core.WPF.Utilities;
using WClipboard.Core.WPF.ViewModels;

namespace WClipboard.App.ViewModels
{
    public class OverviewWindowViewModel : BindableBase, IClipboardObjectsListener, IMainWindowViewModel
    {
        private static ClipboardTriggerType? dragAndDropType;

        private readonly IClipboardObjectsManager clipboardObjectsManager;
        private readonly IClipboardObjectManager clipboardObjectManager;
        private readonly IClipboardFormatsManager clipboardFormatsManager;
        private readonly IInteractablesManager interactablesManager;
        private readonly OverviewWindow _overviewWindow;

        private readonly CloseInteractable clipboadObjectViewModelCloseInteractable;

        public ObservableCollection<ClipboardObjectViewModel> Objects { get; }
        public ListCollectionView ObjectsView { get; }

        public SynchronizationContext SynchronizationContext { get; }

        private List<ClipboardFormat>? dragDropFormats;
        public List<ClipboardFormat>? DragAndDropFormats
        {
            get => dragDropFormats;
            set => SetProperty(ref dragDropFormats, value);
        }

        public ObservableCollection<InteractableState> Interactables { get; }

        IReadOnlyList<InteractableState> IHasInteractables.Interactables => Interactables;

        public Window Window => _overviewWindow;

        public FilterHelper FilterHelper { get; }

        public OverviewWindowViewModel(OverviewWindow overviewWindow)
        {
            if (dragAndDropType == null)
                dragAndDropType = new ClipboardTriggerType("Drag and drop", "DragAndDropIcon", ClipboardTriggerSourceType.Extern, 0);

            SynchronizationContext = SynchronizationContext.Current!;

            Interactables = new BindableObservableCollection<InteractableState>(SynchronizationContext);

            Objects = new BindableObservableCollection<ClipboardObjectViewModel>(SynchronizationContext);
            BindingOperations.EnableCollectionSynchronization(Objects, Objects);

            ObjectsView = new ListCollectionView(Objects);

            FilterHelper = new FilterHelper(SynchronizationContext, ObjectsView);

            clipboardObjectsManager = DiContainer.SP.GetService<IClipboardObjectsManager>();
            clipboardObjectManager = DiContainer.SP.GetService<IClipboardObjectManager>();
            clipboardFormatsManager = DiContainer.SP.GetService<IClipboardFormatsManager>();
            interactablesManager = DiContainer.SP.GetService<IInteractablesManager>();

            _overviewWindow = overviewWindow;
            overviewWindow.Loaded += OverviewWindow_Loaded;
            overviewWindow.Closed += OverviewWindow_Closed;
            overviewWindow.DragEnter += OverviewWindow_DragEnter;
            overviewWindow.DragLeave += OverviewWindow_DragLeave;
            overviewWindow.Drop += OverviewWindow_Drop;
            overviewWindow.AllowDrop = true;

            clipboadObjectViewModelCloseInteractable = new CloseInteractable();

            interactablesManager.AssignStates(this);
        }

        private void OverviewWindow_Loaded(object sender, RoutedEventArgs e)
        {
            clipboardObjectsManager.AddListener(this);

            foreach(var service in DiContainer.SP.GetServices<IAfterMainWindowLoadedListener>())
            {
                service.AfterMainWindowLoaded();
            }
        }

        private void OverviewWindow_Closed(object? sender, EventArgs e)
        {
            clipboardObjectsManager.RemoveListener(this);

            Application.Current.Shutdown(0);
        }

        public void Remove(ClipboardObjectViewModel clipboardObject)
        {
            Objects.Remove(clipboardObject);
            clipboardObjectsManager.TryRemove(clipboardObject.Model, ClipboardObjectRemoveType.Manual);
        }

        private void Add(ClipboardObject clipboardObject)
        {
            var viewModel = new ClipboardObjectViewModel(clipboardObject, this, clipboardObjectManager, interactablesManager, SynchronizationContext);

            Objects.Insert(0, viewModel);

            viewModel.Interactables.Add(clipboadObjectViewModelCloseInteractable.CreateState(viewModel));
        }

        private void OverviewWindow_DragEnter(object sender, DragEventArgs e)
        {
            if (e.AllowedEffects.HasFlag(DragDropEffects.Copy))
            {
                DragAndDropFormats = new List<ClipboardFormat>(clipboardFormatsManager.GetFormats(e.Data.GetFormats(true)));

                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void OverviewWindow_DragLeave(object sender, DragEventArgs e)
        {
            DragAndDropFormats = null;
        }

        private void OverviewWindow_Drop(object sender, DragEventArgs e)
        {
            OverviewWindow_DragLeave(sender, e);
            if (e.AllowedEffects.HasFlag(DragDropEffects.Copy))
            {
                clipboardObjectsManager.ProcessExternalTrigger(new ClipboardTrigger(dragAndDropType!, null), e.Data);
            }
        }

        bool IClipboardObjectsListener.IsInterestedIn(ClipboardObject clipboardObject) => true;
        void IClipboardObjectsListener.OnResolvedTrigger(ResolvedClipboardTrigger result)
        {
            if (result.Object == null)
                return;

            switch (result.ResolvedType)
            {
                case ResolvedClipboardTriggerType.Created:
                    Add(result.Object);
                    break;
                case ResolvedClipboardTriggerType.WClipboardId:
                case ResolvedClipboardTriggerType.EqualsReference:
                    var index = Objects.FirstIndexOf(vm => vm.Model == result.Object);
                    if (index != -1)
                    {
                        Objects.Move(index, 0);
                    }
                    else
                    {
                        Add(result.Object);
                    }
                    break;
            }
        }
        void IClipboardObjectsListener.OnResolvedTriggerUpdated(ResolvedClipboardTrigger result) { }
        void IClipboardObjectsListener.OnClipboardObjectRemoved(ClipboardObject clipboardObject)
        {
            var index = Objects.FirstIndexOf(vm => vm.Model == clipboardObject);
            if (index != -1)
            {
                Objects.RemoveAt(index);
            }
        }

        bool IClipboardObjectsListener.CanRemove(ClipboardObject clipboardObject, ClipboardObjectRemoveType type)
        {
            if(type == ClipboardObjectRemoveType.Manual)
            {
                return !Objects.Any(vm => vm.Model == clipboardObject);
            }
            return true;
        }
    }
}
