#region Using directives
using System;

using treeDiM.StackBuilder.Graphics.Properties;
using treeDiM.StackBuilder.Exporters;

using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class OptionPanelRobot : GLib.Options.OptionsPanel
    {
        #region Constructor
        public OptionPanelRobot()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            FillComboExporters();            
        }
        #endregion

        #region Helpers
        private void FillComboExporters()
        { 
            cbExporter.Items.Clear();
            cbExporter.Items.Add(Resources.ID_ALL);
            foreach (var exporter in ExporterRobot.RobotExporters)
                cbExporter.Items.Add(exporter.Name);

            int iSel = cbExporter.Items.IndexOf(ExporterRobot.DefaultName);
            cbExporter.SelectedIndex = iSel != -1 ? iSel : 0;            
        }
        #endregion

        #region Handlers
        private void OnLoadedExporterChanged(object sender, EventArgs e)
        {
            ExporterRobot.SetDefault(cbExporter.Text);

            var exporter = ExporterRobot.GetByName(cbExporter.Text);
            pbExporterBrand.Visible = null != exporter;
            if (null != exporter)
                pbExporterBrand.Image = exporter.BrandLogo;
        }
        #endregion
    }
}
