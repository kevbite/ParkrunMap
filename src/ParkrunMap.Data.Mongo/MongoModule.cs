using System;
using Autofac;
using MongoDB.Driver;
using ParkrunMap.Domain;

namespace ParkrunMap.Data.Mongo
{
    public class MongoModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UpsertParkrun.Handler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<QueryParkrunByRegion.Handler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<AddParkrunCancellation.Handler>().AsImplementedInterfaces().InstancePerDependency();
          
            var mongoUrl = Environment.GetEnvironmentVariable("MongoDbUrl");

            builder.Register(context => new MongoClient(mongoUrl))
                .AsImplementedInterfaces();

            builder.Register(context => context.Resolve<IMongoClient>().GetDatabase("parkrun-map"))
                .AsImplementedInterfaces();

            builder.Register(context => context.Resolve<IMongoDatabase>().GetCollection<Parkrun>("parkruns"))
                .AsImplementedInterfaces();

            base.Load(builder);
        }
    }
}
