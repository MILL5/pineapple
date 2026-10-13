using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using static Pineapple.Common.Preconditions;
using static Pineapple.Common.Cleanup;
// ReSharper disable UnusedMember.Global

namespace Pineapple.Health
{
    public static class Check
    {
        private static readonly HttpClient _httpClient = new() { Timeout = System.Threading.Timeout.InfiniteTimeSpan };

        public static bool IsRunning(string processName)
        {
            CheckIsNotNullOrWhitespace(nameof(processName), processName);

            var isRunning = false;
            var procs = Process.GetProcesses().OrderBy(p => p.ProcessName);

            try
            {
                foreach (var p in procs)
                {
                    if (p.ProcessName.Contains(processName))
                    {
                        isRunning = true;
                        break;
                    }
                }
            }
            finally
            {
                foreach (var p in procs)
                {
                    SafeMethod(p.Dispose);
                }
            }

            return isRunning;
        }

        public static bool IsAlive(string server, int port)
        {
            CheckIsNotNullOrWhitespace(nameof(server), server);
            CheckIsNotLessThanOrEqualTo(nameof(port), port, 0);

            var isAvailable = false;

            using (var client = new TcpClient())
            {
                try
                {
                    client.Connect(server, port);
                    isAvailable = client.Connected;
                }
                catch
                {
                    // ignored
                }
                finally
                {
                    SafeMethod(client.Close);
                }
            }

            return isAvailable;
        }

        public static bool IsUrlAvailable(this Uri url, int timeoutInMs)
        {
            CheckIsNotLessThanOrEqualTo(nameof(timeoutInMs), timeoutInMs, 0);

            var isAvailable = false;

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                using var cts = new CancellationTokenSource(timeoutInMs);
                using var response = _httpClient.Send(request, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                isAvailable = response.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                // ignored
            }

            return isAvailable;
        }

        public static bool IsUrlAvailable(string url, int timeoutInMs)
        {
            CheckIsWellFormedUri(nameof(url), url);

            return new Uri(url).IsUrlAvailable(timeoutInMs);
        }
    }
}
