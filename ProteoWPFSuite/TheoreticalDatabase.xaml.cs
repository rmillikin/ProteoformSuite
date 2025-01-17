﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using ProteoformSuiteInternal;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Windows.Input;

namespace ProteoWPFSuite
{
    /// <summary>
    /// Interaction logic for TheoreticalDatabase.xaml
    /// </summary>
    public partial class TheoreticalDatabase : UserControl,ISweetForm,ITabbedMDI
    {
        #region Public Constructor
        public TheoreticalDatabase()
        {
            InitializeComponent();
            InitializeParameterSet();
            initial_load = false;
        }
        #endregion Public Constructor

        #region Private Fields

        OpenFileDialog openAccessionListDialog = new OpenFileDialog();
        bool initial_load = true;

        #endregion Private Fields

        #region Public Property

        public List<DataTable> DataTables { get; private set; }

        #endregion Public Property

        #region Private Methods
        
        //new trick in wpf
        private void btn_Make_Databases_Click(object sender, EventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            RunTheGamut(false);
            Mouse.OverrideCursor = null;
        }

        private void Cmbx_DisplayWhichDB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // We do this to prevent firing TabControl's SelectionChanged event unintendedly
            // Reference: https://stackoverflow.com/questions/3659858/in-c-sharp-wpf-why-is-my-tabcontrols-selectionchanged-event-firing-too-often
            e.Handled = true;
            if (!initial_load)
            {
                string table = cmbx_DisplayWhichDB.SelectedItem.ToString();
                if (table == "Target")
                    DisplayUtility.FillDataGridView(dgv_Database, Sweet.lollipop.target_proteoform_community.theoretical_proteoforms.Select(t => new DisplayTheoreticalProteoform(t)));
                else
                    DisplayUtility.FillDataGridView(dgv_Database, Sweet.lollipop.decoy_proteoform_communities[table].theoretical_proteoforms.Select(t => new DisplayTheoreticalProteoform(t)));
            }
            DisplayTheoreticalProteoform.FormatTheoreticalProteoformTable(dgv_Database);
        }

        private void cmb_empty_TextChanged(object sender, TextChangedEventArgs e) { }

        #endregion Private Methods

        #region Public Methods

        public void load_dgv()
        {
            DisplayUtility.FillDataGridView(dgv_Database, Sweet.lollipop.target_proteoform_community.theoretical_proteoforms.Select(t => new DisplayTheoreticalProteoform(t)));
            initialize_table_bindinglist();
            DisplayTheoreticalProteoform.FormatTheoreticalProteoformTable(dgv_Database);
        }

        public List<DataTable> SetTables()
        {
            DataTables = new List<DataTable>
            {
                DisplayTheoreticalProteoform.FormatTheoreticalProteoformTable(Sweet.lollipop.target_proteoform_community.theoretical_proteoforms.Select(t => new DisplayTheoreticalProteoform(t)).ToList(), "TargetDatabase"),
                DisplayUnlocalizedModification.FormatUnlocalizedModificationTable(Sweet.lollipop.theoretical_database.unlocalized_lookup.Values.Select(m => new DisplayUnlocalizedModification(m)).ToList(), "UnlocalizedModifications")
            };
            foreach (KeyValuePair<string, ProteoformCommunity> decoy_community in Sweet.lollipop.decoy_proteoform_communities)
            {
                DataTables.Add(DisplayTheoreticalProteoform.FormatTheoreticalProteoformTable(decoy_community.Value.theoretical_proteoforms.Select(t => new DisplayTheoreticalProteoform(t)).ToList(), decoy_community.Key));
            }
            return DataTables;
        }

        public void FillDataBaseTable(string table)
        {
            if (table == "Target")
                DisplayUtility.FillDataGridView(dgv_Database, Sweet.lollipop.target_proteoform_community.theoretical_proteoforms.Select(t => new DisplayTheoreticalProteoform(t)));
            else if (Sweet.lollipop.decoy_proteoform_communities.ContainsKey(table))
                DisplayUtility.FillDataGridView(dgv_Database, Sweet.lollipop.decoy_proteoform_communities[table].theoretical_proteoforms.Select(t => new DisplayTheoreticalProteoform(t)));
            DisplayTheoreticalProteoform.FormatTheoreticalProteoformTable(dgv_Database);
        }

        public void initialize_table_bindinglist()
        {
            List<string> databases = new List<string> { "Target" };
            if (Sweet.lollipop.decoy_proteoform_communities.Keys.Count > 0)
            {
                foreach (string name in Sweet.lollipop.decoy_proteoform_communities.Keys)
                {
                    databases.Add(name);
                }
            }
            cmbx_DisplayWhichDB.ItemsSource = new BindingList<string>(databases);
            cb_useRandomSeed.IsChecked = Sweet.lollipop.useRandomSeed_decoys;
            nud_randomSeed.Value = Sweet.lollipop.randomSeed_decoys;
            cmbx_DisplayWhichDB.SelectedIndex = 0;
        }

        public void InitializeParameterSet()
        {
            //btn_NeuCode_Lt.IsChecked = Sweet.lollipop.neucode_labeled;
            //btn_NaturalIsotopes.IsChecked = !Sweet.lollipop.neucode_labeled;

            nUD_MaxPTMs.Minimum = 0;
            nUD_MaxPTMs.Maximum = 5;
            nUD_MaxPTMs.Value = Sweet.lollipop.max_ptms;

            nUD_NumDecoyDBs.Minimum = 0;
            nUD_NumDecoyDBs.Maximum = 50;
            nUD_NumDecoyDBs.Value = Sweet.lollipop.decoy_databases;

            nUD_MinPeptideLength.Minimum = 0;
            nUD_MinPeptideLength.Maximum = 20;
            nUD_MinPeptideLength.Value = Sweet.lollipop.min_peptide_length;

            cb_average_mass.IsChecked = Sweet.lollipop.use_average_mass;
            ckbx_combineIdenticalSequences.IsChecked = Sweet.lollipop.combine_identical_sequences;
            ckbx_combineTheoreticalsByMass.IsChecked = Sweet.lollipop.combine_theoretical_proteoforms_byMass;
            cb_limitLargePtmSets.IsChecked = Sweet.lollipop.limit_triples_and_greater;
            cb_useRandomSeed.IsChecked = Sweet.lollipop.useRandomSeed_decoys;
            nud_randomSeed.Value = Sweet.lollipop.randomSeed_decoys;
            ckbx_OxidMeth.IsChecked = Sweet.lollipop.methionine_oxidation;
            ckbx_Meth_Cleaved.IsChecked = Sweet.lollipop.methionine_cleavage;
            ckbx_Carbam.IsChecked = Sweet.lollipop.carbamidomethylation;
            ckbx_combineIdenticalSequences.IsChecked = Sweet.lollipop.combine_identical_sequences;
            ckbx_combineTheoreticalsByMass.IsChecked = Sweet.lollipop.combine_theoretical_proteoforms_byMass;

            tb_modTypesToExclude.Text =
                String.Join(",", Sweet.lollipop.mod_types_to_exclude);

            tb_tableFilter.TextChanged -= tb_tableFilter_TextChanged;
            tb_tableFilter.Text = "";
            tb_tableFilter.TextChanged += tb_tableFilter_TextChanged;

            tb_modTableFilter.TextChanged -= tb_modTableFilter_TextChanged;
            tb_modTableFilter.Text = "";
            tb_modTableFilter.TextChanged += tb_modTableFilter_TextChanged;

            tb_totalTheoreticalProteoforms.Text = Sweet.lollipop.target_proteoform_community.theoretical_proteoforms.Length.ToString();
        }

        public void RunTheGamut(bool full_run)
        {
            if (ReadyToRunTheGamut())
            {
                ClearListsTablesFigures(true);
                Sweet.lollipop.theoretical_database.get_theoretical_proteoforms(Environment.CurrentDirectory);
                FillTablesAndCharts();
                if (!full_run && Sweet.lollipop.bottomupReader.bad_ptms.Count() > 0)
                {
                    MessageBox.Show("Warning: the following PTMs in the .mzid file were not matched with any PTMs in the theoretical database: " +
                        string.Join(", ", Sweet.lollipop.bottomupReader.bad_ptms.Distinct()));
                }
            }
            else
            {
                MessageBox.Show("Please list at least one protein database.");
            }
        }

        public bool ReadyToRunTheGamut()
        {
            return Sweet.lollipop.get_files(Sweet.lollipop.input_files, Purpose.ProteinDatabase).Count() > 0;
        }

        public void ClearListsTablesFigures(bool clear_following)
        {
            dgv_Database.DataSource = null;
            dgv_Database.Rows.Clear();
            dgv_loadFiles.DataSource = null;
            dgv_loadFiles.Rows.Clear();
            dgv_unlocalizedModifications.DataSource = null;
            dgv_unlocalizedModifications.Rows.Clear();
            tb_modTableFilter.Clear();
            tb_tableFilter.Clear();
            tb_totalTheoreticalProteoforms.Clear();
            if (clear_following)
            {
                for (int i = this.MDIParent.forms.IndexOf(this) + 1; i < (this.MDIParent).forms.Count; i++)
                {
                    ISweetForm sweet = (this.MDIParent).forms[i];
                    if (sweet as RawExperimentalComponents == null && (Sweet.lollipop.target_proteoform_community.experimental_proteoforms.Any(e => e.topdown_id) || sweet as AggregatedProteoforms == null))
                    {
                        sweet.ClearListsTablesFigures(false);
                    }
                }
            }
        }

        public void FillTablesAndCharts()
        {
            reload_database_list();
            DisplayUtility.FillDataGridView(dgv_Database, Sweet.lollipop.target_proteoform_community.theoretical_proteoforms.Select(t => new DisplayTheoreticalProteoform(t)));
            initialize_table_bindinglist();
            DisplayTheoreticalProteoform.FormatTheoreticalProteoformTable(dgv_Database);
            DisplayUtility.FillDataGridView(dgv_unlocalizedModifications, Sweet.lollipop.theoretical_database.unlocalized_lookup.Values.Select(m => new DisplayUnlocalizedModification(m)));
            DisplayUnlocalizedModification.FormatUnlocalizedModificationTable(dgv_unlocalizedModifications);
            tb_totalTheoreticalProteoforms.Text = Sweet.lollipop.target_proteoform_community.theoretical_proteoforms.Length.ToString();
        }

        #endregion Public Methods

        #region CHECKBOXES Private Methods

        private void ckbx_combineIdenticalSequences_CheckedChanged(object sender, RoutedEventArgs e)
        {
            Sweet.lollipop.combine_identical_sequences = (bool)ckbx_combineIdenticalSequences.IsChecked;
        }

        private void ckbx_combineTheoreticalsByMass_CheckedChanged(object sender, RoutedEventArgs e)
        {
            Sweet.lollipop.combine_theoretical_proteoforms_byMass = (bool)ckbx_combineTheoreticalsByMass.IsChecked;
        }

        private void ckbx_OxidMeth_CheckedChanged(object sender, RoutedEventArgs e)
        {
            Sweet.lollipop.methionine_oxidation = (bool)ckbx_OxidMeth.IsChecked;
        }

        private void ckbx_Carbam_CheckedChanged(object sender, RoutedEventArgs e)
        {
            Sweet.lollipop.carbamidomethylation = (bool)ckbx_Carbam.IsChecked;
        }

        private void ckbx_Meth_Cleaved_CheckedChanged(object sender, RoutedEventArgs e)
        {
            Sweet.lollipop.methionine_cleavage = (bool)ckbx_Meth_Cleaved.IsChecked;
        }

        private void nUD_MaxPTMs_ValueChanged(object sender, EventArgs e)
        {
            Sweet.lollipop.max_ptms = Convert.ToInt32(nUD_MaxPTMs.Value);
        }

        private void nUD_NumDecoyDBs_ValueChanged(object sender, EventArgs e)
        {
            Sweet.lollipop.decoy_databases = Convert.ToInt32(nUD_NumDecoyDBs.Value);
        }

        private void nUD_MinPeptideLength_ValueChanged(object sender, EventArgs e)
        {
            Sweet.lollipop.min_peptide_length = Convert.ToInt32(nUD_MinPeptideLength.Value);
        }

        private void cb_limitLargePtmSets_CheckedChanged(object sender, RoutedEventArgs e)
        {
            Sweet.lollipop.limit_triples_and_greater = (bool)cb_limitLargePtmSets.IsChecked;
        }

        #endregion CHECKBOXES Private Methods

        #region LOAD DATABASES GRID VIEW Private Methods

        private void dgv_loadFiles_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            drag_drop(e, cmb_loadTable, dgv_loadFiles);
         
        }

        private void dgv_loadFiles_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            e.Effect = System.Windows.Forms.DragDropEffects.All;
        }

        private void drag_drop(System.Windows.Forms.DragEventArgs e, ComboBox cmb, System.Windows.Forms.DataGridView dgv)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            Sweet.lollipop.enter_input_files(files, Lollipop.acceptable_extensions[cmb.SelectedIndex], Lollipop.file_types[cmb.SelectedIndex], Sweet.lollipop.input_files, true);
            DisplayUtility.FillDataGridView(dgv, Sweet.lollipop.get_files(Sweet.lollipop.input_files, Lollipop.file_types[cmb.SelectedIndex]).Select(f => new DisplayInputFile(f)));
            DisplayInputFile.FormatInputFileTable(dgv, Lollipop.file_types[cmb.SelectedIndex]);
        }

        public void reload_database_list()
        {
            cmb_loadTable.Items.Clear();
            // Lollipop.file_lists.ToList().ForEach(itm => cmb_loadTable.Items.Add(itm));//might be a problem // edit: it WAS a problem
            for (int i = 0; i < 4; i++)
                cmb_loadTable.Items.Add(Lollipop.file_lists[i]);
            cmb_loadTable.SelectedIndex = 2;
            DisplayUtility.FillDataGridView(dgv_loadFiles, Sweet.lollipop.get_files(Sweet.lollipop.input_files, Lollipop.file_types[cmb_loadTable.SelectedIndex]).Select(f => new DisplayInputFile(f)));
            DisplayInputFile.FormatInputFileTable(dgv_loadFiles, Lollipop.file_types[cmb_loadTable.SelectedIndex]);
            initialize_table_bindinglist();
        }

        private void tb_tableFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<TheoreticalProteoform> theoreticals_to_display = cmbx_DisplayWhichDB.SelectedItem.ToString() == "Target" ?
                 Sweet.lollipop.target_proteoform_community.theoretical_proteoforms.ToList()
                 : Sweet.lollipop.decoy_proteoform_communities[cmbx_DisplayWhichDB.SelectedItem.ToString()].theoretical_proteoforms.ToList();
            IEnumerable<object> selected_theoreticals = tb_tableFilter.Text == "" ?
                theoreticals_to_display :
                ExtensionMethods.filter(theoreticals_to_display, tb_tableFilter.Text);
            DisplayUtility.FillDataGridView(dgv_Database, selected_theoreticals.OfType<TheoreticalProteoform>().Select(t => new DisplayTheoreticalProteoform(t)));
            DisplayTheoreticalProteoform.FormatTheoreticalProteoformTable(dgv_Database);
        }

        Regex substituteWhitespace = new Regex(@"\s+");
        private void tb_modTypesToExclude_TextChanged(object sender, TextChangedEventArgs e)
        {
            Sweet.lollipop.mod_types_to_exclude = substituteWhitespace.Replace(tb_modTypesToExclude.Text, "").Split(',');
        }
        /*
        private void btn_downloadUniProtPtmList_Click(object sender, EventArgs e)
        {
            Lollipop.enter_uniprot_ptmlist(Environment.CurrentDirectory);
            DisplayUtility.FillDataGridView(dgv_loadFiles, Sweet.lollipop.get_files(Sweet.lollipop.input_files, Lollipop.file_types[cmb_loadTable.SelectedIndex]).Select(f => new DisplayInputFile(f)));
            DisplayInputFile.FormatInputFileTable(dgv_loadFiles, Lollipop.file_types[cmb_loadTable.SelectedIndex]);
            btn_downloadUniProtPtmList.IsEnabled = false;
            SetMakeDatabaseButton();
        }*/

        #endregion LOAD DATABASES GRID VIEW Private Methods

        #region ADD/CLEAR Private Methods

        private void btn_addFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = cmb_loadTable.SelectedItem.ToString();
            openFileDialog.Filter = Lollipop.file_filters[cmb_loadTable.SelectedIndex];
            openFileDialog.Multiselect = true;
            
            if ((bool)openFileDialog.ShowDialog())
            {
                // if (DisplayUtility.CheckForProteinFastas(cmb_loadTable, openFileDialog.FileNames)) return; // todo: implement protein fasta usage
                Sweet.lollipop.enter_input_files(openFileDialog.FileNames, Lollipop.acceptable_extensions[cmb_loadTable.SelectedIndex], Lollipop.file_types[cmb_loadTable.SelectedIndex], Sweet.lollipop.input_files, true);
            }

            DisplayUtility.FillDataGridView(dgv_loadFiles, Sweet.lollipop.get_files(Sweet.lollipop.input_files, Lollipop.file_types[cmb_loadTable.SelectedIndex]).Select(f => new DisplayInputFile(f)));
            DisplayInputFile.FormatInputFileTable(dgv_loadFiles, Lollipop.file_types[cmb_loadTable.SelectedIndex]);
           
        }

        private void btn_clearFiles_Click(object sender, RoutedEventArgs e)
        {
            List<InputFile> files_to_remove = Sweet.lollipop.get_files(Sweet.lollipop.input_files, Lollipop.file_types[cmb_loadTable.SelectedIndex]).ToList();
            Sweet.save_actions.RemoveAll(a => files_to_remove.Any(f => a.Contains(f.complete_path)));
            Sweet.lollipop.input_files = Sweet.lollipop.input_files.Except(files_to_remove).ToList();
            DisplayUtility.FillDataGridView(dgv_loadFiles, Sweet.lollipop.get_files(Sweet.lollipop.input_files, Lollipop.file_types[cmb_loadTable.SelectedIndex]).Select(f => new DisplayInputFile(f)));
            DisplayInputFile.FormatInputFileTable(dgv_loadFiles, Lollipop.file_types[cmb_loadTable.SelectedIndex]);
            //SetMakeDatabaseButton();
        }

        #endregion ADD/CLEAR Private Methods

        #region Modification Names Private Methods

        private void btn_saveModNames_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog d = new SaveFileDialog();
            d.Title = "Modification Names";
            d.Filter = "Modification Names (*.modnames) | *.modnames";

            if (!(bool) d.ShowDialog())
                return;

            Sweet.lollipop.theoretical_database.save_unlocalized_names(d.FileName);
        }

        private void btn_amendModNames_Click(object sender, RoutedEventArgs e)
        {
            Sweet.lollipop.theoretical_database.amend_unlocalized_names(Path.Combine(Environment.CurrentDirectory, "Mods", "stored_mods.modnames"));
            MessageBox.Show("Successfully amended stored modification names.", "Amend Stored Mod Names");
        }

        private void btn_loadModNames_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Title = "Modification Names";
            d.Filter = "Modification Names (*.modnames) | *.modnames";
            d.Multiselect = false;

            if (!(bool)d.ShowDialog())
                return;

            Sweet.lollipop.theoretical_database.load_unlocalized_names(d.FileName);
            DisplayUtility.FillDataGridView(dgv_unlocalizedModifications, Sweet.lollipop.theoretical_database.unlocalized_lookup.Values.Select(m => new DisplayUnlocalizedModification(m)));
            DisplayUnlocalizedModification.FormatUnlocalizedModificationTable(dgv_unlocalizedModifications);
        }

        private void tb_modTableFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            IEnumerable<object> selected_unmods = tb_modTableFilter.Text == "" ?
                Sweet.lollipop.theoretical_database.unlocalized_lookup.Values :
                ExtensionMethods.filter(Sweet.lollipop.theoretical_database.unlocalized_lookup.Values, tb_modTableFilter.Text);
            DisplayUtility.FillDataGridView(dgv_unlocalizedModifications, selected_unmods.OfType<UnlocalizedModification>().Select(u => new DisplayUnlocalizedModification(u)));
            DisplayUnlocalizedModification.FormatUnlocalizedModificationTable(dgv_unlocalizedModifications);
        }

        #endregion Modification Names Private Methods

        private void cb_useRandomSeed_CheckedChanged(object sender, RoutedEventArgs e)
        {
            Sweet.lollipop.useRandomSeed_decoys = (bool)cb_useRandomSeed.IsChecked;
        }

        private void nud_randomSeed_ValueChanged(object sender, EventArgs e)
        {
            Sweet.lollipop.randomSeed_decoys = Convert.ToInt32(nud_randomSeed.Value);
        }

        private void cb_mostAbundantMass_CheckedChanged(object sender, EventArgs e)
        {
            Sweet.lollipop.use_average_mass = (bool)cb_average_mass.IsChecked;
        }

        public ProteoformSweet MDIParent { get; set; }

        private void Cmb_loadTable_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // We do this to prevent firing TabControl's SelectionChanged event unintendedly
            // Reference: https://stackoverflow.com/questions/3659858/in-c-sharp-wpf-why-is-my-tabcontrols-selectionchanged-event-firing-too-often
            e.Handled = true;
        }
    }
}
