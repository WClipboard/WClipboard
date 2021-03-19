namespace WClipboard.Core.WPF.ViewModels
{
    public enum MessageBarType
    {
        Information,
        Warning,
        Error
    }

    public enum MessageBarLevel
    {
        Low,
        Medium,
        High
    }

    public class MessageBarViewModel
    {
        public MessageBarType Type { get; }
        public MessageBarLevel Level { get; }

        public object Content { get; }

        public MessageBarViewModel(MessageBarType type, MessageBarLevel level, object content)
        {
            Type = type;
            Level = level;
            Content = content;
        }
    }
}
