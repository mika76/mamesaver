using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Mamesaver.Models;
using Mamesaver.Services.Configuration;
using Mamesaver.Test.Unit;
using NUnit.Framework;

namespace Mamesaver.Test.Integration
{
    [TestFixture]
    public class GameListStoreTests : MamesaverTests
    {
        private TestGameListStore _store;

        [SetUp]
        public void SetUp()
        {
            _store = new TestGameListStore();
        }

        [Test]
        public void AddGames()
        {
            var games = _store.Get();
            games.Should().BeEmpty("no games should be returned for empty store");

            var newGames = TestGames();

            games.AddRange(newGames);
            _store.Save();

            _store.Get().Should().BeEquivalentTo(games);
        }

        [Test]
        public void GetGameList()
        {
            var newGames = TestGames();
            _store.Get().AddRange(newGames);
            var gameList = _store.GetGameList;

            gameList.Games.Should().BeEquivalentTo(newGames);
            gameList.SelectedGames.Should().BeEquivalentTo(newGames.Where(g => g.Selected));
        }

        [Test]
        public void ChangeSelection()
        {
            var newGames = TestGames();
            _store.Get().AddRange(newGames);

            var targetGame = newGames.First().Name;

            // Update game selection, forcing reload from disk to verify persistence
            _store.ChangeSelection(targetGame, true);

            // Verify selection updated
            var persistedGame = _store.Get(true).FirstOrDefault(g => g.Name == targetGame);
            Assert.NotNull(persistedGame);
            persistedGame.Selected.Should().Be(true);

            // Change again and verify
            _store.ChangeSelection(targetGame, false);
            persistedGame = _store.Get(true).FirstOrDefault(g => g.Name == targetGame);
            Assert.NotNull(persistedGame);

            persistedGame.Selected.Should().Be(false);
        }

        [Test]
        public void UpdateGames()
        {
            var games = _store.Get();
            var newGames = TestGames();
            games.AddRange(newGames);
            _store.Save();

            // Toggle selection state, reloading from disk to verify persistence
            _store.Get().ForEach(g => g.Selected = !g.Selected);
            _store.Save();

            _store.Get(true).Should().BeEquivalentTo(newGames);
        }

        private static List<SelectableGame> TestGames()
        {
            var newGames = new List<SelectableGame>
            {
                new SelectableGame{ Name = "first", Selected = true },
                new SelectableGame{ Name = "second", Selected = false },
                new SelectableGame{ Name = "third", Selected = true }
            };
            return newGames;
        }
    }

    public class TestGameListStore : GameListStore
    {
        public TestGameListStore() => Filename = $"{Guid.NewGuid()}.xml";
        public override string Filename { get; }
    }
}