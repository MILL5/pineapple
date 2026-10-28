using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pineapple.Common;

namespace Pineapple.Tests
{
    [TestClass]
    public class ShortGuidEncodingTests
    {
        // The 1.x encoding: Base64 of Guid.ToByteArray(), '/' -> '_', '+' -> '-', padding removed.
        private static string LegacyEncode(Guid guid) =>
            Convert.ToBase64String(guid.ToByteArray()).Replace("/", "_").Replace("+", "-").Substring(0, 22);

        [TestMethod]
        public void EncodingMatchesVersion1()
        {
            var random = new Random(7);
            var bytes = new byte[16];

            for (var i = 0; i < 10_000; i++)
            {
                random.NextBytes(bytes);
                var guid = new Guid(bytes);
                var legacy = LegacyEncode(guid);

                Assert.AreEqual(legacy, ShortGuid.Encode(guid));
                Assert.AreEqual(guid, ShortGuid.Decode(legacy));
                Assert.IsTrue(ShortGuid.TryParse(legacy, out var parsed));
                Assert.AreEqual(guid, parsed);
            }
        }

        [TestMethod]
        public void NonCanonicalAndStandardBase64InputDecodesLikeVersion1()
        {
            static Guid LegacyDecode(string value) =>
                new(Convert.FromBase64String(value.Replace("_", "/").Replace("-", "+") + "=="));

            foreach (var value in new[] { "WorkNotificationOpened", "Abcdefghijklmnopqrstuv", "ab+cd/ef-gh_ij0123456z" })
                Assert.AreEqual(LegacyDecode(value), ShortGuid.Decode(value), value);
        }

        [TestMethod]
        public void DefaultShortGuidIsEmpty()
        {
            ShortGuid value = default;

            Assert.AreEqual(Guid.Empty, value.Guid);
            Assert.AreEqual(ShortGuid.Empty.Value, value.Value);
            Assert.AreEqual(ShortGuid.Empty.Value, value.ToString());
            Assert.IsTrue(value == ShortGuid.Empty);
        }

        [TestMethod]
        public void InvalidShortGuidIsRejected()
        {
            Assert.IsFalse(ShortGuid.TryParse("!!!!!!!!!!!!!!!!!!!!!!", out _));
            Assert.ThrowsExactly<FormatException>(() => ShortGuid.Decode("!!!!!!!!!!!!!!!!!!!!!!"));
        }
    }
}
