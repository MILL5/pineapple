using System;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Pineapple.Extensions
{
    [Obsolete("Use TryAddSingleton and services.Any(...) from Microsoft.Extensions.DependencyInjection.Extensions.", DiagnosticId = "PINE008", UrlFormat = "https://mill5.github.io/pineapple/guides/migration/#{0}")]
    public static class ServiceCollectionExtensions
    {
        public static bool IsConfigured<T>(this IServiceCollection services) where T : class
        {
            return services.Any(sd => sd.ServiceType == typeof(T));
        }

        public static void Configure<T>(this IServiceCollection services) where T : class
        {
            services.AddSingleton<T>();
        }
    }
}
