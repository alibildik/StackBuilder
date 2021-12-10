
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
            this.graph3DConveyor = new treeDiM.StackBuilder.Graphics.Graphics3DConveyor();
            this.lbCaseOrientConveyor = new System.Windows.Forms.Label();
            this.nudCaseAngle = new System.Windows.Forms.NumericUpDown();
            this.lbMaxNumber = new System.Windows.Forms.Label();
            this.nudMaxNumber = new System.Windows.Forms.NumericUpDown();
            this.lbGripperAngle = new System.Windows.Forms.Label();
            this.nudGrabberAngle = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaseAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGrabberAngle)).BeginInit();
            this.SuspendLayout();
            // 
            // graph3DConveyor
            // 
            resources.ApplyResources(this.graph3DConveyor, "graph3DConveyor");
            this.graph3DConveyor.AngleHoriz = 95D;
            this.graph3DConveyor.CaseAngle = 0;
            this.graph3DConveyor.MaxDropNumber = 0;
            this.graph3DConveyor.Name = "graph3DConveyor";
            this.graph3DConveyor.Packable = null;
            this.graph3DConveyor.TabStop = false;
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
            // lbGripperAngle
            // 
            resources.ApplyResources(this.lbGripperAngle, "lbGripperAngle");
            this.lbGripperAngle.Name = "lbGripperAngle";
            // 
            // nudGrabberAngle
            // 
            resources.ApplyResources(this.nudGrabberAngle, "nudGrabberAngle");
            this.nudGrabberAngle.Increment = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.nudGrabberAngle.Maximum = new decimal(new int[] {
            270,
            0,
            0,
            0});
            this.nudGrabberAngle.Name = "nudGrabberAngle";
            this.nudGrabberAngle.ValueChanged += new System.EventHandler(this.OnSettingsChanged);
            // 
            // UCtrlConveyorSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.nudGrabberAngle);
            this.Controls.Add(this.lbGripperAngle);
            this.Controls.Add(this.nudMaxNumber);
            this.Controls.Add(this.lbMaxNumber);
            this.Controls.Add(this.nudCaseAngle);
            this.Controls.Add(this.lbCaseOrientConveyor);
            this.Controls.Add(this.graph3DConveyor);
            this.Name = "UCtrlConveyorSettings";
            ((System.ComponentModel.ISupportInitialize)(this.nudCaseAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGrabberAngle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Graphics3DConveyor graph3DConveyor;
        private System.Windows.Forms.Label lbCaseOrientConveyor;
        private System.Windows.Forms.NumericUpDown nudCaseAngle;
        private System.Windows.Forms.Label lbMaxNumber;
        private System.Windows.Forms.NumericUpDown nudMaxNumber;
        private System.Windows.Forms.Label lbGripperAngle;
        private System.Windows.Forms.NumericUpDown nudGrabberAngle;
    }
}
