using System.Collections.ObjectModel;
using System.Windows;
using WClipboard.App.Windows;
using WClipboard.Core.DI;
using WClipboard.Core.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using WClipboard.Core.Extensions;
using System.Collections.Generic;
using System.ComponentModel;
using WClipboard.Core.WPF.ViewModels.Commands;
using System.Linq;
using WClipboard.Core.Utilities.Collections;
using WClipboard.Core.WPF.Settings;
using WClipboard.Core.Settings;

namespace WClipboard.App.ViewModels
{
    public class SettingsWindowViewModel : BaseViewModel<SettingsWindow>, IWindowViewModel
    {
        private readonly IUISettingsManager uiSettingsManager;
        private readonly IIOSettingsManager ioSettingsManager;

        public IEnumerable<SettingViewModel> Settings { get; }
        public IEnumerable<object> SettingsTree { get; }

        Window IWindowViewModel.Window => Model;

        public SimpleCommand SaveCommand { get; }
        public SimpleCommand RestoreCommand { get; }
        public SimpleCommand OkCommand { get; }

        private MessageBarViewModel? _restartWarning;
        public MessageBarViewModel? RestartWarning {
            get => _restartWarning;
            set => SetProperty(ref _restartWarning, value);
        }

        public SettingsWindowViewModel(SettingsWindow settingsWindow) : base(settingsWindow)
        {
            uiSettingsManager = DiContainer.SP!.GetRequiredService<IUISettingsManager>();
            ioSettingsManager = DiContainer.SP!.GetRequiredService<IIOSettingsManager>();

            SaveCommand = new SimpleCommand(OnSave, false);
            RestoreCommand = new SimpleCommand(OnRestore, false);
            OkCommand = new SimpleCommand(OnOk);

            Settings = new ObservableCollection<SettingViewModel>(uiSettingsManager.CreateAll().NotNull());
            SettingsTree = BuildTree();

            foreach (var setting in Settings)
            {
                setting.PropertyChanged += Setting_PropertyChanged;
            }

            Model.Closed += SettingsWindow_Closed;
        }

        private IEnumerable<object> BuildTree()
        {
            var sections = new KeyedCollectionFunc<string, SectionSettingViewModel>(ssvm => ssvm.Key);

            foreach(var setting in Settings)
            {
                var key = setting.Model.Key;
                key = key.Substring(0, key.LastIndexOf('.'));

                if(!sections.TryGetValue(key, out var section))
                {
                    section = CreateSectionSetting(key, sections);
                }
                section.Childs.Add(setting);
            }

            return ((IEnumerable<SectionSettingViewModel>)sections).Where(ssvm => !ssvm.Key.Contains('.')).ToList();
        }

        private SectionSettingViewModel CreateSectionSetting(string key, KeyedCollectionFunc<string, SectionSettingViewModel> sections)
        {
            var section = new SectionSettingViewModel(key);
            sections.Add(section);

            var dotIndex = key.LastIndexOf('.');
            if (dotIndex != -1) //not root
            { 
                key = key.Substring(0, dotIndex);
                if (!sections.TryGetValue(key, out var parentSection))
                {
                    parentSection = CreateSectionSetting(key, sections);
                }
                parentSection.Childs.Add(section);
            }

            return section;
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

            Model.Closed -= SettingsWindow_Closed;

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
            Model.Close();
        }
    }
}
