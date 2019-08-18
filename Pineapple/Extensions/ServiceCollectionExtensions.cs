using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Pineapple.Extensions
{
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
