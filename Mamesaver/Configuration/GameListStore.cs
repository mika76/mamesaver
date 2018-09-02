using System.Collections.Generic;
using System.Linq;
using Mamesaver.Configuration.Models;
using Serilog;

namespace Mamesaver.Configuration
{
    /// <inheritdoc />
    /// <summary>
    ///     Settings store containing the list of MAME ROMs and their selection state.
    /// </summary>
    public class GameListStore : SettingsStore<List<SelectableGame>>
    {
        public override string Filename => "gamelist.xml";

        public GameList GetGameList => new GameList(Get());

        /// <summary>
        ///     Changes the selection status of a game and updates the store.
        /// </summary>
        public void ChangeSelection(string gameName, bool selected)
        {
            var selectedGame = Get().FirstOrDefault(game => game.Name == gameName);
            if (selectedGame == null)
            {
                Log.Warning("Game {name} not found in store", gameName);
                return;
            }

            selectedGame.Selected = selected;
            Save();
        }
    }
}