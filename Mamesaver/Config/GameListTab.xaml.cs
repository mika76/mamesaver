using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using DataGridExtensions;
using Mamesaver.Config.Models;
using Mamesaver.Config.ViewModels;

namespace Mamesaver.Config
{
    public partial class GameListTab 
    {
        public GameListTab()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var view = CollectionViewSource.GetDefaultView(GameList.Items);
            view.Filter = item =>
            {
                var gameViewModel = (GameViewModel) item;
                return gameViewModel.Selected;

            };

            view.CollectionChanged += (_, __) =>
            {
                var source = (ListCollectionView)view.SourceCollection;

                ViewModel.FilteredGames.Clear();
                ViewModel.FilteredGames.AddRange(source.Cast<GameViewModel>());
            };
        }

        private void ClearFilters(object sender, RoutedEventArgs e) => GameList.GetFilter().Clear();

        private ConfigFormViewModel ViewModel => (ConfigFormViewModel) DataContext;
    }
}