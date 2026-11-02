using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using H = Pineapple.Common.HashCode;

namespace Pineapple.Tests
{
    /// <summary>
    /// Hash values recorded from Pineapple 1.x (K4os xxHash32). They must never change:
    /// callers persist these hashes, so any implementation must reproduce them exactly.
    /// </summary>
    [TestClass]
    public class HashCodeGoldenTests
    {
        private static readonly Dictionary<string, Func<int>> Cases = new()
        {
            ["int 0"] = () => H.ToHashCode(0),
            ["int -123456"] = () => H.ToHashCode(-123456),
            ["int max"] = () => H.ToHashCode(int.MaxValue),
            ["uint"] = () => H.ToHashCode(4000000000u),
            ["long"] = () => H.ToHashCode(-9876543210123L),
            ["ulong"] = () => H.ToHashCode(18000000000000000000UL),
            ["short"] = () => H.ToHashCode((short)-1234),
            ["ushort"] = () => H.ToHashCode((ushort)65000),
            ["byte"] = () => H.ToHashCode((byte)200),
            ["sbyte"] = () => H.ToHashCode((sbyte)-100),
            ["float"] = () => H.ToHashCode(3.14159f),
            ["double"] = () => H.ToHashCode(-2.718281828459045),
            ["decimal"] = () => H.ToHashCode(12345.6789m),
            ["char"] = () => H.ToHashCode('Z'),
            ["bool t"] = () => H.ToHashCode(true),
            ["bool f"] = () => H.ToHashCode(false),
            ["datetime"] = () => H.ToHashCode(new DateTime(2026, 10, 13, 12, 34, 56, DateTimeKind.Utc)),
            ["string"] = () => H.ToHashCode("Pineapple 🍍 ünïcode"),
            ["string empty"] = () => H.ToHashCode(""),
            ["string long"] = () => H.ToHashCode(new string('x', 5000)),
            ["null string"] = () => H.ToHashCode((string)null),
            ["int? null"] = () => H.ToHashCode((int?)null),
            ["int? 7"] = () => H.ToHashCode((int?)7),
            ["object int"] = () => H.ToHashCode((object)42),
            ["combine2"] = () => H.Combine("a", 1),
            ["combine3"] = () => H.Combine(1L, 2.5, 'c'),
            ["combine7"] = () => H.Combine(1, "two", 3L, 4.0f, 5.0m, true, (byte)7),
            ["instance"] = () => { var h = new H(); h.Add("x"); h.Add(1); h.Add((long?)null); h.Add(2.0); return h.ToHashCode(); },
        };

        public static IEnumerable<object[]> Expected => new (string Name, int Hash)[]
        {
            ("int 0", 148298089),
            ("int -123456", -1558154718),
            ("int max", 1225377669),
            ("uint", 2002079553),
            ("long", 127109327),
            ("ulong", 723649674),
            ("short", -500241661),
            ("ushort", 1825458531),
            ("byte", -1438693211),
            ("sbyte", -1139131728),
            ("float", 1920994726),
            ("double", -83226389),
            ("decimal", -2007615689),
            ("char", -2112525779),
            ("bool t", 949155633),
            ("bool f", -815419330),
            ("datetime", -259300832),
            ("string", -734251931),
            ("string empty", 46947589),
            ("string long", -1702029289),
            ("null string", 148298089),
            ("int? null", 148298089),
            ("int? 7", -427046664),
            ("object int", 1161967057),
            ("combine2", -1555174595),
            ("combine3", -1169964092),
            ("combine7", 211093661),
            ("instance", 1938616560)
        }.Select(x => new object[] { x.Name, x.Hash });

        [TestMethod]
        [DynamicData(nameof(Expected))]
        public void HashIsStableAcrossVersions(string name, int expected)
        {
            Assert.AreEqual(expected, Cases[name](), name);
        }
    }
}
