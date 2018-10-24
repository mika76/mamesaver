using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Mamesaver.Config.Extensions;
using Mamesaver.Config.Filters.ViewModels;
using Mamesaver.Config.ViewModels.GameListTab;
using Mamesaver.Services;

namespace Mamesaver.Config.Filters
{
    public partial class MultipleChoiceFilter
    {
        private static readonly SolidColorBrush ActiveBrush = SystemColors.HighlightBrush;
        private static readonly SolidColorBrush InactiveBrush = new SolidColorBrush(Colors.Gray);

        public MultipleChoiceFilter()
        {
            InitializeComponent();
        }

        public override void BeginInit()
        {
            base.BeginInit();
            this.InitViewModel<MultipleChoiceFilterViewModel>();
        }

        /// <summary>
        ///     Registers the <c>Filter</c> dependency property to indicate the associated filter class.
        /// </summary>
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(MultipleChoiceContentFilter), typeof(MultipleChoiceFilter),
                new FrameworkPropertyMetadata(new MultipleChoiceContentFilter(null),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (sender, e) => ((MultipleChoiceFilter)sender).FilterChanged()));

        /// <summary>
        ///     Registers the <c>Field</c> dependency property to indicate which field in <see cref="Models.GameViewModel"/> 
        ///     is being filtered on.
        /// </summary>
        public static readonly DependencyProperty FieldProperty = 
            DependencyProperty.Register("Field", typeof(string), typeof(MultipleChoiceFilter));

        /// <summary>
        ///     Registers the <c>Visible</c> dependency property to indicate whether the filter should be displayed.
        /// </summary>
        public static readonly DependencyProperty VisibleProperty = 
            DependencyProperty.Register("Visible", typeof(bool?), typeof(MultipleChoiceFilter));

        private ListView _listBox;
        private TextBlock _filterActiveMarker;
        private Control _filterSymbol;

        private MultipleChoiceFilterViewModel ViewModel => (MultipleChoiceFilterViewModel) DataContext;

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

        public bool Visible
        {
            get => (bool)(GetValue(VisibleProperty) ?? true);
            set => SetValue(VisibleProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ViewModel.FilterProperty = Field;
            ViewModel.Visible = Visible;

            ViewModel.BuildFilterValues();

            _listBox = (ListView)Template.FindName("FilterList", this);
            _filterActiveMarker = (TextBlock)Template.FindName("IsFilterActiveMarker", this);
            _filterSymbol = (Control)Template.FindName("FilterSymbol", this);

            if (Filter?.ExcludedItems == null) _listBox?.SelectAll();

            if (!(_listBox?.Items is INotifyCollectionChanged items)) return;
            items.CollectionChanged += CollectionChanged;

            // Handle select all / select none actions
            ViewModel.SelectionChanged += ListBoxSelectionChanged;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => SetIconState();

        /// <summary>
        ///     Sets the filter icon colour and marker, based on whether any filtering is applied.
        /// </summary>
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

            foreach (var item in _listBox.Items.Cast<FilterItemViewModel>().Select(item => item.Value).Except(Filter.ExcludedItems))
            {
                _listBox.SelectedItems.Add(item);
            }
        }

        /// <summary>
        ///     Reconstructs filter based on filter item selection change.
        /// </summary>
        public void OnSelectionChanged()
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

            // Maintain last filter field so we can apply checkbox filtering in a similar fashion to Excel, keeping
            // deselected options visible for the last filter field.
            MultipleChoiceFilterViewModel.LastFilterField = Field;
        }

        private void FilterItemSelectionChanged(object sender, RoutedEventArgs e) => OnSelectionChanged();
        private void ListBoxSelectionChanged(object sender, EventArgs e) => OnSelectionChanged();
    }
}