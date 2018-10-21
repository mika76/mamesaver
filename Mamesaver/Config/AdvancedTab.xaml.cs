using System.ComponentModel;
using Mamesaver.Config.ViewModels.AdvancedTab;
using Mamesaver.Services;

namespace Mamesaver.Config
{
    public partial class AdvancedTab
    {
        public AdvancedTab()
        {
            InitializeComponent();
        }

        public override void BeginInit()
        {
            base.BeginInit();

            var viewModel = ServiceResolver.GetInstance<AdvancedViewModel>();
            viewModel.Initialise();

            DataContext = viewModel;

            // Clear design-mode background
            if (!DesignerProperties.GetIsInDesignMode(this)) ClearValue(BackgroundProperty);
        }
    }
}
