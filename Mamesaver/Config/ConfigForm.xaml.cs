using System.ComponentModel;
using Mamesaver.Config.ViewModels;
using Mamesaver.Services;

namespace Mamesaver.Config
{
    public partial class ConfigForm
    {
        public ConfigForm() => InitializeComponent();

        public override void BeginInit()
        {
            base.BeginInit();

            DataContext = ServiceResolver.GetInstance<ConfigViewModel>();

            // Clear design-mode background
            if (!DesignerProperties.GetIsInDesignMode(this)) ClearValue(BackgroundProperty);
        }
    }
}