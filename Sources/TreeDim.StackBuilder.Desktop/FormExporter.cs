#region Using directives
using System;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using log4net;
using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Exporters;
using System.ComponentModel;
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
            }
            LoadConveyorSettings();
            UpdatePreparationCtrl();

            // fill combo layer types
            FillLayerComboBox();
            OnExportFormatChanged(this, e);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (Analysis is AnalysisCasePallet analysisCasePallet)
                analysisCasePallet.ConveyorSettings = ConveyorSettings;
        }
        private void LoadConveyorSettings()
        {
            PackableBrick packable = null;
            if (Analysis is AnalysisCasePallet analysisCasePallet)
            {
                packable = analysisCasePallet.Content as PackableBrick;
                ConveyorSettings = analysisCasePallet.ConveyorSettings;
            }

            uCtrlConveyorSettings.SetConveyorSettings(packable, ConveyorSettings);
            uCtrlConveyorSettings.ConveyorSettingAddedRemoved += OnSettingAddedRemoved;
        }
        private void OnSettingAddedRemoved(object sender, EventArgs e)
        {
            ConveyorSettings = uCtrlConveyorSettings.ListSettings;
            UpdatePreparationCtrl();
        }
        private void UpdatePreparationCtrl()
        {
            PackableBrick packable = null;
            if (Analysis is AnalysisCasePallet analysisCasePallet)
                packable = analysisCasePallet.Content as PackableBrick;

            layerEditor.SetConveyorSettings(packable, ConveyorSettings);
        }
        public List<ConveyorSetting> ConveyorSettings
        {
            set
            {
                ListConveyorSettings = value;
            }
            get
            {
                if (null == ListConveyorSettings)
                    ListConveyorSettings = new List<ConveyorSetting>();
                if (ListConveyorSettings.Count == 0)
                {
                    ListConveyorSettings.AddRange(
                        new ConveyorSetting[]
                        {
                            new ConveyorSetting(0, 1, 0),
                        }
                        );
                }
                return ListConveyorSettings;
            }
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
            // layers
            cbLayers.Items.Clear();
            for (int i = 0; i < RobotPreparation.LayerTypes.Count; ++i)
            {
                // get layer indexes
                var list = RobotPreparation.ListLayerIndexes;
                // list of occurrence of index
                var result = Enumerable.Range(0, list.Count)
                     .Where(v => list[v] == i)
                     .ToList();
                // join list values & insert 
                cbLayers.Items.Add(string.Join(";", result));
            }
            cbLayers.SelectedIndexChanged += OnSelectedLayerChanged;
            if (RobotPreparation.LayerTypes.Count > 0)
                cbLayers.SelectedIndex = 0;
        }
        private void OnSelectedLayerChanged(object sender, EventArgs e)
        {
            int iSel = cbLayers.SelectedIndex;
            layerEditor.Layer = (-1 != iSel && RobotPreparation.IsValid) ? RobotPreparation.LayerTypes[iSel] : null;
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
                    using (var reader = new StreamReader(stream))
                    {
                        textEditorControl.Text = reader.ReadToEnd();
                    }
                }
                textEditorControl.Update();
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

            // show/hide tabs
            tabCtrlFeatures.TabPages.Clear();
            if (CurrentExporter.UseCoordinateSelector)
                tabCtrlFeatures.TabPages.Add(tabPageSettings);
            if (CurrentExporter.UseAngleSelector)
                tabCtrlFeatures.TabPages.Add(tabPageAngles);
            if (CurrentExporter.UseRobotPreparation)
                tabCtrlFeatures.TabPages.Add(tabPageLayerPrep);
            if (CurrentExporter.UseDockingOffsets)
                tabCtrlFeatures.TabPages.Add(tabPageDockingOffsets);

            // show / hide panel
            textEditorControl.Visible = CurrentExporter.ShowOutput;
            splitContainerVert.Panel2Collapsed = !CurrentExporter.ShowOutput;
            if (CurrentExporter.ShowOutput)
                splitContainerVert.Panel2.Show();
            else
                splitContainerVert.Panel2.Hide();

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
                {
                    if (CurrentExporter.UseDirectExport)
                    {
                        Stream stream = new MemoryStream();
                        CurrentExporter.Export(RobotPreparation, ref stream, true);
                        StreamReader reader = new StreamReader(stream);
                        File.WriteAllText(saveExportFile.FileName, reader.ReadToEnd());
                    }
                    else
                        File.WriteAllLines(saveExportFile.FileName, new string[] { textEditorControl.Text }, System.Text.Encoding.UTF8);
                }
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        #endregion
        #region Private properties
        private ExporterRobot CurrentExporter => ExporterRobot.GetByName(cbFileFormat.SelectedItem.ToString());
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
        private List<ConveyorSetting> ListConveyorSettings = new List<ConveyorSetting>();
        #endregion
    }
}
