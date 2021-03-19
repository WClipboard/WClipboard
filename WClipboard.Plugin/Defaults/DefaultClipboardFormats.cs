using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WClipboard.Core.Clipboard.Format;

namespace WClipboard.Plugin.Defaults
{
    public static class DefaultClipboardFormats
    {
        public static ClipboardFormat Unicode { get; private set; }
        public static ClipboardFormat FileDrop { get; private set; }
        public static ClipboardFormat Bitmap { get; private set; }

        internal static void Setup(IServiceCollection services)
        {
            Unicode = new ClipboardFormat(nameof(Unicode), DataFormats.UnicodeText, "T:Unic", DefaultClipboardFormatCategories.Text);
            FileDrop = new ClipboardFormat(nameof(FileDrop), DataFormats.FileDrop, "T:FiDr", DefaultClipboardFormatCategories.Path);
            Bitmap = new ClipboardFormat(nameof(Bitmap), DataFormats.Bitmap, "T:Bmp", DefaultClipboardFormatCategories.Picture);

            services.AddSingleton(Unicode);
            services.AddSingleton(FileDrop);
            services.AddSingleton(Bitmap);
        }
    }
}
