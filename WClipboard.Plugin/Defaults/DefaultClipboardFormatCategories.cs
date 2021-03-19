using Microsoft.Extensions.DependencyInjection;
using WClipboard.Core.Clipboard.Format;

namespace WClipboard.Plugin.Defaults
{
    public static class DefaultClipboardFormatCategories
    {
        public static ClipboardFormatCategory Text { get; private set; }
        public static ClipboardFormatCategory Path { get; private set; }
        public static ClipboardFormatCategory Picture { get; private set; }

        internal static void Setup(IServiceCollection services)
        {
            Text = new ClipboardFormatCategory(nameof(Text), "T:T");
            Path = new ClipboardFormatCategory(nameof(Path), "FilesIcon");
            Picture = new ClipboardFormatCategory(nameof(Picture), "ImageIcon");

            services.AddSingleton(Text);
            services.AddSingleton(Path);
            services.AddSingleton(Picture);
        }
    }
}
