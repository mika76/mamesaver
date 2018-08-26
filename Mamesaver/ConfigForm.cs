/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Configuration;
using System.Drawing;
using System.Linq;
using Mamesaver.Configuration;
using Mamesaver.Configuration.Models;
using LayoutSettings = Mamesaver.Configuration.Models.LayoutSettings;

namespace Mamesaver
{
    internal partial class ConfigForm : Form
    {
        private Settings _settings;
        private LayoutSettings _layoutSettings;
        private AdvancedSettings _advancedSettings;
        private readonly GeneralSettingsStore _generalSettingsStore;
        private readonly GameListStore _gameListStore;
        private readonly GameList _gameList;
        private readonly GameListBuilder _gameListBuilder;

        #region Variables
        private readonly ListViewSorter _lvwColumnSorter;
        private List<SelectableGame> _selectedGames;
        #endregion

        #region Constructor
        public ConfigForm(
            Settings settings,
            LayoutSettings layoutSettings,
            AdvancedSettings advancedSettings,
            GameList gameList, 
            GeneralSettingsStore generalSettingsStore,
            GameListStore gameListStore,
            GameListBuilder gameListBuilder,
            ListViewSorter listViewSorter)
        {
            _settings = settings;
            _layoutSettings = layoutSettings;
            _advancedSettings = advancedSettings;
            _generalSettingsStore = generalSettingsStore;
            _gameListStore = gameListStore;
            _gameList = gameList;
            _gameListBuilder = gameListBuilder;
            _lvwColumnSorter = listViewSorter;

            InitializeComponent();
        }
        #endregion

        #region Event Handlers
        private void configForm_Load(object sender, EventArgs e)
        {
            // Load game list
            LoadList(_gameList.Games);
            LoadFonts();

            // Load config
            SetFieldsFromSettings();

            displayInGameTitles.CheckedChanged += DisplayInGameTitlesChanged;
            displaySplash.CheckedChanged += DisplaySplashChanged;

            resetToDefaults.Click += ResetToDefaults;

            //other
            lstGames.ListViewItemSorter = _lvwColumnSorter;

            // Progress bar
            gameListProgress.Left = (gameListProgress.Parent.Width - gameListProgress.Width) / 2;
            gameListProgress.Top = (gameListProgress.Parent.Height - gameListProgress.Height) / 2;

            // Set initial game layout state
            DisplayInGameTitlesChanged(displayInGameTitles, null);
            DisplaySplashChanged(displaySplash, null);
        }

        private void SetFieldsFromSettings()
        {
            // General
            txtExec.Text = _settings.ExecutablePath;
            txtCommandLineOptions.Text = _settings.CommandLineOptions;
            txtMinutes.Value = _settings.MinutesPerGame;
            cloneScreen.Checked = _settings.CloneScreen;

            // Layout
            var splashSettings = _layoutSettings.SplashScreen;
            displaySplash.Checked = splashSettings.Enabled;
            splashDuration.Value = splashSettings.DurationSeconds;
            splashScreenFont.SelectedItem = splashSettings.FontSettings.Face;

            var inGameTitleSettings = _layoutSettings.InGameTitles;
            displayInGameTitles.Checked = inGameTitleSettings.Enabled;
            inGameFont.SelectedItem = inGameTitleSettings.FontSettings.Face;
            inGameFontSize.SelectedItem = inGameTitleSettings.FontSettings.Size;

            // Advanced
            debugLogging.Checked = _advancedSettings.DebugLogging;
            skipGameValidation.Checked = _advancedSettings.SkipGameValidation;
        }

        private void ResetToDefaults(object sender, EventArgs e)
        {
            // Preserve MAME executable path
            var executablePath = _settings.ExecutablePath;
            _settings = new Settings { ExecutablePath = executablePath };

            _layoutSettings = _settings.LayoutSettings;
            _advancedSettings = _settings.AdvancedSettings;

            SetFieldsFromSettings();
        }

        private void DisplaySplashChanged(object sender, EventArgs e)
        {
            var checkbox = (CheckBox)sender;
            splashScreenOptions.Enabled = checkbox.Checked;
        }

        private void DisplayInGameTitlesChanged(object sender, EventArgs e)
        {
            var checkbox = (CheckBox) sender;
            inGameTitleOptions.Enabled = checkbox.Checked;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     Rebuilds the list from MAME. This can take a while, therefore it
        ///     is done on a background thread.
        /// </summary>
        private void btnRebuild_Click(object sender, EventArgs e)
        {
            // Identity games which are selected so we can reapply selections after rebuild
            _selectedGames = GetSelectedGames();

            SetControlState(false, btnOk, btnRebuild, btnSelAll, btnSelNone);

            gameListProgress.Value = 0;
            gameListProgress.Visible = true;

            lstGames.BeginUpdate();
            lstGames.Items.Clear();
            lstGames.EndUpdate();
            lblNoGames.Text = @"Num Games: 0";

            try
            {
                ListBuilder.WorkerReportsProgress = true;
                ListBuilder.ProgressChanged += UpdateProgressBar;
                ListBuilder.RunWorkerAsync();
                while (ListBuilder.IsBusy) Application.DoEvents();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            SetControlState(true, btnOk, btnRebuild, btnSelAll, btnSelNone);
            gameListProgress.Visible = false;
        }

        private void SetControlState(bool enabled, params Control[] controls) => controls.ToList().ForEach(control => control.Enabled = enabled);

        private void UpdateProgressBar(object sender, ProgressChangedEventArgs e)
        {
            gameListProgress.Value = e.ProgressPercentage;
        }

        private void btnSelAll_Click(object sender, EventArgs e)
        {
            lstGames.BeginUpdate();
            foreach (ListViewItem item in lstGames.Items)
                item.Checked = true;
            lstGames.EndUpdate();
        }

        private void btnSelNone_Click(object sender, EventArgs e)
        {
            lstGames.BeginUpdate();
            foreach (ListViewItem item in lstGames.Items)
                item.Checked = false;
            lstGames.EndUpdate();

        }

        private void btnExecBrowse_Click(object sender, EventArgs e)
        {
            dlgOpen.FileName = txtExec.Text;

            if (dlgOpen.ShowDialog(this) == DialogResult.OK)
                txtExec.Text = dlgOpen.FileName;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var gameList = BuildGamesList();
            SaveSettings(true, gameList);
            Close();
        }

        private void LoadFonts()
        {
            foreach (var font in FontFamily.Families)
            {
                inGameFont.Items.Add(font.Name);
                splashScreenFont.Items.Add(font.Name);
            }

            // Construct font size list similar to Windows standard font lists
            var fontSizes = Enumerable.Range(8, 20).Where(e => e < 14 || e % 2 == 0).ToList();
            fontSizes.ForEach(size => inGameFontSize.Items.Add(size));
        }

        private void ListBuilder_DoWork(object sender, DoWorkEventArgs e)
        {
            PopulateSettings();

            try
            {
                var gamesList = _gameListBuilder.GetGameList(percentageComplete => ListBuilder.ReportProgress(percentageComplete));
                e.Result = gamesList;
            }
            catch (Exception)
            {
                MessageBox.Show(@"Error running MAME; verify that the executable path is correct.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Result = new List<SelectableGame>();
            }
        }

        private void ListBuilder_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                var gamesList = e.Result as List<SelectableGame>;

                // Select games based on any previous form selection and repopulate form
                ApplySelectionState(gamesList);
                LoadList(gamesList);
            }
            else
                MessageBox.Show(e.Error.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void configForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ListBuilder.IsBusy) ListBuilder.CancelAsync();
        }

        private void lstGames_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == _lvwColumnSorter.SortColumn)
            {
                _lvwColumnSorter.Order = _lvwColumnSorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                _lvwColumnSorter.SortColumn = e.Column;
                _lvwColumnSorter.Order = SortOrder.Ascending;
            }

            lstGames.Sort();
        }
        #endregion

        #region Private Methods

        /// <summary>
        ///     Selects games based on current user selection. This method preserves previous selections
        ///     after a rebuild.
        /// </summary>
        /// <param name="availableGames">all available games</param>
        private void ApplySelectionState(List<SelectableGame> availableGames)
        {
            availableGames.ForEach(game => game.Selected = _selectedGames.Any(selectedGame => selectedGame.Name == game.Name));
        }

        /// <summary>
        ///     Constructs a list of <see cref="SelectableGame"/>s based on the form's game list and selection state.
        /// </summary>
        private List<SelectableGame> BuildGamesList()
        {
            var games = new List<SelectableGame>();

            foreach (ListViewItem item in lstGames.Items)
            {
                var game = (SelectableGame)item.Tag;
                game.Selected = item.Checked;
                games.Add(game);
            }

            return games;
        }

        /// <summary>
        ///     Returns a list of selected games.
        /// </summary>
        private List<SelectableGame> GetSelectedGames()
        {
            return BuildGamesList().Where(game => game.Selected).ToList();
        }

        private void SaveSettings(bool saveGameList = false, List<SelectableGame> gameList = null)
        {
            if (saveGameList)
            {
                _gameListStore.Save(gameList);
            }

            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            PopulateSettings();

            _generalSettingsStore.Save(_settings);
        }

        /// <summary>
        ///     Populates the settings models from the form
        /// </summary>
        private void PopulateSettings()
        {
            // General
            _settings.ExecutablePath = txtExec.Text;
            _settings.CommandLineOptions = txtCommandLineOptions.Text;
            _settings.MinutesPerGame = Convert.ToInt32(txtMinutes.Value);
            _settings.CloneScreen = cloneScreen.Checked;

            // Layout
            _layoutSettings.SplashScreen.Enabled = displaySplash.Checked;
            _layoutSettings.SplashScreen.DurationSeconds = Convert.ToInt32(splashDuration.Value);
            _layoutSettings.SplashScreen.FontSettings.Face = splashScreenFont.Text;

            _layoutSettings.InGameTitles.Enabled = displayInGameTitles.Checked;
            _layoutSettings.InGameTitles.FontSettings.Face = inGameFont.Text;
            _layoutSettings.InGameTitles.FontSettings.Size = Convert.ToInt32(inGameFontSize.Text);

            // Advanced
            _advancedSettings.DebugLogging = debugLogging.Checked;
            _advancedSettings.SkipGameValidation = skipGameValidation.Checked;
        }

        /**
         * Loads an actual list into the listview
         */
        private void LoadList(List<SelectableGame> gamesList)
        {
            lstGames.BeginUpdate();
            lstGames.Items.Clear();
            lstGames.Groups.Clear();

            foreach (SelectableGame game in gamesList)
            {
                ListViewItem item = lstGames.Items.Add(new ListViewItem(new[] { game.Description, game.Year, game.Manufacturer }));
                item.Checked = game.Selected;
                item.Tag = game;
            }

            lstGames.EndUpdate();

            lblNoGames.Text = $@"Num Games: {gamesList.Count}";
        }
        #endregion
    }
}