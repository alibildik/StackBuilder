﻿#region Using directives
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
            textEditorControl.SetFoldingStrategy("XML");

            if (string.IsNullOrEmpty(FormatName))
                FormatName = Properties.Settings.Default.ExportFormatName;

            int iFormat = cbFileFormat.FindStringExact(FormatName);
            cbFileFormat.SelectedIndex = iFormat > -1 ? iFormat : 0;
            cbCoordinates.SelectedIndex = Properties.Settings.Default.ExportCoordinatesMode;

            // analysis to layered
            RobotPreparation = new RobotPreparation(Analysis as AnalysisCasePallet);
            layerEditor.Layer = RobotPreparation.LayerTypes[0];
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
                var exporter = ExporterFactory.GetExporterByName(cbFileFormat.SelectedItem.ToString());
                exporter.PositionCoordinateMode = cbCoordinates.SelectedIndex == 1 ? Exporter.CoordinateMode.CM_COG : Exporter.CoordinateMode.CM_CORNER;
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
        private void OnInputChanged(object sender, EventArgs e)
        {
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
        private Exporter CurrentExporter => ExporterFactory.GetExporterByName(cbFileFormat.SelectedItem.ToString());
        private int SelectedFormatIndex => cbFileFormat.SelectedIndex;
        private string SelectedFormatString => cbFileFormat.SelectedItem.ToString();
        #endregion

        #region Data members
        protected ILog _log = LogManager.GetLogger(typeof(FormExporter));
        public string FormatName { get; set; }
        public AnalysisLayered Analysis { get; set; }
        public RobotPreparation RobotPreparation { get; set; }
        #endregion
    }
}
