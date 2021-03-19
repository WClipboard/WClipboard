using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using WClipboard.Core.WPF.Themes;

namespace WClipboard.Core.WPF.Models.Text
{
    public abstract class TextElementModel
    {
        public object? ToolTip { get; set; }

        private object? background;
        public object? Background
        {
            get => background;
            set
            {
                if (IsValidBrushType(value))
                    background = value;
            }
        }

        private object? foreground;
        public object? Foreground
        {
            get => foreground;
            set
            {
                if (IsValidBrushType(value))
                    foreground = value;
            }
        }

        private bool IsValidBrushType(object? value)
        {
            return value is null || value is Brush || value is FromPalette;
        }

        protected T Create<T>(T target, FrameworkElement coveringElement) where T : TextElement
        {
            if (ToolTip != null)
                target.SetValue(FrameworkContentElement.ToolTipProperty, ToolTip);

            if (Background != null)
            {
                if (Background is FromPalette palette)
                    target.SetValue(TextElement.ForegroundProperty, palette.ProvideValue(coveringElement, TextElement.BackgroundProperty, null));
                else
                    target.SetValue(TextElement.BackgroundProperty, Background);
            }

            if (Foreground != null)
            {
                if (Foreground is FromPalette palette)
                    target.SetValue(TextElement.ForegroundProperty, palette.ProvideValue(coveringElement, TextElement.ForegroundProperty, null));
                else
                    target.SetValue(TextElement.ForegroundProperty, Foreground);
            }

            return target;
        }

        //public FontFamily FontFamily { get; set; }
        //public object FontSize { get; set; }
        //public FontStyle FontStyle { get; set; }
        //public FontWeight FontWeight { get; set; }
        //public object TextEffects { get; set; }
    }
}
