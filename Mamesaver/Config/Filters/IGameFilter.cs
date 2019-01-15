using Mamesaver.Config.Models;

namespace Mamesaver.Config.Filters
{
    public interface IGameFilter
    {
        bool IsMatch(GameViewModel game);
    }
}