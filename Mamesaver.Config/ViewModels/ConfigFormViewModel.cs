using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;
using Mamesaver.Config.Models;
using Mamesaver.Models;
using Mamesaver.Models.Configuration;
using Mamesaver.Services.Categories;
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
        private IEnumerable<GameViewModel> _filteredGames;

        public ConfigFormViewModel(
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

            // TODO shift out of constructor plz
            LoadFonts();
            LoadGames();
        }

        public List<GameViewModel> Games { get; set; }

        public bool? AllSelected
        {
            get => _allSelected;
            set
            {
                if (value != null)
                    foreach (var game in FilteredGames) game.Selected = value.Value;
                _allSelected = value;

                OnPropertyChanged();
            }
        }

        public ICommand OkClick => new DelegateCommand(SaveAndClose);
        public ICommand CancelClick => new DelegateCommand(Close);
        public ICommand ResetToDefaultsClick => new DelegateCommand(ResetToDefaults);
        public ICommand ClearFiltersClick => new DelegateCommand(ClearFilters);
        public ICommand RebuildListClick => new DelegateCommand(RebuildList);
        public ICommand GameSelectionClick => new DelegateCommand(GameSelectionChange);

        private void SaveAndClose()
        {
            _generalSettingsStore.Save();
            _gameListStore.Save();

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

        public IEnumerable<GameViewModel> FilteredGames
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

        public void RebuildList()
        {
            Progress = 0;
            Rebuilding = true;
            Games = new List<GameViewModel>();
            OnPropertyChanged(nameof(GameCount));

            Task.Run(() =>
            {
                try
                {
                    Games = _gameListBuilder
                        .GetGameList(progress => Progress = progress)
                        .Select(ToViewModel)
                        .ToList();
                }
                catch (FileNotFoundException fe)
                {
                    MessageBox.Show(@"Error running MAME; verify that the executable path is correct.", @"Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Log.Error(fe, $"Unable to find MAME at {fe.FileName}");
 
                }
                catch (Exception ex)
                {
                    MessageBox.Show(@"Error running MAME; verify that the configuration is correct.", @"Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Log.Error(ex, "Unable to construct game list");
                }

                Rebuilding = false;
            });
        }

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
            Games = _gameList.Games.Select(ToViewModel).ToList();

            //// TEMP performance testing
            //for(var i = 0; i < 5; i++) Games.AddRange(_gameList.Games);

            //// Populate missing categories
            Games
                .Where(game => game.Category == null)
                .ToList()
                .ForEach(game => game.Category = CategoryParser.GetCategory(game.Name));

            // Maintain a separate list of games displayed by the current filter
            FilteredGames = new List<GameViewModel>(Games).ToList();
        }

        private GameViewModel ToViewModel(SelectableGame game)
        {
            var viewModel = new GameViewModel
            {
                Selected = game.Selected,
                Name = game.Name,
                Year = game.Year,
                Description = game.Description,
                Manufacturer = game.Manufacturer,
                Category = game.Category,
            };

            switch (game.Rotation)
            {
                case "0":
                case "180":
                    viewModel.Rotation = "Horizontal";
                    break;
                case "90":
                case "270":
                    viewModel.Rotation = "Vertical";
                    break;
            }

            return viewModel;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}