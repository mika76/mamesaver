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
            ConfigViewModel.ResetToDefaults += (sender, args) => ResetToDefaults(args.Settings);
        }

        /// <summary>
        ///     Resets hotkey settings to default values.
        /// </summary>
        private void ResetToDefaults(Settings settings)
        {
            _settings = settings;
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
