using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace ParkrunMap.Scraping.Tests
{
    public class ProxyStoreTests
    {
        [Fact]
        public async Task ShouldReturnProxyUri()
        {
            var proxyStore = new ProxyStore();
            var randomProxyAddress = await proxyStore.GetRandomProxyAddress();

            randomProxyAddress.Should().NotBeNull();
        }
    }
}
