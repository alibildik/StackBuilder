
namespace treeDiM.StackBuilder.Graphics.Controls
{
    partial class UCtrlConveyorSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCtrlConveyorSettings));
            this.lbCaseOrientConveyor = new System.Windows.Forms.Label();
            this.nudCaseAngle = new System.Windows.Forms.NumericUpDown();
            this.lbMaxNumber = new System.Windows.Forms.Label();
            this.nudMaxNumber = new System.Windows.Forms.NumericUpDown();
            this.bnAdd = new System.Windows.Forms.Button();
            this.bnRemove = new System.Windows.Forms.Button();
            this.lbGripperAngle = new System.Windows.Forms.Label();
            this.nudGripperAngle = new System.Windows.Forms.NumericUpDown();
            this.lbConveyorSetting = new treeDiM.StackBuilder.Graphics.Controls.CCtrlListBoxConveyor();
            this.graph3DConveyor = new treeDiM.StackBuilder.Graphics.Graphics3DConveyor();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaseAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGripperAngle)).BeginInit();
            this.SuspendLayout();
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
            this.nudCaseAngle.ValueChanged += new System.EventHandler(this.OnSettingsChanged);
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
            this.nudMaxNumber.ValueChanged += new System.EventHandler(this.OnSettingsChanged);
            // 
            // bnAdd
            // 
            resources.ApplyResources(this.bnAdd, "bnAdd");
            this.bnAdd.Name = "bnAdd";
            this.bnAdd.UseVisualStyleBackColor = true;
            this.bnAdd.Click += new System.EventHandler(this.OnSettingAdd);
            // 
            // bnRemove
            // 
            resources.ApplyResources(this.bnRemove, "bnRemove");
            this.bnRemove.Name = "bnRemove";
            this.bnRemove.UseVisualStyleBackColor = true;
            this.bnRemove.Click += new System.EventHandler(this.OnSettingRemove);
            // 
            // lbGripperAngle
            // 
            resources.ApplyResources(this.lbGripperAngle, "lbGripperAngle");
            this.lbGripperAngle.Name = "lbGripperAngle";
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
            // 
            // lbConveyorSetting
            // 
            resources.ApplyResources(this.lbConveyorSetting, "lbConveyorSetting");
            this.lbConveyorSetting.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbConveyorSetting.FormattingEnabled = true;
            this.lbConveyorSetting.Name = "lbConveyorSetting";
            this.lbConveyorSetting.Packable = null;
            // 
            // graph3DConveyor
            // 
            resources.ApplyResources(this.graph3DConveyor, "graph3DConveyor");
            this.graph3DConveyor.CaseAngle = 0;
            this.graph3DConveyor.MaxDropNumber = 0;
            this.graph3DConveyor.Name = "graph3DConveyor";
            this.graph3DConveyor.Packable = null;
            this.graph3DConveyor.TabStop = false;
            // 
            // UCtrlConveyorSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.nudGripperAngle);
            this.Controls.Add(this.lbGripperAngle);
            this.Controls.Add(this.lbConveyorSetting);
            this.Controls.Add(this.bnRemove);
            this.Controls.Add(this.bnAdd);
            this.Controls.Add(this.nudMaxNumber);
            this.Controls.Add(this.lbMaxNumber);
            this.Controls.Add(this.nudCaseAngle);
            this.Controls.Add(this.lbCaseOrientConveyor);
            this.Controls.Add(this.graph3DConveyor);
            this.Name = "UCtrlConveyorSettings";
            ((System.ComponentModel.ISupportInitialize)(this.nudCaseAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGripperAngle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Graphics3DConveyor graph3DConveyor;
        private System.Windows.Forms.Label lbCaseOrientConveyor;
        private System.Windows.Forms.NumericUpDown nudCaseAngle;
        private System.Windows.Forms.Label lbMaxNumber;
        private System.Windows.Forms.NumericUpDown nudMaxNumber;
        private System.Windows.Forms.Button bnAdd;
        private System.Windows.Forms.Button bnRemove;
        private CCtrlListBoxConveyor lbConveyorSetting;
        private System.Windows.Forms.Label lbGripperAngle;
        private System.Windows.Forms.NumericUpDown nudGripperAngle;
    }
}
