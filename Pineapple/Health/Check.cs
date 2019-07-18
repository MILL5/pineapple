using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using static Pineapple.Common.Preconditions;
using static Pineapple.Common.Cleanup;
// ReSharper disable UnusedMember.Global

namespace Pineapple.Health
{
    public static class Check
    {
        public static bool IsRunning(string processName)
        {
            CheckIsNotNullOrWhitespace(nameof(processName), processName);

            var isRunning = false;
            var procs = Process.GetProcesses();

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
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = timeoutInMs;
                request.Method = WebRequestMethods.Http.Get;
                using (var response = (HttpWebResponse)request.GetResponse())
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
