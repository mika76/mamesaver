/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using LayoutSettings = Mamesaver.Configuration.Models.LayoutSettings;

namespace Mamesaver
{
    public partial class BackgroundForm : Form
    {
        /// <summary>
        ///    Scale to apply to MAME logo, relative to display size
        /// </summary>
        private const float MameLogoScale = 0.3f;

        public BackgroundForm(LayoutSettings settings)
        {
            InitializeComponent();

            // Background form is only visible if we are displaying the splash screen on the primary display
            primaryLabel.Visible = secondaryLabel.Visible = false;

            var fontSettings = settings.SplashScreen.FontSettings;

            primaryLabel.Font = new Font(fontSettings.Face, fontSettings.Size, FontStyle.Bold, GraphicsUnit.Point);
            secondaryLabel.Font = new Font(fontSettings.Face, fontSettings.Size * 0.9f, FontStyle.Regular, GraphicsUnit.Point);
        }

        public void SetGameText(string heading, string subheading)
        {
            primaryLabel.Text = heading;
            secondaryLabel.Text = subheading;

            primaryLabel.Visible = secondaryLabel.Visible = true;
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            // Centre and size MAME logo
            mameLogo.Left = (Width - mameLogo.Width) / 2;
            mameLogo.Top = (Height - mameLogo.Height) / 2;

            mameLogo.Width = (int)Math.Round(Width * MameLogoScale);
            mameLogo.Height = (int)Math.Round(Height * MameLogoScale);
        }
    }
}