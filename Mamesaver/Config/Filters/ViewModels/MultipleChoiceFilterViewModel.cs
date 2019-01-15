using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Mamesaver.Config.Models;
using Mamesaver.Config.ViewModels;
using Mamesaver.Config.ViewModels.GameListTab;
using Prism.Commands;

namespace Mamesaver.Config.Filters.ViewModels
{
    /// <summary>
    ///     View model for column filters, displaying a checkbox list of filter options based on a single column. 
    /// </summary>
    public class MultipleChoiceFilterViewModel : InitialisableViewModel
    {
        private readonly GameListViewModel _gameList;
        private bool _visible = true;

        public ICommand SelectionChangedClick => new DelegateCommand(OnSelectionChanged);

        public static readonly SolidColorBrush ActiveBrush = SystemColors.HighlightBrush;
        public static readonly SolidColorBrush InactiveBrush = new SolidColorBrush(Colors.Gray);

        private bool _activeFilterMarkerVisible;
        private Brush _iconBrush;

        /// <summary>
        ///     Colour of the Select All / Select None links, depending on whether filter values are present.
        /// </summary>
        public Brush BulkFilterSelectColour => FilteredSelectableValues.Any()
            ? SystemColors.MenuHighlightBrush
            : SystemColors.ControlDarkBrush;

        public MultipleChoiceFilterViewModel(GameListViewModel gameList)
        {
            _gameList = gameList;
        }

        protected override void PerformInitialise()
        {
            _selectableValues = new List<FilterItemViewModel>();

            _gameList.GamesListRebuilt += (sender, args) =>
            {
                LastFilterField = null;
                RebuildFilterOptions();
            };

            // Update filter values for all filters if any filter has changed
            _gameList.FilterChanged += (sender, args) => RebuildFilterOptions();
            _gameList.FiltersCleared += (sender, args) =>
            {
                LastFilterField = null;

                // Reset any filtered state
                FilteredSelectableValues = SelectableValues;
                SelectAll();
            };

            SetIconState();
        }

        /// <summary>
        ///     Sets the filter icon colour and marker, based on whether any filtering is applied.
        /// </summary>
        public void SetIconState()
        {
            if (SelectableValues.All(value => value.Selected))
            {
                ActiveFilterMarkerVisible = false;
                IconBrush = InactiveBrush;
            }
            else
            {
                ActiveFilterMarkerVisible = true;
                IconBrush = ActiveBrush;
            }
        }

        public bool ActiveFilterMarkerVisible
        {
            get => _activeFilterMarkerVisible;
            set
            {
                if (value == _activeFilterMarkerVisible) return;

                _activeFilterMarkerVisible = value;
                OnPropertyChanged();
            }
        }

        private string _filterProperty;

        /// <summary>
        ///     Name of property in game view model which the filter is applied to.
        /// </summary>
        public string FilterProperty
        {
            get => _filterProperty;
            set
            {
                _filterProperty = value;
                CreateGamePropertyAccessor();
                RebuildFilterOptions();
            }
        }

        /// <summary>
        ///     Most recently-applied filter property
        /// </summary>
        private static string LastFilterField { get; set; }

        public ICommand SelectAllClick => new DelegateCommand(SelectAll);
        public ICommand SelectNoneClick => new DelegateCommand(SelectNone);

        public bool HasFilterableValues => FilteredSelectableValues?.Any() ?? false;

        public List<FilterItemViewModel> FilteredSelectableValues
        {
            get => _filteredSelectableValues;
            set
            {
                if (_filteredSelectableValues == value) return;
                _filteredSelectableValues = value;

                OnPropertyChanged(nameof(BulkFilterSelectColour));
                OnPropertyChanged(nameof(HasFilterableValues));
                OnPropertyChanged();
            }
        }

        public List<FilterItemViewModel> SelectableValues
        {
            get => _selectableValues;
            set
            {
                if (_selectableValues == value) return;
                _selectableValues = value;

                var filteredGameValues = _gameList.FilteredGames.Select(_gameValue);
                FilteredSelectableValues = _selectableValues.Where(item => filteredGameValues.Contains(item.Value)).ToList();

                OnPropertyChanged();
                OnPropertyChanged(nameof(BulkFilterSelectColour));
            }
        }

        public Brush IconBrush
        {
            get => _iconBrush;
            set
            {
                if (_iconBrush != null && _iconBrush.Equals(value)) return;
                _iconBrush = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Select all filter options for the associated column.
        /// </summary>
        private void SelectAll()
        {
            // Don't activate current filter if there are no items to update
            if (!FilteredSelectableValues.Any()) return;

            foreach (var filterItem in FilteredSelectableValues) filterItem.SetSelectionState(true);
            OnSelectionChanged();
        }

        /// <summary>
        ///     Select no filter options for the associated column.
        /// </summary>
        private void SelectNone()
        {
            // Don't activate current filter if there are no items to update
           if (!FilteredSelectableValues.Any()) return;

            foreach (var filterItem in FilteredSelectableValues) filterItem.SetSelectionState(false);
            OnSelectionChanged();
        }

        internal void OnSelectionChanged()
        {
            // Maintain last filter field so we can apply checkbox filtering in a similar fashion to Excel, keeping
            // deselected options visible for the last filter field.
            LastFilterField = FilterProperty;

            SetIconState();
            BuildFilter();
            _gameList.Filter();
        }

        private void BuildFilter()
        {
            _gameList.RegisterFilter(this, new MultipleChoiceContentFilter(FilterProperty, GetDeselectedValues()));
        }

        private List<string> GetDeselectedValues() => SelectableValues.Where(value => !value.Selected).Select(item => item.Value).ToList();

        private void RebuildFilterOptions()
        {
            // Don't filter last active filter list
            if (FilterProperty == LastFilterField) return;

            // Add filter values based on the selected property in the game view model, ordering alphabetically
            //var games = FilterProperty == LastFilterField ? _gameList.Games : _gameList.FilteredGames;

            SelectableValues = _gameList.Games.Select(_gameValue)
                .Distinct()
                .OrderBy(value => value)

                // Maintain previous selection
                .Select(value => new FilterItemViewModel(this, SelectableValues.FirstOrDefault(item => item.Value == value)?.Selected ?? true) { Value = value })
                .ToList();

            // Reconstruct filter
            BuildFilter();
        }

        private Func<GameViewModel, string> _gameValue;
        private List<FilterItemViewModel> _selectableValues, _filteredSelectableValues;

        private void CreateGamePropertyAccessor()
        {
            if (FilterProperty == null) throw new InvalidOperationException($"{nameof(FilterProperty)} must be set.");

            var method = typeof(GameViewModel).GetProperty(FilterProperty)?.GetGetMethod();
            _gameValue = (Func<GameViewModel, string>) Delegate.CreateDelegate(typeof(Func<GameViewModel, string>), method ??  throw new InvalidOperationException());
        }
    }
}