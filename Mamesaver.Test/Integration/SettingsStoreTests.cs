using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Mamesaver.Models;
using Mamesaver.Services.Configuration;
using Mamesaver.Test.Unit;
using NUnit.Framework;

namespace Mamesaver.Test.Integration
{
    [TestFixture]
    [TestOf(typeof(GameListStore))]
    public class SettingsStoreTests : MamesaverTests
    {
        private TestStore _store;

        [SetUp]
        public void SetUp()
        {
            _store = new TestStore();
        }

        [TearDown]
        public void TearDown()
        {
            var settingsFile = _store.GetSettingsFile();
            if (File.Exists(settingsFile)) File.Delete(settingsFile);
        }

        [Test]
        public void DefaultObjectIfNoFile()
        {
            var settings = _store.Get();
            settings.Should().NotBeNull();
            settings.Should().BeEmpty();
        }

        [Test]
        public void SaveNewAndLoad()
        {
            var games = TestGames();
            _store.Save(games);
            _store.Get().Should().BeEquivalentTo(games);
        }

        [Test]
        public void SaveAndLoad()
        {
            var games = TestGames();
            _store.Get().AddRange(games);

            _store.Save();
            _store.Get().Should().BeEquivalentTo(games);
        }

        [Test]
        public void SaveAndUpdate()
        {
            var games = TestGames();
            _store.Get().AddRange(games);

            // Save new file
            _store.Save();

            // Update game state
            games.ForEach(g => g.Selected = !g.Selected);
            _store.Save();
            _store.Get().Should().BeEquivalentTo(games);
        }

        private static List<SelectableGame> TestGames()
        {
            return new List<SelectableGame>
            {
                new SelectableGame { Name = "tempest", Description = "Tempest (rev 3, Revised Hardware)", Year = "1980", Manufacturer = "Atari", Rotation = "270", Selected = true },
                new SelectableGame { Name = "donpachij", Description = "DonPachi (Japan)", Year = "1995", Manufacturer = "Cave (Atlus license)", Rotation = "270", Selected = false }
            };
        }
    }

    public class TestStore : SettingsStore<List<SelectableGame>>
    {
        public TestStore() => Filename = $"{Guid.NewGuid()}.xml"; 
        public override string Filename { get; }
    }
}