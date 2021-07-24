using WClipboard.Core.WPF.Managers;
using WClipboard.Core.WPF.Models;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.WPF.ViewModels;

namespace WClipboard.Core.WPF.Clipboard.Trigger.ViewModel
{
    public class ClipboardTriggerViewModel : BaseViewModel<ClipboardTrigger>
    {
        public Program? ForegroundProgram { get; }
        public Program? DataSourceProgram { get; }

        public object? IconSource { get; }
        public string? Title { get; }

        public ClipboardTriggerViewModel(ClipboardTrigger model, IProgramManager programManager) : base(model)
        {
            if (!(model.ForegroundProgram is null))
            {
                ForegroundProgram = programManager.GetProgram(model.ForegroundProgram);
            }

            if (!(model.DataSourceProgram is null))
            {
                DataSourceProgram = programManager.GetProgram(model.DataSourceProgram);
            }

            if (model.Type is ReferenceClipboardTriggerType || model.DataSourceProgram == model.ForegroundProgram)
            {
                IconSource = model.ForegroundWindow?.IconSource ?? ForegroundProgram?.IconSource ?? DataSourceProgram?.IconSource;
                Title = model.ForegroundWindow?.Title ?? ForegroundProgram?.Name ?? DataSourceProgram?.Name;
            } 
            else
            {
                IconSource = DataSourceProgram?.IconSource ?? model.ForegroundWindow?.IconSource ?? ForegroundProgram?.IconSource;
                Title = DataSourceProgram?.Name ?? model.ForegroundWindow?.Title ?? ForegroundProgram?.Name;
            }
        }
    }
}
