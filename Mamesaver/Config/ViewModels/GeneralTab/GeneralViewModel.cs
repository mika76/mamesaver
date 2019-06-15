using System.Windows.Input;
using Mamesaver.Models.Configuration;
using Microsoft.Win32;
using Prism.Commands;

namespace Mamesaver.Config.ViewModels.GeneralTab
{
    public class GeneralViewModel : InitialisableViewModel
    {
        private Settings _settings;
        private AdvancedSettings _advancedSettings;

        public GeneralViewModel(Settings settings)
        {
            _settings = settings;
            _advancedSettings = settings.AdvancedSettings;
        }

        protected override void PerformInitialise()
        {
            ConfigViewModel.ResetToDefaults += (sender, args) => ResetToDefaults(args.Settings);
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

        /// <summary>
        ///     Opens a file dialog for selection of the MAME executable.
        /// </summary>
        public ICommand SelectMameExecutableClick => new DelegateCommand(() =>
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Executables (*.exe)|*.exe|All files (*.*)|*.*",
                Title = "Path to MAME executable"
            };

            // Update settings if file selected
            if (dialog.ShowDialog() == true) ExecutablePath = dialog.FileName;
        });


        /// <summary>
        ///     Resets general settings to default values.
        /// </summary>
        private void ResetToDefaults(Settings settings)
        {
            // Preserve MAME executable path
            settings.ExecutablePath = _settings.ExecutablePath;
            _settings = settings;
            _advancedSettings = settings.AdvancedSettings;

            OnAllPropertiesChanged();
        }

        /// <summary>
        ///     Path to MAME executable.
        /// </summary>
        public string ExecutablePath
        {
            get => _settings.ExecutablePath;
            set
            {
                if (value == _settings.ExecutablePath) return;
                _settings.ExecutablePath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Options to send to the command line when MAME runs the game.
        /// </summary>
        public string CommandLineOptions
        {
            get => _settings.CommandLineOptions;
            set
            {
                if (value == _settings.CommandLineOptions) return;
                _settings.CommandLineOptions = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Whether the game should be displayed on all screens.
        /// </summary>
        public bool CloneScreen
        {
            get => _settings.CloneScreen;
            set
            {
                if (value == _settings.CloneScreen) return;
                _settings.CloneScreen = value;
                OnPropertyChanged();
            }
        }

        public MamePrimaryScreen PrimaryScreen 
        {
            get => _settings.MamePrimaryScreen;
            set
            {
                if (value == _settings.MamePrimaryScreen) return;
                _settings.MamePrimaryScreen = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Time to run each game.
        /// </summary>
        public int MinutesPerGame
        {
            get => _settings.MinutesPerGame;
            set
            {
                if (value == _settings.MinutesPerGame) return;
                _settings.MinutesPerGame = value;
                OnPropertyChanged();
            }
        }
    }
}