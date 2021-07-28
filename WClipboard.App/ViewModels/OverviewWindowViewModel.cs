using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using WClipboard.App.Settings;
using WClipboard.App.ViewModels.Interactables;
using WClipboard.App.Windows;
using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.DI;
using WClipboard.Core.Extensions;
using WClipboard.Core.Settings;
using WClipboard.Core.Utilities;
using WClipboard.Core.WPF.Clipboard;
using WClipboard.Core.WPF.Clipboard.Trigger;
using WClipboard.Core.WPF.Clipboard.ViewModel;
using WClipboard.Core.WPF.LifeCycle;
using WClipboard.Core.WPF.Managers;
using WClipboard.Core.WPF.Utilities;
using WClipboard.Core.WPF.ViewModels;
using WClipboard.Windows;
using WClipboard.Windows.Helpers;

namespace WClipboard.App.ViewModels
{
    public class OverviewWindowViewModel : BindableBase, IClipboardObjectsListener, IMainWindowViewModel
    {
        private static ClipboardTriggerType? dragAndDropType;

        private readonly OverviewWindow overviewWindow;

        private readonly IClipboardObjectsManager clipboardObjectsManager;
        private readonly IClipboardObjectManager clipboardObjectManager;
        private readonly IClipboardFormatsManager clipboardFormatsManager;
        private readonly IInteractablesManager interactablesManager;
        private readonly ITaskbarIcon taskbarIcon;
        private readonly IIOSetting minimizeToSetting;
        private readonly IProgramManager programManager;

        private readonly CloseInteractable clipboadObjectViewModelCloseInteractable;

        public ConcurrentBindableList<ClipboardObjectViewModel> Objects { get; }
        public ListCollectionView ObjectsView { get; }

        private List<ClipboardFormat>? dragDropFormats;
        public List<ClipboardFormat>? DragAndDropFormats
        {
            get => dragDropFormats;
            set => SetProperty(ref dragDropFormats, value);
        }

        public ConcurrentBindableList<InteractableState> Interactables { get; }

        IReadOnlyList<InteractableState> IHasInteractables.Interactables => Interactables;

        public Window Window => overviewWindow;

        public FilterHelper FilterHelper { get; }

        public OverviewWindowViewModel(OverviewWindow overviewWindow, 
            IClipboardObjectsManager clipboardObjectsManager,
            IClipboardObjectManager clipboardObjectManager,
            IClipboardFormatsManager clipboardFormatsManager,
            IInteractablesManager interactablesManager,
            ITaskbarIcon taskbarIcon,
            IIOSettingsManager settingsManager,
            IProgramManager programManager
            )
        {
            if (dragAndDropType == null)
                dragAndDropType = new CustomClipboardTriggerType("Drag and drop", "DragAndDropIcon");

            Interactables = new ConcurrentBindableList<InteractableState>();

            Objects = new ConcurrentBindableList<ClipboardObjectViewModel>();

            ObjectsView = new ListCollectionView(Objects);

            FilterHelper = new FilterHelper(ObjectsView);

            this.clipboardObjectsManager = clipboardObjectsManager;
            this.clipboardObjectManager = clipboardObjectManager;
            this.clipboardFormatsManager = clipboardFormatsManager;
            this.interactablesManager = interactablesManager;
            this.programManager = programManager;
            this.taskbarIcon = taskbarIcon;
            taskbarIcon.OnMouseAction += TaskbarIcon_OnMouseAction;

            minimizeToSetting = settingsManager.GetSetting(AppUISettingsFactory.MinimizeTo);

            this.overviewWindow = overviewWindow;
            overviewWindow.Loaded += OverviewWindow_Loaded;
            overviewWindow.Closed += OverviewWindow_Closed;
            overviewWindow.DragEnter += OverviewWindow_DragEnter;
            overviewWindow.DragLeave += OverviewWindow_DragLeave;
            overviewWindow.Drop += OverviewWindow_Drop;
            overviewWindow.AllowDrop = true;
            overviewWindow.StateChanged += OverviewWindow_StateChanged;

            clipboadObjectViewModelCloseInteractable = new CloseInteractable();

            interactablesManager.AssignStates(this);
        }

        private void OverviewWindow_StateChanged(object? sender, EventArgs e)
        {
            if (overviewWindow.WindowState == WindowState.Minimized && (minimizeToSetting.Value?.Equals(MinimizeTo.SystemTray) ?? false))
            {
                overviewWindow.ShowInTaskbar = false;
            }
        }

        private void TaskbarIcon_OnMouseAction(object? sender, MouseActionEventArgs e)
        {
            switch (e.MouseAction)
            {
                case MouseAction.LeftClick:
                    if (overviewWindow.WindowState == WindowState.Minimized)
                        overviewWindow.WindowState = WindowState.Normal;
                    if (!overviewWindow.ShowInTaskbar)
                        overviewWindow.ShowInTaskbar = true;
                    overviewWindow.Focus();
                    break;
            }
        }

        private void OverviewWindow_Loaded(object sender, RoutedEventArgs e)
        {
            clipboardObjectsManager.AddListener(this);

            foreach(var service in DiContainer.SP!.GetServices<IAfterMainWindowLoadedListener>())
            {
                service.AfterMainWindowLoaded(this);
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
            var viewModel = new ClipboardObjectViewModel(clipboardObject, this, clipboardObjectManager, interactablesManager, programManager);

            Objects.Insert(0, viewModel);

            var state = clipboadObjectViewModelCloseInteractable.CreateState(viewModel);

            if (!(state is null))
            {
                viewModel.Interactables.Add(state);
            }
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
                var info = WindowInfoHelper.GetFromWpfWindow(Window);
                var _ = clipboardObjectsManager.ProcessClipboardTrigger(new ClipboardTrigger(dragAndDropType!, null, info?.Item2, info?.Item1), e.Data);
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
                    if (!Objects.Move(vm => vm.Model == result.Object, 0))
                    {
                        Add(result.Object);
                    }
                    break;
            }
        }
        void IClipboardObjectsListener.OnResolvedTriggerUpdated(ResolvedClipboardTrigger result) { }
        void IClipboardObjectsListener.OnClipboardObjectRemoved(ClipboardObject clipboardObject)
        {
            Objects.RemoveAll(vm => vm.Model == clipboardObject);
        }

        bool IClipboardObjectsListener.CanRemove(ClipboardObject clipboardObject, ClipboardObjectRemoveType type)
        {
            if (type == ClipboardObjectRemoveType.Manual)
            {
                return !Objects.Any(vm => vm.Model == clipboardObject);
            }
            return true;
        }
    }
}
