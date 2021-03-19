using System.Threading;
using System.Windows;
using WClipboard.Core.WPF.Converters;
using Xunit;

namespace WClipboard.Core.WPF.Tests.Converters
{
    public class BooleanToVisibilityConverterTests
    {
        [Theory]
        [InlineData(true, false, Visibility.Visible)]

        [InlineData(true, true, Visibility.Collapsed)]
        [InlineData(true, true, Visibility.Hidden, Visibility.Hidden)]


        [InlineData(false, true, Visibility.Visible)]

        [InlineData(false, false, Visibility.Collapsed)]
        [InlineData(false, false, Visibility.Hidden, Visibility.Hidden)]
        public void Convert_Should_Work(bool input, bool inverse, Visibility expected, Visibility falseValue = Visibility.Collapsed)
        {
            //act
            var sut = new BooleanToVisibilityConverter
            {
                Inverse = inverse,
                FalseValue = falseValue
            };
            var output = sut.Convert(input, typeof(Visibility), null, Thread.CurrentThread.CurrentCulture);

            //assert
            Assert.Equal(expected, output);
        }

        [Theory]
        [InlineData(Visibility.Visible, false, true)]

        [InlineData(Visibility.Collapsed, true, true)]
        [InlineData(Visibility.Hidden, true, true, Visibility.Hidden)]
        [InlineData(Visibility.Hidden, true, true, Visibility.Collapsed)]


        [InlineData(Visibility.Visible, true, false)]

        [InlineData(Visibility.Collapsed, false, false)]
        [InlineData(Visibility.Hidden, false, false, Visibility.Hidden)]
        [InlineData(Visibility.Hidden, false, false, Visibility.Collapsed)]
        public void ConvertBack_Should_Work(Visibility input, bool inverse, bool expected, Visibility falseValue = Visibility.Collapsed)
        {
            //act
            var sut = new BooleanToVisibilityConverter
            {
                Inverse = inverse,
                FalseValue = falseValue
            };
            var output = sut.ConvertBack(input, typeof(bool), null, Thread.CurrentThread.CurrentCulture);

            //assert
            Assert.Equal(expected, output);
        }
    }
}
