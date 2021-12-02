
namespace treeDiM.StackBuilder.Graphics.TestUCtrlConveyorSettings
{
    partial class FormMain
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
            this.uCtrlConveyor = new treeDiM.StackBuilder.Graphics.Controls.UCtrlConveyorSettings();
            this.SuspendLayout();
            // 
            // uCtrlConveyor
            // 
            this.uCtrlConveyor.BoxProperties = null;
            this.uCtrlConveyor.Location = new System.Drawing.Point(13, 13);
            this.uCtrlConveyor.Name = "uCtrlConveyor";
            this.uCtrlConveyor.Size = new System.Drawing.Size(300, 200);
            this.uCtrlConveyor.TabIndex = 0;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.uCtrlConveyor);
            this.Name = "FormMain";
            this.Text = "FormMain";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.UCtrlConveyorSettings uCtrlConveyor;
    }
}

