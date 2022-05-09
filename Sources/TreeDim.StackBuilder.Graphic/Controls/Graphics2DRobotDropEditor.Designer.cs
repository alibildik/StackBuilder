
namespace treeDiM.StackBuilder.Graphics
{
    partial class Graphics2DRobotDropEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Graphics2DRobotDropEditor));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.cbCorner = new System.Windows.Forms.ComboBox();
            this.chkbMerge = new System.Windows.Forms.CheckBox();
            this.chkbSplit = new System.Windows.Forms.CheckBox();
            this.chkbReorder = new System.Windows.Forms.CheckBox();
            this.lbOrdering = new System.Windows.Forms.Label();
            this.cbConveyorSetting = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboConveyor();
            this.statusStrip.SuspendLayout();
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
            // cbCorner
            // 
            resources.ApplyResources(this.cbCorner, "cbCorner");
            this.cbCorner.DropDownHeight = 120;
            this.cbCorner.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCorner.FormattingEnabled = true;
            this.cbCorner.Items.AddRange(new object[] {
            resources.GetString("cbCorner.Items"),
            resources.GetString("cbCorner.Items1"),
            resources.GetString("cbCorner.Items2"),
            resources.GetString("cbCorner.Items3")});
            this.cbCorner.Name = "cbCorner";
            // 
            // chkbMerge
            // 
            resources.ApplyResources(this.chkbMerge, "chkbMerge");
            this.chkbMerge.Name = "chkbMerge";
            this.chkbMerge.UseVisualStyleBackColor = true;
            this.chkbMerge.Click += new System.EventHandler(this.OnBuildCaseDrop);
            // 
            // chkbSplit
            // 
            resources.ApplyResources(this.chkbSplit, "chkbSplit");
            this.chkbSplit.Name = "chkbSplit";
            this.chkbSplit.UseVisualStyleBackColor = true;
            this.chkbSplit.Click += new System.EventHandler(this.OnSplitDrop);
            // 
            // chkbReorder
            // 
            resources.ApplyResources(this.chkbReorder, "chkbReorder");
            this.chkbReorder.Name = "chkbReorder";
            this.chkbReorder.UseVisualStyleBackColor = true;
            this.chkbReorder.Click += new System.EventHandler(this.OnReorder);
            // 
            // lbOrdering
            // 
            resources.ApplyResources(this.lbOrdering, "lbOrdering");
            this.lbOrdering.Name = "lbOrdering";
            // 
            // cbConveyorSetting
            // 
            resources.ApplyResources(this.cbConveyorSetting, "cbConveyorSetting");
            this.cbConveyorSetting.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbConveyorSetting.DropDownHeight = 120;
            this.cbConveyorSetting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbConveyorSetting.DropDownWidth = 100;
            this.cbConveyorSetting.FormattingEnabled = true;
            this.cbConveyorSetting.Name = "cbConveyorSetting";
            this.cbConveyorSetting.Packable = null;
            this.cbConveyorSetting.SelectedIndexChanged += new System.EventHandler(this.OnConveyorSettingChanged);
            this.cbConveyorSetting.SelectedValueChanged += new System.EventHandler(this.OnDropModeChanged);
            // 
            // Graphics2DRobotDropEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbOrdering);
            this.Controls.Add(this.chkbReorder);
            this.Controls.Add(this.chkbSplit);
            this.Controls.Add(this.chkbMerge);
            this.Controls.Add(this.cbCorner);
            this.Controls.Add(this.cbConveyorSetting);
            this.Controls.Add(this.statusStrip);
            this.Name = "Graphics2DRobotDropEditor";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private Controls.CCtrlComboConveyor cbConveyorSetting;
        private System.Windows.Forms.ComboBox cbCorner;
        private System.Windows.Forms.CheckBox chkbMerge;
        private System.Windows.Forms.CheckBox chkbSplit;
        private System.Windows.Forms.CheckBox chkbReorder;
        private System.Windows.Forms.Label lbOrdering;
    }
}
