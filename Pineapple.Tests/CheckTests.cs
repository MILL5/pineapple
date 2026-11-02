using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pineapple.Health;

namespace Pineapple.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class CheckTests
    {
        [TestMethod]
        public void CheckIsAliveTest()
        {
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            Assert.IsTrue(Check.IsAlive("127.0.0.1", ((IPEndPoint)listener.LocalEndpoint).Port));
        }

        [TestMethod]
        public void CheckIsAliveFailTest()
        {
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            Assert.IsFalse(Check.IsAlive("127.0.0.1", port));
        }

        [TestMethod]
        public async Task CheckIsUrlAvailableTest()
        {
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var server = Task.Run(async () =>
            {
                using var connection = await listener.AcceptTcpClientAsync();
                using var stream = connection.GetStream();
                using var reader = new System.IO.StreamReader(stream, Encoding.ASCII, leaveOpen: true);
                while (!string.IsNullOrEmpty(await reader.ReadLineAsync())) { }
                await stream.WriteAsync(Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\nContent-Length: 0\r\nConnection: close\r\n\r\n"));
            });
            Assert.IsTrue(Check.IsUrlAvailable($"http://127.0.0.1:{((IPEndPoint)listener.LocalEndpoint).Port}", 10000));
            await server;
        }

        [TestMethod]
        public void CheckIsUrlAvailableFailTest()
        {
            Assert.IsFalse(Check.IsUrlAvailable("http://127.0.0.1:0", 1000));
        }

        [TestMethod]
        [Ignore("Requires Visual Studio (devenv) to be running.")]
        public void CheckIsRunningVisualStudioTest()
        {
            Assert.IsTrue(Check.IsRunning("devenv"));
        }

        [TestMethod]
        public void CheckIsRunningVisualStudioBadNameTest()
        {
            Assert.IsFalse(Check.IsRunning("devenv.exe"));
        }

        [TestMethod]
        public void CheckIsRunningTestConsoleTest()
        {
            // Microsoft.Testing.Platform runs tests in-process, so look for this process.
            Assert.IsTrue(Check.IsRunning(System.Diagnostics.Process.GetCurrentProcess().ProcessName));
        }

        [TestMethod]
        public void CheckIsRunningFailTest()
        {
            Assert.IsFalse(Check.IsRunning("not-a-real-process"));
        }
    }
}
