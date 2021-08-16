using System;
using System.Collections.Generic;
using System.Linq;

namespace Lorem.Test.Framework.Optimizely.CMS.Utility
{
    public static class EnumerableExtensions
    {
        public static T PickRandom<T>(this IEnumerable<T> source)
        {
            return source.PickRandom(1).Single();
        }

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int min, int max)
        {
            Random random = new Random();
            int count = random.Next(min, max);

            return source.Shuffle().Take(count);
        }

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
        {
            return source.Shuffle().Take(count).ToList();
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(_ => Guid.NewGuid());
        }
    }
}
