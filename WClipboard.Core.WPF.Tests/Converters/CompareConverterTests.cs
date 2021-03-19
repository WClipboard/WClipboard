using System;
using System.Threading;
using WClipboard.Core.WPF.Converters;
using Xunit;

namespace WClipboard.Core.WPF.Tests.Converters
{
    public class CompareConverterTests
    {
        [Theory]
        [InlineData(1, CompareExpressions.EqualTo, 1, true)]
        [InlineData(1, CompareExpressions.EqualTo, 2, false)]

        [InlineData(2, CompareExpressions.GreaterThan, 1, true)]
        [InlineData(1, CompareExpressions.GreaterThan, 1, false)]
        [InlineData(0, CompareExpressions.GreaterThan, 1, false)]

        [InlineData(2, CompareExpressions.GreaterThanOrEqualTo, 1, true)]
        [InlineData(1, CompareExpressions.GreaterThanOrEqualTo, 1, true)]
        [InlineData(0, CompareExpressions.GreaterThanOrEqualTo, 1, false)]

        [InlineData(0, CompareExpressions.LessThan, 1, true)]
        [InlineData(1, CompareExpressions.LessThan, 1, false)]
        [InlineData(1, CompareExpressions.LessThan, 0, false)]

        [InlineData(0, CompareExpressions.LessThanOrEqualTo, 1, true)]
        [InlineData(1, CompareExpressions.LessThanOrEqualTo, 1, true)]
        [InlineData(1, CompareExpressions.LessThanOrEqualTo, 0, false)]

        [InlineData(1, CompareExpressions.NotEqualTo, 1, false)]
        [InlineData(1, CompareExpressions.NotEqualTo, 2, true)]
        public void Convert_Should_Work(IComparable first, CompareExpressions expression, IComparable second, bool expected)
        {
            //act
            var sut = new CompareConverter()
            {
                With = second,
                Expression = expression
            };
            var output = sut.Convert(first, typeof(bool), null, Thread.CurrentThread.CurrentCulture);

            //assert
            Assert.Equal(expected, output);
        }
    }
}
