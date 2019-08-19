using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Pineapple.Extensions
{
    public static class ImmutableExtensions
    {
        public static IImmutableList<T> Append<T>(this IImmutableList<T> list, T appendThis) where T : class
        {
            var b = ImmutableList.CreateBuilder<T>();

            if (list != null)
            {
                b.AddRange(list);
            }

            b.Add(appendThis);

            return b.ToImmutable();
        }

        public static IImmutableList<T> Append<T>(this IImmutableList<T> list, IEnumerable<T> appendThis) where T : class
        {
            var b = ImmutableList.CreateBuilder<T>();

            if (list != null)
            {
                b.AddRange(list);
            }

            b.AddRange(appendThis);

            return b.ToImmutable();
        }

        public static IImmutableDictionary<TKey, TValue> Append<TKey, TValue>(this IImmutableDictionary<TKey, TValue> dict, TKey key, TValue appendThis)
                where TKey : class
                where TValue : class
        {

            if (dict != null && dict.ContainsKey(key))
                throw new ArgumentException("An element with the same key already exists.");

            var b = ImmutableDictionary.CreateBuilder<TKey, TValue>();

            if (dict != null)
            {
                b.AddRange(dict);
            }

            b.Add(key, appendThis);

            return b.ToImmutable();
        }
   }
}
