using System.ComponentModel;
using Mamesaver.Config.ViewModels.GeneralTab;
using Mamesaver.Services;

namespace Mamesaver.Config
{
    public partial class GeneralTab
    {
        public GeneralTab() => InitializeComponent();

        public override void BeginInit()
        {
            base.BeginInit();

            var viewModel = ServiceResolver.GetInstance<GeneralViewModel>();
            viewModel.Initialise();

            DataContext = viewModel;

            // Clear design-mode background
            if (!DesignerProperties.GetIsInDesignMode(this)) ClearValue(BackgroundProperty);
        }
    }
}