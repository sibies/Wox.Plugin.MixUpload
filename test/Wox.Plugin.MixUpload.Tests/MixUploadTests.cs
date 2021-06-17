using FluentAssertions;
using Xunit;

namespace Wox.Plugin.MixUpload.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void TestMethod1()
        {

            var client = new MixUploadClient(null);
            var r = client.Search("scooter");
            r.Should().BeEmpty();
        }
    }
}
