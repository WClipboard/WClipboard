using System;
using System.Globalization;

namespace WClipboard.Core.WPF.Converters.DateTimeSpan
{
    public class DefaultDateTimeSpanConverter : IDateTimeSpanConverter
    {
        private double roundPoint = 0.75;
        public double RoundPoint {
            get => roundPoint;
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(value), $"The value of the {nameof(RoundPoint)} must be between {0} and {1} but given is {value}");
                else
                    roundPoint = value;
            }
        }

        public string Format { get; set; } = "{0} ({1})";

        public (string Text, TimeSpan ReUpdateOver) Convert(DateTime source, DateTime target, object param, CultureInfo culture)
        {
            var span = target - source;
            string dateTimeFormat;
            string diffFormat;

            if(!(param is string format))
                format = Format;

            if (param is double roundPoint)
            {
                if (roundPoint < 0 || roundPoint > 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(param), $"The value of the {nameof(RoundPoint)} must be between {0} and {1} but given is {roundPoint}");
                }
            }
            else
            {
                roundPoint = RoundPoint;
            }

            TimeSpan nextConvert;

            var days = Round(span.TotalDays);
            if (days > 0)
            {
                dateTimeFormat = "HH:mm dd-MM-yyyy";
                diffFormat = days == 1 ? "yesterday" : $"{days} days ago";
                nextConvert = new TimeSpan(days, (int)(24 * roundPoint), 0, 0, 0) - span;
            }
            else
            {
                dateTimeFormat = "HH:mm";
                var hours = Round(span.TotalHours);
                if (hours > 0)
                {
                    diffFormat = hours == 1 ? "a hour ago" : $"{hours} hours ago";
                    nextConvert = new TimeSpan(0, hours, (int)(60 * roundPoint), 0, 0) - span;
                }
                else
                {
                    var minutes = Round(span.TotalMinutes);
                    if (minutes != 0)
                    {
                        diffFormat = minutes == 1 ? "a minute ago" : $"{minutes} minutes ago";
                        nextConvert = new TimeSpan(0, 0, minutes, (int)(60 * roundPoint), 0) - span;
                    }
                    else
                    {
                        var seconds = Round(span.TotalSeconds);
                        nextConvert = new TimeSpan(0, 0, 0, seconds, (int)(1000 * roundPoint)) - span;
                        if (seconds != 0)
                        {
                            diffFormat = seconds == 1 ? "a second ago" : $"{seconds} seconds ago";
                        }
                        else
                        {
                            diffFormat = "now";
                        }
                    }
                }
            }

            return (string.Format(format, source.ToString(dateTimeFormat), diffFormat), nextConvert);
        }

        private int Round(double value)
        {
            var floor = Math.Floor(value);
            var decimals = value - floor;
            return (int)floor + (decimals > roundPoint ? 1 : 0);
        }
    }
}
