using System.Diagnostics;
using System.Windows.Navigation;

namespace Mamesaver.Config
{
    public partial class AboutTab
    {
        public AboutTab()
        {
            InitializeComponent();
        }

        private void OpenProjectSite(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
