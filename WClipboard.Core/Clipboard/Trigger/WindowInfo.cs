namespace WClipboard.Core.Clipboard.Trigger
{
    public class WindowInfo
    {
        public string Title { get; }
        public object? IconSource { get; }

        public WindowInfo(string title, object? iconSource)
        {
            Title = title;
            IconSource = iconSource;
        }
    }
}
