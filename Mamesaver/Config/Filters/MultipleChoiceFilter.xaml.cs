using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Mamesaver.Config.Extensions;
using Mamesaver.Config.Filters.ViewModels;

namespace Mamesaver.Config.Filters
{
    public partial class MultipleChoiceFilter
    {
        public MultipleChoiceFilter()
        {
            InitializeComponent();
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

            if (Filter?.ExcludedItems == null) _listBox?.SelectAll();

            // Handle select all / select none actions
            ViewModel.SelectionChanged += ListBoxSelectionChanged;
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
            var selected = ViewModel.SelectableValues.Where(s => !s.Selected);

            Filter = new MultipleChoiceContentFilter(selected.Select(item => item.Value));
            ViewModel.SetIconState();

            // Maintain last filter field so we can apply checkbox filtering in a similar fashion to Excel, keeping
            // deselected options visible for the last filter field.
            MultipleChoiceFilterViewModel.LastFilterField = Field;
        }

        private void FilterItemSelectionChanged(object sender, RoutedEventArgs e) => OnSelectionChanged();
        private void ListBoxSelectionChanged(object sender, EventArgs e) => OnSelectionChanged();
    }
}