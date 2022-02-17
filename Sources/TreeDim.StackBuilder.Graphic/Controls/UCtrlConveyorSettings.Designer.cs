
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
            this.gbAdd = new System.Windows.Forms.GroupBox();
            this.bnEdit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaseAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGripperAngle)).BeginInit();
            this.gbAdd.SuspendLayout();
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
            this.nudCaseAngle.ValueChanged += new System.EventHandler(this.OnAddSettingsChanged);
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
            this.nudMaxNumber.ValueChanged += new System.EventHandler(this.OnAddSettingsChanged);
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
            this.nudGripperAngle.ValueChanged += new System.EventHandler(this.OnAddSettingsChanged);
            // 
            // lbConveyorSetting
            // 
            resources.ApplyResources(this.lbConveyorSetting, "lbConveyorSetting");
            this.lbConveyorSetting.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbConveyorSetting.FormattingEnabled = true;
            this.lbConveyorSetting.Name = "lbConveyorSetting";
            this.lbConveyorSetting.Packable = null;
            this.lbConveyorSetting.SelectedIndexChanged += new System.EventHandler(this.OnSelectedSettingChanged);
            // 
            // graph3DConveyor
            // 
            resources.ApplyResources(this.graph3DConveyor, "graph3DConveyor");
            this.graph3DConveyor.CaseAngle = 0;
            this.graph3DConveyor.GripperAngle = 0;
            this.graph3DConveyor.MaxDropNumber = 0;
            this.graph3DConveyor.Name = "graph3DConveyor";
            this.graph3DConveyor.Packable = null;
            this.graph3DConveyor.TabStop = false;
            // 
            // gbAdd
            // 
            resources.ApplyResources(this.gbAdd, "gbAdd");
            this.gbAdd.Controls.Add(this.graph3DConveyor);
            this.gbAdd.Controls.Add(this.nudGripperAngle);
            this.gbAdd.Controls.Add(this.lbGripperAngle);
            this.gbAdd.Controls.Add(this.bnAdd);
            this.gbAdd.Controls.Add(this.lbCaseOrientConveyor);
            this.gbAdd.Controls.Add(this.nudCaseAngle);
            this.gbAdd.Controls.Add(this.lbMaxNumber);
            this.gbAdd.Controls.Add(this.nudMaxNumber);
            this.gbAdd.Name = "gbAdd";
            this.gbAdd.TabStop = false;
            // 
            // bnEdit
            // 
            resources.ApplyResources(this.bnEdit, "bnEdit");
            this.bnEdit.Name = "bnEdit";
            this.bnEdit.UseVisualStyleBackColor = true;
            this.bnEdit.Click += new System.EventHandler(this.OnSettingEdit);
            // 
            // UCtrlConveyorSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bnEdit);
            this.Controls.Add(this.gbAdd);
            this.Controls.Add(this.lbConveyorSetting);
            this.Controls.Add(this.bnRemove);
            this.Name = "UCtrlConveyorSettings";
            ((System.ComponentModel.ISupportInitialize)(this.nudCaseAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGripperAngle)).EndInit();
            this.gbAdd.ResumeLayout(false);
            this.gbAdd.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.GroupBox gbAdd;
        private System.Windows.Forms.Button bnEdit;
    }
}
