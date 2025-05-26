using System;
using System.Linq;
using System.Reflection;
using BikeRentalApp.Infrastructure.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BikeRentalApp.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceLogging(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var ourAsm = typeof(ServiceCollectionExtensions).Assembly;
            var descriptors = services
                .Where(d =>
                    d.ImplementationType != null
                    && d.ServiceType.Name.EndsWith("Service")
                    && d.ImplementationType.Assembly == ourAsm
                    && d.ImplementationType.Namespace?.StartsWith("BikeRentalApp") == true
                )
                .ToList();

            foreach (var d in descriptors)
            {
                var svcType = d.ServiceType;
                var implType = d.ImplementationType!;

                services.Remove(d);
                services.AddScoped(implType);

                services.AddScoped(
                    svcType,
                    sp =>
                    {
                        var decorated = sp.GetRequiredService(implType);
                        var accessor = sp.GetRequiredService<IHttpContextAccessor>();

                        // ILogger<T>
                        var loggerType = typeof(ILogger<>).MakeGenericType(svcType);
                        var loggerInstance = sp.GetRequiredService(loggerType);

                        // create proxy
                        var proxy = DispatchProxy.Create(
                            svcType,
                            typeof(LoggingProxy<>).MakeGenericType(svcType)
                        );

                        // set properties via reflection
                        var proxyType = proxy.GetType();
                        proxyType.GetProperty("Decorated")!.SetValue(proxy, decorated);
                        proxyType.GetProperty("Logger")!.SetValue(proxy, loggerInstance);
                        proxyType.GetProperty("HttpContextAccessor")!.SetValue(proxy, accessor);

                        return proxy;
                    }
                );
            }

            return services;
        }
    }
}
