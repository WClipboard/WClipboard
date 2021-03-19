using System;
using System.Threading;
using System.Windows.Media;
using WClipboard.Core.WPF.Converters;
using Xunit;

namespace WClipboard.Core.WPF.Tests.Converters
{
    public class AlphaColorConverterTests
    {
        [Fact]
        public void Color_Should_Work_From_Object()
        {
            //arrange
            var random = new Random();
            var newA = (byte)random.Next(0, 255);
            var oldColor = Color.FromArgb((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));

            //act
            var sut = new AlphaColorConverter
            {
                A = newA
            };
            var newColor = sut.Convert(oldColor, typeof(Color), null, Thread.CurrentThread.CurrentUICulture);

            //assert
            Assert.Equal(newA, newColor.A);
            Assert.Equal(oldColor.R, newColor.R);
            Assert.Equal(oldColor.G, newColor.G);
            Assert.Equal(oldColor.B, newColor.B);
        }

        [Fact]
        public void Color_Should_Work_From_Parameter()
        {
            //arrange
            var random = new Random();
            var newA = (byte)random.Next(0, 255);
            var oldColor = Color.FromArgb((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));

            //act
            var sut = new AlphaColorConverter();
            var newColor = sut.Convert(oldColor, typeof(Color), newA, Thread.CurrentThread.CurrentUICulture);

            //assert
            Assert.Equal(newA, newColor.A);
            Assert.Equal(oldColor.R, newColor.R);
            Assert.Equal(oldColor.G, newColor.G);
            Assert.Equal(oldColor.B, newColor.B);
        }

        [Fact]
        public void SolidColorBrush_Should_Work_From_Object()
        {
            //arrange
            var random = new Random();
            var newA = (byte)random.Next(0, 255);
            var oldBrush = new SolidColorBrush(Color.FromArgb((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255)));

            //act
            var sut = new AlphaSolidColorBrushConverter
            {
                A = newA
            };
            var newBrush = sut.Convert(oldBrush, typeof(Color), null, Thread.CurrentThread.CurrentUICulture);

            //assert
            Assert.Equal(newA, newBrush.Color.A);
            Assert.Equal(oldBrush.Color.R, newBrush.Color.R);
            Assert.Equal(oldBrush.Color.G, newBrush.Color.G);
            Assert.Equal(oldBrush.Color.B, newBrush.Color.B);
        }

        [Fact]
        public void SolidColorBrush_Should_Work_From_Parameter()
        {
            //arrange
            var random = new Random();
            var newA = (byte)random.Next(0, 255);
            var oldBrush = new SolidColorBrush(Color.FromArgb((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255)));

            //act
            var sut = new AlphaSolidColorBrushConverter();
            var newBrush = sut.Convert(oldBrush, typeof(Color), newA, Thread.CurrentThread.CurrentUICulture);

            //assert
            Assert.Equal(newA, newBrush.Color.A);
            Assert.Equal(oldBrush.Color.R, newBrush.Color.R);
            Assert.Equal(oldBrush.Color.G, newBrush.Color.G);
            Assert.Equal(oldBrush.Color.B, newBrush.Color.B);
        }
    }
}
