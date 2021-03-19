using Moq;
using System;
using WClipboard.Core.Clipboard.Format;
using Xunit;

namespace WClipboard.Core.Tests.Managers
{
    public class ClipboardFormatsManagerTests
    {
        [Fact]
        public void Constructor_Should_Work()
        {
            //assert
            var category = new ClipboardFormatCategory("FormatsCategory", "T:Cat");

            var format1 = new ClipboardFormat("Format1", "Format1", "T:Format1", category);
            var format2 = new ClipboardFormat("Format2", "Format2", "T:Format2", category);

            var categoriesManager = new Mock<IClipboardFormatCategoriesManager>();
            categoriesManager.Setup(x => x.Contains(category)).Returns(true);

            //act
            new ClipboardFormatsManager(new[] { format1, format2 }, categoriesManager.Object);
        }

        [Fact]
        public void Adding_Formats_With_Not_Registered_Categories_Should_Not_Work()
        {
            //assert
            var category = new ClipboardFormatCategory("FormatsCategory", "T:Cat");

            var format1 = new ClipboardFormat("Format1", "Format1", "T:Format1", category);
            var format2 = new ClipboardFormat("Format2", "Format2", "T:Format2", category);

            var categoriesManager = new Mock<IClipboardFormatCategoriesManager>();
            categoriesManager.Setup(x => x.Contains(It.IsAny<ClipboardFormatCategory>())).Returns(false);

            //act
            Assert.Throws<ArgumentException>(() => new ClipboardFormatsManager(new[] { format1, format2 }, categoriesManager.Object));
        }
    }
}
