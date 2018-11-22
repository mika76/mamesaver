using Mamesaver.Config.Extensions;
using Mamesaver.Config.ViewModels.GeneralTab;

namespace Mamesaver.Config
{
    public partial class HelpTab
    {
        public HelpTab()
        {
            InitializeComponent();

            this.InitViewModel<GeneralViewModel>();
            this.InitDesignMode();
        }
    }
}
