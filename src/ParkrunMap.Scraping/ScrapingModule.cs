using System.Net.Http;
using Autofac;
using ParkrunMap.Scraping.Parkruns;

namespace ParkrunMap.Scraping
{
    public class ScrapingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => new ParkrunScraper(x.ResolveNamed<HttpClient>("parkrun-scraper-client"), x.Resolve<GeoXmlParser>()))
                .AsSelf();

            builder.RegisterType<ParkrunXElementValidator>().AsSelf();

            builder.RegisterType<GeoXmlParser>().AsSelf();

            builder.RegisterType<HttpClient>()
                .AsSelf()
                .Named<HttpClient>("parkrun-scraper-client");

            base.Load(builder);
        }
    }
}
