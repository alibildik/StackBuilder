namespace treeDiM.StackBuilder.Graphics.Controls
{
    partial class FormEditConveyorSetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEditConveyorSetting));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.graph3DConveyor = new treeDiM.StackBuilder.Graphics.Graphics3DConveyor();
            this.bnOK = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.nudGripperAngle = new System.Windows.Forms.NumericUpDown();
            this.lbGripperAngle = new System.Windows.Forms.Label();
            this.lbCaseOrientConveyor = new System.Windows.Forms.Label();
            this.nudCaseAngle = new System.Windows.Forms.NumericUpDown();
            this.lbMaxNumber = new System.Windows.Forms.Label();
            this.nudMaxNumber = new System.Windows.Forms.NumericUpDown();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudGripperAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaseAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Name = "statusStrip";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            resources.ApplyResources(this.statusLabel, "statusLabel");
            // 
            // graph3DConveyor
            // 
            this.graph3DConveyor.CaseAngle = 0;
            this.graph3DConveyor.GripperAngle = 0;
            resources.ApplyResources(this.graph3DConveyor, "graph3DConveyor");
            this.graph3DConveyor.MaxDropNumber = 0;
            this.graph3DConveyor.Name = "graph3DConveyor";
            this.graph3DConveyor.Packable = null;
            this.graph3DConveyor.TabStop = false;
            // 
            // bnOK
            // 
            resources.ApplyResources(this.bnOK, "bnOK");
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Name = "bnOK";
            this.bnOK.UseVisualStyleBackColor = true;
            // 
            // bnCancel
            // 
            resources.ApplyResources(this.bnCancel, "bnCancel");
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // nudGripperAngle
            // 
            resources.ApplyResources(this.nudGripperAngle, "nudGripperAngle");
            this.nudGripperAngle.Increment = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.nudGripperAngle.Maximum = new decimal(new int[] {
            270,
            0,
            0,
            0});
            this.nudGripperAngle.Name = "nudGripperAngle";
            this.nudGripperAngle.ValueChanged += new System.EventHandler(this.OnItemChanged);
            // 
            // lbGripperAngle
            // 
            resources.ApplyResources(this.lbGripperAngle, "lbGripperAngle");
            this.lbGripperAngle.Name = "lbGripperAngle";
            // 
            // lbCaseOrientConveyor
            // 
            resources.ApplyResources(this.lbCaseOrientConveyor, "lbCaseOrientConveyor");
            this.lbCaseOrientConveyor.Name = "lbCaseOrientConveyor";
            // 
            // nudCaseAngle
            // 
            resources.ApplyResources(this.nudCaseAngle, "nudCaseAngle");
            this.nudCaseAngle.Increment = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.nudCaseAngle.Maximum = new decimal(new int[] {
            270,
            0,
            0,
            0});
            this.nudCaseAngle.Name = "nudCaseAngle";
            this.nudCaseAngle.ValueChanged += new System.EventHandler(this.OnItemChanged);
            // 
            // lbMaxNumber
            // 
            resources.ApplyResources(this.lbMaxNumber, "lbMaxNumber");
            this.lbMaxNumber.Name = "lbMaxNumber";
            // 
            // nudMaxNumber
            // 
            resources.ApplyResources(this.nudMaxNumber, "nudMaxNumber");
            this.nudMaxNumber.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.nudMaxNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudMaxNumber.Name = "nudMaxNumber";
            this.nudMaxNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudMaxNumber.ValueChanged += new System.EventHandler(this.OnItemChanged);
            // 
            // FormEditConveyorSetting
            // 
            this.AcceptButton = this.bnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.Controls.Add(this.nudGripperAngle);
            this.Controls.Add(this.lbGripperAngle);
            this.Controls.Add(this.lbCaseOrientConveyor);
            this.Controls.Add(this.nudCaseAngle);
            this.Controls.Add(this.lbMaxNumber);
            this.Controls.Add(this.nudMaxNumber);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.graph3DConveyor);
            this.Controls.Add(this.statusStrip);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEditConveyorSetting";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudGripperAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaseAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxNumber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private Graphics3DConveyor graph3DConveyor;
        private System.Windows.Forms.Button bnOK;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.NumericUpDown nudGripperAngle;
        private System.Windows.Forms.Label lbGripperAngle;
        private System.Windows.Forms.Label lbCaseOrientConveyor;
        private System.Windows.Forms.NumericUpDown nudCaseAngle;
        private System.Windows.Forms.Label lbMaxNumber;
        private System.Windows.Forms.NumericUpDown nudMaxNumber;
    }
}