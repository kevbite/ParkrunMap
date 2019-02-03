using Autofac;
using AutoMapper;
using ParkrunMap.Data.Mongo;
using ParkrunMap.FunctionsApp.Course;
using ParkrunMap.FunctionsApp.ParkrunCancellation;
using ParkrunMap.FunctionsApp.Parkruns;
using ParkrunMap.FunctionsApp.QueryParkrunsByBox;
using ParkrunMap.FunctionsApp.QueryParkrunsByRegion;
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

            builder.RegisterType<ParkrunCancellationToUpsertParkrunCancellationMessageProfile>()
                .As<Profile>();

            builder.RegisterType<AddParkrunCancellationMessageToAddParkrunCancellationRequestProfile>()
                .As<Profile>();

            builder.RegisterType<ParkrunToDownloadCourseMessageProfile>()
                .As<Profile>();

            builder.RegisterType<UpdateCourseDetailsMessageToUpdateParkrunCourseDetailsRequest>()
                .As<Profile>();

            builder.RegisterType<CloudBlockBlobUpdater>()
                .AsSelf();

            builder.RegisterType<PolygonCreator>()
                .As<IPolygonCreator>();

            base.Load(builder);
        }

        private static void RegisterFunctions(ContainerBuilder builder)
        {
            builder.RegisterType<DownloadGeoXmlTimerFunction>().AsSelf();
            builder.RegisterType<ParseGeoXmlFunction>().AsSelf();       
            builder.RegisterType<UpsertParkrunQueueFunction>().AsSelf();
            builder.RegisterType<QueryParkrunsByRegionFunction>().AsSelf();

            builder.RegisterType<ParseCancellationsPageFunction>().AsSelf();
            builder.RegisterType<AddParkrunCancellationQueueFunction>().AsSelf();
            builder.RegisterType<DownloadCancellationsPageFunction>().AsSelf();
            builder.RegisterType<QueueUpCourseDownloadsFunction>().AsSelf();
            builder.RegisterType<DownloadCourseFunction>().AsSelf();
            builder.RegisterType<ParseCourseFunction>().AsSelf();
            builder.RegisterType<UpdateCourseDetailsFunction>().AsSelf();
            builder.RegisterType<QueryParkrunsByGeoBoxFunction>().AsSelf();
            
        }
    }
}