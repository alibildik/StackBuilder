﻿namespace treeDiM.StackBuilder.Desktop
{
    partial class FormNewHAnalysis
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNewHAnalysis));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.bnNext = new System.Windows.Forms.Button();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.tbName = new System.Windows.Forms.TextBox();
            this.lbDescription = new System.Windows.Forms.Label();
            this.lbName = new System.Windows.Forms.Label();
            this.splitContainerHoriz1 = new System.Windows.Forms.SplitContainer();
            this.splitContainerHoriz2 = new System.Windows.Forms.SplitContainer();
            this.splitContainerVert = new System.Windows.Forms.SplitContainer();
            this.bnAddRow = new System.Windows.Forms.Button();
            this.gridContent = new SourceGrid.Grid();
            this.splitContainerSolutions = new System.Windows.Forms.SplitContainer();
            this.graphCtrl = new treeDiM.StackBuilder.Graphics.Graphics3DControl();
            this.gridSolutions = new SourceGrid.Grid();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz1)).BeginInit();
            this.splitContainerHoriz1.Panel1.SuspendLayout();
            this.splitContainerHoriz1.Panel2.SuspendLayout();
            this.splitContainerHoriz1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz2)).BeginInit();
            this.splitContainerHoriz2.Panel1.SuspendLayout();
            this.splitContainerHoriz2.Panel2.SuspendLayout();
            this.splitContainerHoriz2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).BeginInit();
            this.splitContainerVert.Panel1.SuspendLayout();
            this.splitContainerVert.Panel2.SuspendLayout();
            this.splitContainerVert.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerSolutions)).BeginInit();
            this.splitContainerSolutions.Panel1.SuspendLayout();
            this.splitContainerSolutions.Panel2.SuspendLayout();
            this.splitContainerSolutions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrl)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Name = "statusStrip";
            // 
            // statusLabel
            // 
            resources.ApplyResources(this.statusLabel, "statusLabel");
            this.statusLabel.Name = "statusLabel";
            // 
            // bnNext
            // 
            resources.ApplyResources(this.bnNext, "bnNext");
            this.bnNext.Name = "bnNext";
            this.bnNext.UseVisualStyleBackColor = true;
            // 
            // tbDescription
            // 
            resources.ApplyResources(this.tbDescription, "tbDescription");
            this.tbDescription.Name = "tbDescription";
            // 
            // tbName
            // 
            resources.ApplyResources(this.tbName, "tbName");
            this.tbName.Name = "tbName";
            // 
            // lbDescription
            // 
            resources.ApplyResources(this.lbDescription, "lbDescription");
            this.lbDescription.Name = "lbDescription";
            // 
            // lbName
            // 
            resources.ApplyResources(this.lbName, "lbName");
            this.lbName.Name = "lbName";
            // 
            // splitContainerHoriz1
            // 
            resources.ApplyResources(this.splitContainerHoriz1, "splitContainerHoriz1");
            this.splitContainerHoriz1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerHoriz1.Name = "splitContainerHoriz1";
            // 
            // splitContainerHoriz1.Panel1
            // 
            resources.ApplyResources(this.splitContainerHoriz1.Panel1, "splitContainerHoriz1.Panel1");
            this.splitContainerHoriz1.Panel1.Controls.Add(this.tbName);
            this.splitContainerHoriz1.Panel1.Controls.Add(this.lbName);
            this.splitContainerHoriz1.Panel1.Controls.Add(this.tbDescription);
            this.splitContainerHoriz1.Panel1.Controls.Add(this.lbDescription);
            // 
            // splitContainerHoriz1.Panel2
            // 
            resources.ApplyResources(this.splitContainerHoriz1.Panel2, "splitContainerHoriz1.Panel2");
            this.splitContainerHoriz1.Panel2.Controls.Add(this.splitContainerHoriz2);
            // 
            // splitContainerHoriz2
            // 
            resources.ApplyResources(this.splitContainerHoriz2, "splitContainerHoriz2");
            this.splitContainerHoriz2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerHoriz2.Name = "splitContainerHoriz2";
            // 
            // splitContainerHoriz2.Panel1
            // 
            resources.ApplyResources(this.splitContainerHoriz2.Panel1, "splitContainerHoriz2.Panel1");
            this.splitContainerHoriz2.Panel1.Controls.Add(this.splitContainerVert);
            // 
            // splitContainerHoriz2.Panel2
            // 
            resources.ApplyResources(this.splitContainerHoriz2.Panel2, "splitContainerHoriz2.Panel2");
            this.splitContainerHoriz2.Panel2.Controls.Add(this.bnNext);
            // 
            // splitContainerVert
            // 
            resources.ApplyResources(this.splitContainerVert, "splitContainerVert");
            this.splitContainerVert.Name = "splitContainerVert";
            // 
            // splitContainerVert.Panel1
            // 
            resources.ApplyResources(this.splitContainerVert.Panel1, "splitContainerVert.Panel1");
            this.splitContainerVert.Panel1.Controls.Add(this.bnAddRow);
            this.splitContainerVert.Panel1.Controls.Add(this.gridContent);
            // 
            // splitContainerVert.Panel2
            // 
            resources.ApplyResources(this.splitContainerVert.Panel2, "splitContainerVert.Panel2");
            this.splitContainerVert.Panel2.Controls.Add(this.splitContainerSolutions);
            // 
            // bnAddRow
            // 
            resources.ApplyResources(this.bnAddRow, "bnAddRow");
            this.bnAddRow.Name = "bnAddRow";
            this.bnAddRow.UseVisualStyleBackColor = true;
            this.bnAddRow.Click += new System.EventHandler(this.OnAddRow);
            // 
            // gridContent
            // 
            resources.ApplyResources(this.gridContent, "gridContent");
            this.gridContent.EnableSort = true;
            this.gridContent.Name = "gridContent";
            this.gridContent.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridContent.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.gridContent.TabStop = true;
            this.gridContent.ToolTipText = "";
            // 
            // splitContainerSolutions
            // 
            resources.ApplyResources(this.splitContainerSolutions, "splitContainerSolutions");
            this.splitContainerSolutions.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerSolutions.Name = "splitContainerSolutions";
            // 
            // splitContainerSolutions.Panel1
            // 
            resources.ApplyResources(this.splitContainerSolutions.Panel1, "splitContainerSolutions.Panel1");
            this.splitContainerSolutions.Panel1.Controls.Add(this.graphCtrl);
            // 
            // splitContainerSolutions.Panel2
            // 
            resources.ApplyResources(this.splitContainerSolutions.Panel2, "splitContainerSolutions.Panel2");
            this.splitContainerSolutions.Panel2.Controls.Add(this.gridSolutions);
            // 
            // graphCtrl
            // 
            resources.ApplyResources(this.graphCtrl, "graphCtrl");
            this.graphCtrl.Name = "graphCtrl";
            this.graphCtrl.Viewer = null;
            // 
            // gridSolutions
            // 
            resources.ApplyResources(this.gridSolutions, "gridSolutions");
            this.gridSolutions.EnableSort = true;
            this.gridSolutions.Name = "gridSolutions";
            this.gridSolutions.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridSolutions.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.gridSolutions.TabStop = true;
            this.gridSolutions.ToolTipText = "";
            // 
            // FormNewHAnalysis
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerHoriz1);
            this.Controls.Add(this.statusStrip);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormNewHAnalysis";
            this.ShowInTaskbar = false;
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.splitContainerHoriz1.Panel1.ResumeLayout(false);
            this.splitContainerHoriz1.Panel1.PerformLayout();
            this.splitContainerHoriz1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz1)).EndInit();
            this.splitContainerHoriz1.ResumeLayout(false);
            this.splitContainerHoriz2.Panel1.ResumeLayout(false);
            this.splitContainerHoriz2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz2)).EndInit();
            this.splitContainerHoriz2.ResumeLayout(false);
            this.splitContainerVert.Panel1.ResumeLayout(false);
            this.splitContainerVert.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).EndInit();
            this.splitContainerVert.ResumeLayout(false);
            this.splitContainerSolutions.Panel1.ResumeLayout(false);
            this.splitContainerSolutions.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerSolutions)).EndInit();
            this.splitContainerSolutions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.Button bnNext;
        protected System.Windows.Forms.TextBox tbDescription;
        protected System.Windows.Forms.TextBox tbName;
        protected System.Windows.Forms.Label lbDescription;
        protected System.Windows.Forms.Label lbName;
        private System.Windows.Forms.SplitContainer splitContainerHoriz1;
        private System.Windows.Forms.SplitContainer splitContainerHoriz2;
        private System.Windows.Forms.SplitContainer splitContainerVert;
        private SourceGrid.Grid gridContent;
        private System.Windows.Forms.SplitContainer splitContainerSolutions;
        private Graphics.Graphics3DControl graphCtrl;
        private SourceGrid.Grid gridSolutions;
        private System.Windows.Forms.Button bnAddRow;
    }
}