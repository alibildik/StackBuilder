﻿namespace TreeDim.StackBuilder.Desktop
{
    partial class DockContentAnalysisCaseOfBoxes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockContentAnalysisCaseOfBoxes));
            this.splitContainerHoriz = new System.Windows.Forms.SplitContainer();
            this.splitContainerHorizInside = new System.Windows.Forms.SplitContainer();
            this.splitContainerVertInside = new System.Windows.Forms.SplitContainer();
            this.pictureBoxCase = new System.Windows.Forms.PictureBox();
            this.pictureBoxPallet = new System.Windows.Forms.PictureBox();
            this.trackBarAngleVert = new System.Windows.Forms.TrackBar();
            this.trackBarAngleHoriz = new System.Windows.Forms.TrackBar();
            this.gridSolutions = new SourceGrid.Grid();
            this.toolStrip_view = new System.Windows.Forms.ToolStrip();
            this.toolStripCornerView_0 = new System.Windows.Forms.ToolStripButton();
            this.toolStripCornerView_90 = new System.Windows.Forms.ToolStripButton();
            this.toolStripCornerView_180 = new System.Windows.Forms.ToolStripButton();
            this.toolStripCornerView_270 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripFrontView = new System.Windows.Forms.ToolStripButton();
            this.toolStripRightView = new System.Windows.Forms.ToolStripButton();
            this.toolStripBackView = new System.Windows.Forms.ToolStripButton();
            this.toolStripLeftView = new System.Windows.Forms.ToolStripButton();
            this.toolStripTopView = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripShowImages = new System.Windows.Forms.ToolStripButton();
            this.splitContainerHoriz.Panel1.SuspendLayout();
            this.splitContainerHoriz.Panel2.SuspendLayout();
            this.splitContainerHoriz.SuspendLayout();
            this.splitContainerHorizInside.Panel1.SuspendLayout();
            this.splitContainerHorizInside.Panel2.SuspendLayout();
            this.splitContainerHorizInside.SuspendLayout();
            this.splitContainerVertInside.Panel1.SuspendLayout();
            this.splitContainerVertInside.Panel2.SuspendLayout();
            this.splitContainerVertInside.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCase)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPallet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAngleVert)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAngleHoriz)).BeginInit();
            this.toolStrip_view.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerHoriz
            // 
            resources.ApplyResources(this.splitContainerHoriz, "splitContainerHoriz");
            this.splitContainerHoriz.Name = "splitContainerHoriz";
            // 
            // splitContainerHoriz.Panel1
            // 
            this.splitContainerHoriz.Panel1.Controls.Add(this.splitContainerHorizInside);
            // 
            // splitContainerHoriz.Panel2
            // 
            this.splitContainerHoriz.Panel2.Controls.Add(this.gridSolutions);
            // 
            // splitContainerHorizInside
            // 
            resources.ApplyResources(this.splitContainerHorizInside, "splitContainerHorizInside");
            this.splitContainerHorizInside.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerHorizInside.Name = "splitContainerHorizInside";
            // 
            // splitContainerHorizInside.Panel1
            // 
            this.splitContainerHorizInside.Panel1.Controls.Add(this.splitContainerVertInside);
            // 
            // splitContainerHorizInside.Panel2
            // 
            this.splitContainerHorizInside.Panel2.Controls.Add(this.trackBarAngleVert);
            this.splitContainerHorizInside.Panel2.Controls.Add(this.trackBarAngleHoriz);
            // 
            // splitContainerVertInside
            // 
            resources.ApplyResources(this.splitContainerVertInside, "splitContainerVertInside");
            this.splitContainerVertInside.Name = "splitContainerVertInside";
            // 
            // splitContainerVertInside.Panel1
            // 
            this.splitContainerVertInside.Panel1.Controls.Add(this.pictureBoxCase);
            // 
            // splitContainerVertInside.Panel2
            // 
            this.splitContainerVertInside.Panel2.Controls.Add(this.pictureBoxPallet);
            // 
            // pictureBoxCase
            // 
            resources.ApplyResources(this.pictureBoxCase, "pictureBoxCase");
            this.pictureBoxCase.Name = "pictureBoxCase";
            this.pictureBoxCase.TabStop = false;
            this.pictureBoxCase.SizeChanged += new System.EventHandler(this.pictureBox_SizeChanged);
            // 
            // pictureBoxPallet
            // 
            resources.ApplyResources(this.pictureBoxPallet, "pictureBoxPallet");
            this.pictureBoxPallet.Name = "pictureBoxPallet";
            this.pictureBoxPallet.TabStop = false;
            this.pictureBoxPallet.SizeChanged += new System.EventHandler(this.pictureBox_SizeChanged);
            // 
            // trackBarAngleVert
            // 
            resources.ApplyResources(this.trackBarAngleVert, "trackBarAngleVert");
            this.trackBarAngleVert.LargeChange = 15;
            this.trackBarAngleVert.Maximum = 90;
            this.trackBarAngleVert.Name = "trackBarAngleVert";
            this.trackBarAngleVert.SmallChange = 15;
            this.trackBarAngleVert.TickFrequency = 15;
            this.trackBarAngleVert.Value = 45;
            this.trackBarAngleVert.ValueChanged += new System.EventHandler(this.onAngleVertChanged);
            // 
            // trackBarAngleHoriz
            // 
            resources.ApplyResources(this.trackBarAngleHoriz, "trackBarAngleHoriz");
            this.trackBarAngleHoriz.LargeChange = 45;
            this.trackBarAngleHoriz.Maximum = 360;
            this.trackBarAngleHoriz.Name = "trackBarAngleHoriz";
            this.trackBarAngleHoriz.SmallChange = 45;
            this.trackBarAngleHoriz.TickFrequency = 90;
            this.trackBarAngleHoriz.Value = 225;
            this.trackBarAngleHoriz.ValueChanged += new System.EventHandler(this.onAngleHorizChanged);
            // 
            // gridSolutions
            // 
            this.gridSolutions.AcceptsInputChar = false;
            resources.ApplyResources(this.gridSolutions, "gridSolutions");
            this.gridSolutions.EnableSort = false;
            this.gridSolutions.Name = "gridSolutions";
            this.gridSolutions.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridSolutions.SelectionMode = SourceGrid.GridSelectionMode.Row;
            this.gridSolutions.SpecialKeys = SourceGrid.GridSpecialKeys.Arrows;
            this.gridSolutions.TabStop = true;
            this.gridSolutions.ToolTipText = "";
            // 
            // toolStrip_view
            // 
            this.toolStrip_view.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripCornerView_0,
            this.toolStripCornerView_90,
            this.toolStripCornerView_180,
            this.toolStripCornerView_270,
            this.toolStripSeparator1,
            this.toolStripFrontView,
            this.toolStripRightView,
            this.toolStripBackView,
            this.toolStripLeftView,
            this.toolStripTopView,
            this.toolStripSeparator2,
            this.toolStripShowImages});
            resources.ApplyResources(this.toolStrip_view, "toolStrip_view");
            this.toolStrip_view.Name = "toolStrip_view";
            // 
            // toolStripCornerView_0
            // 
            this.toolStripCornerView_0.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripCornerView_0.Image = global::TreeDim.StackBuilder.Desktop.Properties.Resources.View0;
            resources.ApplyResources(this.toolStripCornerView_0, "toolStripCornerView_0");
            this.toolStripCornerView_0.Name = "toolStripCornerView_0";
            this.toolStripCornerView_0.Click += new System.EventHandler(this.onViewCorner_0);
            // 
            // toolStripCornerView_90
            // 
            this.toolStripCornerView_90.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripCornerView_90.Image = global::TreeDim.StackBuilder.Desktop.Properties.Resources.View90;
            resources.ApplyResources(this.toolStripCornerView_90, "toolStripCornerView_90");
            this.toolStripCornerView_90.Name = "toolStripCornerView_90";
            this.toolStripCornerView_90.Click += new System.EventHandler(this.onViewCorner_90);
            // 
            // toolStripCornerView_180
            // 
            this.toolStripCornerView_180.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripCornerView_180.Image = global::TreeDim.StackBuilder.Desktop.Properties.Resources.View180;
            resources.ApplyResources(this.toolStripCornerView_180, "toolStripCornerView_180");
            this.toolStripCornerView_180.Name = "toolStripCornerView_180";
            this.toolStripCornerView_180.Click += new System.EventHandler(this.onViewCorner_180);
            // 
            // toolStripCornerView_270
            // 
            this.toolStripCornerView_270.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripCornerView_270.Image = global::TreeDim.StackBuilder.Desktop.Properties.Resources.View270;
            resources.ApplyResources(this.toolStripCornerView_270, "toolStripCornerView_270");
            this.toolStripCornerView_270.Name = "toolStripCornerView_270";
            this.toolStripCornerView_270.Click += new System.EventHandler(this.onViewCorner_270);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripFrontView
            // 
            this.toolStripFrontView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripFrontView.Image = global::TreeDim.StackBuilder.Desktop.Properties.Resources.View_1;
            resources.ApplyResources(this.toolStripFrontView, "toolStripFrontView");
            this.toolStripFrontView.Name = "toolStripFrontView";
            this.toolStripFrontView.Click += new System.EventHandler(this.onViewSideFront);
            // 
            // toolStripRightView
            // 
            this.toolStripRightView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripRightView.Image = global::TreeDim.StackBuilder.Desktop.Properties.Resources.View_2;
            resources.ApplyResources(this.toolStripRightView, "toolStripRightView");
            this.toolStripRightView.Name = "toolStripRightView";
            this.toolStripRightView.Click += new System.EventHandler(this.onViewSideRight);
            // 
            // toolStripBackView
            // 
            this.toolStripBackView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBackView.Image = global::TreeDim.StackBuilder.Desktop.Properties.Resources.View_3;
            resources.ApplyResources(this.toolStripBackView, "toolStripBackView");
            this.toolStripBackView.Name = "toolStripBackView";
            this.toolStripBackView.Click += new System.EventHandler(this.onViewSideRear);
            // 
            // toolStripLeftView
            // 
            this.toolStripLeftView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripLeftView.Image = global::TreeDim.StackBuilder.Desktop.Properties.Resources.View_4;
            resources.ApplyResources(this.toolStripLeftView, "toolStripLeftView");
            this.toolStripLeftView.Name = "toolStripLeftView";
            this.toolStripLeftView.Click += new System.EventHandler(this.onViewSideLeft);
            // 
            // toolStripTopView
            // 
            this.toolStripTopView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripTopView.Image = global::TreeDim.StackBuilder.Desktop.Properties.Resources.View_Top;
            resources.ApplyResources(this.toolStripTopView, "toolStripTopView");
            this.toolStripTopView.Name = "toolStripTopView";
            this.toolStripTopView.Click += new System.EventHandler(this.onViewTop);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // toolStripShowImages
            // 
            this.toolStripShowImages.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripShowImages.Image = global::TreeDim.StackBuilder.Desktop.Properties.Resources.Image;
            resources.ApplyResources(this.toolStripShowImages, "toolStripShowImages");
            this.toolStripShowImages.Name = "toolStripShowImages";
            this.toolStripShowImages.Click += new System.EventHandler(this.toolStripShowImages_Click);
            // 
            // DockContentAnalysisCaseOfBoxes
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerHoriz);
            this.Controls.Add(this.toolStrip_view);
            this.Name = "DockContentAnalysisCaseOfBoxes";
            this.ShowIcon = false;
            this.splitContainerHoriz.Panel1.ResumeLayout(false);
            this.splitContainerHoriz.Panel2.ResumeLayout(false);
            this.splitContainerHoriz.ResumeLayout(false);
            this.splitContainerHorizInside.Panel1.ResumeLayout(false);
            this.splitContainerHorizInside.Panel2.ResumeLayout(false);
            this.splitContainerHorizInside.Panel2.PerformLayout();
            this.splitContainerHorizInside.ResumeLayout(false);
            this.splitContainerVertInside.Panel1.ResumeLayout(false);
            this.splitContainerVertInside.Panel2.ResumeLayout(false);
            this.splitContainerVertInside.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCase)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPallet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAngleVert)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAngleHoriz)).EndInit();
            this.toolStrip_view.ResumeLayout(false);
            this.toolStrip_view.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip_view;
        private System.Windows.Forms.ToolStripButton toolStripCornerView_0;
        private System.Windows.Forms.ToolStripButton toolStripCornerView_90;
        private System.Windows.Forms.ToolStripButton toolStripCornerView_180;
        private System.Windows.Forms.ToolStripButton toolStripCornerView_270;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripFrontView;
        private System.Windows.Forms.ToolStripButton toolStripRightView;
        private System.Windows.Forms.ToolStripButton toolStripBackView;
        private System.Windows.Forms.ToolStripButton toolStripLeftView;
        private System.Windows.Forms.ToolStripButton toolStripTopView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripShowImages;
        private System.Windows.Forms.SplitContainer splitContainerHoriz;
        private SourceGrid.Grid gridSolutions;
        private System.Windows.Forms.SplitContainer splitContainerHorizInside;
        //private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainerVertInside;
        private System.Windows.Forms.TrackBar trackBarAngleVert;
        private System.Windows.Forms.TrackBar trackBarAngleHoriz;
        private System.Windows.Forms.PictureBox pictureBoxCase;
        private System.Windows.Forms.PictureBox pictureBoxPallet;
    }
}