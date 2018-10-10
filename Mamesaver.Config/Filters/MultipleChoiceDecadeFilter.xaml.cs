using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DataGridExtensions;
using Mamesaver.Config.Filters.ViewModels;
using Mamesaver.Services;

namespace Mamesaver.Config.Filters
{
    // FIXME lots copied between here and the regular filter
    // FIXME decade parsing is awful
    public partial class MultipleChoiceDecadeFilter
    {
        private static readonly SolidColorBrush ActiveBrush = SystemColors.HighlightBrush;
        private static readonly SolidColorBrush InactiveBrush = new SolidColorBrush(Colors.Gray);

        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(MultipleChoiceDecadeContentFilter), typeof(MultipleChoiceDecadeFilter),
                new FrameworkPropertyMetadata(new MultipleChoiceDecadeContentFilter(null),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (sender, e) => ((MultipleChoiceDecadeFilter)sender).FilterChanged()));

        private ListView _listBox;
        private TextBlock _filterActiveMarker;
        private Control _filterSymbol;


        public MultipleChoiceDecadeFilter()
        {
            InitializeComponent();

            // Resolve view model statically to fulfill no-arg constructor for controls
            DataContext = ServiceResolver.GetInstance<DecadeFilterViewModel>();
        }

        public MultipleChoiceDecadeContentFilter Filter
        {
            get => (MultipleChoiceDecadeContentFilter)GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _listBox = (ListView)Template.FindName("FilterList", this);
            _filterActiveMarker = (TextBlock)Template.FindName("IsFilterActiveMarker", this);
            _filterSymbol = (Control)Template.FindName("FilterSymbol", this);

            if (!(_listBox?.Items is INotifyCollectionChanged items)) return;
            items.CollectionChanged += CollectionChanged;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Filter?.ExcludedItems == null || (bool)!Filter.ExcludedItems?.Any())
            {
                _listBox.SelectAll();
            }
            else
            {
                var checkedItems = _listBox.Items.Cast<string>().Except(Filter.ExcludedItems).ToList();
                foreach (var item in checkedItems) _listBox.SelectedItems.Add(item);
            }

            UpdateGlobalState();
        }

        private void UpdateGlobalState()
        {
            Brush iconBrush;

            if (Filter?.ExcludedItems == null || (bool)!Filter.ExcludedItems?.Any())
            {
                _filterActiveMarker.Visibility = Visibility.Hidden;
                iconBrush = InactiveBrush;
            }
            else
            {
                _filterActiveMarker.Visibility = Visibility.Visible;
                iconBrush = ActiveBrush;
            }

            _filterActiveMarker.Foreground = iconBrush;
            _filterSymbol.Foreground = iconBrush;
        }

        private void FilterChanged()
        {
            UpdateGlobalState();

            if (Filter?.ExcludedItems == null || !Filter.ExcludedItems.Any())
            {
                _listBox?.SelectAll();
                return;
            }

            foreach (var item in _listBox.Items.Cast<string>().Except(Filter.ExcludedItems))
            {
                _listBox.SelectedItems.Add(item);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var excludedItems = Filter?.ExcludedItems ?? new string[0];

            var selectedItems = _listBox.SelectedItems.Cast<string>().ToArray();
            var unselectedItems = _listBox.Items.Cast<string>().Except(selectedItems).ToArray();

            excludedItems = excludedItems.Except(selectedItems).Concat(unselectedItems).Distinct().ToArray();

            Filter = new MultipleChoiceDecadeContentFilter(excludedItems);
        }

        private void UnselectAllClick(object sender, RoutedEventArgs e) => _listBox.UnselectAll();
        private void SelectAllClick(object sender, RoutedEventArgs e) => _listBox.SelectAll();
    }

    public class MultipleChoiceDecadeContentFilter : IContentFilter
    {
        public MultipleChoiceDecadeContentFilter(IEnumerable<string> excludedItems) => ExcludedItems = excludedItems?.ToArray();

        public IList<string> ExcludedItems { get; }

        public bool IsMatch(object value)
        {
            if (value == null) return false;
            var year = (string) value;

            // Normalise unknown years within a known decade
            if (year.EndsWith("?")) year = $"{year.TrimEnd('?')}0";

            // Return generic category if the year can't be parsed
            if (!int.TryParse(year, out var parsedYear)) return !ExcludedItems.Contains("Other");

            // FIXME super mank
            var excludedDecades = ExcludedItems
                .Where(i => i != "Other")
                .Select(i => int.Parse(i.TrimEnd('s')));
            return !excludedDecades.Any(decade => parsedYear >= decade && parsedYear < decade + 10);
        }
    }
}