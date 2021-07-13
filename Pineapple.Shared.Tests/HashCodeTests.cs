using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HashCode = Pineapple.Common.HashCode;
// ReSharper disable StringLiteralTypo
// ReSharper disable AssignmentIsFullyDiscarded
// ReSharper disable UnusedParameter.Local
// ReSharper disable VariableHidesOuterVariable

namespace Pineapple.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]

    public class HashCodeTests
    {
        [TestMethod]
        public void HashCodeConstructorTest()
        {
            _ = new HashCode();
        }

        private int GetHashCodeScenario1()
        {
            var v1 = "Hello";
            var v2 = int.MinValue;
            var v3 = long.MinValue;
            var v4 = byte.MinValue;
            var v5 = sbyte.MinValue;
            var v6 = short.MinValue;
            var v7 = float.MinValue;
            var v8 = double.MinValue;
            var v9 = decimal.MinValue;
            var v10 = uint.MinValue;
            var v11 = ulong.MinValue;
            var v12 = 'B';
            var v13 = DateTime.MinValue;
            var v14 = (int?)int.MinValue;
            var v15 = (long?)long.MinValue;
            var v16= (byte?)byte.MinValue;
            var v17 = (sbyte?)sbyte.MinValue;
            var v18 = (short?)short.MinValue;
            var v19 = (float?)float.MinValue;
            var v20 = (double?)double.MinValue;
            var v21 = (decimal?)decimal.MinValue;
            var v22 = (uint?)uint.MinValue;
            var v23 = (ulong?)ulong.MinValue;
            var v24 = (char?)'B';
            var v25 = (DateTime?)DateTime.MinValue;
            
            var hc = new HashCode();

            hc.Add(v1);
            hc.Add(v2);
            hc.Add(v3);
            hc.Add(v4);
            hc.Add(v5);
            hc.Add(v6);
            hc.Add(v7);
            hc.Add(v8);
            hc.Add(v9);
            hc.Add(v10);
            hc.Add(v11);
            hc.Add(v12);
            hc.Add(v14);
            hc.Add(v15);
            hc.Add(v16);
            hc.Add(v17);
            hc.Add(v18);
            hc.Add(v19);
            hc.Add(v20);
            hc.Add(v21);
            hc.Add(v22);
            hc.Add(v23);
            hc.Add(v24);
            hc.Add(v25);

            return hc.ToHashCode();
        }

        private int GetHashCodeScenario2()
        {
            var v1 = "Hello";
            var v2 = int.MinValue;
            var v3 = long.MinValue;
            var v4 = byte.MinValue;
            var v5 = sbyte.MinValue;
            var v6 = short.MinValue;
            var v7 = float.MinValue;

            return HashCode.Combine(v1, v2, v3, v4, v5, v6, v7);
        }

        private int GetHashCodeScenario3()
        {
            var v1 = "Hello";
            var v2 = int.MinValue;
            var v3 = long.MinValue;
            var v4 = byte.MinValue;
            var v5 = sbyte.MinValue;
            var v6 = short.MinValue;

            return HashCode.Combine(v1, v2, v3, v4, v5, v6);
        }

        private int GetHashCodeScenario4()
        {
            var v1 = "Hello";
            var v2 = int.MinValue;
            var v3 = long.MinValue;
            var v4 = byte.MinValue;
            var v5 = sbyte.MinValue;

            return HashCode.Combine(v1, v2, v3, v4, v5);
        }

        private int GetHashCodeScenario5()
        {
            var v1 = "Hello";
            var v2 = int.MinValue;
            var v3 = long.MinValue;
            var v4 = byte.MinValue;

            return HashCode.Combine(v1, v2, v3, v4);
        }

        private int GetHashCodeScenario6()
        {
            var v1 = "Hello";
            var v2 = int.MinValue;
            var v3 = long.MinValue;

            return HashCode.Combine(v1, v2, v3);
        }

        private int GetHashCodeScenario7()
        {
            var v1 = "Hello";
            var v2 = int.MinValue;

            return HashCode.Combine(v1, v2);
        }

        [TestMethod]
        public void HashCodeScenarioOneTest()
        {
            var v1 = GetHashCodeScenario1();
            var v2 = GetHashCodeScenario1();

            Assert.AreEqual(v1, v2, "Must be equal.");
            Assert.AreEqual(-1492178786, v1, "Must be repeatable across application lifetime.");

            Debug.WriteLine($@"Hashcode={v1}");
        }

        [TestMethod]
        public void HashCodeScenarioTwoTest()
        {
            var v1 = GetHashCodeScenario2();
            var v2 = GetHashCodeScenario2();

            Assert.AreEqual(v1, v2, "Must be equal.");
            Assert.AreEqual(-1487638711, v1, "Must be repeatable across application lifetime.");

            Debug.WriteLine($@"Hashcode={v1}");
        }

        [TestMethod]
        public void HashCodeScenarioThreeTest()
        {
            var v1 = GetHashCodeScenario3();
            var v2 = GetHashCodeScenario3();

            Assert.AreEqual(v1, v2, "Must be equal.");
            Assert.AreEqual(-1523258412, v1, "Must be repeatable across application lifetime.");

            Debug.WriteLine($@"Hashcode={v1}");
        }

        [TestMethod]
        public void HashCodeScenarioFourTest()
        {
            var v1 = GetHashCodeScenario4();
            var v2 = GetHashCodeScenario4();

            Assert.AreEqual(v1, v2, "Must be equal.");
            Assert.AreEqual(-1967585112, v1, "Must be repeatable across application lifetime.");

            Debug.WriteLine($@"Hashcode={v1}");
        }

        [TestMethod]
        public void HashCodeScenarioFiveTest()
        {
            var v1 = GetHashCodeScenario5();
            var v2 = GetHashCodeScenario5();

            Assert.AreEqual(v1, v2, "Must be equal.");
            Assert.AreEqual(2096229656, v1, "Must be repeatable across application lifetime.");

            Debug.WriteLine($@"Hashcode={v1}");
        }

        [TestMethod]
        public void HashCodeScenarioSixTest()
        {
            var v1 = GetHashCodeScenario6();
            var v2 = GetHashCodeScenario6();

            Assert.AreEqual(v1, v2, "Must be equal.");
            Assert.AreEqual(-1628016087, v1, "Must be repeatable across application lifetime.");

            Debug.WriteLine($@"Hashcode={v1}");
        }

        [TestMethod]
        public void HashCodeScenarioSevenTest()
        {
            var v1 = GetHashCodeScenario7();
            var v2 = GetHashCodeScenario7();

            Assert.AreEqual(v1, v2, "Must be equal.");
            Assert.AreEqual(-839096971, v1, "Must be repeatable across application lifetime.");

            Debug.WriteLine($@"Hashcode={v1}");
        }


        [TestMethod]
        public void HashCodeIntTest()
        {
            int i = 5;

            var hc1 = new HashCode();
            hc1.Add(i);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(i);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void HashCodeShortTest()
        {
            short i = 5;

            var hc1 = new HashCode();
            hc1.Add(i);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(i);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void HashCodeUShortTest()
        {
            ushort i = 5;

            var hc1 = new HashCode();
            hc1.Add(i);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(i);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void HashCodeByteTest()
        {
            byte i = 5;

            var hc1 = new HashCode();
            hc1.Add(i);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(i);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void HashCodeSByteTest()
        {
            sbyte i = -5;

            var hc1 = new HashCode();
            hc1.Add(i);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(i);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void HashCodeLongTest()
        {
            long i = 5;

            var hc1 = new HashCode();
            hc1.Add(i);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(i);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void HashCodeFloatTest()
        {
            float i = 5.0F;

            var hc1 = new HashCode();
            hc1.Add(i);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(i);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void HashCodeDoubleTest()
        {
            double i = 5.0;

            var hc1 = new HashCode();
            hc1.Add(i);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(i);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void HashCodeDecimalTest()
        {
            decimal i = 5000.0M;

            var hc1 = new HashCode();
            hc1.Add(i);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(i);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void HashCodeStringTest()
        {
            string s = "Hello";

            var hc1 = new HashCode();
            hc1.Add(s);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(s);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void HashCodeBoolTest()
        {
            bool b = true;

            var hc1 = new HashCode();
            hc1.Add(b);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(b);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void HashCodeNullableIntTest()
        {
            int? i = 5;

            var hc1 = new HashCode();
            hc1.Add(i);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(i);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void HashCodeNullableShortTest()
        {
            short? i = 5;

            var hc1 = new HashCode();
            hc1.Add(i);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(i);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void HashCodeNullableUShortTest()
        {
            ushort? i = 5;

            var hc1 = new HashCode();
            hc1.Add(i);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(i);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void HashCodeNullableByteTest()
        {
            byte? i = 5;

            var hc1 = new HashCode();
            hc1.Add(i);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(i);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void HashCodeNullableSByteTest()
        {
            sbyte? i = -5;

            var hc1 = new HashCode();
            hc1.Add(i);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(i);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void HashCodeNullableLongTest()
        {
            long? i = 5;

            var hc1 = new HashCode();
            hc1.Add(i);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(i);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void HashCodeNullableFloatTest()
        {
            float? i = 5.0F;

            var hc1 = new HashCode();
            hc1.Add(i);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(i);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void HashCodeNullableDoubleTest()
        {
            double? i = 5.0;

            var hc1 = new HashCode();
            hc1.Add(i);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(i);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void HashCodeNullableDecimalTest()
        {
            decimal? i = 5000.0M;

            var hc1 = new HashCode();
            hc1.Add(i);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(i);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void HashCodeNullOneTest()
        {
            string s = null;

            var hc1 = new HashCode();
            hc1.Add(s);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(s);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void HashCodeNullTwoTest()
        {
            int? i = null;

            var hc1 = new HashCode();
            hc1.Add(i);
            int v1 = hc1.ToHashCode();

            var v2 = HashCode.ToHashCode(i);

            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void HashCodeEqualsTest()
        {
            var hc1 = new HashCode();
            var hc2 = new HashCode();

            hc1.Equals(hc2);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void HashCodeGetHashCodeTest()
        {
            var hc1 = new HashCode();

            hc1.GetHashCode();
        }
    }
}