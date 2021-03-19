#nullable enable

namespace WClipboard.Core.Clipboard.Format
{
    public sealed class ClipboardFormat
    {
        public string Name { get; }
        public string Format { get; }
        public object IconSource { get; }

        public ClipboardFormatCategory Category { get; }

        public ClipboardFormat(string name, string format, object iconSource, ClipboardFormatCategory category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new System.ArgumentException("Is whitespace", nameof(name));

            Name = name;
            Format = format;
            IconSource = iconSource;
            Category = category;
        }
    }
}
