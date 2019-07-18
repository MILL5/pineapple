using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pineapple.Common;
// ReSharper disable StringLiteralTypo
// ReSharper disable AssignmentIsFullyDiscarded
// ReSharper disable UnusedParameter.Local
// ReSharper disable VariableHidesOuterVariable

namespace Pineapple.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]

    public class ShortGuidTests
    {
        [TestMethod]
        public void ShortGuidConstructorTest()
        {
            _ = new ShortGuid(Guid.Empty);
            _ = new ShortGuid(Guid.NewGuid());
            _ = new ShortGuid("7f95325d-dbeb-435f-bab3-df2e10da361b");
            _ = new ShortGuid("BwMDCgwDDwICCQYKBAcNCA");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShortGuidConstructorNotGuidTest()
        {
            _ = new ShortGuid("This Is Not A Guid");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShortGuidConstructorNullTest()
        {
            _ = new ShortGuid(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShortGuidConstructorEmptyTest()
        {
            _ = new ShortGuid(string.Empty);
        }

        [TestMethod]
        public void ShortGuidNewGuidTest()
        {
            _ = ShortGuid.NewGuid();
        }

        [TestMethod]
        public void ShortGuidEmptyGuidTest()
        {
            var sg = ShortGuid.Empty;

            Assert.AreEqual(sg, Guid.Empty);
            Assert.AreEqual(Guid.Empty, (Guid)sg);
        }

        [TestMethod]
        public void ShortGuidEqualsTest()
        {
            const string x = "fe66b08f-a1bd-4d40-9711-4e1469c5193a";
            const string y = "07bb23cd-7b24-42b4-8467-3f0be7fa9ea5";

            Guid xg = new Guid(x);
            var xsg = new ShortGuid(x);
            var xsg2 = new ShortGuid(x);

            Assert.IsTrue(xsg.Equals(xsg2));
            Assert.IsTrue(xsg.Equals(x));
            Assert.IsTrue(xsg.Equals(xg));
            Assert.IsTrue(xsg.Equals(xsg2));
            Assert.IsTrue(xsg.Equals(xsg2.ToString()));
            Assert.IsTrue(xsg.Equals(xsg2.Value));
            Assert.IsFalse(xsg.Equals($"bad@{xsg2.Value}@bad"));


            Guid yg = new Guid(y);
            var ysg = new ShortGuid(y);

            Assert.IsFalse(xsg.Equals(ysg));
            Assert.IsFalse(xsg.Equals(y));
            Assert.IsFalse(xsg.Equals(yg));
            Assert.IsFalse(xsg.Equals(ysg));
            Assert.IsFalse(xsg.Equals(ysg.ToString()));
            Assert.IsFalse(xsg.Equals(ysg.Value));
            Assert.IsFalse(xsg.Equals($"bad@{ysg.Value}@bad"));

            const string gs = "DwUBBA8PDAAPBAYFBAEEDw";

            var sg1 = new ShortGuid(gs);
            var sg2 = new ShortGuid(gs);

            Assert.IsTrue(sg1 == sg2);
            Assert.IsTrue(ShortGuid.Empty.Equals(new ShortGuid(Guid.Empty)));
        }

        [TestMethod]
        public void ShortGuidEncodeTest()
        {
            var g = Guid.NewGuid();

            var sg1 = ShortGuid.Encode(g);
            var sg2 = ShortGuid.Encode(g.ToString());

            Assert.AreEqual(sg1, sg2);
            Assert.AreEqual(sg2, sg1);
        }

        [TestMethod]
        public void ShortGuidGuidTest()
        {
            var g = Guid.NewGuid();
            var sg = new ShortGuid(g);

            Assert.AreEqual(sg, g, "ShortGuid is not equal to Guid [Constructor]");
            Assert.AreEqual(g, (Guid)sg, "ShortGuid is not equal to Guid [Constructor]");

            g = Guid.NewGuid();
            sg.Guid = g;

            Assert.AreEqual(sg, g, "ShortGuid is not equal to Guid [Guid Property]");
            Assert.AreEqual(g, (Guid)sg, "ShortGuid is not equal to Guid [Guid Property]");
        }

        [TestMethod]
        public void ShortGuidValueTest()
        {
            var g = Guid.NewGuid();
            var sg = new ShortGuid(g);
            var s = sg.Value;

            Assert.AreEqual(sg, g, "Guid is not equal to ShortGuid");
            Assert.AreEqual(sg, s, "String is not equal to ShortGuid");
            Assert.AreEqual(g, (Guid)sg, "Guid is not equal to ShortGuid");
            Assert.AreEqual(s, (string)sg, "String is not equal to ShortGuid");
        }

        [TestMethod]
        public void ShortGuidNotEqualsTest()
        {
            const string gs1 = "DwUBBA8PDAAPBAYFBAEEDw";
            const string gs2 = "BwYMDgAMBAAOBAUOBAsMBQ";

            var sg1 = new ShortGuid(gs1);
            var sg2 = new ShortGuid(gs2);

            Assert.IsTrue(sg1 != sg2, "NotEquals operator is not working");
            Assert.AreNotEqual(sg1, sg2, "Equals operator is not working");
            Assert.AreNotEqual(sg2, sg1, "Equals operator is not working");
        }

        [TestMethod]
        public void ShortGuidImplicitConversionTest()
        {
            void TestGuid(Guid guid)
            {
                // DO NOTHING
            }

            void TestString(string guid)
            {
                // DO NOTHING
            }

            void TestFromGuid(ShortGuid sg)
            {
                // DO NOTHING
            }

            void TestFromString(ShortGuid sg)
            {
                // DO NOTHING
            }

            const string gs = "DwUBBA8PDAAPBAYFBAEEDw";
            var sg = new ShortGuid(gs);

            TestGuid(sg);
            TestString(sg);

            const string gs1 = "fe66b08f-a1bd-4d40-9711-4e1469c5193a";
            var g = new Guid(gs1);

            TestFromGuid(g);
            TestFromString(gs1);
        }

        [TestMethod]
        public void ShortGuidDecodeTest()
        {
            const string gs = "DwUBBA8PDAAPBAYFBAEEDw";
            const string gs1 = "0401050f-0f0f-000c-0f04-06050401040f";

            var g1 = ShortGuid.Decode(gs);
            var g2 = ShortGuid.Decode(gs1);

            Assert.AreEqual(g1, g2);
            Assert.AreEqual(g2, g1);
        }

        [TestMethod]
        public void ShortGuidSetValueTest()
        {
            const string g1 = "DwUBBA8PDAAPBAYFBAEEDw";
            const string g2 = "0401050f-0f0f-000c-0f04-06050401040f";

            var sg = new ShortGuid(Guid.Empty);

            sg.Value = g1;

            Assert.AreEqual(sg, g1);

            sg.Value = g2;

            Assert.AreEqual(sg, g2);
        }
    }
}