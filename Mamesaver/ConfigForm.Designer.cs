/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

namespace Mamesaver
{
    partial class ConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            this.label1 = new System.Windows.Forms.Label();
            this.txtExec = new System.Windows.Forms.TextBox();
            this.btnExecBrowse = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lstGames = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colYear = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colManufacturer = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnRebuild = new System.Windows.Forms.Button();
            this.btnSelNone = new System.Windows.Forms.Button();
            this.btnSelAll = new System.Windows.Forms.Button();
            this.ListBuilder = new System.ComponentModel.BackgroundWorker();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.cloneScreen = new System.Windows.Forms.CheckBox();
            this.txtMinutes = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCommandLineOptions = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.gameListProgress = new System.Windows.Forms.ProgressBar();
            this.lblNoGames = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.splashScreenOptions = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.splashDuration = new System.Windows.Forms.NumericUpDown();
            this.splashScreenFont = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.inGameTitleOptions = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.inGameFont = new System.Windows.Forms.ComboBox();
            this.inGameFontSize = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.displaySplash = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.displayInGameTitles = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.debugLogging = new System.Windows.Forms.CheckBox();
            this.includeImperfectEmulation = new System.Windows.Forms.CheckBox();
            this.resetToDefaults = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMinutes)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.splashScreenOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splashDuration)).BeginInit();
            this.inGameTitleOptions.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Path to MAME executable:";
            // 
            // txtExec
            // 
            this.txtExec.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExec.Location = new System.Drawing.Point(9, 33);
            this.txtExec.Name = "txtExec";
            this.txtExec.Size = new System.Drawing.Size(378, 20);
            this.txtExec.TabIndex = 1;
            // 
            // btnExecBrowse
            // 
            this.btnExecBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecBrowse.Location = new System.Drawing.Point(393, 30);
            this.btnExecBrowse.Name = "btnExecBrowse";
            this.btnExecBrowse.Size = new System.Drawing.Size(41, 23);
            this.btnExecBrowse.TabIndex = 2;
            this.btnExecBrowse.Text = "...";
            this.btnExecBrowse.UseVisualStyleBackColor = true;
            this.btnExecBrowse.Click += new System.EventHandler(this.btnExecBrowse_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(304, 385);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "&OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(385, 385);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lstGames
            // 
            this.lstGames.AllowColumnReorder = true;
            this.lstGames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstGames.CheckBoxes = true;
            this.lstGames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colYear,
            this.colManufacturer});
            this.lstGames.FullRowSelect = true;
            this.lstGames.Location = new System.Drawing.Point(6, 38);
            this.lstGames.MultiSelect = false;
            this.lstGames.Name = "lstGames";
            this.lstGames.ShowItemToolTips = true;
            this.lstGames.Size = new System.Drawing.Size(428, 293);
            this.lstGames.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstGames.TabIndex = 7;
            this.lstGames.UseCompatibleStateImageBehavior = false;
            this.lstGames.View = System.Windows.Forms.View.Details;
            this.lstGames.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lstGames_ColumnClick);
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 191;
            // 
            // colYear
            // 
            this.colYear.Text = "Year";
            // 
            // colManufacturer
            // 
            this.colManufacturer.Text = "Manufacturer";
            this.colManufacturer.Width = 152;
            // 
            // btnRebuild
            // 
            this.btnRebuild.Location = new System.Drawing.Point(6, 9);
            this.btnRebuild.Name = "btnRebuild";
            this.btnRebuild.Size = new System.Drawing.Size(75, 23);
            this.btnRebuild.TabIndex = 8;
            this.btnRebuild.Text = "&Rebuild List";
            this.btnRebuild.UseVisualStyleBackColor = true;
            this.btnRebuild.Click += new System.EventHandler(this.btnRebuild_Click);
            // 
            // btnSelNone
            // 
            this.btnSelNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelNone.Location = new System.Drawing.Point(359, 9);
            this.btnSelNone.Name = "btnSelNone";
            this.btnSelNone.Size = new System.Drawing.Size(75, 23);
            this.btnSelNone.TabIndex = 9;
            this.btnSelNone.Text = "Select &None";
            this.btnSelNone.UseVisualStyleBackColor = true;
            this.btnSelNone.Click += new System.EventHandler(this.btnSelNone_Click);
            // 
            // btnSelAll
            // 
            this.btnSelAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelAll.Location = new System.Drawing.Point(278, 9);
            this.btnSelAll.Name = "btnSelAll";
            this.btnSelAll.Size = new System.Drawing.Size(75, 23);
            this.btnSelAll.TabIndex = 10;
            this.btnSelAll.Text = "Select &All";
            this.btnSelAll.UseVisualStyleBackColor = true;
            this.btnSelAll.Click += new System.EventHandler(this.btnSelAll_Click);
            // 
            // ListBuilder
            // 
            this.ListBuilder.WorkerSupportsCancellation = true;
            this.ListBuilder.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ListBuilder_DoWork);
            this.ListBuilder.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ListBuilder_RunWorkerCompleted);
            // 
            // dlgOpen
            // 
            this.dlgOpen.DefaultExt = "*.exe";
            this.dlgOpen.FileName = "openFileDialog1";
            this.dlgOpen.Filter = "EXE Files|*.exe|All files|*.*";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(448, 363);
            this.tabControl1.TabIndex = 12;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.cloneScreen);
            this.tabPage1.Controls.Add(this.txtMinutes);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.txtCommandLineOptions);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.txtExec);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.btnExecBrowse);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(440, 337);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // cloneScreen
            // 
            this.cloneScreen.AutoSize = true;
            this.cloneScreen.Location = new System.Drawing.Point(9, 169);
            this.cloneScreen.Name = "cloneScreen";
            this.cloneScreen.Size = new System.Drawing.Size(155, 17);
            this.cloneScreen.TabIndex = 7;
            this.cloneScreen.Text = "Clone MAME to all monitors";
            this.cloneScreen.UseVisualStyleBackColor = true;
            // 
            // txtMinutes
            // 
            this.txtMinutes.Location = new System.Drawing.Point(8, 133);
            this.txtMinutes.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this.txtMinutes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtMinutes.Name = "txtMinutes";
            this.txtMinutes.Size = new System.Drawing.Size(80, 20);
            this.txtMinutes.TabIndex = 6;
            this.txtMinutes.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(94, 135);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "minutes";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Interval:";
            // 
            // txtCommandLineOptions
            // 
            this.txtCommandLineOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCommandLineOptions.Location = new System.Drawing.Point(9, 83);
            this.txtCommandLineOptions.Name = "txtCommandLineOptions";
            this.txtCommandLineOptions.Size = new System.Drawing.Size(378, 20);
            this.txtCommandLineOptions.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Command line options:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.gameListProgress);
            this.tabPage2.Controls.Add(this.lblNoGames);
            this.tabPage2.Controls.Add(this.lstGames);
            this.tabPage2.Controls.Add(this.btnSelAll);
            this.tabPage2.Controls.Add(this.btnRebuild);
            this.tabPage2.Controls.Add(this.btnSelNone);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(440, 337);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Game List";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // gameListProgress
            // 
            this.gameListProgress.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.gameListProgress.Location = new System.Drawing.Point(113, 164);
            this.gameListProgress.Name = "gameListProgress";
            this.gameListProgress.Size = new System.Drawing.Size(220, 23);
            this.gameListProgress.TabIndex = 12;
            this.gameListProgress.Visible = false;
            // 
            // lblNoGames
            // 
            this.lblNoGames.AutoSize = true;
            this.lblNoGames.Location = new System.Drawing.Point(87, 14);
            this.lblNoGames.Name = "lblNoGames";
            this.lblNoGames.Size = new System.Drawing.Size(77, 13);
            this.lblNoGames.TabIndex = 11;
            this.lblNoGames.Text = "Num Games: 0";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.splashScreenOptions);
            this.tabPage3.Controls.Add(this.inGameTitleOptions);
            this.tabPage3.Controls.Add(this.label11);
            this.tabPage3.Controls.Add(this.label12);
            this.tabPage3.Controls.Add(this.displaySplash);
            this.tabPage3.Controls.Add(this.label10);
            this.tabPage3.Controls.Add(this.displayInGameTitles);
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(440, 337);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Layout";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // splashScreenOptions
            // 
            this.splashScreenOptions.Controls.Add(this.label7);
            this.splashScreenOptions.Controls.Add(this.splashDuration);
            this.splashScreenOptions.Controls.Add(this.splashScreenFont);
            this.splashScreenOptions.Controls.Add(this.label6);
            this.splashScreenOptions.Controls.Add(this.label8);
            this.splashScreenOptions.Location = new System.Drawing.Point(8, 159);
            this.splashScreenOptions.Name = "splashScreenOptions";
            this.splashScreenOptions.Size = new System.Drawing.Size(345, 114);
            this.splashScreenOptions.TabIndex = 21;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Duration:";
            // 
            // splashDuration
            // 
            this.splashDuration.Location = new System.Drawing.Point(3, 17);
            this.splashDuration.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.splashDuration.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.splashDuration.Name = "splashDuration";
            this.splashDuration.Size = new System.Drawing.Size(80, 20);
            this.splashDuration.TabIndex = 11;
            this.splashDuration.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // splashScreenFont
            // 
            this.splashScreenFont.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.splashScreenFont.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.splashScreenFont.FormattingEnabled = true;
            this.splashScreenFont.Location = new System.Drawing.Point(3, 64);
            this.splashScreenFont.Name = "splashScreenFont";
            this.splashScreenFont.Size = new System.Drawing.Size(332, 21);
            this.splashScreenFont.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(89, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "seconds";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1, 47);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Font:";
            // 
            // inGameTitleOptions
            // 
            this.inGameTitleOptions.Controls.Add(this.label5);
            this.inGameTitleOptions.Controls.Add(this.inGameFont);
            this.inGameTitleOptions.Controls.Add(this.inGameFontSize);
            this.inGameTitleOptions.Location = new System.Drawing.Point(8, 59);
            this.inGameTitleOptions.Name = "inGameTitleOptions";
            this.inGameTitleOptions.Size = new System.Drawing.Size(345, 44);
            this.inGameTitleOptions.TabIndex = 20;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Font:";
            // 
            // inGameFont
            // 
            this.inGameFont.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.inGameFont.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.inGameFont.FormattingEnabled = true;
            this.inGameFont.Location = new System.Drawing.Point(3, 17);
            this.inGameFont.Name = "inGameFont";
            this.inGameFont.Size = new System.Drawing.Size(276, 21);
            this.inGameFont.TabIndex = 3;
            // 
            // inGameFontSize
            // 
            this.inGameFontSize.FormattingEnabled = true;
            this.inGameFontSize.Location = new System.Drawing.Point(285, 17);
            this.inGameFontSize.Name = "inGameFontSize";
            this.inGameFontSize.Size = new System.Drawing.Size(50, 21);
            this.inGameFontSize.TabIndex = 4;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 120);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(74, 13);
            this.label11.TabIndex = 19;
            this.label11.Text = "Splash screen";
            // 
            // label12
            // 
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label12.Location = new System.Drawing.Point(11, 127);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(400, 6);
            this.label12.TabIndex = 18;
            // 
            // displaySplash
            // 
            this.displaySplash.AutoSize = true;
            this.displaySplash.Location = new System.Drawing.Point(11, 136);
            this.displaySplash.Name = "displaySplash";
            this.displaySplash.Size = new System.Drawing.Size(59, 17);
            this.displaySplash.TabIndex = 5;
            this.displaySplash.Text = "Enable";
            this.displaySplash.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 17);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(69, 13);
            this.label10.TabIndex = 17;
            this.label10.Text = "In-game titles";
            // 
            // displayInGameTitles
            // 
            this.displayInGameTitles.AutoSize = true;
            this.displayInGameTitles.Checked = true;
            this.displayInGameTitles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.displayInGameTitles.Location = new System.Drawing.Point(11, 36);
            this.displayInGameTitles.Name = "displayInGameTitles";
            this.displayInGameTitles.Size = new System.Drawing.Size(59, 17);
            this.displayInGameTitles.TabIndex = 13;
            this.displayInGameTitles.Text = "Enable";
            this.displayInGameTitles.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label9.Location = new System.Drawing.Point(11, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(400, 2);
            this.label9.TabIndex = 16;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.debugLogging);
            this.tabPage4.Controls.Add(this.includeImperfectEmulation);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(440, 337);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Advanced";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // debugLogging
            // 
            this.debugLogging.AutoSize = true;
            this.debugLogging.Location = new System.Drawing.Point(9, 40);
            this.debugLogging.Name = "debugLogging";
            this.debugLogging.Size = new System.Drawing.Size(95, 17);
            this.debugLogging.TabIndex = 1;
            this.debugLogging.Text = "Debug logging";
            this.debugLogging.UseVisualStyleBackColor = true;
            // 
            // includeImperfectEmulation
            // 
            this.includeImperfectEmulation.AutoSize = true;
            this.includeImperfectEmulation.Location = new System.Drawing.Point(9, 17);
            this.includeImperfectEmulation.Name = "includeImperfectEmulation";
            this.includeImperfectEmulation.Size = new System.Drawing.Size(211, 17);
            this.includeImperfectEmulation.TabIndex = 0;
            this.includeImperfectEmulation.Text = "Include games with imperfect emulation";
            this.includeImperfectEmulation.UseVisualStyleBackColor = true;
            // 
            // resetToDefaults
            // 
            this.resetToDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resetToDefaults.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.resetToDefaults.Location = new System.Drawing.Point(12, 385);
            this.resetToDefaults.Name = "resetToDefaults";
            this.resetToDefaults.Size = new System.Drawing.Size(119, 23);
            this.resetToDefaults.TabIndex = 14;
            this.resetToDefaults.Text = "&Reset to Defaults";
            this.resetToDefaults.UseVisualStyleBackColor = true;
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(472, 416);
            this.Controls.Add(this.resetToDefaults);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(480, 450);
            this.Name = "ConfigForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mamesaver Config";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.configForm_FormClosing);
            this.Load += new System.EventHandler(this.configForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMinutes)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.splashScreenOptions.ResumeLayout(false);
            this.splashScreenOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splashDuration)).EndInit();
            this.inGameTitleOptions.ResumeLayout(false);
            this.inGameTitleOptions.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtExec;
        private System.Windows.Forms.Button btnExecBrowse;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListView lstGames;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colYear;
        private System.Windows.Forms.ColumnHeader colManufacturer;
        private System.Windows.Forms.Button btnRebuild;
        private System.Windows.Forms.Button btnSelNone;
        private System.Windows.Forms.Button btnSelAll;
        private System.ComponentModel.BackgroundWorker ListBuilder;
        private System.Windows.Forms.OpenFileDialog dlgOpen;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtCommandLineOptions;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblNoGames;
        private System.Windows.Forms.NumericUpDown txtMinutes;
        private System.Windows.Forms.CheckBox cloneScreen;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ComboBox inGameFontSize;
        private System.Windows.Forms.ComboBox inGameFont;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox displaySplash;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown splashDuration;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox displayInGameTitles;
        private System.Windows.Forms.ComboBox splashScreenFont;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button resetToDefaults;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel splashScreenOptions;
        private System.Windows.Forms.Panel inGameTitleOptions;
        private System.Windows.Forms.ProgressBar gameListProgress;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.CheckBox debugLogging;
        private System.Windows.Forms.CheckBox includeImperfectEmulation;
    }
}