using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Mamesaver.Config.ViewModels;

namespace Mamesaver.Config.Filters.ViewModels
{
    public class DecadeFilterViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> Decades { get; set; }


        public DecadeFilterViewModel(ConfigFormViewModel configForm)
        {
            LoadDecades(configForm);
        }

        public void LoadDecades(ConfigFormViewModel configForm)
        {
            var decades = configForm.Games
                .Select(g => ToDecade(g.Year))
                .Distinct()
                .OrderBy(d => d);

            Decades = new ObservableCollection<string>(decades);
        }

        private static string ToDecade(string year)
        {
            return !int.TryParse(year, out var yearValue) ? "Other" : $"{yearValue / 10 * 10}s";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}