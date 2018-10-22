using Mamesaver.Config.ViewModels;
using Mamesaver.Models;

using static Mamesaver.Config.Helpers.SortablePropertyHelper;

namespace Mamesaver.Config.Models
{
    /// <summary>
    ///     View model for a single game's metadata displayed in a <c>DataGrid</c>.
    /// </summary>
    public class GameViewModel : ViewModel
    {
        private readonly SelectableGame _game;

        public GameViewModel(SelectableGame game) => _game = game;

        public bool Selected
        {
            get => _game.Selected;
            set
            {
                if (value == _game.Selected) return;
                _game.Selected = value;
                OnPropertyChanged();
            }
        }

        public string Name => _game.Name;
        public string Description => _game.Description;
        public string Year => _game.Year;
        public string Manufacturer => _game.Manufacturer;

        public string Rotation
        {
            get
            {
                switch (_game.Rotation)
                {
                case "0":
                case "180":
                    return "Horizontal";
                case "90":
                case "270":
                    return "Vertical";
                    default:
                        return null;
                }
            }
        }
        public string Category => _game.Category;

        /// <summary>
        ///     Sort property for game manufacturer, performing a secondary sort on game description.
        /// </summary>
        public string ManufacturerSort => ToSortableProperty(Manufacturer, Description);

        /// <summary>
        ///     Sort property for game category, performing a secondary sort on game description.
        /// </summary>
        public string CategorySort => ToSortableProperty(Category, Description);

        /// <summary>
        ///     Sort property for game year, performing a secondary sort on game description.
        /// </summary>
        public string YearSort => ToSortableProperty(Year, Description);

        /// <summary>
        ///     Sort property for game rotation, performing a secondary sort on game description.
        /// </summary>
        public string RotationSort => ToSortableProperty(Rotation, Description);

        /// <summary>
        ///     Filter property for game selection.
        /// </summary>
        public string SelectedFilter => $"{Selected}";

        /// <summary>
        ///     Filter property for game year, converting year into decade ranges.
        /// </summary>
        public string YearFilter
        {
            get
            {
                if (Year == null) return "Other";
                var yearSort = Year;

                // Normalise unknown years within a known decade
                if (yearSort.EndsWith("?")) yearSort = $"{yearSort.TrimEnd('?')}0";

                return !int.TryParse(yearSort, out var yearValue) ? "Other" : $"{yearValue / 10 * 10}s";
            }
        }
   }
}