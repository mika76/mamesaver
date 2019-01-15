using Mamesaver.Config.Extensions;
using Mamesaver.Config.ViewModels;

namespace Mamesaver.Config
{
    public partial class ConfigForm
    {
        public ConfigForm()
        {
            InitializeComponent();

            this.InitViewModel<ConfigViewModel>();
            this.InitDesignMode();
        }
    }
}