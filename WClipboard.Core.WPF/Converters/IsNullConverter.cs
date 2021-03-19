using System;
using System.Globalization;

namespace WClipboard.Core.WPF.Converters
{
    public class IsNullConverter : BaseConverter<object, bool>
    {
        public bool Inverse { get; set; } = false;

        public override bool Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            var inverse = parameter as bool? ?? Inverse;

            return value == null ^ inverse;
        }
    }
}
