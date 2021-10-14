namespace treeDiM.EdgeCrushTest
{
    partial class FormCardboardQualityList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCardboardQualityList));
            this.splitContainerVertical = new System.Windows.Forms.SplitContainer();
            this.grid = new SourceGrid.Grid();
            this.bnExit = new System.Windows.Forms.Button();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonUploadCardboardQualities = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVertical)).BeginInit();
            this.splitContainerVertical.Panel1.SuspendLayout();
            this.splitContainerVertical.Panel2.SuspendLayout();
            this.splitContainerVertical.SuspendLayout();
            this.toolStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerVertical
            // 
            resources.ApplyResources(this.splitContainerVertical, "splitContainerVertical");
            this.splitContainerVertical.Name = "splitContainerVertical";
            // 
            // splitContainerVertical.Panel1
            // 
            this.splitContainerVertical.Panel1.Controls.Add(this.grid);
            // 
            // splitContainerVertical.Panel2
            // 
            this.splitContainerVertical.Panel2.Controls.Add(this.bnExit);
            // 
            // grid
            // 
            resources.ApplyResources(this.grid, "grid");
            this.grid.EnableSort = false;
            this.grid.Name = "grid";
            this.grid.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grid.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.grid.TabStop = true;
            this.grid.ToolTipText = "";
            // 
            // bnExit
            // 
            resources.ApplyResources(this.bnExit, "bnExit");
            this.bnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnExit.Name = "bnExit";
            this.bnExit.UseVisualStyleBackColor = true;
            // 
            // toolStripMain
            // 
            this.toolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonNew,
            this.toolStripSeparator1,
            this.toolStripButtonUploadCardboardQualities});
            resources.ApplyResources(this.toolStripMain, "toolStripMain");
            this.toolStripMain.Name = "toolStripMain";
            // 
            // toolStripButtonNew
            // 
            this.toolStripButtonNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonNew, "toolStripButtonNew");
            this.toolStripButtonNew.Name = "toolStripButtonNew";
            this.toolStripButtonNew.Click += new System.EventHandler(this.OnNewCardboardQuality);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripButtonUploadCardboardQualities
            // 
            this.toolStripButtonUploadCardboardQualities.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonUploadCardboardQualities, "toolStripButtonUploadCardboardQualities");
            this.toolStripButtonUploadCardboardQualities.Name = "toolStripButtonUploadCardboardQualities";
            this.toolStripButtonUploadCardboardQualities.Click += new System.EventHandler(this.OnUploadDefaultCardboardQualities);
            // 
            // FormCardboardQualityList
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnExit;
            this.Controls.Add(this.splitContainerVertical);
            this.Controls.Add(this.toolStripMain);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCardboardQualityList";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.splitContainerVertical.Panel1.ResumeLayout(false);
            this.splitContainerVertical.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVertical)).EndInit();
            this.splitContainerVertical.ResumeLayout(false);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton toolStripButtonNew;
        private System.Windows.Forms.SplitContainer splitContainerVertical;
        private System.Windows.Forms.Button bnExit;
        private SourceGrid.Grid grid;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonUploadCardboardQualities;
    }
}