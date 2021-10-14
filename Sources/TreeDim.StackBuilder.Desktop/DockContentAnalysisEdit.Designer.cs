namespace treeDiM.StackBuilder.Desktop
{
    partial class DockContentAnalysisEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockContentAnalysisEdit));
            this.splitContainerHoriz = new System.Windows.Forms.SplitContainer();
            this.splitContainerVert = new System.Windows.Forms.SplitContainer();
            this.graphCtrlSolution = new treeDiM.StackBuilder.Graphics.Graphics3DControl();
            this.gridSolution = new SourceGrid.Grid();
            this.gbLayer = new System.Windows.Forms.GroupBox();
            this.chkbInterlayer = new System.Windows.Forms.CheckBox();
            this.cbInterlayer = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered();
            this.cbLayerType = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboLayer();
            this.bnSymmetryX = new System.Windows.Forms.Button();
            this.bnSymetryY = new System.Windows.Forms.Button();
            this.tbClickLayer = new System.Windows.Forms.TextBox();
            this.toolStripAnalysis = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonBack = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonReport = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonExport = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonExportGLB = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExportOBJ = new System.Windows.Forms.ToolStripButton();
            this.toolStripSepExport3D = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonScreenShot = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz)).BeginInit();
            this.splitContainerHoriz.Panel1.SuspendLayout();
            this.splitContainerHoriz.Panel2.SuspendLayout();
            this.splitContainerHoriz.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).BeginInit();
            this.splitContainerVert.Panel1.SuspendLayout();
            this.splitContainerVert.Panel2.SuspendLayout();
            this.splitContainerVert.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrlSolution)).BeginInit();
            this.gbLayer.SuspendLayout();
            this.toolStripAnalysis.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerHoriz
            // 
            resources.ApplyResources(this.splitContainerHoriz, "splitContainerHoriz");
            this.splitContainerHoriz.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerHoriz.Name = "splitContainerHoriz";
            // 
            // splitContainerHoriz.Panel1
            // 
            this.splitContainerHoriz.Panel1.Controls.Add(this.splitContainerVert);
            // 
            // splitContainerHoriz.Panel2
            // 
            this.splitContainerHoriz.Panel2.Controls.Add(this.gbLayer);
            this.splitContainerHoriz.Panel2.Controls.Add(this.tbClickLayer);
            // 
            // splitContainerVert
            // 
            resources.ApplyResources(this.splitContainerVert, "splitContainerVert");
            this.splitContainerVert.Name = "splitContainerVert";
            // 
            // splitContainerVert.Panel1
            // 
            this.splitContainerVert.Panel1.Controls.Add(this.graphCtrlSolution);
            // 
            // splitContainerVert.Panel2
            // 
            this.splitContainerVert.Panel2.Controls.Add(this.gridSolution);
            // 
            // graphCtrlSolution
            // 
            this.graphCtrlSolution.AngleHoriz = 45D;
            resources.ApplyResources(this.graphCtrlSolution, "graphCtrlSolution");
            this.graphCtrlSolution.Name = "graphCtrlSolution";
            this.graphCtrlSolution.Viewer = null;
            // 
            // gridSolution
            // 
            this.gridSolution.ColumnsCount = 2;
            resources.ApplyResources(this.gridSolution, "gridSolution");
            this.gridSolution.EnableSort = false;
            this.gridSolution.Name = "gridSolution";
            this.gridSolution.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridSolution.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.gridSolution.TabStop = true;
            this.gridSolution.ToolTipText = "";
            // 
            // gbLayer
            // 
            resources.ApplyResources(this.gbLayer, "gbLayer");
            this.gbLayer.Controls.Add(this.chkbInterlayer);
            this.gbLayer.Controls.Add(this.cbInterlayer);
            this.gbLayer.Controls.Add(this.cbLayerType);
            this.gbLayer.Controls.Add(this.bnSymmetryX);
            this.gbLayer.Controls.Add(this.bnSymetryY);
            this.gbLayer.Name = "gbLayer";
            this.gbLayer.TabStop = false;
            // 
            // chkbInterlayer
            // 
            resources.ApplyResources(this.chkbInterlayer, "chkbInterlayer");
            this.chkbInterlayer.Name = "chkbInterlayer";
            this.chkbInterlayer.UseVisualStyleBackColor = true;
            this.chkbInterlayer.CheckedChanged += new System.EventHandler(this.OnChkbInterlayerClicked);
            // 
            // cbInterlayer
            // 
            resources.ApplyResources(this.cbInterlayer, "cbInterlayer");
            this.cbInterlayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInterlayer.FormattingEnabled = true;
            this.cbInterlayer.Name = "cbInterlayer";
            this.cbInterlayer.SelectedIndexChanged += new System.EventHandler(this.OnInterlayerChanged);
            // 
            // cbLayerType
            // 
            resources.ApplyResources(this.cbLayerType, "cbLayerType");
            this.cbLayerType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbLayerType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLayerType.FormattingEnabled = true;
            this.cbLayerType.Name = "cbLayerType";
            this.cbLayerType.SelectedIndexChanged += new System.EventHandler(this.OnLayerTypeChanged);
            // 
            // bnSymmetryX
            // 
            resources.ApplyResources(this.bnSymmetryX, "bnSymmetryX");
            this.bnSymmetryX.Name = "bnSymmetryX";
            this.bnSymmetryX.UseVisualStyleBackColor = true;
            this.bnSymmetryX.Click += new System.EventHandler(this.OnReflectionX);
            // 
            // bnSymetryY
            // 
            resources.ApplyResources(this.bnSymetryY, "bnSymetryY");
            this.bnSymetryY.Name = "bnSymetryY";
            this.bnSymetryY.UseVisualStyleBackColor = true;
            this.bnSymetryY.Click += new System.EventHandler(this.OnReflectionY);
            // 
            // tbClickLayer
            // 
            resources.ApplyResources(this.tbClickLayer, "tbClickLayer");
            this.tbClickLayer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbClickLayer.Name = "tbClickLayer";
            // 
            // toolStripAnalysis
            // 
            this.toolStripAnalysis.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonBack,
            this.toolStripSeparator2,
            this.toolStripButtonReport,
            this.toolStripSeparator1,
            this.toolStripButtonExport,
            this.toolStripSeparator3,
            this.toolStripButtonExportGLB,
            this.toolStripButtonExportOBJ,
            this.toolStripSepExport3D,
            this.toolStripButtonScreenShot});
            resources.ApplyResources(this.toolStripAnalysis, "toolStripAnalysis");
            this.toolStripAnalysis.Name = "toolStripAnalysis";
            // 
            // toolStripButtonBack
            // 
            this.toolStripButtonBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonBack, "toolStripButtonBack");
            this.toolStripButtonBack.Name = "toolStripButtonBack";
            this.toolStripButtonBack.Click += new System.EventHandler(this.OnBack);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // toolStripButtonReport
            // 
            this.toolStripButtonReport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonReport, "toolStripButtonReport");
            this.toolStripButtonReport.Name = "toolStripButtonReport";
            this.toolStripButtonReport.Click += new System.EventHandler(this.OnGenerateReport);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripButtonExport
            // 
            this.toolStripButtonExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonExport, "toolStripButtonExport");
            this.toolStripButtonExport.Name = "toolStripButtonExport";
            this.toolStripButtonExport.Click += new System.EventHandler(this.OnGenerateExport);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // toolStripButtonExportGLB
            // 
            this.toolStripButtonExportGLB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonExportGLB, "toolStripButtonExportGLB");
            this.toolStripButtonExportGLB.Name = "toolStripButtonExportGLB";
            this.toolStripButtonExportGLB.Click += new System.EventHandler(this.OnExportGLB);
            // 
            // toolStripButtonExportOBJ
            // 
            this.toolStripButtonExportOBJ.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonExportOBJ, "toolStripButtonExportOBJ");
            this.toolStripButtonExportOBJ.Name = "toolStripButtonExportOBJ";
            this.toolStripButtonExportOBJ.Click += new System.EventHandler(this.OnExportOBJ);
            // 
            // toolStripSepExport3D
            // 
            this.toolStripSepExport3D.Name = "toolStripSepExport3D";
            resources.ApplyResources(this.toolStripSepExport3D, "toolStripSepExport3D");
            // 
            // toolStripButtonScreenShot
            // 
            this.toolStripButtonScreenShot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonScreenShot, "toolStripButtonScreenShot");
            this.toolStripButtonScreenShot.Name = "toolStripButtonScreenShot";
            this.toolStripButtonScreenShot.Click += new System.EventHandler(this.OnScreenShot);
            // 
            // DockContentAnalysisEdit
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerHoriz);
            this.Controls.Add(this.toolStripAnalysis);
            this.Name = "DockContentAnalysisEdit";
            this.ShowInTaskbar = false;
            this.splitContainerHoriz.Panel1.ResumeLayout(false);
            this.splitContainerHoriz.Panel2.ResumeLayout(false);
            this.splitContainerHoriz.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz)).EndInit();
            this.splitContainerHoriz.ResumeLayout(false);
            this.splitContainerVert.Panel1.ResumeLayout(false);
            this.splitContainerVert.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).EndInit();
            this.splitContainerVert.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrlSolution)).EndInit();
            this.gbLayer.ResumeLayout(false);
            this.gbLayer.PerformLayout();
            this.toolStripAnalysis.ResumeLayout(false);
            this.toolStripAnalysis.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public Graphics.Graphics3DControl graphCtrlSolution;
        protected System.Windows.Forms.SplitContainer splitContainerHoriz;
        protected System.Windows.Forms.SplitContainer splitContainerVert;
        private System.Windows.Forms.ToolStrip toolStripAnalysis;
        private System.Windows.Forms.ToolStripButton toolStripButtonReport;
        private System.Windows.Forms.GroupBox gbLayer;
        private System.Windows.Forms.CheckBox chkbInterlayer;
        protected Graphics.Controls.CCtrlComboFiltered cbInterlayer;
        private Graphics.Controls.CCtrlComboLayer cbLayerType;
        private System.Windows.Forms.Button bnSymmetryX;
        private System.Windows.Forms.Button bnSymetryY;
        private System.Windows.Forms.TextBox tbClickLayer;
        protected SourceGrid.Grid gridSolution;
        private System.Windows.Forms.ToolStripButton toolStripButtonBack;
        private System.Windows.Forms.ToolStripButton toolStripButtonExport;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSepExport3D;
        private System.Windows.Forms.ToolStripButton toolStripButtonScreenShot;
        private System.Windows.Forms.ToolStripButton toolStripButtonExportGLB;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonExportOBJ;
    }
}