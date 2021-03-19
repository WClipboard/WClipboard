using System;
using System.Globalization;

namespace WClipboard.Core.WPF.Converters.DateTimeSpan
{
    public interface IDateTimeSpanConverter
    {
        (string Text, TimeSpan ReUpdateOver) Convert(DateTime source, DateTime target, object param, CultureInfo culture);
    }
}
