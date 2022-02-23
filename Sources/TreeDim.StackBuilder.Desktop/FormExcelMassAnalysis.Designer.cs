
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormExcelMassAnalysis));
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
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusStripLabel});
            this.statusStrip.Name = "statusStrip";
            // 
            // statusStripLabel
            // 
            resources.ApplyResources(this.statusStripLabel, "statusStripLabel");
            this.statusStripLabel.Name = "statusStripLabel";
            // 
            // bnDownloadSampleSheet
            // 
            resources.ApplyResources(this.bnDownloadSampleSheet, "bnDownloadSampleSheet");
            this.bnDownloadSampleSheet.Name = "bnDownloadSampleSheet";
            this.bnDownloadSampleSheet.UseVisualStyleBackColor = true;
            this.bnDownloadSampleSheet.Click += new System.EventHandler(this.OnDownloadSampleSheet);
            // 
            // cbSheets
            // 
            resources.ApplyResources(this.cbSheets, "cbSheets");
            this.cbSheets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSheets.FormattingEnabled = true;
            this.cbSheets.Name = "cbSheets";
            this.cbSheets.SelectedIndexChanged += new System.EventHandler(this.OnSheetChanged);
            // 
            // lbSheet
            // 
            resources.ApplyResources(this.lbSheet, "lbSheet");
            this.lbSheet.Name = "lbSheet";
            // 
            // lbExcelFile
            // 
            resources.ApplyResources(this.lbExcelFile, "lbExcelFile");
            this.lbExcelFile.Name = "lbExcelFile";
            // 
            // bnCancel
            // 
            resources.ApplyResources(this.bnCancel, "bnCancel");
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnCompute
            // 
            resources.ApplyResources(this.bnCompute, "bnCompute");
            this.bnCompute.Name = "bnCompute";
            this.bnCompute.UseVisualStyleBackColor = true;
            this.bnCompute.Click += new System.EventHandler(this.OnCompute);
            // 
            // fileSelectExcel
            // 
            resources.ApplyResources(this.fileSelectExcel, "fileSelectExcel");
            this.fileSelectExcel.Name = "fileSelectExcel";
            this.fileSelectExcel.FileNameChanged += new System.EventHandler(this.OnFilePathChanged);
            // 
            // FormExcelMassAnalysis
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
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