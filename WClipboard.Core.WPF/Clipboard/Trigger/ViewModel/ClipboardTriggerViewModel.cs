using WClipboard.Core.DI;
using WClipboard.Core.WPF.Managers;
using WClipboard.Core.WPF.Models;
using Microsoft.Extensions.DependencyInjection;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.WPF.ViewModels;

namespace WClipboard.Core.WPF.Clipboard.Trigger.ViewModel
{
    public class ClipboardTriggerViewModel : BaseViewModel<ClipboardTrigger>
    {
        public Program? Program { get; }

        public ClipboardTriggerViewModel(ClipboardTrigger model) : base(model)
        {
            if (model.WindowInfo != null)
            {
                Program = DiContainer.SP.GetService<IProgramManager>().GetProgram(model.WindowInfo.ProcessInfo);
            }
        }
    }
}
