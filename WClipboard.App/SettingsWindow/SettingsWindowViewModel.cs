using System.Collections.ObjectModel;
using System.Windows;
using WClipboard.Core.WPF.ViewModels;
using WClipboard.Core.Extensions;
using System.Collections.Generic;
using System.ComponentModel;
using WClipboard.Core.WPF.ViewModels.Commands;
using System.Linq;
using WClipboard.Core.WPF.Settings;
using WClipboard.Core.Settings;
using WClipboard.Core.Utilities;

namespace WClipboard.App.SettingsWindow
{
    public class SettingsWindowViewModel : BindableBase, IWindowViewModel
    {
        private readonly IUISettingsManager uiSettingsManager;
        private readonly IIOSettingsManager ioSettingsManager;
        private readonly SettingsWindow settingsWindow;

        public IReadOnlyCollection<SettingViewModel> Settings { get; }
        public IReadOnlyCollection<object> SettingsTree { get; }

        Window IWindowViewModel.Window => settingsWindow;

        public SimpleCommand SaveCommand { get; }
        public SimpleCommand RestoreCommand { get; }
        public SimpleCommand OkCommand { get; }

        private MessageBarViewModel? _restartWarning;
        public MessageBarViewModel? RestartWarning {
            get => _restartWarning;
            set => SetProperty(ref _restartWarning, value);
        }

        public SettingsWindowViewModel(IUISettingsManager uiSettingsManager, IIOSettingsManager ioSettingsManager, Window callerWindow)
        {
            this.uiSettingsManager = uiSettingsManager;
            this.ioSettingsManager = ioSettingsManager;

            SaveCommand = new SimpleCommand(OnSave, false);
            RestoreCommand = new SimpleCommand(OnRestore, false);
            OkCommand = new SimpleCommand(OnOk);

            Settings = new ObservableCollection<SettingViewModel>(uiSettingsManager.CreateAll().NotNull());
            SettingsTree = BuildTree();

            foreach (var setting in Settings)
            {
                setting.PropertyChanged += Setting_PropertyChanged;
            }

            settingsWindow = new SettingsWindow()
            {
                DataContext = this,
                Owner = callerWindow
            };
            settingsWindow.Closed += SettingsWindow_Closed;

            settingsWindow.ShowDialog();
        }

        private IReadOnlyCollection<object> BuildTree()
        {
            var list = new List<object>(Settings.OrderBy(s => s.Model.Key));

            var lastSubKeys = new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                if(list[i] is SettingViewModel setting)
                {
                    var subKeys = setting.Model.Key.Split('.');
                    var subKeyPosition = 0;
                    for(; subKeyPosition < subKeys.Length - 1; subKeyPosition++)
                    {
                        if(lastSubKeys.Count <= subKeyPosition || lastSubKeys[subKeyPosition] != subKeys[subKeyPosition])
                        {
                            list.Insert(i, new HeaderViewModel(string.Join(".", subKeys[0..(subKeyPosition + 1)])));
                            i += 1;
                            if (lastSubKeys.Count > subKeyPosition)
                            {
                                lastSubKeys.RemoveRange(subKeyPosition, lastSubKeys.Count - subKeyPosition);
                            }
                            lastSubKeys.Add(subKeys[subKeyPosition]);
                        }
                    }

                    if (lastSubKeys.Count > subKeyPosition)
                    {
                        lastSubKeys.RemoveRange(subKeyPosition, lastSubKeys.Count - subKeyPosition);
                    }
                }
            }

            return list;
        }

        private void Setting_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var setting = sender as SettingViewModel;

            if (setting is null)
                return;

            if (e.PropertyName == nameof(SettingViewModel.IsApplied))
            {
                if(!setting.IsApplied)
                {
                    SaveCommand.CanExecute = true;
                    if (!Settings.Any(s => s.IsChanged && s.IsApplied && s.Applier.ChangeEffect == SettingChangeEffect.RestartRequired))
                    {
                        RestartWarning = null;
                    }
                } 
                else
                {
                    SaveCommand.CanExecute = Settings.Any(s => !s.IsApplied);
                    if(setting.IsChanged && setting.Applier.ChangeEffect == SettingChangeEffect.RestartRequired)
                    {
                        RestartWarning = new MessageBarViewModel(MessageBarType.Warning, MessageBarLevel.Medium, "You changed a setting that requires a restart");
                    }
                }
            } 
            else if(e.PropertyName == nameof(SettingViewModel.IsChanged))
            {
                if (setting.IsChanged)
                {
                    RestoreCommand.CanExecute = true;
                    if (setting.IsApplied && setting.Applier.ChangeEffect == SettingChangeEffect.RestartRequired)
                    {
                        RestartWarning = new MessageBarViewModel(MessageBarType.Warning, MessageBarLevel.Medium, "You changed a setting that requires a restart");
                    }
                }
                else
                {
                    RestoreCommand.CanExecute = Settings.Any(s => s.IsChanged);
                    if(!Settings.Any(s => s.IsChanged && s.IsApplied && s.Applier.ChangeEffect == SettingChangeEffect.RestartRequired))
                    {
                        RestartWarning = null;
                    }
                }
            }
        }

        private void SettingsWindow_Closed(object? sender, System.EventArgs e)
        {
            ioSettingsManager.Save();

            settingsWindow.Closed -= SettingsWindow_Closed;

            foreach (var setting in Settings)
            {
                setting.PropertyChanged -= Setting_PropertyChanged;
            }
        }

        private void OnSave(object? parameter)
        {
            foreach(var setting in Settings)
            {
                setting.Save();
            }
        }

        private void OnRestore(object? parameter)
        {
            foreach(var setting in Settings)
            {
                setting.Restore();
            }
        }

        private void OnOk(object? parameter)
        {
            settingsWindow.Close();
        }
    }
}
