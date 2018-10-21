using Mamesaver.Config.Extensions;
using Mamesaver.Config.ViewModels.GeneralTab;

namespace Mamesaver.Config
{
    public partial class GeneralTab
    {
        public GeneralTab() => InitializeComponent();

        public override void BeginInit()
        {
            base.BeginInit();

            this.InitViewModel<GeneralViewModel>();
            this.InitDesignMode();
        }
    }
}