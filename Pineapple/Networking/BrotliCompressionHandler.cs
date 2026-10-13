using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Pineapple.Networking
{
    /// <summary>
    /// A <see cref="DelegatingHandler"/> that transparently decompresses <c>Content-Encoding: br</c> responses
    /// using <see cref="BrotliStream"/>.
    /// </summary>
    [Obsolete("Use AutomaticDecompression = DecompressionMethods.All on HttpClientHandler or SocketsHttpHandler.", DiagnosticId = "PINE001", UrlFormat = "https://mill5.github.io/pineapple/guides/migration/#{0}")]
    public class BrotliCompressionHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.Content is null || !response.Content.Headers.ContentEncoding.Contains("br"))
                return response;

            var original = response.Content;
            var compressed = await original.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var decompressed = new MemoryStream();

            await using (var brotli = new BrotliStream(compressed, CompressionMode.Decompress))
            {
                await brotli.CopyToAsync(decompressed, cancellationToken).ConfigureAwait(false);
            }

            decompressed.Position = 0;

            var content = new StreamContent(decompressed);

            foreach (var header in original.Headers)
            {
                if (header.Key is "Content-Encoding" or "Content-Length")
                    continue;

                content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            response.Content = content;
            original.Dispose();

            return response;
        }
    }
}
