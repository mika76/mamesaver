using Mamesaver.Config.Models;
using Mamesaver.Config.ViewModels.GameListTab;

namespace Mamesaver.Config.Filters
{
    public class GlobalSelectionContentFilter : IGameFilter
    {
        private readonly GameListViewModel _gameList;
        public GlobalSelectionContentFilter(GameListViewModel gameList) => _gameList = gameList;

        public bool IsMatch(GameViewModel game)
        {
            return _gameList.GlobalFilter?.FilterMode != FilterMode.SelectedGames || game.Selected;
        }
    }
}