using Autofac;
using MediatR;

namespace ParkrunMap.FunctionsApp
{
    public class MediatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();

            // request & notification handlers
            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();

                return t => c.Resolve(t);
            });

            base.Load(builder);
        }
    }
}