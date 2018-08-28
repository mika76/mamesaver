using System.Collections.Generic;
using Mamesaver.Configuration.Models;

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
    }
}