using System;
using System.Globalization;
using System.Windows.Data;

namespace WClipboard.Core.WPF.Converters
{
    public abstract class BaseConverter<TFrom, TTo> : IValueConverter
    {
        object? IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is TFrom formattedValue)
                return Convert(formattedValue, targetType, parameter, culture);
            else if (value == null && default(TFrom) == null)
                return Convert(default!, targetType, parameter, culture);
            else if (value != null && Type.GetTypeCode(typeof(TFrom)) != TypeCode.Object && Type.GetTypeCode(value.GetType()) != TypeCode.Object)
                return Convert((TFrom)System.Convert.ChangeType(value, typeof(TFrom)), targetType, parameter, culture);
            else
                return FallbackConvert(value);
        }

        public abstract TTo Convert(TFrom value, Type targetType, object? parameter, CultureInfo culture);

        protected virtual TTo FallbackConvert(object? value)
        {
            throw new InvalidCastException($"Input must be {typeof(TFrom).FullName} but is {value?.GetType().FullName ?? "null"} and cannot be casted");
        }

        object? IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is TTo formattedValue)
                return ConvertBack(formattedValue, targetType, parameter, culture);
            else if (value == null && default(TTo) == null)
                return ConvertBack(default!, targetType, parameter, culture);
            else if (value != null && Type.GetTypeCode(typeof(TTo)) != TypeCode.Object && Type.GetTypeCode(value.GetType()) != TypeCode.Object)
                return ConvertBack((TTo)System.Convert.ChangeType(value, typeof(TTo)), targetType, parameter, culture);
            else
                return FallbackConvertBack(value);
        }

        protected virtual TFrom FallbackConvertBack(object? value)
        {
            throw new InvalidCastException($"Input must be {typeof(TTo).FullName} but is {value?.GetType().FullName ?? "null"} and cannot be casted");
        }

        public virtual TFrom ConvertBack(TTo value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"Cannot convert back from {typeof(TTo).FullName} to {typeof(TFrom).FullName}");
        }
    }

    public abstract class BaseConverter : IValueConverter
    {
        public abstract object Convert(object? value, Type targetType, object? parameter, CultureInfo culture);

        public virtual object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"Cannot convert back in {GetType().FullName}");
        }
    }
}
