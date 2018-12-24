using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ParkrunMap.FunctionsApp.Tests
{
    public class AutoMapperTests
    {
        [Fact]
        public void AssertConfigurationIsValid()
        {
            var mapperConfiguration = Container.Instance.Resolve<MapperConfiguration>(Mock.Of<ILogger>());

            mapperConfiguration.AssertConfigurationIsValid();
        }
    }
}
