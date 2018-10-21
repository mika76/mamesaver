using System;
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

namespace Mamesaver.Config
{
    // FIXME move more stuff to view model plz

    public partial class GameListTab
    {
        private ICollectionView _view;
        private GameListViewModel _viewModel;

        public GameListTab() => InitializeComponent();

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _view = CollectionViewSource.GetDefaultView(GameList.Items);
            _view.CollectionChanged += OnCollectionChanged;

            _viewModel.GlobalFilterChange += OnGlobalFilterChange;
        }

        public override void BeginInit()
        {
            base.BeginInit();

            _viewModel = this.InitViewModel<GameListViewModel>();
            this.InitDesignMode();
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