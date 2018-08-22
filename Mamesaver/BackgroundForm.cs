/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System.Windows.Forms;

namespace Mamesaver
{
    public partial class BackgroundForm : Form
    {
        public BackgroundForm()
        {
            InitializeComponent();
            lblData1.Visible = lblData2.Visible = false; // hide on the clone screens
        }

        public void SetGameText(string heading, string subheading)
        {
            lblData1.Visible = lblData2.Visible = true;
            lblData1.Text = heading;
            lblData2.Text = subheading;
        }
    }
}