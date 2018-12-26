using Autofac;
using AutoMapper;
using ParkrunMap.Data.Mongo;
using ParkrunMap.FunctionsApp.DownloadGeoXml;
using ParkrunMap.FunctionsApp.ParseGeoXml;
using ParkrunMap.FunctionsApp.QueryParkrunsByRegion;
using ParkrunMap.FunctionsApp.UpsertParkrun;
using ParkrunMap.Scraping;

namespace ParkrunMap.FunctionsApp
{
    public class FunctionsAppModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<MediatorModule>();
            builder.RegisterModule<ScrapingModule>();
            builder.RegisterModule<AutoMapperModule>();
            builder.RegisterModule<MongoModule>();
            
            RegisterFunctions(builder);

            builder.RegisterType<ParkrunToUpsertParkrunMessageProfile>()
                .As<Profile>();

            base.Load(builder);
        }

        private static void RegisterFunctions(ContainerBuilder builder)
        {
            builder.RegisterType<DownloadGeoXmlTimerFunction>().AsSelf();
            builder.RegisterType<ParseGeoXmlFunction>().AsSelf();       
            builder.RegisterType<UpsertParkrunQueueFunction>().AsSelf();
            builder.RegisterType<QueryParkrunsByRegionFunction>().AsSelf();
        }
    }
}