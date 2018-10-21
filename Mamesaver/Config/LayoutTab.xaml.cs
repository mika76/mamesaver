using System.ComponentModel;
using Mamesaver.Config.ViewModels.LayoutTab;
using Mamesaver.Services;

namespace Mamesaver.Config
{
    public partial class LayoutTab
    {
        public LayoutTab() => InitializeComponent();

        public override void BeginInit()
        {
            base.BeginInit();

            var viewModel = ServiceResolver.GetInstance<LayoutViewModel>();
            viewModel.Initialise();
            DataContext = viewModel;

            // Clear design-mode background
            if (!DesignerProperties.GetIsInDesignMode(this)) ClearValue(BackgroundProperty);
         }
    }
}