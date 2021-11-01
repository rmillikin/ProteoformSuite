using ProteoformSuiteInternal;
using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;
using ProteoformExplorer.Core;
using ProteoformExplorer.GuiFunctions;
using ProteoformExplorer.Wpf;
using System.Linq;
using System;

namespace ProteoWPFSuite
{
    /// <summary>
    /// Interaction logic for SpectrumViewer.xaml
    /// </summary>
    public partial class SpectrumViewer : UserControl,
        ITabbedMDI, ISweetForm
    {
        public static string DisplayedText = @"Test";

        public SpectrumViewer()
        {
            InitializeComponent();

            var dataLoadingPage = new DataLoading();
            _NavigationFrame.Navigate(dataLoadingPage);
            Dashboard.ProteoformVisualization = true;
        }

        public ProteoformSweet MDIParent { get; set; }

        public List<DataTable> DataTables { get; private set; }

        public void InitializeFromProteoformSuiteData(ProteoformCommunity proteoformCommunity)
        {
            
        }

        public void ClearListsTablesFigures(bool clear_following_forms)
        {

        }

        public void FillTablesAndCharts()
        {

        }

        public void InitializeParameterSet()
        {

        }

        public bool ReadyToRunTheGamut()
        {
            return true;
        }

        public void RunTheGamut(bool full_run)
        {

        }

        public List<DataTable> SetTables()
        {
            return null;
        }

        private void AddFiles()
        {

        }
    }
}
