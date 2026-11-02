using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pineapple.Networking;

namespace Pineapple.Tests;

[TestClass]
public class BrotliCompressionTests
{
    private sealed class CompressedResponse : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using var output = new MemoryStream();
            using (var compressor = new BrotliStream(output, CompressionLevel.Fastest, true))
                compressor.Write(new byte[4096]);
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(output.ToArray()) };
            response.Content.Headers.ContentEncoding.Add("br");
            response.Content.Headers.Add("X-Test", "preserved");
            return Task.FromResult(response);
        }
    }

    [TestMethod]
    public async Task DecodingPreservesHeadersAndRejectsOversizedPayloads()
    {
        using var client = new HttpClient(new BrotliCompressionHandler(4096) { InnerHandler = new CompressedResponse() });
        using var response = await client.GetAsync("http://localhost/test");
        Assert.AreEqual(4096, (await response.Content.ReadAsByteArrayAsync()).Length);
        Assert.IsFalse(response.Content.Headers.ContentEncoding.Contains("br"));
        Assert.IsTrue(response.Content.Headers.Contains("X-Test"));
        using var limited = new HttpClient(new BrotliCompressionHandler(4095) { InnerHandler = new CompressedResponse() });
        await Assert.ThrowsExactlyAsync<HttpRequestException>(() => limited.GetAsync("http://localhost/test"));
    }
}
