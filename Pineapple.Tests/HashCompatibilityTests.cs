using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pineapple.Tests;

[TestClass]
public class HashCompatibilityTests
{
    [TestMethod]
    public async Task PersistedVectorsMatchInSeparateProcesses()
    {
        int[] expected = { 148298089, 46947589, -234433905, 1703021275, 1509677505, 67608159,
            483163611, -1048394023, 464368690, 1495824247, -1766008628, 767482102,
            1176165888, 1529585070, -942010315, 1721292338, -24103831, 949155633,
            -815419330, 46947589, -1016710255, -1016710255, 1038666854 };
        for (int attempt = 0; attempt < 2; attempt++)
        {
            var start = new ProcessStartInfo(Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet")
            { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
            start.ArgumentList.Add(Path.Combine(AppContext.BaseDirectory, "hash-probe", "Pineapple.HashCompatibilityProbe.dll"));
            start.Environment["DOTNET_ROLL_FORWARD"] = "LatestPatch";
            using var process = Process.Start(start);
            var output = process.StandardOutput.ReadToEndAsync();
            var errors = process.StandardError.ReadToEndAsync();
            try { await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(20)); }
            catch { if (!process.HasExited) process.Kill(entireProcessTree: true); throw; }
            Assert.AreEqual(0, process.ExitCode, await errors);
            CollectionAssert.AreEqual(expected, JsonSerializer.Deserialize<int[]>(await output));
        }
    }
}
