using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;
using Mamesaver.Models;
using Mamesaver.Models.Configuration;
using Mamesaver.Services.Categories;
using Mamesaver.Services.Configuration;
using Prism.Commands;

namespace Mamesaver.Config.ViewModels
{
    public class ConfigFormViewModel : INotifyPropertyChanged
    {
        private readonly Settings _settings;
        private readonly LayoutSettings _layoutSettings;
        private readonly AdvancedSettings _advancedSettings;
        private readonly GameList _gameList;
        private readonly GeneralSettingsStore _generalSettingsStore;
        private readonly GameListStore _gameListStore;
        //private readonly GameListBuilder _gameListBuilder;
        private const string DefaultFont = "Arial";
        private const int DefaultFontSize = 13;

        private bool? _allSelected = true;
        private bool _cloneToAllMonitors;
        private string _commandLineOptions;
        private IEnumerable<SelectableGame> _filteredGames;
        private bool _inGameTitlesEnabled;
        private string _mamePath;
        private bool _splashEnabled;
        private bool _includeImperfectEmulation;
        private bool _debugLogging;

        public ConfigFormViewModel(
            Settings settings,
            LayoutSettings layoutSettings,
            AdvancedSettings advancedSettings,
            GameList gameList,
            GeneralSettingsStore generalSettingsStore,
            GameListStore gameListStore)
            //GameListBuilder gameListBuilder)
        {
            _settings = settings;
            _layoutSettings = layoutSettings;
            _advancedSettings = advancedSettings;
            _gameList = gameList;
            _generalSettingsStore = generalSettingsStore;
            _gameListStore = gameListStore;
            //_gameListBuilder = gameListBuilder;

            LoadFonts();
            LoadGames();
        }

        public List<SelectableGame> Games { get; set; }

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
            Close();
        }

        private void Close()
        {
            Application.Current.Shutdown();
        }

        private void ResetToDefaults()
        {

        }


        public IEnumerable<SelectableGame> FilteredGames
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

        public string MamePath
        {
            get => _mamePath;
            set
            {
                if (value == _mamePath) return;
                _mamePath = value;
                OnPropertyChanged();
            }
        }

        public string CommandLineOptions
        {
            get => _commandLineOptions;
            set
            {
                if (value == _commandLineOptions) return;
                _commandLineOptions = value;
                OnPropertyChanged();
            }
        }

        public bool CloneToAllMonitors
        {
            get => _cloneToAllMonitors;
            set
            {
                if (value == _cloneToAllMonitors) return;
                _cloneToAllMonitors = value;
                OnPropertyChanged();
            }
        } 

        public List<string> Fonts { get; set; } = new List<string>();
        public List<int> FontSizes { get; set; } = new List<int>();

        public bool SplashEnabled
        {
            get => _splashEnabled;
            set
            {
                if (value == _splashEnabled) return;
                _splashEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool InGameTitlesEnabled
        {
            get => _inGameTitlesEnabled;
            set
            {
                if (value == _inGameTitlesEnabled) return;
                _inGameTitlesEnabled = value;
                OnPropertyChanged();
            }
        }

        public int SplashDuration { get; set; } = 3;

        public string InGameFont { get; set; } = DefaultFont;
        public int InGameFontSize { get; set; } = DefaultFontSize;
        public string SplashFont { get; set; } = DefaultFont;

        public bool HotkeysEnabled { get; set; }

        public int Interval { get; set; } = 5;

        public bool IncludeImperfectEmulation
        {
            get => _includeImperfectEmulation;
            set
            {
                if (value == _includeImperfectEmulation) return;
                _includeImperfectEmulation = value;
                OnPropertyChanged();
            }
        }

        public bool DebugLogging
        {
            get => _debugLogging;
            set
            {
                if (value == _debugLogging) return;
                _debugLogging = value;
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

        public void RebuildList() => LoadGames();
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
            Games = _gameList.Games;

            // TEMP performance testing
            for(var i = 0; i < 5; i++) Games.AddRange(_gameList.Games);

            // Populate missing categories
            Games
                .Where(game => game.Category == null)
                .ToList()
                .ForEach(game => game.Category = CategoryParser.GetCategory(game.Name));

            // Maintain a separate list of games displayed by the current filter
            FilteredGames = new List<SelectableGame>(Games);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}