#region Using directives
using System;
using System.Windows.Forms;
using System.IO;

using log4net;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Exporters;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormExporter : Form
    {
        #region Constructor
        public FormExporter()
        {
            InitializeComponent();
        }
        #endregion
        #region Override form
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            FillFormatComboBox();
            cbCoordinates.SelectedIndex = Properties.Settings.Default.ExportCoordinatesMode;

            if (Analysis is AnalysisCasePallet analysisCasePallet)
            {
                RobotPreparation = new RobotPreparation(Analysis as AnalysisCasePallet);
                RobotPreparation.LayerModified += RobotPreparationModified;
            }
            // fill combo layer types
            FillLayerComboBox();
            OnExportFormatChanged(this, e);
        }

        private void FillFormatComboBox()
        {
            // fill combo box with available exporters
            cbFileFormat.Items.Clear();
            foreach (var exporter in ExporterRobot.GetRobotExporters())
                cbFileFormat.Items.Add(exporter.Name);
            // select depending on FormatName
            int iSel = cbFileFormat.Items.Count - 1;
            if (iSel > 0)
            { 
                int iFormat = cbFileFormat.FindStringExact(FormatName);
                iSel = iFormat != -1 ? iFormat : 0;                
            }
            cbFileFormat.SelectedIndex = iSel;
        }

        private void FillLayerComboBox()
        { 
            for (int i = 0; i < RobotPreparation.LayerTypes.Count; ++i)
                cbLayers.Items.Add($"{i+1}");
            cbLayers.SelectedIndexChanged += OnSelectedLayerChanged;
            if (RobotPreparation.LayerTypes.Count > 0)
                cbLayers.SelectedIndex = 0;
        }
        private void OnSelectedLayerChanged(object sender, EventArgs e)
        {
            int iSel = cbLayers.SelectedIndex;
            if (-1 != iSel)
                layerEditor.Layer = RobotPreparation.LayerTypes[iSel];
            layerEditor.Invalidate();
            RobotPreparation.Update();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Properties.Settings.Default.ExportFormatName = cbFileFormat.SelectedItem.ToString();
            Properties.Settings.Default.ExportCoordinatesMode = cbCoordinates.SelectedIndex;
            Properties.Settings.Default.Save();
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Escape)
                return false;
            return base.ProcessDialogKey(keyData);
        }
        #endregion
        #region Compute
        private void Recompute()
        {
            try
            {
                Stream stream = new MemoryStream();
                var exporter = ExporterRobot.GetByName(cbFileFormat.SelectedItem.ToString()) as ExporterRobot;
                exporter.PositionCoordinateMode = cbCoordinates.SelectedIndex == 1 ? ExporterRobot.CoordinateMode.CM_COG : ExporterRobot.CoordinateMode.CM_CORNER;
                if (exporter.UseRobotPreparation)
                    exporter.Export(RobotPreparation, ref stream);
                else
                    exporter.Export(Analysis, ref stream);

                // to text edit control
                using (StreamReader reader = new StreamReader(stream))
                    textEditorControl.Text = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }

        private void RobotPreparationModified()
        {
            if (null == RobotPreparation)
                return;
            try
            {

                Stream stream = new MemoryStream();
                var exporter = ExporterRobot.GetByName(cbFileFormat.SelectedItem.ToString()) as ExporterRobot;
                exporter.Export(RobotPreparation, ref stream);
                // to text edit control
                using (StreamReader reader = new StreamReader(stream))
                    textEditorControl.Text = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        #endregion
        #region EventHandler
        private void OnExportFormatChanged(object sender, EventArgs e)
        {
            if (null == CurrentExporter)
            {

                return;
            }

            // save format 
            FormatName = CurrentExporter.Name;

            layerEditor.Visible = CurrentExporter.UseRobotPreparation;
            lbLayers.Visible = CurrentExporter.UseRobotPreparation;
            cbLayers.Visible = CurrentExporter.UseRobotPreparation;

            lbCoordinates.Visible = CurrentExporter.ShowSelectorCoordinateMode;
            cbCoordinates.Visible = CurrentExporter.ShowSelectorCoordinateMode;


            try
            {
                // set folding strategy to XML ?
                if (string.Equals(CurrentExporter.Extension, "xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    textEditorControl.FoldingStrategy = "XML";
                    textEditorControl.SetHighlighting("XML");
                }
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }

            OnInputChanged(sender, e);
        }
        private void OnInputChanged(object sender, EventArgs e)
        {
            Recompute();
        }
        private void OnExport(object sender, EventArgs e)
        {
            try
            {
                var exporter = CurrentExporter;
                if (null == exporter) return;

                saveExportFile.CheckFileExists = false;
                saveExportFile.Filter = $"(*.{exporter.Extension})|*.{exporter.Extension}|All files (*.*)|*.*";
                saveExportFile.DefaultExt = exporter.Extension;

                if (DialogResult.OK == saveExportFile.ShowDialog())
                    File.WriteAllLines(saveExportFile.FileName, new string[] { textEditorControl.Text }, System.Text.Encoding.UTF8);
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        #endregion
        #region Public properties
        private ExporterRobot CurrentExporter => ExporterRobot.GetByName(cbFileFormat.SelectedItem.ToString());
        private int SelectedFormatIndex => cbFileFormat.SelectedIndex;
        private string SelectedFormatString => cbFileFormat.SelectedItem.ToString();
        #endregion
        #region Data members
        protected ILog _log = LogManager.GetLogger(typeof(FormExporter));
        private string FormatName
        {
            get => Properties.Settings.Default.ExportFormatName;
            set => Properties.Settings.Default.ExportFormatName = value;
        }
        public AnalysisLayered Analysis { get; set; }
        public RobotPreparation RobotPreparation { get; set; }
        #endregion


    }
}
