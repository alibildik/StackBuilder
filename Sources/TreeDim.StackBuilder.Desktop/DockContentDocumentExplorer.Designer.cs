namespace treeDiM.StackBuilder.Desktop
{
    partial class DockContentDocumentExplorer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockContentDocumentExplorer));
            this.ContextMenuDock = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.DockableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AutoHideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._documentTreeView = new treeDiM.StackBuilder.Desktop.AnalysisTreeView();
            this.ContextMenuDock.SuspendLayout();
            this.SuspendLayout();
            // 
            // ContextMenuDock
            // 
            resources.ApplyResources(this.ContextMenuDock, "ContextMenuDock");
            this.ContextMenuDock.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DockableToolStripMenuItem,
            this.AutoHideToolStripMenuItem});
            this.ContextMenuDock.Name = "ContextMenuStrip";
            // 
            // DockableToolStripMenuItem
            // 
            resources.ApplyResources(this.DockableToolStripMenuItem, "DockableToolStripMenuItem");
            this.DockableToolStripMenuItem.Checked = true;
            this.DockableToolStripMenuItem.CheckOnClick = true;
            this.DockableToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DockableToolStripMenuItem.Name = "DockableToolStripMenuItem";
            this.DockableToolStripMenuItem.Click += new System.EventHandler(this.OnMenuItemDockable);
            // 
            // AutoHideToolStripMenuItem
            // 
            resources.ApplyResources(this.AutoHideToolStripMenuItem, "AutoHideToolStripMenuItem");
            this.AutoHideToolStripMenuItem.CheckOnClick = true;
            this.AutoHideToolStripMenuItem.Name = "AutoHideToolStripMenuItem";
            this.AutoHideToolStripMenuItem.Click += new System.EventHandler(this.OnMenuItemAutoHide);
            // 
            // _documentTreeView
            // 
            resources.ApplyResources(this._documentTreeView, "_documentTreeView");
            this._documentTreeView.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this._documentTreeView.Name = "_documentTreeView";
            // 
            // DockContentDocumentExplorer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CloseButton = false;
            this.ControlBox = false;
            this.Controls.Add(this._documentTreeView);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.HideOnClose = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DockContentDocumentExplorer";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.ShowInTaskbar = false;
            this.TabPageContextMenuStrip = this.ContextMenuDock;
            this.ContextMenuDock.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        internal System.Windows.Forms.ContextMenuStrip ContextMenuDock;
        internal System.Windows.Forms.ToolStripMenuItem DockableToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem AutoHideToolStripMenuItem;
        internal AnalysisTreeView _documentTreeView;
    }
}