using Mamesaver.Config.Extensions;
using Mamesaver.Config.ViewModels.LayoutTab;

namespace Mamesaver.Config
{
    public partial class LayoutTab
    {
        public LayoutTab() => InitializeComponent();

        public override void BeginInit()
        {
            base.BeginInit();

            this.InitViewModel<LayoutViewModel>();
            this.InitDesignMode();
         }
    }
}