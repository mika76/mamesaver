using Mamesaver.Config.ViewModels;

namespace Mamesaver.Config
{
    public partial class ConfigForm
    {
        private readonly ConfigFormViewModel _viewModel;

        public ConfigForm(ConfigFormViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
        }

        public override void BeginInit()
        {
            base.BeginInit();

            _viewModel.Initialise();
            DataContext = _viewModel;
        }
    }
}