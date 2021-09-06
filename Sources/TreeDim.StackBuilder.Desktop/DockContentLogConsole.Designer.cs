namespace treeDiM.StackBuilder.Desktop
{
    partial class DockContentLogConsole
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
            if (disposing)
                log4net.Appender.RichTextBoxAppender.SetRichTextBox(null, "RichTextBoxAppender");
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
            this.richTextBoxLog = new System.Windows.Forms.RichTextBox();
            this.ContextMenuDock = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.DockableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AutoHideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenuDock.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBoxLog
            // 
            this.richTextBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxLog.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxLog.Name = "richTextBoxLog";
            this.richTextBoxLog.Size = new System.Drawing.Size(784, 111);
            this.richTextBoxLog.TabIndex = 0;
            this.richTextBoxLog.Text = "";
            // 
            // ContextMenuDock
            // 
            this.ContextMenuDock.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DockableToolStripMenuItem,
            this.AutoHideToolStripMenuItem,
            this.HideToolStripMenuItem});
            this.ContextMenuDock.Name = "ContextMenuStrip1";
            this.ContextMenuDock.Size = new System.Drawing.Size(181, 92);
            this.ContextMenuDock.Text = "Window Position";
            // 
            // DockableToolStripMenuItem
            // 
            this.DockableToolStripMenuItem.Checked = true;
            this.DockableToolStripMenuItem.CheckOnClick = true;
            this.DockableToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DockableToolStripMenuItem.Name = "DockableToolStripMenuItem";
            this.DockableToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.DockableToolStripMenuItem.Text = "Dock";
            this.DockableToolStripMenuItem.Click += new System.EventHandler(this.OnMenuItemDockable);
            // 
            // AutoHideToolStripMenuItem
            // 
            this.AutoHideToolStripMenuItem.CheckOnClick = true;
            this.AutoHideToolStripMenuItem.Name = "AutoHideToolStripMenuItem";
            this.AutoHideToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.AutoHideToolStripMenuItem.Text = "Auto Hide";
            this.AutoHideToolStripMenuItem.Click += new System.EventHandler(this.OnMenuItemAutoHide);
            // 
            // HideToolStripMenuItem
            // 
            this.HideToolStripMenuItem.Name = "HideToolStripMenuItem";
            this.HideToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.HideToolStripMenuItem.Text = "Hide";
            this.HideToolStripMenuItem.Click += new System.EventHandler(this.OnMenuItemHide);
            // 
            // DockContentLogConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 111);
            this.CloseButton = false;
            
            this.ControlBox = false;
            this.Controls.Add(this.richTextBoxLog);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HideOnClose = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DockContentLogConsole";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottom;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TabPageContextMenuStrip = this.ContextMenuDock;
            this.Text = "Log console";
            this.ContextMenuDock.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBoxLog;
        internal System.Windows.Forms.ContextMenuStrip ContextMenuDock;
        internal System.Windows.Forms.ToolStripMenuItem DockableToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem AutoHideToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem HideToolStripMenuItem;
    }
}