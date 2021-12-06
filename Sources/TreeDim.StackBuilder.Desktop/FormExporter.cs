﻿#region Using directives
using System;
using System.Windows.Forms;
using System.IO;

using log4net;
using Sharp3D.Math.Core;

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

            // set folding strategy to XML ? *** either XML or CS ***
            textEditorControl.FoldingStrategy = "XML";
            textEditorControl.SetHighlighting("XML");

            FillFormatComboBox();
            cbCoordinates.SelectedIndex = Properties.Settings.Default.ExportCoordinatesMode;
            try { DockingOffsets = Vector3D.Parse(Properties.Settings.Default.DockingOffsets); } catch (Exception /*ex*/) { }

            if (Analysis is AnalysisCasePallet analysisCasePallet)
            {
                RobotPreparation = new RobotPreparation(analysisCasePallet);
                RobotPreparation.LayerModified += RobotPreparationModified;
                uCtrlConveyorSettings.BoxProperties = analysisCasePallet.Content as PackableBrick;
                uCtrlConveyorSettings.ValueChanged += OnInputChanged;


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
            Properties.Settings.Default.DockingOffsets = DockingOffsets.ToString();
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
            // sanity check
            if (null == RobotPreparation) return;
            try
            {
                RobotPreparation.AngleItem = uCtrlConveyorSettings.AngleCase;
                RobotPreparation.AngleGrabber = uCtrlConveyorSettings.AngleGrabber;
                RobotPreparation.DockingOffsets = DockingOffsets;

                var exporter = ExporterRobot.GetByName(cbFileFormat.SelectedItem.ToString());
                if (exporter.UseRobotPreparation)
                    RobotPreparationModified();
                else
                {
                    Stream stream = new MemoryStream();
                    exporter.PositionCoordinateMode = cbCoordinates.SelectedIndex == 1 ? ExporterRobot.CoordinateMode.CM_COG : ExporterRobot.CoordinateMode.CM_CORNER;
                    exporter.Export(Analysis, ref stream);

                    // to text edit control
                    using (StreamReader reader = new StreamReader(stream))
                        textEditorControl.Text = reader.ReadToEnd();
                }
                textEditorControl.Update();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
            finally
            {
                _log.Info(textEditorControl.Text);
            }
        }
        private void RobotPreparationModified()
        {
            if (null == RobotPreparation)
                return;
            try
            {

                Stream stream = new MemoryStream();
                var exporter = ExporterRobot.GetByName(cbFileFormat.SelectedItem.ToString());
                exporter.Export(RobotPreparation, ref stream);
                // to text edit control
                StreamReader reader = new StreamReader(stream);
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
            if (DesignMode) return;
            if (null == CurrentExporter) return;

            pbBrandLogo.Image = CurrentExporter.BrandLogo;
            
            // save format 
            FormatName = CurrentExporter.Name;
            tabCtrlFeatures.TabPages.Clear();
            if (CurrentExporter.UseCoordinateSelector)
                tabCtrlFeatures.TabPages.Add(tabPageSettings);
            if (CurrentExporter.UseAngleSelector)
                tabCtrlFeatures.TabPages.Add(tabPageAngles);
            if (CurrentExporter.UseRobotPreparation)
                tabCtrlFeatures.TabPages.Add(tabPageLayerPrep);
            if (CurrentExporter.UseDockingOffsets)
                tabCtrlFeatures.TabPages.Add(tabPageDockingOffsets);
 
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
        #region Private properties
        private ExporterRobot CurrentExporter => ExporterRobot.GetByName(cbFileFormat.SelectedItem.ToString());
        private int SelectedFormatIndex => cbFileFormat.SelectedIndex;
        private string SelectedFormatString => cbFileFormat.SelectedItem.ToString();
        private string FormatName
        {
            get => Properties.Settings.Default.ExportFormatName;
            set => Properties.Settings.Default.ExportFormatName = value;
        }
        private Vector3D DockingOffsets
        {
            get => uCtrlDockingOffset.Value;
            set => uCtrlDockingOffset.Value = value;
        }
        #endregion
        #region Data members
        protected ILog _log = LogManager.GetLogger(typeof(FormExporter));
        public AnalysisLayered Analysis { get; set; }
        public RobotPreparation RobotPreparation { get; set; }
        #endregion
    }
}
