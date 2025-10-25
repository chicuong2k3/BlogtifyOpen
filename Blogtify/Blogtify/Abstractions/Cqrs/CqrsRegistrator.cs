using System.Reflection;

namespace Blogtify.Abstractions.Cqrs;

public static class CqrsRegistrator
{
    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        services.AddMediatR(configure =>
        {
            configure.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
            //configure.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
            //configure.AddOpenBehavior(typeof(CachePipelineBehavior<,>));
        });

        return services;
    }
}
