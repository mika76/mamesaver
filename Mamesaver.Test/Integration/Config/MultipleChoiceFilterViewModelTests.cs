using System;
using System.Linq;
using FluentAssertions;
using Mamesaver.Config.Filters.ViewModels;
using Mamesaver.Config.Models;
using Mamesaver.Config.ViewModels.GameListTab;
using Mamesaver.Models.Configuration;
using Mamesaver.Test.Unit;
using NUnit.Framework;

using static Mamesaver.Test.Integration.Config.TestData;

namespace Mamesaver.Test.Integration.Config
{
    [TestFixture]
    [TestOf(typeof(MultipleChoiceFilterViewModel))]
    public class MultipleChoiceFilterViewModelTests : MamesaverTests
    {
        private MultipleChoiceFilterViewModel _manufacturerViewModel, _categoryViewModel;
        private GameListViewModel _gameListViewModel;

        private int GameCount(Func<GameViewModel, bool> predicate) => _gameListViewModel.Games.Count(predicate);
        private int FilteredGameCount(Func<GameViewModel, bool> predicate) => _gameListViewModel.FilteredGames.Count(predicate);
        private Func<FilterItemViewModel> AtariFilter => () => _manufacturerViewModel.SelectableValues.First(v => v.Value == Manufacturers.Atari);

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            GetInstance<GameList>().Games = GameList();

            _gameListViewModel = GetInstance<GameListViewModel>();
            _manufacturerViewModel = GetInstance<MultipleChoiceFilterViewModel>();
            _categoryViewModel = GetInstance<MultipleChoiceFilterViewModel>();

            _gameListViewModel.Initialise();
            _categoryViewModel.Initialise();
            _manufacturerViewModel.Initialise();

            _manufacturerViewModel.FilterProperty = nameof(GameViewModel.Manufacturer);
            _categoryViewModel.FilterProperty = nameof(GameViewModel.Category);
        }

        [SetUp]
        public void SetUp()
        {
            _manufacturerViewModel.SelectableValues.ForEach(v => v.Selected = true);
            _categoryViewModel.SelectableValues.ForEach(v => v.Selected = true);

            _manufacturerViewModel.SelectAllClick.Execute(null);
            _categoryViewModel.SelectAllClick.Execute(null);
        }

        [Test]
        public void FilterValuesPopulated()
        {
            // Manufacturer
            _manufacturerViewModel.SelectableValues.Should().HaveCount(3);
            _manufacturerViewModel.SelectableValues
                .Select(s => s.Value)
                .Should()
                .BeEquivalentTo(Manufacturers.Atari, Manufacturers.Cave, Manufacturers.Sega);

            _manufacturerViewModel.SelectableValues
                .Select(s => s.Selected)
                .Should()
                .AllBeEquivalentTo(true, "all filter values should be initially selected");

            // Category 
            _categoryViewModel.SelectableValues.Should().HaveCount(2);
            _categoryViewModel.SelectableValues
                .Select(s => s.Value)
                .Should()
                .BeEquivalentTo(Category.Fighter, Category.Shooter);

            _categoryViewModel.SelectableValues
                .Select(s => s.Selected)
                .Should()
                .AllBeEquivalentTo(true, "all filter values should be initially selected");
        }

        [Test]
        [Description("Verifies that filtered game list changes when filters are changed")]
        public void GameFilterChanges()
        {
            var atariPredicate = AtariPredicate();
            var initialGameCount = GameCount(atariPredicate);
            var initialFilteredGameCount = FilteredGameCount(atariPredicate);

            // Sanity check of game list state
            initialGameCount.Should().BeGreaterThan(0, "games should be present in master list");
            initialFilteredGameCount.Should().Be(initialGameCount, "initial state of filtered games should be same as master game list");

            // Deselect filter item
            AtariFilter().Selected = false;

            GameCount(atariPredicate).Should().Be(initialGameCount, "filter deselection shouldn't change master game list");
            FilteredGameCount(atariPredicate).Should().Be(0, "filter selection should remove games from filter list");

            AtariFilter().Selected = true;
            FilteredGameCount(atariPredicate).Should().Be(initialFilteredGameCount, "selecting filter should re-add games to filter list");
        }

        [Test]
        public void ActiveFilterContainsDeselectedValues()
        {
            // Verify that after deselecting value, the selected and deselected values are present on active filter
            _categoryViewModel.SelectableValues.First(v => v.Value == Category.Fighter).Selected = false;
            _categoryViewModel.FilteredSelectableValues.Select(v => v.Value).Should().BeEquivalentTo(Category.Fighter, Category.Shooter);

            // Change active filter, verifying that deselected values in initial filter are now affected
            _manufacturerViewModel.FilteredSelectableValues.ForEach(v => v.Selected = false);
            _categoryViewModel.FilteredSelectableValues.Should().BeEmpty();

            _manufacturerViewModel.FilteredSelectableValues.ForEach(v => v.Selected = true);
            _categoryViewModel.FilteredSelectableValues.Should().NotBeEmpty();
        }

        [Test]
        public void FilterIconChanged()
        {
            VerifyNoActiveFilterIcon();

            // Filter on manufacturer
            _manufacturerViewModel.FilteredSelectableValues.First().Selected = false;

            // Verify manufacturer filter active
            _manufacturerViewModel.ActiveFilterMarkerVisible.Should().BeTrue();
            _manufacturerViewModel.IconBrush.Should().Be(MultipleChoiceFilterViewModel.ActiveBrush);

            _categoryViewModel.ActiveFilterMarkerVisible.Should().BeFalse();
            _categoryViewModel.IconBrush.Should().Be(MultipleChoiceFilterViewModel.InactiveBrush);

            // Filter on category
            _categoryViewModel.FilteredSelectableValues.First().Selected = false;

            // Verify manufacturer and category filter active 
            _manufacturerViewModel.ActiveFilterMarkerVisible.Should().BeTrue();
            _manufacturerViewModel.IconBrush.Should().Be(MultipleChoiceFilterViewModel.ActiveBrush);

            _categoryViewModel.ActiveFilterMarkerVisible.Should().BeTrue();
            _categoryViewModel.IconBrush.Should().Be(MultipleChoiceFilterViewModel.ActiveBrush);
        }

        private void VerifyNoActiveFilterIcon()
        {
            _manufacturerViewModel.ActiveFilterMarkerVisible.Should().BeFalse();
            _manufacturerViewModel.IconBrush.Should().Be(MultipleChoiceFilterViewModel.InactiveBrush);

            _categoryViewModel.ActiveFilterMarkerVisible.Should().BeFalse();
            _categoryViewModel.IconBrush.Should().Be(MultipleChoiceFilterViewModel.InactiveBrush);
        }

        [Test]
        public void FilterIconClearedOnClearFilter()
        {
            _manufacturerViewModel.FilteredSelectableValues.First().Selected = false;
            _categoryViewModel.FilteredSelectableValues.First().Selected = false;

            _gameListViewModel.ClearFiltersClick.Execute(null);

            VerifyNoActiveFilterIcon();
        }

        [Test]
        [Description("Verifies that changing a filter perform a filter on other filters")]
        public void SecondaryFilterUpdated()
        {
            _manufacturerViewModel.FilteredSelectableValues.Should().BeEquivalentTo(_manufacturerViewModel.SelectableValues);
            _manufacturerViewModel.FilteredSelectableValues
                .Select(v => v.Value)
                .Should()
                .BeEquivalentTo(Manufacturers.Atari, Manufacturers.Cave, Manufacturers.Sega);

            _categoryViewModel.SelectableValues.First(v => v.Value == Category.Fighter).Selected = false;

            // Verify that secondary filter is updated
            _manufacturerViewModel.FilteredSelectableValues
                .Select(v => v.Value)
                .Should()
                .BeEquivalentTo(Manufacturers.Atari, Manufacturers.Sega);

            // Verify that master filter list not updated
            _manufacturerViewModel.SelectableValues
                .Select(v => v.Value)
                .Should()
                .BeEquivalentTo(Manufacturers.Atari, Manufacturers.Cave, Manufacturers.Sega);
        }

        [Test]
        public void BulkSelection()
        {
            int FilterCount() => FilteredGameCount(g => true);

            var initialFilterCount = FilterCount();
            initialFilterCount.Should().BeGreaterThan(0, "games should be present to filter");

            _manufacturerViewModel.SelectNoneClick.Execute(null);
            FilterCount().Should().Be(0, "select none should remove all games from filter list");
            _manufacturerViewModel.SelectableValues.Should().OnlyContain(v => !v.Selected, "filter values should all be deselected after select none");

            _manufacturerViewModel.SelectAllClick.Execute(null);
            FilterCount().Should().Be(initialFilterCount, "all games should be present after select all");
            _manufacturerViewModel.SelectableValues.Should().OnlyContain(v => v.Selected, "filter values should all be selected after select none");
        }

        private static Func<GameViewModel, bool> AtariPredicate()
        {
            return g => g.Manufacturer == Manufacturers.Atari;
        }
    }
}
