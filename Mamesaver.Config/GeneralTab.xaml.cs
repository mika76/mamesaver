using System.Windows;
using Mamesaver.Config.ViewModels;
using Microsoft.Win32;

namespace Mamesaver.Config
{
    public partial class GeneralTab
    {
        public GeneralTab() => InitializeComponent();

        private void SelectMameExecutable(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Executables (*.exe)|*.exe|All files (*.*)|*.*",
                Title = "Path to MAME executable"
            };

            if (dialog.ShowDialog() == true) ViewModel.MamePath = dialog.FileName;
        }

        private ConfigFormViewModel ViewModel => (ConfigFormViewModel) DataContext;
    }
}