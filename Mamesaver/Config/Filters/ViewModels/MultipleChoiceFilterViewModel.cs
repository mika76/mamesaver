using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Mamesaver.Config.ViewModels;
using Mamesaver.Config.ViewModels.GameListTab;
using Prism.Commands;

namespace Mamesaver.Config.Filters.ViewModels
{
    /// <summary>
    ///     View model for column filters, displaying a checkbo list of filter options based on a single column.
    /// </summary>
    public class MultipleChoiceFilterViewModel : InitialisableViewModel
    {
        private readonly GameListViewModel _gameList;
        private bool _visible = true;

        public event EventHandler SelectionChanged;

        public MultipleChoiceFilterViewModel(GameListViewModel gameList)
        {
            _gameList = gameList;
        }

        protected override void PerformInitialise()
        {
            _gameList.FiltersCleared += (sender, args) => SelectAll();
            SelectableValues = new ObservableCollection<FilterItemViewModel>();

            // Update filter values when game list is rebuilt
            _gameList.GameListRebuilt += (sender, args) => BuildFilterValues();
        }

        /// <summary>
        ///     Name of property in game view model which the filter is applied to.
        /// </summary>
        public string FilterProperty { get; set; }

        public ICommand SelectAllClick => new DelegateCommand(SelectAll);
        public ICommand SelectNoneClick => new DelegateCommand(SelectNone);

        public ObservableCollection<FilterItemViewModel> SelectableValues { get; set; }

        /// <summary>
        ///     Whether the filter controls are visible. This is used to perform explicit filtering via external components.
        /// </summary>
        public bool Visible
        {
            get => _visible;
            set
            {
                if (value == _visible) return;
                _visible = value;
                OnPropertyChanged();
            }
        }

        private void SelectAll()
        {
            foreach (var filterItem in SelectableValues) filterItem.Selected = true;
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SelectNone()
        {
            foreach (var filterItem in SelectableValues) filterItem.Selected = false;
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Select(string value, bool selected)
        {
            var selectableValue = SelectableValues.FirstOrDefault(v => v.Value == value);
            if (selectableValue == null) return;

            selectableValue.Selected = selected;
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public void BuildFilterValues()
        {
            if (string.IsNullOrEmpty(FilterProperty)) return;

            // Add filter values based on the selected property in the game view model, ordering alphabetically.
            SelectableValues.Clear();
            SelectableValues.AddRange(_gameList.FilteredGames
                .Select(
                    game => (string) game
                        .GetType()
                        .GetProperty(FilterProperty)?.GetValue(game)
                )
                .Distinct()
                .OrderBy(value => value)
                .Select(value => new FilterItemViewModel { Selected = true, Value = value }));

            OnPropertyChanged(nameof(SelectableValues));
        }
    }
}