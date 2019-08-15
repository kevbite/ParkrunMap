using System.Net.Http;
using Autofac;
using ParkrunMap.Scraping.Cancellations;
using ParkrunMap.Scraping.Course;
using ParkrunMap.Scraping.Parkruns;
using ParkrunMap.Scraping.Statistics;

namespace ParkrunMap.Scraping
{
    public class ScrapingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => new CoursePageDownloader(x.ResolveNamed<HttpClient>("course-page-client")))
                .AsSelf();

            builder.Register(x => new CancellationsPageDownloader(x.ResolveNamed<HttpClient>("cancellations-page-client")))
                .AsSelf();

            builder.Register(x => new GeoXmlDownloader(x.ResolveNamed<HttpClient>("geo-xml-client")))
                .AsSelf();

            builder.Register(x => new EventsJsonDownloader(x.ResolveNamed<HttpClient>("events-json-client")))
                .AsSelf();

            builder.RegisterType<ParkrunXElementValidator>().AsSelf();

            builder.RegisterType<CourseParser>().AsSelf();

            builder.RegisterType<StatisticsParser>().AsSelf();

            builder.RegisterType<CancellationsParser>().AsSelf();

            builder.RegisterType<GeoXmlParser>().AsSelf();
            
            builder.RegisterType<ProxyStore>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<ScrapingHttpClientFactory>()
                .AsSelf()
                .SingleInstance();

            builder.Register(ctx => ctx.Resolve<ScrapingHttpClientFactory>().Create())
                .AsSelf()
                .Named<HttpClient>("geo-xml-client");

            builder.Register(ctx => ctx.Resolve<ScrapingHttpClientFactory>().Create())
                .AsSelf()
                .Named<HttpClient>("events-json-client");


            builder.Register(ctx => ctx.Resolve<ScrapingHttpClientFactory>().Create())
                .AsSelf()
                .Named<HttpClient>("cancellations-page-client");

            builder.Register(ctx => ctx.Resolve<ScrapingHttpClientFactory>().Create())
                .AsSelf()
                .Named<HttpClient>("course-page-client");

            base.Load(builder);
        }
    }
}
