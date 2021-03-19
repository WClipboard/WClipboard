namespace WClipboard.Core.WPF.Clipboard
{
    public abstract class ClipboardObjectProperty
    {
        public bool PreventAutoRemoval { get; }

        protected ClipboardObjectProperty(bool preventAutoRemoval = false)
        {
            PreventAutoRemoval = preventAutoRemoval;
        }
    }
}
