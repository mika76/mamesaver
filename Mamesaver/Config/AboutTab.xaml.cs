using System.ComponentModel;
using Mamesaver.Config.ViewModels.AboutTab;
using Mamesaver.Services;

namespace Mamesaver.Config
{
    public partial class AboutTab
    {
        public AboutTab() => InitializeComponent();

        public override void BeginInit()
        {
            base.BeginInit();

            var viewModel = ServiceResolver.GetInstance<AboutViewModel>();
            viewModel.Initialise();

            DataContext = viewModel;

            // Clear design-mode background
            if (!DesignerProperties.GetIsInDesignMode(this)) ClearValue(BackgroundProperty);
         }
   }
}