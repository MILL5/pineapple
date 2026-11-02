using System;
using Microsoft.Extensions.DependencyInjection;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Networking
{
    [Obsolete("Use AutomaticDecompression = DecompressionMethods.All on HttpClientHandler or SocketsHttpHandler.", DiagnosticId = "PINE001", UrlFormat = "https://mill5.github.io/pineapple/guides/migration/#{0}")]
    public static class HttpClientBuildExtensions
    {
        public static IServiceCollection AddBrotliCompression(this IServiceCollection services)
        {
            CheckIsNotNull(nameof(services), services);

            return services.AddTransient<BrotliCompressionHandler>();
        }

        public static IHttpClientBuilder AddBrotliCompression(this IHttpClientBuilder httpClientBuilder)
        {
            CheckIsNotNull(nameof(httpClientBuilder), httpClientBuilder);

            return httpClientBuilder.AddHttpMessageHandler<BrotliCompressionHandler>();
        }

        public static IHttpClientBuilder AddBrotliCompression(this IHttpClientBuilder httpClientBuilder, IServiceCollection services)
        {
            CheckIsNotNull(nameof(httpClientBuilder), httpClientBuilder);

            services.AddBrotliCompression();

            return httpClientBuilder.AddHttpMessageHandler<BrotliCompressionHandler>();
        }
    }
}