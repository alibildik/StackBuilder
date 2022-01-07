
namespace treeDiM.StackBuilder.Desktop
{
    partial class FormExcelMassAnalysis
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
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusStripLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.bnDownloadSampleSheet = new System.Windows.Forms.Button();
            this.cbSheets = new System.Windows.Forms.ComboBox();
            this.lbSheet = new System.Windows.Forms.Label();
            this.lbExcelFile = new System.Windows.Forms.Label();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnCompute = new System.Windows.Forms.Button();
            this.fileSelectExcel = new treeDiM.UserControls.FileSelect();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusStripLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 262);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(684, 22);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusStripLabel
            // 
            this.statusStripLabel.Name = "statusStripLabel";
            this.statusStripLabel.Size = new System.Drawing.Size(66, 17);
            this.statusStripLabel.Text = "statusLabel";
            // 
            // bnDownloadSampleSheet
            // 
            this.bnDownloadSampleSheet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnDownloadSampleSheet.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.bnDownloadSampleSheet.Location = new System.Drawing.Point(412, 28);
            this.bnDownloadSampleSheet.Name = "bnDownloadSampleSheet";
            this.bnDownloadSampleSheet.Size = new System.Drawing.Size(179, 23);
            this.bnDownloadSampleSheet.TabIndex = 25;
            this.bnDownloadSampleSheet.Text = "Download sample sheet";
            this.bnDownloadSampleSheet.UseVisualStyleBackColor = true;
            this.bnDownloadSampleSheet.Click += new System.EventHandler(this.OnDownloadSampleSheet);
            // 
            // cbSheets
            // 
            this.cbSheets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSheets.FormattingEnabled = true;
            this.cbSheets.Location = new System.Drawing.Point(130, 30);
            this.cbSheets.Name = "cbSheets";
            this.cbSheets.Size = new System.Drawing.Size(221, 21);
            this.cbSheets.TabIndex = 24;
            this.cbSheets.SelectedIndexChanged += new System.EventHandler(this.OnSheetChanged);
            // 
            // lbSheet
            // 
            this.lbSheet.AutoSize = true;
            this.lbSheet.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbSheet.Location = new System.Drawing.Point(7, 33);
            this.lbSheet.Name = "lbSheet";
            this.lbSheet.Size = new System.Drawing.Size(35, 13);
            this.lbSheet.TabIndex = 23;
            this.lbSheet.Text = "Sheet";
            // 
            // lbExcelFile
            // 
            this.lbExcelFile.AutoSize = true;
            this.lbExcelFile.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbExcelFile.Location = new System.Drawing.Point(7, 9);
            this.lbExcelFile.Name = "lbExcelFile";
            this.lbExcelFile.Size = new System.Drawing.Size(49, 13);
            this.lbExcelFile.TabIndex = 22;
            this.lbExcelFile.Text = "Excel file";
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(597, 4);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 26;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnCompute
            // 
            this.bnCompute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCompute.Location = new System.Drawing.Point(597, 236);
            this.bnCompute.Name = "bnCompute";
            this.bnCompute.Size = new System.Drawing.Size(75, 23);
            this.bnCompute.TabIndex = 27;
            this.bnCompute.Text = "Compute";
            this.bnCompute.UseVisualStyleBackColor = true;
            this.bnCompute.Click += new System.EventHandler(this.OnCompute);
            // 
            // fileSelectExcel
            // 
            this.fileSelectExcel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileSelectExcel.Location = new System.Drawing.Point(130, 4);
            this.fileSelectExcel.Name = "fileSelectExcel";
            this.fileSelectExcel.Size = new System.Drawing.Size(461, 20);
            this.fileSelectExcel.TabIndex = 21;
            this.fileSelectExcel.FileNameChanged += new System.EventHandler(this.OnFilePathChanged);
            // 
            // FormExcelMassAnalysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(684, 284);
            this.Controls.Add(this.bnCompute);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnDownloadSampleSheet);
            this.Controls.Add(this.cbSheets);
            this.Controls.Add(this.lbSheet);
            this.Controls.Add(this.lbExcelFile);
            this.Controls.Add(this.fileSelectExcel);
            this.Controls.Add(this.statusStrip);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormExcelMassAnalysis";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Mass Excel Analysis  (1 Pack Per Row)";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusStripLabel;
        private System.Windows.Forms.Button bnDownloadSampleSheet;
        private System.Windows.Forms.ComboBox cbSheets;
        private System.Windows.Forms.Label lbSheet;
        private System.Windows.Forms.Label lbExcelFile;
        private UserControls.FileSelect fileSelectExcel;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Button bnCompute;
    }
}