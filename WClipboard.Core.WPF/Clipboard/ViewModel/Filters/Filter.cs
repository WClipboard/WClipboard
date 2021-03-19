#nullable enable

namespace WClipboard.Core.WPF.Clipboard.ViewModel.Filters
{
    public abstract class Filter
    {
        public string Text { get; }
        public object? IconSource { get; }

        protected Filter(string text, object? iconSource)
        {
            Text = text;
            IconSource = iconSource;
        }

        public abstract bool Passes(ClipboardObjectViewModel clipboardObjectViewModel);
    }
}
