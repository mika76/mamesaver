using System.Linq;

namespace Mamesaver.Config.Helpers
{
    public class SortablePropertyHelper
    {
        /// <summary>
        ///     Returns a string which can be used by <c>DataGrid</c> to sort a property with a primary and
        ///     secondary sort. The returned value should be used in <c>SortMemberPath</c>.
        /// </summary>
        public static string ToSortableProperty(string primary, string secondary) => $"{primary}:{secondary}";

        /// <summary>
        ///     Returns the primary property from a combined sort propert created by <see cref="ToSortableProperty"/>,
        ///     or <c>null</c> if not found.
        /// </summary>
        public static string GetPrimarySort(string sortableProperty) => sortableProperty.Split(':').FirstOrDefault();
    }
}
