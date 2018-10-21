using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;
using Prism.Commands;

namespace Mamesaver.Config.ViewModels.AboutTab
{
    public class AboutViewModel
    {
        public void Initialise() => Version = GetVersion();

        /// <summary>
        ///     Opens the Mamesaver project site in a browser.
        /// </summary>
        public ICommand OpenProjectSiteClick => new DelegateCommand(() =>
            Process.Start(new ProcessStartInfo("https://github.com/mika76/mamesaver")));

        /// <summary>
        ///     Version number label.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        ///     Returns the version of the application based on the assembly file version.
        /// </summary>
        /// <returns></returns>
        private static string GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);

            return $@"{fileVersion.FileVersion}";
        }
    }
}