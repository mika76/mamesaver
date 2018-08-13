/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

namespace Mamesaver
{
    partial class BackgroundForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblData2 = new System.Windows.Forms.Label();
            this.lblData1 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblData2
            // 
            this.lblData2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblData2.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblData2.ForeColor = System.Drawing.Color.White;
            this.lblData2.Location = new System.Drawing.Point(0, 337);
            this.lblData2.Name = "lblData2";
            this.lblData2.Size = new System.Drawing.Size(488, 46);
            this.lblData2.TabIndex = 0;
            this.lblData2.Text = "Text";
            this.lblData2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblData1
            // 
            this.lblData1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblData1.Font = new System.Drawing.Font("Arial", 20.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblData1.ForeColor = System.Drawing.Color.White;
            this.lblData1.Location = new System.Drawing.Point(0, 291);
            this.lblData1.Name = "lblData1";
            this.lblData1.Size = new System.Drawing.Size(488, 46);
            this.lblData1.TabIndex = 1;
            this.lblData1.Text = "Text";
            this.lblData1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(0, 383);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(488, 23);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            // 
            // BackgroundForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(488, 406);
            this.Controls.Add(this.lblData1);
            this.Controls.Add(this.lblData2);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "BackgroundForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BackgroundForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label lblData2;
        public System.Windows.Forms.Label lblData1;
        private System.Windows.Forms.Label label1;

    }
}