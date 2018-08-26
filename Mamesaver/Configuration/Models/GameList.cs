using System.Collections.Generic;
using System.Linq;

namespace Mamesaver.Configuration.Models
{
    public class GameList
    {
        public GameList(List<SelectableGame> games) => Games = games;

        /// <summary>
        ///     All available ROMs.
        /// </summary>
        public List<SelectableGame> Games { get; set; }

        /// <summary>
        ///     Returns a list of games which have been selected by the user.
        /// </summary>
        public List<Game> SelectedGames => Games.Where(game => game.Selected).Cast<Game>().ToList();
    }
}