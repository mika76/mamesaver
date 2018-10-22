using Mamesaver.Config.ViewModels;

namespace Mamesaver.Config.Filters.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    ///     View model for a single filter item in <see cref="T:Mamesaver.Config.Filters.ViewModels.MultipleChoiceFilterViewModel" />.
    /// </summary>
    public class FilterItemViewModel : ViewModel
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
    }
}