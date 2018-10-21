using Mamesaver.Config.Extensions;
using Mamesaver.Config.ViewModels;

namespace Mamesaver.Config
{
    public partial class ConfigForm
    {
        public ConfigForm() => InitializeComponent();

        public override void BeginInit()
        {
            base.BeginInit();

            this.InitViewModel<ConfigViewModel>();
            this.InitDesignMode();
        }
    }
}