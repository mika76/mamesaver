using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Mamesaver.Annotations;
using Mamesaver.Config.Models;
using Mamesaver.Config.ViewModels;
using Mamesaver.Models.Configuration;
using Prism.Commands;

namespace Mamesaver.Config.Filters
{
    public class MultipleChoiceFilterViewModel : INotifyPropertyChanged
    {
        private readonly ConfigFormViewModel _configFormViewModel;

        public event EventHandler SelectionChanged;

        public MultipleChoiceFilterViewModel(ConfigFormViewModel configFormViewModel)
        {
            _configFormViewModel = configFormViewModel;
            _configFormViewModel.FiltersCleared += (sender, args) => SelectAll();
        }

        /// <summary>
        ///     Name of property in game view model which the filter is applied to.
        /// </summary>
        public string FilterProperty { get; set; }

        public ICommand SelectAllClick => new DelegateCommand(SelectAll);
        public ICommand SelectNoneClick => new DelegateCommand(SelectNone);

        public ObservableCollection<FilterItemViewModel> SelectableValues { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private void SelectAll()
        {
            foreach (var filterItem in SelectableValues)
            {
                filterItem.Selected = true;
            }

            SelectionChanged?.Invoke(this, new EventArgs());
        }

        private void SelectNone()
        {
            foreach (var filterItem in SelectableValues)
            {
                filterItem.Selected = false;
            }

            SelectionChanged?.Invoke(this, new EventArgs());
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Initialise()
        {
            SelectableValues = new ObservableCollection<FilterItemViewModel>();

            // Update filter values when game list is rebuilt
            _configFormViewModel.GameListRebuilt += (sender, args) => BuildFilterValues();
        }

        public void BuildFilterValues()
        {
            if (string.IsNullOrEmpty(FilterProperty)) return;

            SelectableValues.Clear();
            SelectableValues.AddRange(_configFormViewModel.FilteredGames.Select(
                game => (string) game
                    .GetType()
                    .GetProperty(FilterProperty)?.GetValue(game)
            ).Distinct().OrderBy(g => g).Select(g => new FilterItemViewModel { Selected = true, Value = g }));

            // Inform listeners of selection changes
            // FIXME does this work? Is it needed?
            //foreach (var value in SelectableValues)
            //{
            //    value.PropertyChanged += (sender, args) =>
            //    {
            //        if (args.PropertyName == nameof(FilterItemViewModel.Selected)) SelectionChanged?.Invoke(this, new EventArgs());
            //    };
            //}

            OnPropertyChanged(nameof(SelectableValues));
        }
    }

    public class FilterItemViewModel : INotifyPropertyChanged
    {
        private bool _selected;

        public string Value { get; set; }

        public bool Selected
        {
            get => _selected;
            set
            {
                if (value == _selected) return;
                _selected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}