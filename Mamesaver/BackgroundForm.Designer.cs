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
            this.components = new System.ComponentModel.Container();
            this.secondaryLabel = new System.Windows.Forms.Label();
            this.primaryLabel = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.mameLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.mameLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // secondaryLabel
            // 
            this.secondaryLabel.BackColor = System.Drawing.Color.Transparent;
            this.secondaryLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.secondaryLabel.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryLabel.ForeColor = System.Drawing.Color.White;
            this.secondaryLabel.Location = new System.Drawing.Point(0, 366);
            this.secondaryLabel.Name = "secondaryLabel";
            this.secondaryLabel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 9);
            this.secondaryLabel.Size = new System.Drawing.Size(488, 40);
            this.secondaryLabel.TabIndex = 0;
            this.secondaryLabel.Text = "Text";
            this.secondaryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // primaryLabel
            // 
            this.primaryLabel.BackColor = System.Drawing.Color.Transparent;
            this.primaryLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.primaryLabel.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryLabel.ForeColor = System.Drawing.Color.White;
            this.primaryLabel.Location = new System.Drawing.Point(0, 320);
            this.primaryLabel.Name = "primaryLabel";
            this.primaryLabel.Size = new System.Drawing.Size(488, 46);
            this.primaryLabel.TabIndex = 1;
            this.primaryLabel.Text = "Text";
            this.primaryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // mameLogo
            // 
            this.mameLogo.BackgroundImage = global::Mamesaver.Properties.Resources.MAMELogoTM;
            this.mameLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.mameLogo.Location = new System.Drawing.Point(84, 77);
            this.mameLogo.Name = "mameLogo";
            this.mameLogo.Size = new System.Drawing.Size(314, 191);
            this.mameLogo.TabIndex = 2;
            this.mameLogo.TabStop = false;
            // 
            // BackgroundForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(488, 406);
            this.ControlBox = false;
            this.Controls.Add(this.mameLogo);
            this.Controls.Add(this.primaryLabel);
            this.Controls.Add(this.secondaryLabel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BackgroundForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.mameLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label secondaryLabel;
        public System.Windows.Forms.Label primaryLabel;
        public System.Windows.Forms.PictureBox mameLogo;
        private System.Windows.Forms.ImageList imageList1;
    }
}