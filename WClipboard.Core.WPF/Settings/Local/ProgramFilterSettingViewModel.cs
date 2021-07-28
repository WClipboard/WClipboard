using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using WClipboard.Core.Extensions;
using WClipboard.Core.Settings;
using WClipboard.Core.WPF.Managers;
using WClipboard.Core.WPF.Models;
using WClipboard.Core.WPF.Utilities;

namespace WClipboard.Core.WPF.Settings.Local
{
    public class ProgramFilterSettingViewModel : SettingViewModel<List<Program>>
    {
        private readonly IProgramManager programManager;

        private string searchText = string.Empty;
        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value, string.IsNullOrEmpty(value) || !isSelectedSearchProgramUpdating).OnChanged(RefreshSearchPrograms);
        }

        private ConcurrentBindableList<Program> searchPrograms;

        public ConcurrentBindableList<Program> SearchPrograms
        {
            get => searchPrograms;
            set => SetProperty(ref searchPrograms, value);
        }

        private bool isSelectedSearchProgramUpdating = false;

        public Program? SelectedSearchProgram
        {
            get => null;
            set
            {
                isSelectedSearchProgramUpdating = true;
                if (!(value is null))
                {
                    var v = Value.ToList(Value.Count);
                    v.Add(value);
                    Value = v;

                    if (!string.IsNullOrEmpty(SearchText))
                    {
                        SearchText = string.Empty;
                    }
                    else
                    {
                        searchPrograms.Remove(value);
                    }
                }
                isSelectedSearchProgramUpdating = false;
            }
        }

        public ICommand BrowseCommand { get; }
        public ICommand RemoveProgramCommand { get; }

        public ProgramFilterSettingViewModel(ISetting model, ISettingApplier<List<Program>> settingsApplier, string description, IProgramManager programManager) : base(model, settingsApplier, description)
        {
            this.programManager = programManager;

            searchPrograms = new ConcurrentBindableList<Program>(programManager.GetCurrentKnownPrograms().Except(Value));

            BrowseCommand = new SimpleCommand(Browse, _ => true);
            RemoveProgramCommand = new SimpleCommand<Program>(RemoveProgram, _ => true);

            ScanStartMenu();
        }

        private async void ScanStartMenu()
        {
            await programManager.ScanStartMenu().ConfigureAwait(false);
            RefreshSearchPrograms();
        }

        private void RefreshSearchPrograms()
        {
            searchPrograms.ReplaceAll(programManager.GetCurrentKnownPrograms().Except(Value).Where(p => p.Name.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase)));
        }

        private void Browse(object? _)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Program",
                Filter = "Program (*.exe)|*.exe|All files|(*)",
                ValidateNames = true,
                CheckPathExists = true,
                CheckFileExists = false,
                DefaultExt = ".exe",
                DereferenceLinks = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
            };
            if (dialog.ShowDialog() == true)
            {
                var programToAdd = programManager.GetProgram(dialog.FileName);

                var v = Value.ToList(Value.Count);
                v.Add(programToAdd);
                Value = v;

                searchPrograms.Remove(programToAdd);
            }
        }

        private void RemoveProgram(Program program)
        {
            var v = Value.ToList(Value.Count);
            v.Remove(program);
            Value = v;

            searchPrograms.Add(program);
        }
    }
}
