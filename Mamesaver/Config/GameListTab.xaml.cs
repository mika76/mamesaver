using Mamesaver.Config.Extensions;
using Mamesaver.Config.ViewModels.GameListTab;

namespace Mamesaver.Config
{
    public partial class GameListTab
    {
        public GameListTab()
        {
            this.InitViewModel<GameListViewModel>();

            InitializeComponent();
            this.InitDesignMode();
        }
    }
}