using Mamesaver.Models.Configuration;

namespace Mamesaver.Config.ViewModels.HotkeysTab
{
    public class HotKeysViewModel : InitialisableViewModel
    {
        private Settings _settings;

        public HotKeysViewModel(Settings settings)
        {
            _settings = settings;
        }

        protected override void PerformInitialise()
        {
            ConfigViewModel.ResetToDefaults += (sender, args) => ResetToDefaults();
        }
 
        /// <summary>
        ///     Resets hotkey settings to default values.
        /// </summary>
        private void ResetToDefaults()
        {
            _settings = new Settings();
            var arse = HotKeysEnabled;
            OnAllPropertiesChanged();
        }

        public bool HotKeysEnabled
        {
            get => _settings.HotKeys;
            set
            {
                if (value == _settings.HotKeys) return;
                _settings.HotKeys = value;
                OnPropertyChanged();
            }
        }
   }
}
