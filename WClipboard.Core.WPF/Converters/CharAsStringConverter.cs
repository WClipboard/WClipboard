using System;
using System.Globalization;

namespace WClipboard.Core.WPF.Converters
{
    public class CharAsStringConverter : BaseConverter<char, string>
    {
        public override string Convert(char value, Type targetType, object? parameter, CultureInfo culture)
        {
            return new string(value, 1);
        }

        public override char ConvertBack(string value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value[0];
        }
    }
}
