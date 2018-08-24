/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System.Drawing;
using System.Windows.Forms;
using Mamesaver.Models.Settings;

namespace Mamesaver
{
    public partial class BackgroundForm : Form
    {
        public BackgroundForm()
        {
            InitializeComponent();
            primaryLabel.Visible = secondaryLabel.Visible = false; // hide on the clone screens

            var fontSettings = FontSettings();

            primaryLabel.Font = new Font(fontSettings.Face, fontSettings.Size, FontStyle.Bold, GraphicsUnit.Point);
            secondaryLabel.Font = new Font(fontSettings.Face, fontSettings.Size * 0.9f, FontStyle.Regular, GraphicsUnit.Point);
        }

        public void SetGameText(string heading, string subheading)
        {
            primaryLabel.Visible = secondaryLabel.Visible = true;
            primaryLabel.Text = heading;
            secondaryLabel.Text = subheading;
        }

        private static FontSettings FontSettings()
        {
            var settings = SettingStores.General.Get();

            var splashSettings = settings.LayoutSettings.SplashScreen;
            return splashSettings.FontSettings;
        }
    }
}