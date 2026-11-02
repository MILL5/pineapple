using System;
using System.Text.Json;
using HashCode = Pineapple.Common.HashCode;

object[] values = { null, "", "Hello", new string('x', 257), int.MinValue, uint.MaxValue,
    long.MinValue, ulong.MaxValue, short.MinValue, ushort.MaxValue, byte.MaxValue,
    sbyte.MinValue, 1.25f, -123.5d, 123.456m, new DateTime(2020, 1, 2), 'Ω', true, false };
if (!BitConverter.IsLittleEndian) throw new PlatformNotSupportedException("Persisted vectors require little-endian encoding.");
var results = new int[values.Length + 4];
var hash = new HashCode();
results[values.Length] = hash.ToHashCode();
for (int i = 0; i < values.Length; i++)
{
    results[i] = HashCode.ToHashCode(values[i]);
    hash.Add(values[i]);
}
results[values.Length + 1] = hash.ToHashCode();
results[values.Length + 2] = hash.ToHashCode();
hash.Add("tail");
results[values.Length + 3] = hash.ToHashCode();
Console.WriteLine(JsonSerializer.Serialize(results));
