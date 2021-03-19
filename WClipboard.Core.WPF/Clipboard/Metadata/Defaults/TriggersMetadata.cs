using WClipboard.Core.WPF.Clipboard.Trigger.ViewModel;
using WClipboard.Core.WPF.Clipboard.ViewModel;
using WClipboard.Core.WPF.Utilities;

namespace WClipboard.Core.WPF.Clipboard.Metadata.Defaults
{
    public class TriggersMetadata : ClipboardObjectMetadata
    {
        public BindableObservableCollection<ClipboardTriggerViewModel> Triggers { get; }

        public TriggersMetadata(ClipboardObjectViewModel clipboardObject) : base("TimeIcon", "History")
        {
            Triggers = clipboardObject.Triggers;
        }
    }
}
