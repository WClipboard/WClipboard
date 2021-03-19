using System;
using System.Globalization;

namespace WClipboard.Core.WPF.Converters
{
    [Flags]
    public enum CompareExpressions
    {
        LessThan = 1,
        EqualTo = 2,
        GreaterThan = 4,
        LessThanOrEqualTo = LessThan | EqualTo,
        GreaterThanOrEqualTo = GreaterThan | EqualTo,
        NotEqualTo = LessThan | GreaterThan,
    }

    public class CompareConverter : BaseConverter<IComparable, bool>
    {
        public CompareExpressions Expression { get; set; }
        public object? With { get; set; }

        public override bool Convert(IComparable value, Type targetType, object? parameter, CultureInfo culture)
        {
            var first = value;
            var second = With;

            if (!(parameter is CompareExpressions expression))
            {
                expression = Expression;

                if (parameter != null)
                {
                    second = parameter;
                }
            }

            if (first is IConvertible && second is IConvertible)
            {
                second = System.Convert.ChangeType(second, first.GetType(), culture);
            }

            return Check(first, expression, second);
        }

        private bool Check(IComparable first, CompareExpressions expressions, object? second)
        {
            var dif = first.CompareTo(second);

            bool result = false;

            if (expressions.HasFlag(CompareExpressions.LessThan))
            {
                result |= dif < 0;
            }

            if (expressions.HasFlag(CompareExpressions.EqualTo))
            {
                result |= dif == 0;
            }

            if (expressions.HasFlag(CompareExpressions.GreaterThan))
            {
                result |= dif > 0;
            }

            return result;
        }
    }
}
