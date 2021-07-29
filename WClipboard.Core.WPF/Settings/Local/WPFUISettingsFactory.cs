using System.Collections.Generic;
using WClipboard.Core.Settings;
using WClipboard.Core.Settings.Defaults;
using WClipboard.Core.WPF.Models;
using WClipboard.Core.WPF.Settings.Defaults;
using WClipboard.Core.WPF.Themes;
using System.Linq;
using WClipboard.Core.Extensions;
using WClipboard.Core.WPF.Managers;

namespace WClipboard.Core.WPF.Settings.Local
{
    public class WPFUISettingsFactory : BaseUISettingsFactory
    {
        private readonly IProgramManager programManager;

        public WPFUISettingsFactory(IProgramManager programManager) : base(new []
        {
            SettingConsts.ThemeKey,
            SettingConsts.OwnerProgramClipboardFilterKey,
            SettingConsts.Windows10HistoryFilterKey,
            SettingConsts.Windows10CloudFilterKey,
        })
        {
            this.programManager = programManager;
        }

        public override SettingViewModel? Create(ISetting model)
        {
            return model.Key switch
            {
                SettingConsts.ThemeKey => new ComboBoxSettingViewModel<Theme>(model, new ResolvableIOSettingApplier<Theme>(SettingChangeMode.OnSave, SettingChangeEffect.RestartRequired), (IKeyedCollectionSetting<string, Theme>)model, "Application theme"),
                SettingConsts.OwnerProgramClipboardFilterKey => new ProgramFilterSettingViewModel(model, new FuncIOSettingsApplier<List<Program>, List<string>>(SettingChangeMode.Direct, SettingChangeEffect.AtOnce, (programs) => programs.Select(p => p.Path).NotNull().ToList(programs.Count), new List<Program>(
                    ((ListSetting<string>)model).Value.Select(p => programManager.GetProgram(p))
                )), "Ignore clipboard content from specific programs", programManager),
                SettingConsts.Windows10HistoryFilterKey => new CheckBoxSettingViewModel(model, new IOSettingApplier<bool?>(SettingChangeMode.Direct, SettingChangeEffect.AtOnce), "Ignore clipboard content when the data owner does not want it to show up in Windows 10 Clipboard History"),
                SettingConsts.Windows10CloudFilterKey => new CheckBoxSettingViewModel(model, new IOSettingApplier<bool?>(SettingChangeMode.Direct, SettingChangeEffect.AtOnce), "Ignore clipboard content when the data owner does not want it to be uploaded to the cloud"),
                _ => null,
            };
        }
    }
}
