using System;
using System.Collections.Generic;
using System.Linq;

namespace Mamesaver.Extensions
{
    public static class EnumerableExtensions
    {
        public static Random Random = new Random();

        /// <summary>
        ///     Determines if a number matches a collection of other numbers
        /// </summary>
        public static bool In<T>(this T needle, params T[] haystack) => haystack.Contains(needle);

        /// <summary>
        ///     Shuffles a list using the Fisher-Yates algorithm
        /// </summary>
        public static List<T> Shuffle<T>(this List<T> list)
        {
            for (var i = 0; i < list.Count; i++) list.Swap(i, Random.Next(i, list.Count));
            return list;
        }

        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
