using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using static Pineapple.Common.Preconditions;

namespace Pineapple.Extensions
{
    public static class ImmutableExtensions
    {
        public static IImmutableList<T> Append<T>(this IImmutableList<T> list, T appendThis) where T : class
        {
            CheckIsNotNull(nameof(list), list);

            var b = ImmutableList.CreateBuilder<T>();

            b.AddRange(list);
            b.Add(appendThis);

            return b.ToImmutable();
        }

        public static IImmutableList<T> Append<T>(this IImmutableList<T> list, IEnumerable<T> appendThis) where T : class
        {
            CheckIsNotNull(nameof(list), list);

            var b = ImmutableList.CreateBuilder<T>();

            b.AddRange(list);
            b.AddRange(appendThis);

            return b.ToImmutable();
        }
    }
}
