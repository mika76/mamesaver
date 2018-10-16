using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mamesaver.Annotations;
using Mamesaver.Models;

namespace Mamesaver.Config.Models
{
    public class GameViewModel  : INotifyPropertyChanged
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

        //public string ManufacturerSort => $"{Manufacturer}:{Description}";
        //public string CategorySort => $"{Category}:{Description}";
        //public string YearSort => $"{Year}:{Description}";
        //public string RotationSort => $"{Rotation}:{Description}";

        public string YearSort
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}