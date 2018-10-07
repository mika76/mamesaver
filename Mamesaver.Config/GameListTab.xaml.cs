using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DataGridExtensions;
using Mamesaver.Config.ViewModels;
using Mamesaver.Models;

namespace Mamesaver.Config
{
    public partial class GameListTab 
    {
        public GameListTab()
        {
            InitializeComponent();
            GameList.GetFilter().FilterChanged += FilterChanged;
            GameList.Sorting += SortingChanged;
        }

        private void SortingChanged(object sender, DataGridSortingEventArgs e)
        {
            var column = e.Column;
            var direction = (column.SortDirection != ListSortDirection.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending;

            //set the sort order on the column
            column.SortDirection = direction;

            // TODO write me
            // See https://stackoverflow.com/a/18218963/2115261

            // Prevent default sorting behaviour
            e.Handled = true;
        }

        private void FilterChanged(object sender, EventArgs e) => ViewModel.FilteredGames = GameList.Items.OfType<SelectableGame>();
        private void ClearFilters(object sender, RoutedEventArgs e) => GameList.GetFilter().Clear();

        private ConfigFormViewModel ViewModel => (ConfigFormViewModel) DataContext;
    }

    //internal class GameSort : IComparer
    //{
    //    public GameSort(ListSortDirection direction)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public int Compare(object x, object y)
    //    {
    //        var firstGame = x as SelectableGame;
    //        var secondGame = y as SelectableGame;

    //        if (x == null || y == null) return 0;

    //        return 0;
    //    }
    //}
}