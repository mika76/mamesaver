using Mamesaver.Models.Configuration;

namespace Mamesaver.Config.ViewModels.AdvancedTab
{
    public class AdvancedViewModel : InitialisableViewModel
    {
        private AdvancedSettings _advancedSettings;

        public AdvancedViewModel(Settings settings)
        {
            _advancedSettings = settings.AdvancedSettings;
        }

        protected override void PerformInitialise()
        {
            ConfigViewModel.ResetToDefaults += (sender, args) => ResetToDefaults(args.Settings);
        }

        /// <summary>
        ///     Resets advanced settings to default values.
        /// </summary>
        private void ResetToDefaults(Settings settings)
        {
            _advancedSettings = settings.AdvancedSettings;
            OnAllPropertiesChanged();
        }
 
        public bool IncludeImperfectEmulation
        {
            get => _advancedSettings.IncludeImperfectEmulation;
            set
            {
                if (value == _advancedSettings.IncludeImperfectEmulation) return;
                _advancedSettings.IncludeImperfectEmulation = value;
                OnPropertyChanged();
            }
        }

        public bool DebugLogging
        {
            get => _advancedSettings.DebugLogging;
            set
            {
                if (value == _advancedSettings.DebugLogging) return;
                _advancedSettings.DebugLogging = value;
                OnPropertyChanged();
            }
        }
    }
}
