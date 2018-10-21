using Mamesaver.Config.Extensions;
using Mamesaver.Config.ViewModels.AdvancedTab;

namespace Mamesaver.Config
{
    public partial class AdvancedTab
    {
        public AdvancedTab() => InitializeComponent();

        public override void BeginInit()
        {
            base.BeginInit();
            
            this.InitViewModel<AdvancedViewModel>();
            this.InitDesignMode();
        }
    }
}
