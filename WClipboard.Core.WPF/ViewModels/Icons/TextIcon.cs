using System.Windows.Media;

#nullable enable

namespace WClipboard.Core.WPF.ViewModels.Icons
{
    public class TextIcon
    {
        public string Text { get; }

        public FontFamily FontFamily { get; }

        public TextIcon(string text, FontFamily? fontFamily = null)
        {
            Text = text;
            FontFamily = fontFamily ?? new FontFamily("Consolas");
        }
    }
}
