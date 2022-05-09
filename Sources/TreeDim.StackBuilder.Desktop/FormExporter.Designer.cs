namespace treeDiM.StackBuilder.Desktop
{
    partial class FormExporter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormExporter));
            this.splitContainerHoriz = new System.Windows.Forms.SplitContainer();
            this.pbBrandLogo = new System.Windows.Forms.PictureBox();
            this.bnSave = new System.Windows.Forms.Button();
            this.bnClose = new System.Windows.Forms.Button();
            this.lbFormat = new System.Windows.Forms.Label();
            this.cbFileFormat = new System.Windows.Forms.ComboBox();
            this.splitContainerVert = new System.Windows.Forms.SplitContainer();
            this.tabCtrlFeatures = new System.Windows.Forms.TabControl();
            this.tabPageSettings = new System.Windows.Forms.TabPage();
            this.cbCoordinates = new System.Windows.Forms.ComboBox();
            this.lbCoordinates = new System.Windows.Forms.Label();
            this.tabPagePickConfigurations = new System.Windows.Forms.TabPage();
            this.uCtrlConveyorSettings = new treeDiM.StackBuilder.Graphics.Controls.UCtrlConveyorSettings();
            this.tabPageLayerPrep = new System.Windows.Forms.TabPage();
            this.splitContainerLayer = new System.Windows.Forms.SplitContainer();
            this.bnRegenerate = new System.Windows.Forms.Button();
            this.cbLayers = new System.Windows.Forms.ComboBox();
            this.lbLayers = new System.Windows.Forms.Label();
            this.layerEditor = new treeDiM.StackBuilder.Graphics.Graphics2DRobotDropEditor();
            this.tabPageDockingOffsets = new System.Windows.Forms.TabPage();
            this.uCtrlDockingOffset = new treeDiM.Basics.UCtrlTriDouble();
            this.textEditorControl = new ICSharpCode.TextEditor.TextEditorControlEx();
            this.saveExportFile = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz)).BeginInit();
            this.splitContainerHoriz.Panel1.SuspendLayout();
            this.splitContainerHoriz.Panel2.SuspendLayout();
            this.splitContainerHoriz.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbBrandLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).BeginInit();
            this.splitContainerVert.Panel1.SuspendLayout();
            this.splitContainerVert.Panel2.SuspendLayout();
            this.splitContainerVert.SuspendLayout();
            this.tabCtrlFeatures.SuspendLayout();
            this.tabPageSettings.SuspendLayout();
            this.tabPagePickConfigurations.SuspendLayout();
            this.tabPageLayerPrep.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLayer)).BeginInit();
            this.splitContainerLayer.Panel1.SuspendLayout();
            this.splitContainerLayer.Panel2.SuspendLayout();
            this.splitContainerLayer.SuspendLayout();
            this.tabPageDockingOffsets.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerHoriz
            // 
            resources.ApplyResources(this.splitContainerHoriz, "splitContainerHoriz");
            this.splitContainerHoriz.Name = "splitContainerHoriz";
            // 
            // splitContainerHoriz.Panel1
            // 
            this.splitContainerHoriz.Panel1.Controls.Add(this.pbBrandLogo);
            this.splitContainerHoriz.Panel1.Controls.Add(this.bnSave);
            this.splitContainerHoriz.Panel1.Controls.Add(this.bnClose);
            this.splitContainerHoriz.Panel1.Controls.Add(this.lbFormat);
            this.splitContainerHoriz.Panel1.Controls.Add(this.cbFileFormat);
            // 
            // splitContainerHoriz.Panel2
            // 
            this.splitContainerHoriz.Panel2.Controls.Add(this.splitContainerVert);
            // 
            // pbBrandLogo
            // 
            resources.ApplyResources(this.pbBrandLogo, "pbBrandLogo");
            this.pbBrandLogo.Name = "pbBrandLogo";
            this.pbBrandLogo.TabStop = false;
            // 
            // bnSave
            // 
            resources.ApplyResources(this.bnSave, "bnSave");
            this.bnSave.Name = "bnSave";
            this.bnSave.UseVisualStyleBackColor = true;
            this.bnSave.Click += new System.EventHandler(this.OnExport);
            // 
            // bnClose
            // 
            resources.ApplyResources(this.bnClose, "bnClose");
            this.bnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnClose.Name = "bnClose";
            this.bnClose.UseVisualStyleBackColor = true;
            // 
            // lbFormat
            // 
            resources.ApplyResources(this.lbFormat, "lbFormat");
            this.lbFormat.Name = "lbFormat";
            // 
            // cbFileFormat
            // 
            this.cbFileFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFileFormat.FormattingEnabled = true;
            resources.ApplyResources(this.cbFileFormat, "cbFileFormat");
            this.cbFileFormat.Name = "cbFileFormat";
            this.cbFileFormat.SelectedIndexChanged += new System.EventHandler(this.OnExportFormatChanged);
            // 
            // splitContainerVert
            // 
            resources.ApplyResources(this.splitContainerVert, "splitContainerVert");
            this.splitContainerVert.Name = "splitContainerVert";
            // 
            // splitContainerVert.Panel1
            // 
            this.splitContainerVert.Panel1.Controls.Add(this.tabCtrlFeatures);
            // 
            // splitContainerVert.Panel2
            // 
            this.splitContainerVert.Panel2.Controls.Add(this.textEditorControl);
            // 
            // tabCtrlFeatures
            // 
            this.tabCtrlFeatures.Controls.Add(this.tabPageSettings);
            this.tabCtrlFeatures.Controls.Add(this.tabPagePickConfigurations);
            this.tabCtrlFeatures.Controls.Add(this.tabPageLayerPrep);
            this.tabCtrlFeatures.Controls.Add(this.tabPageDockingOffsets);
            resources.ApplyResources(this.tabCtrlFeatures, "tabCtrlFeatures");
            this.tabCtrlFeatures.Name = "tabCtrlFeatures";
            this.tabCtrlFeatures.SelectedIndex = 0;
            // 
            // tabPageSettings
            // 
            this.tabPageSettings.Controls.Add(this.cbCoordinates);
            this.tabPageSettings.Controls.Add(this.lbCoordinates);
            resources.ApplyResources(this.tabPageSettings, "tabPageSettings");
            this.tabPageSettings.Name = "tabPageSettings";
            this.tabPageSettings.UseVisualStyleBackColor = true;
            // 
            // cbCoordinates
            // 
            this.cbCoordinates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCoordinates.FormattingEnabled = true;
            this.cbCoordinates.Items.AddRange(new object[] {
            resources.GetString("cbCoordinates.Items"),
            resources.GetString("cbCoordinates.Items1")});
            resources.ApplyResources(this.cbCoordinates, "cbCoordinates");
            this.cbCoordinates.Name = "cbCoordinates";
            this.cbCoordinates.SelectedIndexChanged += new System.EventHandler(this.OnInputChanged);
            // 
            // lbCoordinates
            // 
            resources.ApplyResources(this.lbCoordinates, "lbCoordinates");
            this.lbCoordinates.Name = "lbCoordinates";
            // 
            // tabPagePickConfigurations
            // 
            this.tabPagePickConfigurations.Controls.Add(this.uCtrlConveyorSettings);
            resources.ApplyResources(this.tabPagePickConfigurations, "tabPagePickConfigurations");
            this.tabPagePickConfigurations.Name = "tabPagePickConfigurations";
            this.tabPagePickConfigurations.UseVisualStyleBackColor = true;
            // 
            // uCtrlConveyorSettings
            // 
            this.uCtrlConveyorSettings.AngleCase = 0;
            this.uCtrlConveyorSettings.BoxProperties = null;
            resources.ApplyResources(this.uCtrlConveyorSettings, "uCtrlConveyorSettings");
            this.uCtrlConveyorSettings.GripperAngle = 0;
            this.uCtrlConveyorSettings.ListSettings = null;
            this.uCtrlConveyorSettings.MaxDropNumber = 1;
            this.uCtrlConveyorSettings.Name = "uCtrlConveyorSettings";
            // 
            // tabPageLayerPrep
            // 
            this.tabPageLayerPrep.Controls.Add(this.splitContainerLayer);
            resources.ApplyResources(this.tabPageLayerPrep, "tabPageLayerPrep");
            this.tabPageLayerPrep.Name = "tabPageLayerPrep";
            this.tabPageLayerPrep.UseVisualStyleBackColor = true;
            // 
            // splitContainerLayer
            // 
            resources.ApplyResources(this.splitContainerLayer, "splitContainerLayer");
            this.splitContainerLayer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerLayer.Name = "splitContainerLayer";
            // 
            // splitContainerLayer.Panel1
            // 
            this.splitContainerLayer.Panel1.Controls.Add(this.bnRegenerate);
            this.splitContainerLayer.Panel1.Controls.Add(this.cbLayers);
            this.splitContainerLayer.Panel1.Controls.Add(this.lbLayers);
            // 
            // splitContainerLayer.Panel2
            // 
            this.splitContainerLayer.Panel2.Controls.Add(this.layerEditor);
            // 
            // bnRegenerate
            // 
            resources.ApplyResources(this.bnRegenerate, "bnRegenerate");
            this.bnRegenerate.Name = "bnRegenerate";
            this.bnRegenerate.UseVisualStyleBackColor = true;
            this.bnRegenerate.Click += new System.EventHandler(this.OnRegenerateLayer);
            // 
            // cbLayers
            // 
            this.cbLayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLayers.FormattingEnabled = true;
            resources.ApplyResources(this.cbLayers, "cbLayers");
            this.cbLayers.Name = "cbLayers";
            this.cbLayers.SelectedIndexChanged += new System.EventHandler(this.OnSelectedLayerChanged);
            // 
            // lbLayers
            // 
            resources.ApplyResources(this.lbLayers, "lbLayers");
            this.lbLayers.Name = "lbLayers";
            // 
            // layerEditor
            // 
            resources.ApplyResources(this.layerEditor, "layerEditor");
            this.layerEditor.Layer = null;
            this.layerEditor.Name = "layerEditor";
            // 
            // tabPageDockingOffsets
            // 
            this.tabPageDockingOffsets.Controls.Add(this.uCtrlDockingOffset);
            resources.ApplyResources(this.tabPageDockingOffsets, "tabPageDockingOffsets");
            this.tabPageDockingOffsets.Name = "tabPageDockingOffsets";
            this.tabPageDockingOffsets.UseVisualStyleBackColor = true;
            // 
            // uCtrlDockingOffset
            // 
            resources.ApplyResources(this.uCtrlDockingOffset, "uCtrlDockingOffset");
            this.uCtrlDockingOffset.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.uCtrlDockingOffset.Name = "uCtrlDockingOffset";
            this.uCtrlDockingOffset.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlDockingOffset.ValueX = 0D;
            this.uCtrlDockingOffset.ValueY = 0D;
            this.uCtrlDockingOffset.ValueZ = 0D;
            this.uCtrlDockingOffset.ValueChanged += new treeDiM.Basics.UCtrlTriDouble.ValueChangedDelegate(this.OnInputChanged);
            // 
            // textEditorControl
            // 
            resources.ApplyResources(this.textEditorControl, "textEditorControl");
            this.textEditorControl.FoldingStrategy = "XML";
            this.textEditorControl.Name = "textEditorControl";
            this.textEditorControl.SyntaxHighlighting = "XML";
            // 
            // FormExporter
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnClose;
            this.Controls.Add(this.splitContainerHoriz);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormExporter";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.splitContainerHoriz.Panel1.ResumeLayout(false);
            this.splitContainerHoriz.Panel1.PerformLayout();
            this.splitContainerHoriz.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz)).EndInit();
            this.splitContainerHoriz.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbBrandLogo)).EndInit();
            this.splitContainerVert.Panel1.ResumeLayout(false);
            this.splitContainerVert.Panel2.ResumeLayout(false);
            this.splitContainerVert.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).EndInit();
            this.splitContainerVert.ResumeLayout(false);
            this.tabCtrlFeatures.ResumeLayout(false);
            this.tabPageSettings.ResumeLayout(false);
            this.tabPageSettings.PerformLayout();
            this.tabPagePickConfigurations.ResumeLayout(false);
            this.tabPageLayerPrep.ResumeLayout(false);
            this.splitContainerLayer.Panel1.ResumeLayout(false);
            this.splitContainerLayer.Panel1.PerformLayout();
            this.splitContainerLayer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLayer)).EndInit();
            this.splitContainerLayer.ResumeLayout(false);
            this.tabPageDockingOffsets.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerHoriz;
        private System.Windows.Forms.ComboBox cbCoordinates;
        private System.Windows.Forms.Label lbCoordinates;
        private System.Windows.Forms.Label lbFormat;
        private System.Windows.Forms.ComboBox cbFileFormat;
        private ICSharpCode.TextEditor.TextEditorControlEx textEditorControl;
        private System.Windows.Forms.Button bnSave;
        private System.Windows.Forms.Button bnClose;
        private System.Windows.Forms.SaveFileDialog saveExportFile;
        private System.Windows.Forms.SplitContainer splitContainerVert;
        private Graphics.Graphics2DRobotDropEditor layerEditor;
        private System.Windows.Forms.SplitContainer splitContainerLayer;
        private System.Windows.Forms.Label lbLayers;
        private System.Windows.Forms.ComboBox cbLayers;
        private System.Windows.Forms.TabControl tabCtrlFeatures;
        private System.Windows.Forms.TabPage tabPageSettings;
        private System.Windows.Forms.TabPage tabPageLayerPrep;
        private System.Windows.Forms.PictureBox pbBrandLogo;
        private System.Windows.Forms.TabPage tabPagePickConfigurations;
        private Graphics.Controls.UCtrlConveyorSettings uCtrlConveyorSettings;
        private System.Windows.Forms.TabPage tabPageDockingOffsets;
        private treeDiM.Basics.UCtrlTriDouble uCtrlDockingOffset;
        private System.Windows.Forms.Button bnRegenerate;
    }
}