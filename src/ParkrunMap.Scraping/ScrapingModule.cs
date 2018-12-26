using System.Net.Http;
using Autofac;
using ParkrunMap.Scraping.Parkruns;

namespace ParkrunMap.Scraping
{
    public class ScrapingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => new GeoXmlDownloader(x.ResolveNamed<HttpClient>("geo-xml-client")))
                .AsSelf();

            builder.RegisterType<ParkrunXElementValidator>().AsSelf();

            builder.RegisterType<GeoXmlParser>().AsSelf();

            builder.RegisterType<HttpClient>()
                .AsSelf()
                .Named<HttpClient>("geo-xml-client");

            base.Load(builder);
        }
    }
}
