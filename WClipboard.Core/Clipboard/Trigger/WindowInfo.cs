namespace WClipboard.Core.Clipboard.Trigger
{
    public class WindowInfo
    {
        public string Title { get; }
        public object? IconSource { get; }

        public ProcessInfo ProcessInfo { get; }

        public WindowInfo(string title, object? iconSource, ProcessInfo processInfo)
        {
            Title = title ?? throw new System.ArgumentNullException(nameof(title));
            IconSource = iconSource;
            ProcessInfo = processInfo ?? throw new System.ArgumentNullException(nameof(processInfo));
        }
    }
}
