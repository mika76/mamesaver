using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mamesaver.Models.Configuration;

namespace Mamesaver.Config.ViewModels.LayoutTab
{
    public class LayoutViewModel : InitialisableViewModel
    {
        private LayoutSettings _settings;

        public List<string> Fonts { get; set; } = new List<string>();
        public List<int> FontSizes { get; set; } = new List<int>();

        public LayoutViewModel(Settings settings)
        {
            _settings = settings.LayoutSettings;
            ConfigViewModel.ResetToDefaults += (sender, args) => ResetToDefaults();
        }

        /// <summary>
        ///     Resets layout settings to default values.
        /// </summary>
        private void ResetToDefaults()
        {
            _settings = new Settings().LayoutSettings;
            OnAllPropertiesChanged();
        }
        protected override void PerformInitialise() => LoadFonts();

        /// <summary>
        ///     Whether the splash screen with game information is displayed before each game.
        /// </summary>
        public bool SplashEnabled
        {
            get => _settings.SplashScreen.Enabled;
            set
            {
                if (value == _settings.SplashScreen.Enabled) return;
                _settings.SplashScreen.Enabled = value;
                OnPropertyChanged();
            }
        }

        public bool InGameTitlesEnabled
        {
            get => _settings.InGameTitles.Enabled;
            set
            {
                if (value == _settings.InGameTitles.Enabled) return;
                _settings.InGameTitles.Enabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     The number of seconds that the game intro screen (the screen with MAME logo) is shown.
        /// </summary>
        public int SplashDuration
        {
            get => _settings.SplashScreen.DurationSeconds;
            set
            {
                if (value == _settings.SplashScreen.DurationSeconds) return;
                _settings.SplashScreen.DurationSeconds = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     In-game font face.
        /// </summary>
        public string InGameFont
        {
            get => _settings.InGameTitles.FontSettings.Face;
            set
            {
                if (value == _settings.InGameTitles.FontSettings.Face) return;
                _settings.InGameTitles.FontSettings.Face = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     In-game font size, in points.
        /// </summary>
        public int InGameFontSize
        {
            get => _settings.InGameTitles.FontSettings.Size;
            set
            {
                if (value == _settings.InGameTitles.FontSettings.Size) return;
                _settings.InGameTitles.FontSettings.Size = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Splash screen font face.
        /// </summary>
        public string SplashFont
        {
            get => _settings.SplashScreen.FontSettings.Face;
            set
            {
                if (value == _settings.SplashScreen.FontSettings.Face) return;
                _settings.SplashScreen.FontSettings.Face = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Populates font face and font size lists.
        /// </summary>
        private void LoadFonts()
        {
            foreach (var font in FontFamily.Families) Fonts.Add(font.Name);

            // Construct font size list similar to Windows standard font lists
            var fontSizes = Enumerable.Range(8, 20).Where(e => e < 14 || e % 2 == 0).ToList();
            fontSizes.ForEach(size => FontSizes.Add(size));
        }
   }
}