using System;
using System.Linq;
using System.Windows;
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
            GameList.GetFilter().FilterChanged += FilterChanged;
        }

        private void FilterChanged(object sender, EventArgs e) => ViewModel.FilteredGames = GameList.Items.OfType<GameViewModel>();
        private void ClearFilters(object sender, RoutedEventArgs e) => GameList.GetFilter().Clear();

        private ConfigFormViewModel ViewModel => (ConfigFormViewModel) DataContext;
    }
}