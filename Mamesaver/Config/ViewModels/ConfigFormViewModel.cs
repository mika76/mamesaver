using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Mamesaver.Annotations;
using Mamesaver.Config.Models;
using Mamesaver.Models.Configuration;
using Mamesaver.Services;
using Mamesaver.Services.Configuration;
using Mamesaver.Services.Mame;
using Prism.Commands;
using Serilog;

namespace Mamesaver.Config.ViewModels
{
    public class ConfigFormViewModel : INotifyPropertyChanged
    {
        private Settings _settings;
        private LayoutSettings _layoutSettings;
        private AdvancedSettings _advancedSettings;
        private readonly GameList _gameList;
        private readonly GeneralSettingsStore _generalSettingsStore;
        private readonly GameListStore _gameListStore;
        private readonly GameListBuilder _gameListBuilder;

        private bool? _allSelected = true;
        private ObservableCollection<GameViewModel> _filteredGames;
        private List<GameViewModel> _selectedGames;

        public ConfigFormViewModel(
            ServiceResolver serviceResolver,
            Settings settings,
            LayoutSettings layoutSettings,
            AdvancedSettings advancedSettings,
            GameList gameList,
            GeneralSettingsStore generalSettingsStore,
            GameListStore gameListStore,
            GameListBuilder gameListBuilder)
        {
            _settings = settings;
            _layoutSettings = layoutSettings;
            _advancedSettings = advancedSettings;
            _gameList = gameList;
            _generalSettingsStore = generalSettingsStore;
            _gameListStore = gameListStore;
            _gameListBuilder = gameListBuilder;

            _games = new ObservableCollection<GameViewModel>();
            _filteredGames = new ObservableCollection<GameViewModel>();

            // Force initialisation of static resolver
            _ = serviceResolver;
        }

        public void Initialise()
        {
            LoadFonts();
            LoadGames();
        }

        public ObservableCollection<GameViewModel> Games
        {
            get => _games;
            set
            {
                if (Equals(value, _games)) return;
                _games = value;
                OnPropertyChanged(nameof(GameCount));
            }
        }

        public bool? AllSelected
        {
            get => _allSelected;
            set
            {
                if (value != null)
                {
                    foreach (var game in FilteredGames) game.Selected = value.Value;
                }

                _allSelected = value;

                OnPropertyChanged();
            }
        }

        public ICommand OkClick => new DelegateCommand(SaveAndClose);
        public ICommand CancelClick => new DelegateCommand(Close);
        public ICommand ResetToDefaultsClick => new DelegateCommand(ResetToDefaults);
        public ICommand ClearFiltersClick => new DelegateCommand(ClearFilters);
        public ICommand RebuildListClick => new DelegateCommand(async () =>
        {
             await RebuildList();
        });

        public ICommand GameSelectionClick => new DelegateCommand(GameSelectionChange);

        private void SaveAndClose()
        {
            _generalSettingsStore.Save(_settings);
            _gameListStore.Save(_gameList.Games);

            Close();
        }

        private void Close()
        {
            Application.Current.Shutdown();
        }

        private void ResetToDefaults()
        {
            // Preserve MAME executable path
            var executablePath = _settings.ExecutablePath;
            _settings = new Settings { ExecutablePath = executablePath };

            _layoutSettings = _settings.LayoutSettings;
            _advancedSettings = _settings.AdvancedSettings;

            // Notify all properties
            OnPropertyChanged("");
        }

        public ObservableCollection<GameViewModel> FilteredGames
        {
            get => _filteredGames;
            set
            {
                if (Equals(value, _filteredGames)) return;
                _filteredGames = value;
                OnPropertyChanged();
                GameSelectionChange();
            }
        }

        public string GameCount => $"Num Games: {Games.Count}";

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

        public List<string> Fonts { get; set; } = new List<string>();
        public List<int> FontSizes { get; set; } = new List<int>();

        public bool SplashEnabled
        {
            get => _layoutSettings.SplashScreen.Enabled;
            set
            {
                if (value == _layoutSettings.SplashScreen.Enabled) return;
                _layoutSettings.SplashScreen.Enabled = value;
                OnPropertyChanged();
            }
        }

        public bool InGameTitlesEnabled
        {
            get => _layoutSettings.InGameTitles.Enabled;
            set
            {
                if (value == _layoutSettings.InGameTitles.Enabled) return;
                _layoutSettings.InGameTitles.Enabled = value;
                OnPropertyChanged();
            }
        }

        public int SplashDuration
        {
            get => _layoutSettings.SplashScreen.DurationSeconds;
            set
            {
                if (value == _layoutSettings.SplashScreen.DurationSeconds) return;
                _layoutSettings.SplashScreen.DurationSeconds = value;
                OnPropertyChanged();
            }
        }

        public string InGameFont
        {
            get => _layoutSettings.InGameTitles.FontSettings.Face;
            set
            {
                if (value == _layoutSettings.InGameTitles.FontSettings.Face) return;
                _layoutSettings.InGameTitles.FontSettings.Face = value;
                OnPropertyChanged();
            }
        }

        public int InGameFontSize
        {
            get => _layoutSettings.InGameTitles.FontSettings.Size;
            set
            {
                if (value == _layoutSettings.InGameTitles.FontSettings.Size) return;
                _layoutSettings.InGameTitles.FontSettings.Size = value;
                OnPropertyChanged();
            }
        }

        public string SplashFont
        {
            get => _layoutSettings.InGameTitles.FontSettings.Face;
            set
            {
                if (value == _layoutSettings.InGameTitles.FontSettings.Face) return;
                _layoutSettings.InGameTitles.FontSettings.Face = value;
                OnPropertyChanged();
            }
        }

        public bool HotkeysEnabled
        {
            get => _settings.HotKeys;
            set
            {
                if (value == _settings.HotKeys) return;
                _settings.HotKeys = value;
                OnPropertyChanged();
            }
        }

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
        ///     Version number label, based on the assembly file version.
        /// </summary>
        public object Version
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);

                return $@"{fileVersion.FileVersion}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void GameSelectionChange()
        {
            var allSelected = FilteredGames.Any() && FilteredGames.All(g => g.Selected);
            var allDeselected = !FilteredGames.Any() || FilteredGames.All(g => !g.Selected);

            if (allSelected) AllSelected = true;
            else if (allDeselected) AllSelected = false;
            else AllSelected = null;
        }

        public bool Rebuilding
        {
            get => _rebuilding;
            set
            {
                if (value == _rebuilding) return;
                _rebuilding = value;
                OnPropertyChanged();
            }
        }

        private int _progress;
        private bool _rebuilding;
        private ObservableCollection<GameViewModel> _games;

        public int Progress
        {
            get => _progress;
            set
            {
                if (value == _progress) return;
                _progress = value;
                OnPropertyChanged();
            }
        }

        public async Task RebuildList()
        {
            // Set UI state
            Progress = 0;
            Rebuilding = true;

            // Identity games which are selected so we can reapply selections after rebuild
            _selectedGames = GetSelectedGames();
            Games.Clear();

            // Build game list
            try
            {
                _gameList.Games = await Task.Run(() => _gameListBuilder
                    .GetGameList(progress => Progress = progress)
                    .ToList());

                LoadGames();

                // Select games based on any previous user selection
                ApplySelectionState(Games);

                OnPropertyChanged();
                Rebuilding = false;
            }
            catch (FileNotFoundException fe)
            {
                Rebuilding = false;
                MessageBox.Show(@"Error running MAME; verify that the executable path is correct.", @"Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Error(fe, $"Unable to find MAME at {fe.FileName}");
            }
            catch (Exception ex)
            {
                Rebuilding = false;
                MessageBox.Show(@"Error running MAME; verify that the configuration is correct.", @"Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Error(ex, "Unable to construct game list");
            }
        }

        /// <summary>
        ///     Selects games based on current user selection. This method preserves previous selections
        ///     after a rebuild.
        /// </summary>
        /// <param name="availableGames">all available games</param>
        private void ApplySelectionState(ObservableCollection<GameViewModel> availableGames)
        {
            if (_selectedGames == null) throw new InvalidOperationException("Selected games not initialised");

            availableGames
                .ToList()
                .ForEach(game => game.Selected = _selectedGames.Any(selectedGame => selectedGame.Name == game.Name));
        }

        /// <summary>
        ///     Returns a list of selected games.
        /// </summary>
        private List<GameViewModel> GetSelectedGames() => Games.Where(game => game.Selected).ToList();

        private void ClearFilters() => LoadGames();

        private void LoadFonts()
        {
            foreach (var font in FontFamily.Families) Fonts.Add(font.Name);

            // Construct font size list similar to Windows standard font lists
            var fontSizes = Enumerable.Range(8, 20).Where(e => e < 14 || e % 2 == 0).ToList();
            fontSizes.ForEach(size => FontSizes.Add(size));
        }

        private void LoadGames()
        {
            Games.AddRange(_gameList.Games.Select(game => new GameViewModel(game)));

            //// TEMP performance testing
            //for(var i = 0; i < 5; i++) Games.AddRange(_gameList.Games);

            // Maintain a separate list of games displayed by the current filter
            FilteredGames.Clear();
            FilteredGames.AddRange(Games);

            // Set global selection state
            GameSelectionChange();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}