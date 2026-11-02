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
    public class BrotliCompressionHandler : DelegatingHandler
    {
        private readonly long _maxDecodedBytes;

        public BrotliCompressionHandler() : this(64L * 1024 * 1024) { }

        public BrotliCompressionHandler(long maxDecodedBytes)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(maxDecodedBytes, 1);
            _maxDecodedBytes = maxDecodedBytes;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.Content is null || !response.Content.Headers.ContentEncoding.Contains("br"))
                return response;

            var original = response.Content;
            var decompressed = new MemoryStream();

            try
            {
                var compressed = await original.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                await using var brotli = new BrotliStream(compressed, CompressionMode.Decompress);
                var buffer = new byte[81920];
                int count;
                while ((count = await brotli.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) != 0)
                {
                    if (decompressed.Length > _maxDecodedBytes - count)
                        throw new HttpRequestException("Brotli response exceeds the decoded size limit.");
                    await decompressed.WriteAsync(buffer.AsMemory(0, count), cancellationToken).ConfigureAwait(false);
                }
            }
            catch
            {
                decompressed.Dispose();
                response.Dispose();
                throw;
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
