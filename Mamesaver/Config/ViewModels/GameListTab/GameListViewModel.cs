using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Mamesaver.Config.Models;
using Mamesaver.Models.Configuration;
using Mamesaver.Properties;
using Mamesaver.Services.Configuration;
using Mamesaver.Services.Mame;
using Prism.Commands;
using Serilog;

namespace Mamesaver.Config.ViewModels.GameListTab
{
    public class GameListViewModel : INotifyPropertyChanged
    {
        public delegate void GlobalFilterEventHandler(object sender, GlobalFilterEventArgs e);

        private static readonly FilterOption AllGamesFilter = new FilterOption("All games", FilterMode.AllGames);

        private static readonly FilterOption SelectedGamesFilter =
            new FilterOption("Selected games", FilterMode.SelectedGames);

        private readonly GameList _gameList;
        private readonly GameListBuilder _gameListBuilder;
        private readonly GameListStore _gameListStore;

        private ObservableCollection<GameViewModel> _filteredGames;
        private ObservableCollection<GameViewModel> _games;
        private List<GameViewModel> _selectedGames;

        private FilterOption _globalFilter = AllGamesFilter;
        private bool? _allSelected = true;
        private int _progress;
        private bool _rebuilding;


        public GameListViewModel(
            GameList gameList,
            GameListBuilder gameListBuilder,
            GameListStore gameListStore)
        {
            _gameList = gameList;
            _gameListBuilder = gameListBuilder;
            _gameListStore = gameListStore;

            _games = new ObservableCollection<GameViewModel>();
            _filteredGames = new ObservableCollection<GameViewModel>();

            ConfigViewModel.Save += (sender, args) => Save();
        }

        public bool? AllSelected
        {
            get => _allSelected;
            set
            {
                if (value != null)
                    foreach (var game in FilteredGames)
                        game.Selected = value.Value;

                _allSelected = value;

                OnPropertyChanged();
            }
        }

        public ICommand ApplyGlobalFilterClick => new DelegateCommand(ApplyGlobalFilter);
        public ICommand ClearFiltersClick => new DelegateCommand(ClearFilters);

        public ICommand RebuildListClick => new DelegateCommand(async () => { await RebuildList(); });

        public ObservableCollection<FilterOption> FilterOptions { get; set; } = new ObservableCollection<FilterOption>
        {
            AllGamesFilter,
            SelectedGamesFilter
        };

        public FilterOption GlobalFilter
        {
            get => _globalFilter;
            set
            {
                if (Equals(value, _globalFilter)) return;
                _globalFilter = value;

                OnPropertyChanged();
            }
        }

        public string GameCount => $"Num Games: {Games.Count}";

        public ICollectionView GamesView { get; set; }


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

        /// <summary>
        ///     Games which are displayed by the current filter.
        /// </summary>
        public ObservableCollection<GameViewModel> FilteredGames
        {
            get => _filteredGames;
            set
            {
                if (Equals(value, _filteredGames)) return;
                _filteredGames = value;
                OnPropertyChanged();
                SetGlobalSelectionState();
            }
        }

        public ICommand GameSelectionClick => new DelegateCommand(SetGlobalSelectionState);

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler GameListRebuilt;
        public event GlobalFilterEventHandler GlobalFilterChange;
        public event EventHandler FiltersCleared;

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
                OnPropertyChanged();
                Rebuilding = false;

                GameListRebuilt?.Invoke(this, EventArgs.Empty);
            }
            catch (FileNotFoundException fe)
            {
                Rebuilding = false;
                MessageBox.Show(@"Error running MAME; verify that the executable path is correct.", @"Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Error(fe, $"Unable to find MAME at {fe.FileName}");
            }
            catch (Exception ex)
            {
                Rebuilding = false;
                MessageBox.Show(@"Error running MAME; verify that the configuration is correct.", @"Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (_selectedGames == null) return;

            availableGames
                .ToList()
                .ForEach(game => game.Selected = _selectedGames.Any(selectedGame => selectedGame.Name == game.Name));
        }

        /// <summary>
        ///     Returns a list of selected games.
        /// </summary>
        private List<GameViewModel> GetSelectedGames()
        {
            return Games.Where(game => game.Selected).ToList();
        }


        private void ClearFilters()
        {
            LoadGames();
            FiltersCleared?.Invoke(this, EventArgs.Empty);
            GlobalFilter = AllGamesFilter;
        }

        public void Initialise()
        {
            LoadGames();
        }

        private void SetGlobalSelectionState()
        {
            var allSelected = FilteredGames.Any() && FilteredGames.All(g => g.Selected);
            var allDeselected = !FilteredGames.Any() || FilteredGames.All(g => !g.Selected);

            if (allSelected) AllSelected = true;
            else if (allDeselected) AllSelected = false;
            else AllSelected = null;
        }

        private void LoadGames()
        {
            //// Clear existing game event handling
            //GameSelectionEventRegistration(false);

            Games.Clear();
            Games.AddRange(_gameList.Games.Select(game => new GameViewModel(game)));
            GamesView = CollectionViewSource.GetDefaultView(Games);

            //// TEMP performance testing
            //for(var i = 0; i < 5; i++) Games.AddRange(_gameList.Games);

            // Maintain a separate list of games displayed by the current filter
            FilteredGames.Clear();
            FilteredGames.AddRange(Games);

            //// Register event to refresh filter state on game selection
            //GameSelectionEventRegistration(true);

            // Select games based on any previous user selection
            ApplySelectionState(Games);

            // Set global selection state
            SetGlobalSelectionState();

            OnPropertyChanged(nameof(GameCount));
        }

        public void ApplyGlobalFilter()
        {
            GlobalFilterChange?.Invoke(this, new GlobalFilterEventArgs(_globalFilter.FilterMode));
        }

        public void Save()
        {
            _gameListStore.Save(_gameList.Games);
        }
 
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
   }
}