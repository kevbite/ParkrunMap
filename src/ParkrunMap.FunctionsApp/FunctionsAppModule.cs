using Autofac;
using AutoMapper;
using ParkrunMap.Data.Mongo;
using ParkrunMap.FunctionsApp.PullParkruns;
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
            builder.RegisterType<PullParkrunsTimerFunction>().AsSelf();
            builder.RegisterType<UpsertParkrunQueueFunction>().AsSelf();
        }
    }
}