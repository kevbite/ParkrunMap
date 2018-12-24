using Autofac;
using Microsoft.Extensions.Logging;

namespace ParkrunMap.FunctionsApp
{
    public class Container
    {
        private readonly IContainer _container;

        public static Container Instance { get; } = new Container();

        public Container()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule<FunctionsAppModule>();
            
             _container = builder.Build();
        }
        
        public TService Resolve<TService>(ILogger log)
        {
            return _container.Resolve<TService>(TypedParameter.From(log));
        }
    }
}