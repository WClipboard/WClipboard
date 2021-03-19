using System;
using System.Globalization;
using System.Windows;

namespace WClipboard.Core.WPF.Converters
{
    public class BooleanToVisibilityConverter : BaseConverter<bool, Visibility>
    {
        public bool Inverse { get; set; } = false;

        public Visibility FalseValue { get; set; } = Visibility.Collapsed;

        public override Visibility Convert(bool value, Type targetType, object? parameter, CultureInfo culture)
        {
            var inverse = parameter as bool? ?? Inverse;
            var falseValue = parameter as Visibility? ?? FalseValue;

            return (value ^ inverse) ? Visibility.Visible : falseValue;
        }

        public override bool ConvertBack(Visibility value, Type targetType, object? parameter, CultureInfo culture)
        {
            var inverse = parameter as bool? ?? Inverse;

            return value == Visibility.Visible ^ inverse;
        }
    }
}
