using System.Net.Http;
using Autofac;
using ParkrunMap.Scraping.Cancellations;
using ParkrunMap.Scraping.Course;
using ParkrunMap.Scraping.Parkruns;

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

            builder.RegisterType<ParkrunXElementValidator>().AsSelf();

            builder.RegisterType<CourseParser>().AsSelf();

            builder.RegisterType<CancellationsParser>().AsSelf();

            builder.RegisterType<GeoXmlParser>().AsSelf();

            builder.RegisterType<HttpClient>()
                .AsSelf()
                .Named<HttpClient>("geo-xml-client");

            builder.RegisterType<HttpClient>()
                .AsSelf()
                .Named<HttpClient>("cancellations-page-client");

            builder.Register(ctx => new HttpClient(new RedirectHandler(new HttpClientHandler())))
                .AsSelf()
                .Named<HttpClient>("course-page-client");

            base.Load(builder);
        }
    }
}
