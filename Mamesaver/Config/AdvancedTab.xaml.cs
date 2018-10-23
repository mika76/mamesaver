using Mamesaver.Config.Extensions;
using Mamesaver.Config.ViewModels.AdvancedTab;

namespace Mamesaver.Config
{
    public partial class AdvancedTab
    {
        public AdvancedTab()
        {
            InitializeComponent();

            this.InitViewModel<AdvancedViewModel>();
            this.InitDesignMode();
        }
    }
}