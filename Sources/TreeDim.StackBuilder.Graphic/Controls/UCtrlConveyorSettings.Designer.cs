
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
            this.pbConveyor = new System.Windows.Forms.PictureBox();
            this.lbCaseOrientConveyor = new System.Windows.Forms.Label();
            this.nudCaseAngle = new System.Windows.Forms.NumericUpDown();
            this.lbMaxNumber = new System.Windows.Forms.Label();
            this.nudMaxNumber = new System.Windows.Forms.NumericUpDown();
            this.lbGripperAngle = new System.Windows.Forms.Label();
            this.nudGrabberAngle = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.pbConveyor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaseAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGrabberAngle)).BeginInit();
            this.SuspendLayout();
            // 
            // pbConveyor
            // 
            this.pbConveyor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbConveyor.Location = new System.Drawing.Point(4, 4);
            this.pbConveyor.Name = "pbConveyor";
            this.pbConveyor.Size = new System.Drawing.Size(293, 219);
            this.pbConveyor.TabIndex = 0;
            this.pbConveyor.TabStop = false;
            this.pbConveyor.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
            // 
            // lbCaseOrientConveyor
            // 
            this.lbCaseOrientConveyor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbCaseOrientConveyor.AutoSize = true;
            this.lbCaseOrientConveyor.Location = new System.Drawing.Point(3, 231);
            this.lbCaseOrientConveyor.Name = "lbCaseOrientConveyor";
            this.lbCaseOrientConveyor.Size = new System.Drawing.Size(60, 13);
            this.lbCaseOrientConveyor.TabIndex = 1;
            this.lbCaseOrientConveyor.Text = "Case angle";
            // 
            // nudCaseAngle
            // 
            this.nudCaseAngle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudCaseAngle.Increment = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.nudCaseAngle.Location = new System.Drawing.Point(132, 229);
            this.nudCaseAngle.Maximum = new decimal(new int[] {
            270,
            0,
            0,
            0});
            this.nudCaseAngle.Name = "nudCaseAngle";
            this.nudCaseAngle.Size = new System.Drawing.Size(63, 20);
            this.nudCaseAngle.TabIndex = 2;
            this.nudCaseAngle.ValueChanged += new System.EventHandler(this.OnSettingsChanged);
            // 
            // lbMaxNumber
            // 
            this.lbMaxNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbMaxNumber.AutoSize = true;
            this.lbMaxNumber.Location = new System.Drawing.Point(3, 280);
            this.lbMaxNumber.Name = "lbMaxNumber";
            this.lbMaxNumber.Size = new System.Drawing.Size(68, 13);
            this.lbMaxNumber.TabIndex = 3;
            this.lbMaxNumber.Text = "Max. number";
            // 
            // nudMaxNumber
            // 
            this.nudMaxNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudMaxNumber.Location = new System.Drawing.Point(132, 278);
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
            this.nudMaxNumber.Size = new System.Drawing.Size(63, 20);
            this.nudMaxNumber.TabIndex = 4;
            this.nudMaxNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudMaxNumber.ValueChanged += new System.EventHandler(this.OnSettingsChanged);
            // 
            // lbGripperAngle
            // 
            this.lbGripperAngle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbGripperAngle.AutoSize = true;
            this.lbGripperAngle.Location = new System.Drawing.Point(3, 256);
            this.lbGripperAngle.Name = "lbGripperAngle";
            this.lbGripperAngle.Size = new System.Drawing.Size(70, 13);
            this.lbGripperAngle.TabIndex = 5;
            this.lbGripperAngle.Text = "Gripper angle";
            // 
            // nudGrabberAngle
            // 
            this.nudGrabberAngle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudGrabberAngle.Increment = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.nudGrabberAngle.Location = new System.Drawing.Point(132, 254);
            this.nudGrabberAngle.Maximum = new decimal(new int[] {
            270,
            0,
            0,
            0});
            this.nudGrabberAngle.Name = "nudGrabberAngle";
            this.nudGrabberAngle.Size = new System.Drawing.Size(63, 20);
            this.nudGrabberAngle.TabIndex = 6;
            this.nudGrabberAngle.ValueChanged += new System.EventHandler(this.OnSettingsChanged);
            // 
            // UCtrlConveyorSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.nudGrabberAngle);
            this.Controls.Add(this.lbGripperAngle);
            this.Controls.Add(this.nudMaxNumber);
            this.Controls.Add(this.lbMaxNumber);
            this.Controls.Add(this.nudCaseAngle);
            this.Controls.Add(this.lbCaseOrientConveyor);
            this.Controls.Add(this.pbConveyor);
            this.Name = "UCtrlConveyorSettings";
            this.Size = new System.Drawing.Size(300, 300);
            ((System.ComponentModel.ISupportInitialize)(this.pbConveyor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaseAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGrabberAngle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbConveyor;
        private System.Windows.Forms.Label lbCaseOrientConveyor;
        private System.Windows.Forms.NumericUpDown nudCaseAngle;
        private System.Windows.Forms.Label lbMaxNumber;
        private System.Windows.Forms.NumericUpDown nudMaxNumber;
        private System.Windows.Forms.Label lbGripperAngle;
        private System.Windows.Forms.NumericUpDown nudGrabberAngle;
    }
}
