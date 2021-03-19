using System;
using System.Globalization;
using System.Windows.Media;

namespace WClipboard.Core.WPF.Converters
{
    public class AlphaColorConverter : BaseConverter<Color, Color>
    {
        public byte A { get; set; }

        public override Color Convert(Color value, Type targetType, object? parameter, CultureInfo culture)
        {
            var a = A;
            if (parameter is IConvertible convertible)
                a = convertible.ToByte(culture);

            return ConvertColor(a, value);
        }

        internal static Color ConvertColor(byte a, Color color)
        {
            return Color.FromArgb(
                a,
                color.R,
                color.G,
                color.B
            );
        }
    }

    public class AlphaSolidColorBrushConverter : BaseConverter<SolidColorBrush, SolidColorBrush>
    {
        public byte A { get; set; }

        public override SolidColorBrush Convert(SolidColorBrush value, Type targetType, object? parameter, CultureInfo culture)
        {
            var a = A;
            if (parameter is IConvertible convertible)
                a = convertible.ToByte(culture);

            return new SolidColorBrush(AlphaColorConverter.ConvertColor(a, value.Color));
        }
    }
}
