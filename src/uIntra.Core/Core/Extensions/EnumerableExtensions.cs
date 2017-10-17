﻿using System.Collections.Generic;
using System.Linq;

namespace uIntra.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ToEnumerableOfOne<T>(this T obj)
        {
            if (obj != null) yield return obj;
        }

        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.Any();
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int divider = 2)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / divider)
                .Select(x => x.Select(v => v.Value));
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T item) =>
            source.Concat(item.ToEnumerableOfOne());
    }
}
