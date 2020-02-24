using System;
using System.Net.Http;
using Autofac;
using AutoMapper;
using ParkrunMap.Data.Mongo;
using ParkrunMap.FunctionsApp.Course;
using ParkrunMap.FunctionsApp.ParkrunCancellation;
using ParkrunMap.FunctionsApp.ParkrunFeatures;
using ParkrunMap.FunctionsApp.Parkruns;
using ParkrunMap.FunctionsApp.ParkrunStatistics;
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

            builder.RegisterType<ParkrunQuestionnaireResponseAggregationToUpdateParkrunFeaturesRequestProfile>()
                .As<Profile>();
            
            builder.RegisterType<UpdateCourseDetailsMessageToUpdateParkrunCourseDetailsRequest>()
                .As<Profile>();

            builder.RegisterType<CloudBlockBlobUpdater>()
                .AsSelf();

            builder.RegisterType<PolygonCreator>()
                .As<IPolygonCreator>();

            builder.RegisterType<ParkrunOverrides>()
                .AsSelf();

            builder.RegisterType<HttpClient>()
                .AsSelf()
                .Named<HttpClient>("questionnaire-response-downloader-client");

            var googleApiKey = Environment.GetEnvironmentVariable("GoogleApiKey");

            builder.Register(x => new QuestionnaireResponseDownloader(x.ResolveNamed<HttpClient>("questionnaire-response-downloader-client"), googleApiKey))
                .AsSelf();

            builder.RegisterType<ParkrunQuestionnaireResponseAggregator>()
                .AsSelf();

            base.Load(builder);
        }

        private static void RegisterFunctions(ContainerBuilder builder)
        {
            builder.RegisterType<DownloadEventsJsonTimerFunction>().AsSelf();
            builder.RegisterType<ParseEventsJsonFunction>().AsSelf();       
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
            builder.RegisterType<DownloadQuestionnaireResponsesTimerFunction>().AsSelf();
            builder.RegisterType<QuestionnaireResponseAggregatorQueueFunction>().AsSelf();
            builder.RegisterType<UpdateParkrunFeaturesQueueFunction>().AsSelf();
            builder.RegisterType<UpdateParkrunStatisticsFunction>().AsSelf();
            builder.RegisterType<ParseStatisticsFunction>().AsSelf();
        }
    }
}