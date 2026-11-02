using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pineapple.Common;

namespace Pineapple.Tests;

[TestClass]
public class ShortGuidCompatibilityTests
{
    [TestMethod]
    [DataRow("WorkNotificationOpened", "36e48a5a-628b-277e-1ab6-2a273a97a779")]
    [DataRow("Abcdefghijklmnopqrstuv", "791db701-21f8-398a-259a-7a29aabb2dba")]
    public void LegacyAliasesRetainTheirDecodedGuidAcrossRuntimes(string alias, string expected)
    {
        Assert.IsTrue(ShortGuid.TryParse(alias, out var parsed));
        Assert.AreEqual(Guid.Parse(expected), parsed);
        Assert.AreEqual(parsed, new ShortGuid(alias).Guid);
        Assert.AreEqual(parsed, ShortGuid.Decode(ShortGuid.Encode(parsed)));
    }
}
