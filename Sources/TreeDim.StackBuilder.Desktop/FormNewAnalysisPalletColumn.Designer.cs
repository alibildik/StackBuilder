namespace treeDiM.StackBuilder.Desktop
{
    partial class FormNewAnalysisPalletColumn
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
            this.graphCtrl = new treeDiM.StackBuilder.Graphics.Graphics3DControl();
            this.cbInputPallet1 = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered();
            this.cbInputPallet2 = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered();
            this.lbInputPallet1 = new System.Windows.Forms.Label();
            this.lbInputPallet2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrl)).BeginInit();
            this.SuspendLayout();
            // 
            // tbDescription
            // 
            this.tbDescription.Size = new System.Drawing.Size(666, 20);
            // 
            // graphCtrl
            // 
            this.graphCtrl.AngleHoriz = 225D;
            this.graphCtrl.Location = new System.Drawing.Point(247, 60);
            this.graphCtrl.Name = "graphCtrl";
            this.graphCtrl.Size = new System.Drawing.Size(550, 445);
            this.graphCtrl.TabIndex = 13;
            this.graphCtrl.Viewer = null;
            // 
            // cbInputPallet1
            // 
            this.cbInputPallet1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInputPallet1.FormattingEnabled = true;
            this.cbInputPallet1.Location = new System.Drawing.Point(104, 75);
            this.cbInputPallet1.Name = "cbInputPallet1";
            this.cbInputPallet1.Size = new System.Drawing.Size(121, 21);
            this.cbInputPallet1.TabIndex = 14;
            this.cbInputPallet1.SelectedIndexChanged += new System.EventHandler(this.OnInputChanged);
            // 
            // cbInputPallet2
            // 
            this.cbInputPallet2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInputPallet2.FormattingEnabled = true;
            this.cbInputPallet2.Location = new System.Drawing.Point(104, 102);
            this.cbInputPallet2.Name = "cbInputPallet2";
            this.cbInputPallet2.Size = new System.Drawing.Size(121, 21);
            this.cbInputPallet2.TabIndex = 15;
            this.cbInputPallet2.SelectedValueChanged += new System.EventHandler(this.OnInputChanged);
            // 
            // lbInputPallet1
            // 
            this.lbInputPallet1.AutoSize = true;
            this.lbInputPallet1.Location = new System.Drawing.Point(6, 78);
            this.lbInputPallet1.Name = "lbInputPallet1";
            this.lbInputPallet1.Size = new System.Drawing.Size(68, 13);
            this.lbInputPallet1.TabIndex = 16;
            this.lbInputPallet1.Text = "Input pallet 1";
            // 
            // lbInputPallet2
            // 
            this.lbInputPallet2.AutoSize = true;
            this.lbInputPallet2.Location = new System.Drawing.Point(6, 105);
            this.lbInputPallet2.Name = "lbInputPallet2";
            this.lbInputPallet2.Size = new System.Drawing.Size(68, 13);
            this.lbInputPallet2.TabIndex = 17;
            this.lbInputPallet2.Text = "Input pallet 2";
            // 
            // FormNewAnalysisPalletColumn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.lbInputPallet2);
            this.Controls.Add(this.lbInputPallet1);
            this.Controls.Add(this.cbInputPallet2);
            this.Controls.Add(this.cbInputPallet1);
            this.Controls.Add(this.graphCtrl);
            this.Name = "FormNewAnalysisPalletColumn";
            this.Text = "Pallet column...";
            this.Controls.SetChildIndex(this.lbName, 0);
            this.Controls.SetChildIndex(this.lbDescription, 0);
            this.Controls.SetChildIndex(this.tbName, 0);
            this.Controls.SetChildIndex(this.tbDescription, 0);
            this.Controls.SetChildIndex(this.graphCtrl, 0);
            this.Controls.SetChildIndex(this.cbInputPallet1, 0);
            this.Controls.SetChildIndex(this.cbInputPallet2, 0);
            this.Controls.SetChildIndex(this.lbInputPallet1, 0);
            this.Controls.SetChildIndex(this.lbInputPallet2, 0);
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Graphics.Graphics3DControl graphCtrl;
        private Graphics.Controls.CCtrlComboFiltered cbInputPallet1;
        private Graphics.Controls.CCtrlComboFiltered cbInputPallet2;
        private System.Windows.Forms.Label lbInputPallet1;
        private System.Windows.Forms.Label lbInputPallet2;
    }
}