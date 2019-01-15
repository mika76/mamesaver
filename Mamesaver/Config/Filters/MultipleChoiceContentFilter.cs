using System.Collections.Generic;
using System.Linq;
using Mamesaver.Config.Helpers;
using Mamesaver.Config.Models;

namespace Mamesaver.Config.Filters
{
    public class MultipleChoiceContentFilter  : IGameFilter
    {
        private readonly string _filterProperty;
        public IList<string> ExcludedItems { get; set; }

        public MultipleChoiceContentFilter(string filterProperty, IEnumerable<string> excludedItems)
        {
            _filterProperty = filterProperty;
            ExcludedItems = excludedItems?.ToArray();
        }

        public bool IsMatch(GameViewModel game)
        {
            var rawValue = typeof(GameViewModel)
                .GetProperty(_filterProperty)?.GetValue(game);

            var value = SortablePropertyHelper.GetPrimarySort((string) rawValue);
            return ExcludedItems?.Contains(value) != true;
        }
    }
}