﻿namespace treeDiM.StackBuilder.Desktop
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
            this.bnSave = new System.Windows.Forms.Button();
            this.bnClose = new System.Windows.Forms.Button();
            this.cbCoordinates = new System.Windows.Forms.ComboBox();
            this.lbCoordinates = new System.Windows.Forms.Label();
            this.lbFormat = new System.Windows.Forms.Label();
            this.cbFileFormat = new System.Windows.Forms.ComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainerLayer = new System.Windows.Forms.SplitContainer();
            this.cbLayers = new System.Windows.Forms.ComboBox();
            this.lbLayers = new System.Windows.Forms.Label();
            this.layerEditor = new treeDiM.StackBuilder.Graphics.Graphics2DRobotDropEditor();
            this.textEditorControl = new ICSharpCode.TextEditor.TextEditorControlEx();
            this.saveExportFile = new System.Windows.Forms.SaveFileDialog();
            this.tabCtrlFeatures = new System.Windows.Forms.TabControl();
            this.tabPageSettings = new System.Windows.Forms.TabPage();
            this.tabPageLayerPrep = new System.Windows.Forms.TabPage();
            this.tabPageAngles = new System.Windows.Forms.TabPage();
            this.pbBrandLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz)).BeginInit();
            this.splitContainerHoriz.Panel1.SuspendLayout();
            this.splitContainerHoriz.Panel2.SuspendLayout();
            this.splitContainerHoriz.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLayer)).BeginInit();
            this.splitContainerLayer.Panel1.SuspendLayout();
            this.splitContainerLayer.Panel2.SuspendLayout();
            this.splitContainerLayer.SuspendLayout();
            this.tabCtrlFeatures.SuspendLayout();
            this.tabPageSettings.SuspendLayout();
            this.tabPageLayerPrep.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbBrandLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerHoriz
            // 
            this.splitContainerHoriz.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerHoriz.Location = new System.Drawing.Point(0, 0);
            this.splitContainerHoriz.Name = "splitContainerHoriz";
            this.splitContainerHoriz.Orientation = System.Windows.Forms.Orientation.Horizontal;
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
            this.splitContainerHoriz.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainerHoriz.Size = new System.Drawing.Size(800, 594);
            this.splitContainerHoriz.SplitterDistance = 120;
            this.splitContainerHoriz.TabIndex = 0;
            // 
            // bnSave
            // 
            this.bnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnSave.Location = new System.Drawing.Point(713, 37);
            this.bnSave.Name = "bnSave";
            this.bnSave.Size = new System.Drawing.Size(75, 23);
            this.bnSave.TabIndex = 5;
            this.bnSave.Text = "Export to file...";
            this.bnSave.UseVisualStyleBackColor = true;
            this.bnSave.Click += new System.EventHandler(this.OnExport);
            // 
            // bnClose
            // 
            this.bnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnClose.Location = new System.Drawing.Point(713, 8);
            this.bnClose.Name = "bnClose";
            this.bnClose.Size = new System.Drawing.Size(75, 23);
            this.bnClose.TabIndex = 4;
            this.bnClose.Text = "Close";
            this.bnClose.UseVisualStyleBackColor = true;
            // 
            // cbCoordinates
            // 
            this.cbCoordinates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCoordinates.FormattingEnabled = true;
            this.cbCoordinates.Items.AddRange(new object[] {
            "Case corner",
            "Case center"});
            this.cbCoordinates.Location = new System.Drawing.Point(91, 6);
            this.cbCoordinates.Name = "cbCoordinates";
            this.cbCoordinates.Size = new System.Drawing.Size(140, 21);
            this.cbCoordinates.TabIndex = 3;
            this.cbCoordinates.SelectedIndexChanged += new System.EventHandler(this.OnInputChanged);
            // 
            // lbCoordinates
            // 
            this.lbCoordinates.AutoSize = true;
            this.lbCoordinates.Location = new System.Drawing.Point(5, 9);
            this.lbCoordinates.Name = "lbCoordinates";
            this.lbCoordinates.Size = new System.Drawing.Size(63, 13);
            this.lbCoordinates.TabIndex = 2;
            this.lbCoordinates.Text = "Coordinates";
            // 
            // lbFormat
            // 
            this.lbFormat.AutoSize = true;
            this.lbFormat.Location = new System.Drawing.Point(5, 13);
            this.lbFormat.Name = "lbFormat";
            this.lbFormat.Size = new System.Drawing.Size(55, 13);
            this.lbFormat.TabIndex = 1;
            this.lbFormat.Text = "File format";
            // 
            // cbFileFormat
            // 
            this.cbFileFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFileFormat.FormattingEnabled = true;
            this.cbFileFormat.Location = new System.Drawing.Point(91, 10);
            this.cbFileFormat.Name = "cbFileFormat";
            this.cbFileFormat.Size = new System.Drawing.Size(140, 21);
            this.cbFileFormat.TabIndex = 0;
            this.cbFileFormat.SelectedIndexChanged += new System.EventHandler(this.OnExportFormatChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabCtrlFeatures);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.textEditorControl);
            this.splitContainer1.Size = new System.Drawing.Size(800, 470);
            this.splitContainer1.SplitterDistance = 380;
            this.splitContainer1.TabIndex = 1;
            // 
            // splitContainerLayer
            // 
            this.splitContainerLayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerLayer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerLayer.IsSplitterFixed = true;
            this.splitContainerLayer.Location = new System.Drawing.Point(3, 3);
            this.splitContainerLayer.Name = "splitContainerLayer";
            this.splitContainerLayer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerLayer.Panel1
            // 
            this.splitContainerLayer.Panel1.Controls.Add(this.cbLayers);
            this.splitContainerLayer.Panel1.Controls.Add(this.lbLayers);
            // 
            // splitContainerLayer.Panel2
            // 
            this.splitContainerLayer.Panel2.Controls.Add(this.layerEditor);
            this.splitContainerLayer.Size = new System.Drawing.Size(366, 438);
            this.splitContainerLayer.SplitterDistance = 35;
            this.splitContainerLayer.TabIndex = 3;
            // 
            // cbLayers
            // 
            this.cbLayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLayers.FormattingEnabled = true;
            this.cbLayers.Location = new System.Drawing.Point(84, 6);
            this.cbLayers.Name = "cbLayers";
            this.cbLayers.Size = new System.Drawing.Size(140, 21);
            this.cbLayers.TabIndex = 1;
            // 
            // lbLayers
            // 
            this.lbLayers.AutoSize = true;
            this.lbLayers.Location = new System.Drawing.Point(3, 9);
            this.lbLayers.Name = "lbLayers";
            this.lbLayers.Size = new System.Drawing.Size(38, 13);
            this.lbLayers.TabIndex = 2;
            this.lbLayers.Text = "Layers";
            // 
            // layerEditor
            // 
            this.layerEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layerEditor.Layer = null;
            this.layerEditor.Location = new System.Drawing.Point(0, 0);
            this.layerEditor.Name = "layerEditor";
            this.layerEditor.Size = new System.Drawing.Size(366, 399);
            this.layerEditor.TabIndex = 0;
            // 
            // textEditorControl
            // 
            this.textEditorControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEditorControl.FoldingStrategy = "XML";
            this.textEditorControl.Font = new System.Drawing.Font("Courier New", 10F);
            this.textEditorControl.Location = new System.Drawing.Point(0, 0);
            this.textEditorControl.Name = "textEditorControl";
            this.textEditorControl.Size = new System.Drawing.Size(416, 470);
            this.textEditorControl.SyntaxHighlighting = "XML";
            this.textEditorControl.TabIndex = 0;
            // 
            // tabCtrlFeatures
            // 
            this.tabCtrlFeatures.Controls.Add(this.tabPageSettings);
            this.tabCtrlFeatures.Controls.Add(this.tabPageAngles);
            this.tabCtrlFeatures.Controls.Add(this.tabPageLayerPrep);
            this.tabCtrlFeatures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCtrlFeatures.Location = new System.Drawing.Point(0, 0);
            this.tabCtrlFeatures.Name = "tabCtrlFeatures";
            this.tabCtrlFeatures.SelectedIndex = 0;
            this.tabCtrlFeatures.Size = new System.Drawing.Size(380, 470);
            this.tabCtrlFeatures.TabIndex = 4;
            // 
            // tabPageSettings
            // 
            this.tabPageSettings.Controls.Add(this.cbCoordinates);
            this.tabPageSettings.Controls.Add(this.lbCoordinates);
            this.tabPageSettings.Location = new System.Drawing.Point(4, 22);
            this.tabPageSettings.Name = "tabPageSettings";
            this.tabPageSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSettings.Size = new System.Drawing.Size(372, 444);
            this.tabPageSettings.TabIndex = 0;
            this.tabPageSettings.Text = "Settings";
            this.tabPageSettings.UseVisualStyleBackColor = true;
            // 
            // tabPageLayerPrep
            // 
            this.tabPageLayerPrep.Controls.Add(this.splitContainerLayer);
            this.tabPageLayerPrep.Location = new System.Drawing.Point(4, 22);
            this.tabPageLayerPrep.Name = "tabPageLayerPrep";
            this.tabPageLayerPrep.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLayerPrep.Size = new System.Drawing.Size(372, 444);
            this.tabPageLayerPrep.TabIndex = 1;
            this.tabPageLayerPrep.Text = "Layer preparation";
            this.tabPageLayerPrep.UseVisualStyleBackColor = true;
            // 
            // tabPageAngles
            // 
            this.tabPageAngles.Location = new System.Drawing.Point(4, 22);
            this.tabPageAngles.Name = "tabPageAngles";
            this.tabPageAngles.Size = new System.Drawing.Size(372, 444);
            this.tabPageAngles.TabIndex = 2;
            this.tabPageAngles.Text = "Conveyor & Gripper angles";
            this.tabPageAngles.UseVisualStyleBackColor = true;
            // 
            // pbBrandLogo
            // 
            this.pbBrandLogo.Location = new System.Drawing.Point(238, 6);
            this.pbBrandLogo.Name = "pbBrandLogo";
            this.pbBrandLogo.Size = new System.Drawing.Size(100, 100);
            this.pbBrandLogo.TabIndex = 6;
            this.pbBrandLogo.TabStop = false;
            // 
            // FormExporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnClose;
            this.ClientSize = new System.Drawing.Size(800, 594);
            this.Controls.Add(this.splitContainerHoriz);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormExporter";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Export...";
            this.splitContainerHoriz.Panel1.ResumeLayout(false);
            this.splitContainerHoriz.Panel1.PerformLayout();
            this.splitContainerHoriz.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz)).EndInit();
            this.splitContainerHoriz.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainerLayer.Panel1.ResumeLayout(false);
            this.splitContainerLayer.Panel1.PerformLayout();
            this.splitContainerLayer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLayer)).EndInit();
            this.splitContainerLayer.ResumeLayout(false);
            this.tabCtrlFeatures.ResumeLayout(false);
            this.tabPageSettings.ResumeLayout(false);
            this.tabPageSettings.PerformLayout();
            this.tabPageLayerPrep.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbBrandLogo)).EndInit();
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
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Graphics.Graphics2DRobotDropEditor layerEditor;
        private System.Windows.Forms.SplitContainer splitContainerLayer;
        private System.Windows.Forms.Label lbLayers;
        private System.Windows.Forms.ComboBox cbLayers;
        private System.Windows.Forms.TabControl tabCtrlFeatures;
        private System.Windows.Forms.TabPage tabPageSettings;
        private System.Windows.Forms.TabPage tabPageLayerPrep;
        private System.Windows.Forms.PictureBox pbBrandLogo;
        private System.Windows.Forms.TabPage tabPageAngles;
    }
}