using Mamesaver.Config.ViewModels;

namespace Mamesaver.Config.Filters.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    ///     View model for a single filter item in <see cref="T:Mamesaver.Config.Filters.ViewModels.MultipleChoiceFilterViewModel" />.
    /// </summary>
    public class FilterItemViewModel : ViewModel
    {
        private readonly MultipleChoiceFilterViewModel _filter;
        private bool _selected;

        public string Value { get; set; }

        public FilterItemViewModel(MultipleChoiceFilterViewModel filter, bool selected = true)
        {

            _filter = filter;
            _selected = selected;
        }

        /// <summary>
        ///     Sets selection state without firing events or notifying other components.
        /// </summary>
        public void SetSelectionState(bool selected)
        {
            _selected = selected;
            OnPropertyChanged(nameof(Selected));
        }

        public bool Selected
        {
            get => _selected;
            set
            {
                if (value == _selected) return;
                _selected = value;

                OnPropertyChanged();
                _filter.OnSelectionChanged();
            }
        }
    }
}