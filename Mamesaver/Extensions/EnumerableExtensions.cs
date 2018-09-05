using System.Linq;

namespace Mamesaver.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        ///     Determines if a number matches a collection of other numbers
        /// </summary>
        public static bool In<T>(this T needle, params T[] haystack) => haystack.Contains(needle);
    }
}
