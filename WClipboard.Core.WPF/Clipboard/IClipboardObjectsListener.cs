using System.Threading;
using WClipboard.Core.WPF.Clipboard.Trigger;

namespace WClipboard.Core.WPF.Clipboard
{
    public interface IClipboardObjectsListener
    { 
        bool IsInterestedIn(ClipboardObject clipboardObject);
        void OnResolvedTrigger(ResolvedClipboardTrigger result);
        void OnResolvedTriggerUpdated(ResolvedClipboardTrigger result);
        void OnClipboardObjectRemoved(ClipboardObject clipboardObject);
        bool CanRemove(ClipboardObject clipboardObject, ClipboardObjectRemoveType reason);
    }
}
