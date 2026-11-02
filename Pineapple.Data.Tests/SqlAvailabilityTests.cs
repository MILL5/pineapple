using System.Net;
using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlCheck = Pineapple.Data.SqlClient.Check;

namespace Pineapple.Data.Tests;

[TestClass]
public class SqlAvailabilityTests
{
    [TestMethod]
    public void CheckReportsTcpReachabilityRatherThanAuthentication()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        Assert.IsTrue(SqlCheck.IsSqlServerAvailable($"Server=tcp:127.0.0.1,{port};Database=ignored"));
        listener.Stop();
        Assert.IsFalse(SqlCheck.IsSqlServerAvailable($"Server=127.0.0.1,{port};Database=ignored"));
    }
}
