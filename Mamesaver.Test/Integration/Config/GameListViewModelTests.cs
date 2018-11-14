using System.Linq;
using FluentAssertions;
using Mamesaver.Config.ViewModels.GameListTab;
using Mamesaver.Models.Configuration;
using Mamesaver.Test.Unit;
using NUnit.Framework;

using static Mamesaver.Test.Integration.Config.TestData;

namespace Mamesaver.Test.Integration.Config
{
    [TestFixture]
    [TestOf(typeof(GameListViewModel))]
    public class GameListViewModelTests : MamesaverTests
    {
        private GameListViewModel _viewModel;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            GetInstance<GameList>().Games = GameList();

            _viewModel = GetInstance<GameListViewModel>();
            _viewModel.Initialise();
        }

        [SetUp]
        public void SetUp()
        {
            _viewModel.FilteredGames.ForEach(g => g.Selected = true);
            _viewModel.AllSelected = true;
        }

        [Test]
        [Description("Verifies the game count label is updated with game selection")]
        public void GameCount()
        {
            _viewModel.GameCount.Should().Be("No. games: 5 (5 selected)");
            _viewModel.Games.ForEach(g => g.Selected = false);

            _viewModel.GameCount.Should().Be("No. games: 5 (0 selected)");

            _viewModel.Games.ForEach(g => g.Selected = true);
            _viewModel.GameCount.Should().Be("No. games: 5 (5 selected)");
        }

        [Test]
        [Description("Verifies the global game selection checkbox state is updated when games are manually selected")]
        public void GlobalSelectionState()
        {
            _viewModel.AllSelected.Should().BeTrue();
            _viewModel.FilteredGames[0].Selected = false;
            _viewModel.GameSelectionClick.Execute(null);

            _viewModel.AllSelected.Should().BeNull();

            _viewModel.FilteredGames.ForEach(g => g.Selected = false);
            _viewModel.GameSelectionClick.Execute(null);

            _viewModel.AllSelected.Should().BeFalse();

            _viewModel.FilteredGames.ForEach(g => g.Selected = true);
            _viewModel.GameSelectionClick.Execute(null);

            _viewModel.AllSelected.Should().BeTrue();
        }

        [Test]
        public void ChangeGlobalSelection()
        {
            _viewModel.Games.All(g => g.Selected).Should().BeTrue("all games should initially be selected");

            _viewModel.AllSelected = false;
            _viewModel.Games.All(g => g.Selected).Should().BeFalse("all games should be deselected");

            _viewModel.AllSelected = true;
            _viewModel.Games.All(g => g.Selected).Should().BeTrue("all games should be selected");
        }

        [Test]
        public void GlobalFilter()
        {
            _viewModel.FilteredGames.Should().BeEquivalentTo(_viewModel.Games);

            // Select half the games
            for (var i = 0; i < _viewModel.Games.Count; i++)
            {
                _viewModel.Games[i].Selected = i % 2 == 0;
            }

            var selectedGames = _viewModel.Games.Where(g => g.Selected).ToList();

            // Apply filter
            _viewModel.GlobalFilter = GameListViewModel.SelectedGamesFilter;
            _viewModel.ApplyGlobalFilterClick.Execute(true);

            // Verify only selected games displayed
            _viewModel.FilteredGames.Should().BeEquivalentTo(selectedGames);

            // Apply filter
            _viewModel.GlobalFilter = GameListViewModel.AllGamesFilter;
            _viewModel.ApplyGlobalFilterClick.Execute(true);

            // Verify only selected games displayed
            _viewModel.FilteredGames.Should().BeEquivalentTo(_viewModel.Games);
 
        }
    }
}
