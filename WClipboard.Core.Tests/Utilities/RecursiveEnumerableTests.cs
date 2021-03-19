using System.Linq;
using WClipboard.Core.Utilities;
using Xunit;

namespace WClipboard.Core.Tests.Utilities
{
    public class RecursiveEnumerableTests
    {
        [Fact]
        public void Should_Work()
        {
            int start = 0;
            int end = 20;
            Assert.Equal(Enumerable.Range(start, end - start), RecursiveEnumerable.Get(start, i => i += 1, end));
        }
    }
}
