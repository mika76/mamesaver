using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DataGridExtensions;
using Mamesaver.Services;
using SystemColors = System.Windows.SystemColors;

namespace Mamesaver.Config.Filters
{
    public partial class MultipleChoiceFilter
    {
        private static readonly SolidColorBrush ActiveBrush = SystemColors.HighlightBrush;
        private static readonly SolidColorBrush InactiveBrush = new SolidColorBrush(Colors.Gray);

        private readonly MultipleChoiceFilterViewModel _viewModel;

        public MultipleChoiceFilter()
        {
            _viewModel = ServiceResolver.GetInstance<MultipleChoiceFilterViewModel>();

            InitializeComponent();
        }

        public override void BeginInit()
        {
            base.BeginInit();

            _viewModel.Initialise();
            DataContext = _viewModel;
        }

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
        public static readonly DependencyProperty FieldProperty = DependencyProperty.Register("Field", typeof(string), typeof(MultipleChoiceFilter));

        private ListView _listBox;
        private TextBlock _filterActiveMarker;
        private Control _filterSymbol;

        public MultipleChoiceContentFilter Filter
        {
            get => (MultipleChoiceContentFilter)GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }

        public string Field
        {
            get => (string)GetValue(FieldProperty);
            set => SetValue(FieldProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var dataContext = (MultipleChoiceFilterViewModel)DataContext;
            dataContext.FilterProperty = Field;
            dataContext.BuildFilterValues();

            _listBox = (ListView)Template.FindName("FilterList", this);
            _filterActiveMarker = (TextBlock)Template.FindName("IsFilterActiveMarker", this);
            _filterSymbol = (Control)Template.FindName("FilterSymbol", this);

            if (Filter?.ExcludedItems == null) _listBox?.SelectAll();

            if (!(_listBox?.Items is INotifyCollectionChanged items)) return;
            items.CollectionChanged += CollectionChanged;
            dataContext.SelectionChanged += ListBoxSelectionChanged;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => SetIconState();

        private void SetIconState()
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
                var checkedItems = _listBox.Items
                    .Cast<FilterItemViewModel>()
                    .Select(f => f.Value).Except(Filter.ExcludedItems)
                    .ToList();

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

        private void OnSelectionChanged()
        {
            var excludedItems = Filter?.ExcludedItems ?? new string[0];

            var selectedItems = _listBox.SelectedItems
                .Cast<FilterItemViewModel>()
                .Where(filter => filter.Selected)
                .Select(filter => filter.Value)
                .ToArray();

            var unselectedItems = _listBox.Items
                .Cast<FilterItemViewModel>()
                .Where(filter => !filter.Selected)
                .Select(filter => filter.Value)
                .Except(selectedItems)
                .ToArray();

            excludedItems = excludedItems.Except(selectedItems).Concat(unselectedItems).Distinct().ToArray();

            Filter = new MultipleChoiceContentFilter(excludedItems);
            SetIconState();
        }

        private void FilterItemSelectionChanged(object sender, RoutedEventArgs e) => OnSelectionChanged();
        private void ListBoxSelectionChanged(object sender, EventArgs e) => OnSelectionChanged();
    }

    public class MultipleChoiceContentFilter : IContentFilter
    {
        // FIXME do we need this? We have a selection checkbox
        public IList<string> ExcludedItems { get; }

        public MultipleChoiceContentFilter(IEnumerable<string> excludedItems) => ExcludedItems = excludedItems?.ToArray();

        public bool IsMatch(object rawValue)
        {
            if (!(rawValue is string)) return false;

            // FIXME filter icon not changing

            // TODO comment and tidy plz
            var value = ((string)rawValue).Split(':').FirstOrDefault();
            return ExcludedItems?.Contains(value) != true;
        }
    }
}