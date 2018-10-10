using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Mamesaver.Config.Models
{
    public class GameViewModel  : INotifyPropertyChanged
    {
        private bool _selected = true;

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

        public string Name { get; set; }
        public string Description { get; set; }
        public string Year { get; set; }

        public string Manufacturer { get; set; }
        public string Rotation { get; set; }
        public string Category { get; set; }

        public string ManufacturerSort => $"{Manufacturer}:{Description}";
        public string CategorySort => $"{Category}:{Description}";
        public string YearSort => $"{Year}:{Description}";
        public string RotationSort => $"{Rotation}:{Description}";

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}