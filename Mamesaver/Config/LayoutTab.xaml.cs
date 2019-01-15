using Mamesaver.Config.Extensions;
using Mamesaver.Config.ViewModels.LayoutTab;

namespace Mamesaver.Config
{
    public partial class LayoutTab
    {
        public LayoutTab()
        {
            InitializeComponent();

            this.InitViewModel<LayoutViewModel>();
            this.InitDesignMode();
        }
    }
}