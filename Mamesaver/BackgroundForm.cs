/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Text;
namespace Mamesaver
{
    public partial class BackgroundForm : Form
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
            IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);

        private PrivateFontCollection fonts = new PrivateFontCollection();

        Font myFont;
        public BackgroundForm()
        {
            InitializeComponent();
            byte[] fontData = Properties.Resources.PressStart2P;
            IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            uint dummy = 0;
            fonts.AddMemoryFont(fontPtr, Properties.Resources.PressStart2P.Length);
            AddFontMemResourceEx(fontPtr, (uint)Properties.Resources.PressStart2P.Length, IntPtr.Zero, ref dummy);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);

            myFont = new Font(fonts.Families[0], 16.0F);
            
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
         
        }


       

        private void tmrFade_Tick(object sender, EventArgs e)
        {
            this.Opacity += 0.3;
            if (this.Opacity >= .95)
            {
                this.Opacity = 1;
                tmrFade.Enabled = false;
            }
        }

        private void BackgroundForm_Load(object sender, EventArgs e)
        {
            lblData1.Font = myFont;
            lblData2.Font = myFont;
        }
    }
}