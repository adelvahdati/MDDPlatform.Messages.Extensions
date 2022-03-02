using MDDPlatform.Messages.Commands;
using MDDPlatform.Messages.Events;
using MDDPlatform.Messages.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace MDDPlatform.Messages.Extensions.Handlers
{
    public static class Extensions
    {
        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var modules = assembly.GetModules();        
                foreach (var module in modules)
                {
                    Type[] moduleTypes = module.GetTypes();
                    foreach (Type moduleType in moduleTypes)
                    {
                        if (!moduleType.IsAbstract && moduleType.IsClass)
                        {
                            var interfaces = moduleType.GetInterfaces();
                            foreach (var item in interfaces)
                            {
                                if (item.IsGenericType && item.GetGenericTypeDefinition().IsHandler())
                                {
                                    Type interfaceType = item.GetGenericTypeDefinition();
                                    Type[] interfaceArgumentsType = item.GetGenericArguments();
                                    Type handlerInterface = interfaceType.MakeGenericType(interfaceArgumentsType);
                                    services.AddScoped(handlerInterface, moduleType);
                                }
                            }
                        }
                    }
                }
            }
            return services;
        }

        internal static bool IsHandler(this Type type)
        {
            if (type == typeof(ICommandHandler<>))
                return true;

            if (type == typeof(IEventHandler<>))
                return true;

            if (type == typeof(IQueryHandler<,>))
                return true;

            return false;
        }
    }
}