using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Mamesaver.Config.Extensions;
using Mamesaver.Config.Filters;
using Mamesaver.Config.Filters.ViewModels;
using Mamesaver.Config.Models;
using Mamesaver.Config.ViewModels.GameListTab;

namespace Mamesaver.Config
{
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

        /// <summary>
        ///     Aligns internal state of filtered games in the view model with the filtered items
        ///     in the datagrid.
        /// </summary>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var source = (ListCollectionView)_view.SourceCollection;

            _viewModel.FilteredGames.Clear();
            _viewModel.FilteredGames.AddRange(source.Cast<GameViewModel>());
        }

        /// <summary>
        ///     Applies filtering to the selected game column in response to the global filter selection.
        /// </summary>
        /// <remarks>
        ///     This is an inelegant solution, and uses a hidden <see cref="MultipleChoiceFilter"/> bound to the 
        ///     checkbox column in order to perform the filtering. This is because the internal filtering behaviour 
        ///     provided by <c>DataGridExtensions</c> isn't exposed. As we are already bypassing much of this library,
        ///     it is recommended to remove it and rewrite the parts that are used.
        /// </remarks>
        private void OnGlobalFilterChange(object sender, GlobalFilterEventArgs e)
        {
            // Retrieve filter associated with the selected game column
            var multipleChoiceFilter = GameList.FindVisualChild<MultipleChoiceFilter>(child => child.Field == nameof(GameViewModel.SelectedFilter));
            if (multipleChoiceFilter == null) return;

            var filterViewModel = (MultipleChoiceFilterViewModel)multipleChoiceFilter.DataContext;

            // Explicitly toggle the deselected games option, based on the user's selected global fitler mode
            filterViewModel.Select(false.ToString(), e.FilterMode == FilterMode.AllGames);
            _view.Refresh();
        }
    }
}