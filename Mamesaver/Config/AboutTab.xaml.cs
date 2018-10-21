using Mamesaver.Config.Extensions;
using Mamesaver.Config.ViewModels.AboutTab;

namespace Mamesaver.Config
{
    public partial class AboutTab
    {
        public AboutTab() => InitializeComponent();

        public override void BeginInit()
        {
            base.BeginInit();

            this.InitViewModel<AboutViewModel>();
            this.InitDesignMode();
        }
    }
}