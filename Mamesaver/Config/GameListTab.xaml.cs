using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using DataGridExtensions;
using Mamesaver.Config.Extensions;
using Mamesaver.Config.Filters;
using Mamesaver.Config.Models;
using Mamesaver.Config.ViewModels.GameListTab;
using Mamesaver.Services;

namespace Mamesaver.Config
{
    // FIXME move more stuff to view model plz

    public partial class GameListTab
    {
        private ICollectionView _view;
        private GameListViewModel _viewModel;

        public GameListTab() => InitializeComponent();

        public override void BeginInit()
        {
            base.BeginInit();

            _viewModel = ServiceResolver.GetInstance<GameListViewModel>();
            _viewModel.Initialise();
            DataContext = _viewModel;

            DataContextChanged += OnDataContextChanged;

            // Clear design-mode background
            if (!DesignerProperties.GetIsInDesignMode(this)) ClearValue(BackgroundProperty);
         }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _view = CollectionViewSource.GetDefaultView(GameList.Items);

            _viewModel.GlobalFilterChange += OnGlobalFilterChange;
            _view.CollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var source = (ListCollectionView)_view.SourceCollection;

            _viewModel.FilteredGames.Clear();
            _viewModel.FilteredGames.AddRange(source.Cast<GameViewModel>());
        }

        private void OnGlobalFilterChange(object sender, GlobalFilterEventArgs e)
        {
            var multipleChoiceFilter = GameList.FindVisualChild<MultipleChoiceFilter>(child => child.Field == nameof(GameViewModel.SelectedFilter));
            if (multipleChoiceFilter == null) return;

            var filterViewModel = (MultipleChoiceFilterViewModel)multipleChoiceFilter.DataContext;

            filterViewModel.Select(false.ToString(), e.FilterMode == FilterMode.AllGames);
            _view.Refresh();
        }

        private void ClearFilters(object sender, RoutedEventArgs e) => GameList.GetFilter().Clear();
    }
}