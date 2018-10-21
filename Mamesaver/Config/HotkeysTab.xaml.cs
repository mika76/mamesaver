using System.ComponentModel;
using Mamesaver.Config.ViewModels.HotkeysTab;
using Mamesaver.Services;

namespace Mamesaver.Config
{
    public partial class HotKeysTab
    {
        public HotKeysTab() => InitializeComponent();

        public override void BeginInit()
        {
            base.BeginInit();

            var viewModel = ServiceResolver.GetInstance<HotKeysViewModel>();
            viewModel.Initialise();

            DataContext = viewModel;

            // Clear design-mode background
            if (!DesignerProperties.GetIsInDesignMode(this)) ClearValue(BackgroundProperty);
        }
    }
}
