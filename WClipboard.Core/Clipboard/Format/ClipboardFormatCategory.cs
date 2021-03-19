namespace WClipboard.Core.Clipboard.Format
{
    public sealed class ClipboardFormatCategory
    {
        public string Name { get; }
        public object IconSource { get; }

        public ClipboardFormatCategory(string name, object iconSource)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new System.ArgumentException("Is null or whitespace", nameof(name));

            Name = name;
            IconSource = iconSource ?? throw new System.ArgumentNullException(nameof(iconSource));
        }
    }
}
