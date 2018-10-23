using Mamesaver.Config.Extensions;
using Mamesaver.Config.ViewModels.HotkeysTab;

namespace Mamesaver.Config
{
    public partial class HotKeysTab
    {
        public HotKeysTab()
        {
            InitializeComponent();

            this.InitViewModel<HotKeysViewModel>();
            this.InitDesignMode();
        }
    }
}