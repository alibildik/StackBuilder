
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
            this.cbConveyorSetting = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboConveyor();
            this.cbCorner = new System.Windows.Forms.ComboBox();
            this.chkbMerge = new System.Windows.Forms.CheckBox();
            this.chkbSplit = new System.Windows.Forms.CheckBox();
            this.chkbReorder = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 328);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(450, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(39, 17);
            this.statusLabel.Text = "Ready";
            // 
            // cbConveyorSetting
            // 
            this.cbConveyorSetting.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbConveyorSetting.DropDownHeight = 120;
            this.cbConveyorSetting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbConveyorSetting.DropDownWidth = 100;
            this.cbConveyorSetting.FormattingEnabled = true;
            this.cbConveyorSetting.IntegralHeight = false;
            this.cbConveyorSetting.ItemHeight = 35;
            this.cbConveyorSetting.Location = new System.Drawing.Point(1, 2);
            this.cbConveyorSetting.Name = "cbConveyorSetting";
            this.cbConveyorSetting.Packable = null;
            this.cbConveyorSetting.Size = new System.Drawing.Size(85, 41);
            this.cbConveyorSetting.TabIndex = 2;
            this.cbConveyorSetting.SelectedIndexChanged += new System.EventHandler(this.OnConveyorSettingChanged);
            this.cbConveyorSetting.SelectedValueChanged += new System.EventHandler(this.OnDropModeChanged);
            // 
            // cbCorner
            // 
            this.cbCorner.DropDownHeight = 120;
            this.cbCorner.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCorner.FormattingEnabled = true;
            this.cbCorner.IntegralHeight = false;
            this.cbCorner.ItemHeight = 13;
            this.cbCorner.Items.AddRange(new object[] {
            "Lower-left",
            "Lower-right",
            "Upper-right",
            "Upper-left"});
            this.cbCorner.Location = new System.Drawing.Point(303, 2);
            this.cbCorner.MaxDropDownItems = 4;
            this.cbCorner.Name = "cbCorner";
            this.cbCorner.Size = new System.Drawing.Size(100, 21);
            this.cbCorner.TabIndex = 5;
            this.cbCorner.SelectedValueChanged += new System.EventHandler(this.OnNumberingCornerChanged);
            // 
            // chkbMerge
            // 
            this.chkbMerge.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkbMerge.AutoSize = true;
            this.chkbMerge.Image = ((System.Drawing.Image)(resources.GetObject("chkbMerge.Image")));
            this.chkbMerge.Location = new System.Drawing.Point(89, 2);
            this.chkbMerge.Name = "chkbMerge";
            this.chkbMerge.Size = new System.Drawing.Size(38, 38);
            this.chkbMerge.TabIndex = 7;
            this.chkbMerge.UseVisualStyleBackColor = true;
            this.chkbMerge.Click += new System.EventHandler(this.OnBuildCaseDrop);
            // 
            // chkbSplit
            // 
            this.chkbSplit.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkbSplit.AutoSize = true;
            this.chkbSplit.Image = ((System.Drawing.Image)(resources.GetObject("chkbSplit.Image")));
            this.chkbSplit.Location = new System.Drawing.Point(128, 2);
            this.chkbSplit.Name = "chkbSplit";
            this.chkbSplit.Size = new System.Drawing.Size(38, 38);
            this.chkbSplit.TabIndex = 8;
            this.chkbSplit.UseVisualStyleBackColor = true;
            this.chkbSplit.Click += new System.EventHandler(this.OnSplitDrop);
            // 
            // chkbReorder
            // 
            this.chkbReorder.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkbReorder.AutoSize = true;
            this.chkbReorder.Image = ((System.Drawing.Image)(resources.GetObject("chkbReorder.Image")));
            this.chkbReorder.Location = new System.Drawing.Point(405, 2);
            this.chkbReorder.Name = "chkbReorder";
            this.chkbReorder.Size = new System.Drawing.Size(38, 38);
            this.chkbReorder.TabIndex = 9;
            this.chkbReorder.UseVisualStyleBackColor = true;
            this.chkbReorder.Click += new System.EventHandler(this.OnReorder);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(203, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Automatic ordering";
            // 
            // Graphics2DRobotDropEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkbReorder);
            this.Controls.Add(this.chkbSplit);
            this.Controls.Add(this.chkbMerge);
            this.Controls.Add(this.cbCorner);
            this.Controls.Add(this.cbConveyorSetting);
            this.Controls.Add(this.statusStrip);
            this.Name = "Graphics2DRobotDropEditor";
            this.Size = new System.Drawing.Size(450, 350);
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
        private System.Windows.Forms.Label label1;
    }
}
