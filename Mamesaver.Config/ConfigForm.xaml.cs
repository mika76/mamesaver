using Mamesaver.Config.ViewModels;

namespace Mamesaver.Config
{
    public partial class ConfigForm
    {
        public ConfigForm(ConfigFormViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
  }
}