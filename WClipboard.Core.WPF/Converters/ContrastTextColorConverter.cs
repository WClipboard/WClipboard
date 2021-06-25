using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using WClipboard.Core.WPF.Extensions;

namespace WClipboard.Core.WPF.Converters
{
    public class ContrastTextColorConverter : BaseConverter<object, object>
    {
        private const string DARK_TEXT_KEY = "DarkTextColor";
        private const string LIGHT_TEXT_KEY = "LightTextColor";

        public override object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            var color = value as Color? ?? 
                (value as SolidColorBrush)?.Color ?? 
                throw new ArgumentException($"{nameof(value)} must be of type {nameof(Color)} or {nameof(SolidColorBrush)}", nameof(value));
            if (color.A != 255)
            {
                throw new NotSupportedException($"{nameof(value)} must be nontransparant to use in a {nameof(ContrastTextColorConverter)}");
            }

            var lightColor = Application.Current.FindResource<Color>(LIGHT_TEXT_KEY);
            var darkColor = Application.Current.FindResource<Color>(DARK_TEXT_KEY);

            var lightContrast = ColorExtensions.ContrastWith(lightColor, color);
            var darkContrast = ColorExtensions.ContrastWith(darkColor, color);

            var contrastColor = lightContrast > darkContrast ? lightColor : darkColor;

            if (targetType.IsAssignableFrom(typeof(SolidColorBrush))) {
                return new SolidColorBrush(contrastColor);
            } else if (targetType.IsAssignableFrom(typeof(Color))) {
                return contrastColor;
            } else if (value is SolidColorBrush) {
                return new SolidColorBrush(contrastColor);
            } else {
                return contrastColor;
            }
        }
    }
}
