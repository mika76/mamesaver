/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Configuration;
using System.Linq;

namespace Mamesaver
{
    public partial class ConfigForm : Form
    {
        #region Variables
        private ListViewSorter lvwColumnSorter = null;
        private List<SelectableGame> selectedGames;
        #endregion

        #region Constructor
        public ConfigForm()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers
        private void configForm_Load(object sender, EventArgs e)
        {
            //load list
            List<SelectableGame> gameList = Settings.LoadGameList();
            LoadList(gameList);

            //load config
            txtExec.Text = Settings.ExecutablePath;
            txtCommandLineOptions.Text = Settings.CommandLineOptions;
            txtMinutes.Value = Settings.Minutes;

            //other
            lvwColumnSorter = new ListViewSorter();
            lstGames.ListViewItemSorter = lvwColumnSorter;

            cloneScreen.Checked = Settings.CloneScreen;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Rebuilds the list from Mame. This can take a while, therefore it
        /// is done on a background thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRebuild_Click(object sender, EventArgs e)
        {
            // Identity games which are selected so we can reapply selections after rebuild
            selectedGames = GetSelectedGames();

            btnOk.Enabled = tabControl1.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            lstGames.BeginUpdate();
            lstGames.Items.Clear();
            lstGames.EndUpdate();
            lblNoGames.Text = "No Games: 0";

            picBuilding.Visible = true;

            SaveSettings();

            try
            {
                ListBuilder.RunWorkerAsync();

                while (ListBuilder.IsBusy)
                    Application.DoEvents();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            picBuilding.Visible = false;
            this.Cursor = Cursors.Default;
            btnOk.Enabled = tabControl1.Enabled = true;
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
            List<SelectableGame> gameList = BuildGamesList();
            SaveSettings(true, gameList);
            this.Close();
        }

        private void ListBuilder_DoWork(object sender, DoWorkEventArgs e)
        {
            List<SelectableGame> gamesList = GameListBuilder.GetGameList();
            e.Result = gamesList;
        }

        private void ListBuilder_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                List<SelectableGame> gamesList = e.Result as List<SelectableGame>;

                // Select games based on any previous form selection and repopulate form
                ApplySelectionState(gamesList);
                LoadList(gamesList);
            }
            else
                MessageBox.Show(e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void configForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ListBuilder.IsBusy) ListBuilder.CancelAsync();
        }

        private void lstGames_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                    lvwColumnSorter.Order = SortOrder.Descending;
                else
                    lvwColumnSorter.Order = SortOrder.Ascending;
            }
            else
            {
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
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
            availableGames.ForEach(game => game.Selected = selectedGames.Any(selectedGame => selectedGame.Name == game.Name));
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

        private void SaveSettings()
        {
            SaveSettings(false, null);
        }

        private void SaveSettings(bool saveGameList, List<SelectableGame> gameList)
        {
            if (saveGameList) Settings.SaveGameList(gameList);

            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Settings.ExecutablePath = txtExec.Text;
            Settings.CommandLineOptions = txtCommandLineOptions.Text;
            Settings.Minutes = Convert.ToInt32(txtMinutes.Value);
            Settings.CloneScreen = cloneScreen.Checked;
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
                ListViewItem item = lstGames.Items.Add(new ListViewItem(new string[] { game.Description, game.Year, game.Manufacturer }));
                item.Checked = game.Selected;
                item.Tag = game;
            }

            lstGames.EndUpdate();

            lblNoGames.Text = "No Games: " + gamesList.Count.ToString();
        }
        #endregion

   }
}