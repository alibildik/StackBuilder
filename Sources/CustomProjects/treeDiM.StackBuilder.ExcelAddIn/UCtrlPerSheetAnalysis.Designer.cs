namespace treeDiM.StackBuilder.ExcelAddIn
{
    partial class UCtrlPerSheetAnalysis
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
            this.bnCompute = new System.Windows.Forms.Button();
            this.uCtrlLayerList = new treeDiM.StackBuilder.Graphics.UCtrlLayerList();
            this.uCtrlCaseOrientation = new treeDiM.StackBuilder.Graphics.UCtrlCaseOrientation();
            this.SuspendLayout();
            // 
            // bnCompute
            // 
            this.bnCompute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnCompute.Location = new System.Drawing.Point(7, 624);
            this.bnCompute.Name = "bnCompute";
            this.bnCompute.Size = new System.Drawing.Size(75, 23);
            this.bnCompute.TabIndex = 0;
            this.bnCompute.Text = "Compute";
            this.bnCompute.UseVisualStyleBackColor = true;
            this.bnCompute.Click += new System.EventHandler(this.OnCompute);
            // 
            // uCtrlLayerList
            // 
            this.uCtrlLayerList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uCtrlLayerList.AutoScroll = true;
            this.uCtrlLayerList.Location = new System.Drawing.Point(0, 124);
            this.uCtrlLayerList.Name = "uCtrlLayerList";
            this.uCtrlLayerList.SingleSelection = false;
            this.uCtrlLayerList.Size = new System.Drawing.Size(297, 371);
            this.uCtrlLayerList.TabIndex = 1;
            // 
            // uCtrlCaseOrientation
            // 
            this.uCtrlCaseOrientation.AllowedOrientations = new bool[] {
        false,
        false,
        true};
            this.uCtrlCaseOrientation.Location = new System.Drawing.Point(5, 7);
            this.uCtrlCaseOrientation.Name = "uCtrlCaseOrientation";
            this.uCtrlCaseOrientation.Size = new System.Drawing.Size(290, 110);
            this.uCtrlCaseOrientation.TabIndex = 2;
            // 
            // UCtrlPerSheetAnalysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uCtrlCaseOrientation);
            this.Controls.Add(this.uCtrlLayerList);
            this.Controls.Add(this.bnCompute);
            this.Name = "UCtrlPerSheetAnalysis";
            this.Size = new System.Drawing.Size(300, 650);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bnCompute;
        private Graphics.UCtrlLayerList uCtrlLayerList;
        private Graphics.UCtrlCaseOrientation uCtrlCaseOrientation;
    }
}
