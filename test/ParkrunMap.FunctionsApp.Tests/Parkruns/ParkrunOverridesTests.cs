using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using FluentAssertions;
using ParkrunMap.FunctionsApp.Parkruns;
using ParkrunMap.Scraping.Parkruns;
using Xunit;

namespace ParkrunMap.FunctionsApp.Tests.Parkruns
{
    public class ParkrunOverridesTests
    {
        [Fact]
        public void ShouldNotOverride()
        {
            var fixture = new Fixture();

            var parkrun = fixture.Create<GeoXmlParkrun>();

            var actual = new ParkrunOverrides().Apply(parkrun);

            actual.Should().BeEquivalentTo(parkrun);
        }

        [Fact]
        public void ShouldOverrideLocation()
        {
            var fixture = new Fixture();
            fixture.Customizations.Add(new ParkrunWebsiteDomainArg("www.parkrun.ie"));
            fixture.Customizations.Add(new ParkrunWebsitePathArg("/tymon"));
            var parkrun = fixture.Build<GeoXmlParkrun>()
                .Create();

            var actual = new ParkrunOverrides().Apply(parkrun);

            actual.Latitude.Should().Be(53.304650);
            actual.Longitude.Should().Be(-6.341203);
            actual.Should().BeEquivalentTo(parkrun, opt => opt.Excluding(x => x.Latitude).Excluding(x => x.Longitude));
        }
    }

    public class ParkrunWebsiteDomainArg : ISpecimenBuilder
    {
        private readonly string _value;

        public ParkrunWebsiteDomainArg(string value)
        {
            _value = value;
        }

        public object Create(object request, ISpecimenContext context)
        {
            var pi = request as ParameterInfo;
            if (pi == null)
                return new NoSpecimen();

            if (pi.Member.DeclaringType != typeof(GeoXmlParkrun) ||
                pi.ParameterType != typeof(string) ||
                pi.Name != "websiteDomain")
                return new NoSpecimen();

            return _value;
        }
    }

    public class ParkrunWebsitePathArg : ISpecimenBuilder
    {
        private readonly string _value;

        public ParkrunWebsitePathArg(string value)
        {
            _value = value;
        }

        public object Create(object request, ISpecimenContext context)
        {
            var pi = request as ParameterInfo;
            if (pi == null)
                return new NoSpecimen();

            if (pi.Member.DeclaringType != typeof(GeoXmlParkrun) ||
                pi.ParameterType != typeof(string) ||
                pi.Name != "websitePath")
                return new NoSpecimen();

            return _value;
        }
    }
}
