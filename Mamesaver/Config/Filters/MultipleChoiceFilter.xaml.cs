using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DataGridExtensions;
using SystemColors = System.Windows.SystemColors;

namespace Mamesaver.Config.Filters
{
    // FIXME lots copied between here and the decade filter
    public partial class MultipleChoiceFilter
    {
        private static readonly SolidColorBrush ActiveBrush = SystemColors.HighlightBrush;
        private static readonly SolidColorBrush InactiveBrush = new SolidColorBrush(Colors.Gray);

        /// <summary>
        ///     Identifies the <c>Filter</c> dependency property
        /// </summary>
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(MultipleChoiceContentFilter), typeof(MultipleChoiceFilter),
                new FrameworkPropertyMetadata(new MultipleChoiceContentFilter(null),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (sender, e) => ((MultipleChoiceFilter)sender).FilterChanged()));

        /// <summary>
        ///     Identifies the <c>Field</c> dependency property
        /// </summary>
        public static readonly DependencyProperty FieldProperty =
            DependencyProperty.Register("Field", typeof(MultipleChoiceContentFilter), typeof(MultipleChoiceFilter),
                new FrameworkPropertyMetadata(new MultipleChoiceContentFilter(null)));

        private ListView _listBox;
        private TextBlock _filterActiveMarker;
        private Control _filterSymbol;

        public MultipleChoiceFilter()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Field in the backing model that the filter sources data from.
        /// </summary>
        public string Field
        {
            get => (string)GetValue(FieldProperty);
            set => SetValue(FieldProperty, value);
        }

        public MultipleChoiceContentFilter Filter
        {
            get => (MultipleChoiceContentFilter)GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _listBox = (ListView)Template.FindName("FilterList", this);
            _filterActiveMarker = (TextBlock)Template.FindName("IsFilterActiveMarker", this);
            _filterSymbol = (Control)Template.FindName("FilterSymbol", this);

            if (Filter?.ExcludedItems == null) _listBox?.SelectAll();

            if (!(_listBox?.Items is INotifyCollectionChanged items)) return;
            items.CollectionChanged += CollectionChanged;

            // FIXME need to sort!
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Brush iconBrush;
            if (Filter?.ExcludedItems == null || (bool)!Filter.ExcludedItems?.Any())
            {
                _listBox.SelectAll();
                _filterActiveMarker.Visibility = Visibility.Hidden;

                iconBrush = InactiveBrush;
            }
            else
            {
                var checkedItems = _listBox.Items.Cast<string>().Except(Filter.ExcludedItems).ToList();
                foreach (var item in checkedItems) _listBox.SelectedItems.Add(item);

                _filterActiveMarker.Visibility = Visibility.Visible;

                iconBrush = ActiveBrush;
            }

            _filterActiveMarker.Foreground = iconBrush;
            _filterSymbol.Foreground = iconBrush;
        }

        private void FilterChanged()
        {
            if (Filter?.ExcludedItems == null)
            {
                _listBox?.SelectAll();
                return;
            }

            if (_listBox?.SelectedItems.Count != 0) return;

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

            Filter = new MultipleChoiceContentFilter(excludedItems);
        }

        private void UnselectAllClick(object sender, RoutedEventArgs e) => _listBox.UnselectAll();
        private void SelectAllClick(object sender, RoutedEventArgs e) => _listBox.SelectAll();
    }

    public class MultipleChoiceContentFilter : IContentFilter
    {
        public MultipleChoiceContentFilter(IEnumerable<string> excludedItems) => ExcludedItems = excludedItems?.ToArray();

        public IList<string> ExcludedItems { get; }

        public bool IsMatch(object value) => ExcludedItems?.Contains(value as string) != true;
    }
}