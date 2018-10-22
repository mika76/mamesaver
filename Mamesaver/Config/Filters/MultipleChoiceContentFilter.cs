using System.Collections.Generic;
using System.Linq;
using DataGridExtensions;
using Mamesaver.Config.Helpers;

namespace Mamesaver.Config.Filters
{
    public class MultipleChoiceContentFilter : IContentFilter
    {
        public IList<string> ExcludedItems { get; set; }

        public MultipleChoiceContentFilter(IEnumerable<string> excludedItems) => ExcludedItems = excludedItems?.ToArray();

        public bool IsMatch(object rawValue)
        {
            if (!(rawValue is string)) return false;

            var value = SortablePropertyHelper.GetPrimarySort((string) rawValue);
            return ExcludedItems?.Contains(value) != true;
        }
    }
}