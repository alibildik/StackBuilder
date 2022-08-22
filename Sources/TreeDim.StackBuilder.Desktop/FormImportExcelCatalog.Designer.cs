namespace treeDiM.StackBuilder.Desktop
{
    partial class FormImportExcelCatalog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImportExcelCatalog));
            this.splitContainerHoriz = new System.Windows.Forms.SplitContainer();
            this.chkbOverwrite = new System.Windows.Forms.CheckBox();
            this.bnDownload = new System.Windows.Forms.Button();
            this.bnStop = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.bnCancel = new System.Windows.Forms.Button();
            this.lbExcelFilePath = new System.Windows.Forms.Label();
            this.excelFileSelect = new treeDiM.UserControls.FileSelect();
            this.bnImport = new System.Windows.Forms.Button();
            this.richTextBoxLog = new System.Windows.Forms.RichTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz)).BeginInit();
            this.splitContainerHoriz.Panel1.SuspendLayout();
            this.splitContainerHoriz.Panel2.SuspendLayout();
            this.splitContainerHoriz.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerHoriz
            // 
            resources.ApplyResources(this.splitContainerHoriz, "splitContainerHoriz");
            this.splitContainerHoriz.Name = "splitContainerHoriz";
            // 
            // splitContainerHoriz.Panel1
            // 
            this.splitContainerHoriz.Panel1.Controls.Add(this.chkbOverwrite);
            this.splitContainerHoriz.Panel1.Controls.Add(this.bnDownload);
            this.splitContainerHoriz.Panel1.Controls.Add(this.bnStop);
            this.splitContainerHoriz.Panel1.Controls.Add(this.progressBar);
            this.splitContainerHoriz.Panel1.Controls.Add(this.bnCancel);
            this.splitContainerHoriz.Panel1.Controls.Add(this.lbExcelFilePath);
            this.splitContainerHoriz.Panel1.Controls.Add(this.excelFileSelect);
            this.splitContainerHoriz.Panel1.Controls.Add(this.bnImport);
            // 
            // splitContainerHoriz.Panel2
            // 
            this.splitContainerHoriz.Panel2.Controls.Add(this.richTextBoxLog);
            // 
            // chkbOverwrite
            // 
            resources.ApplyResources(this.chkbOverwrite, "chkbOverwrite");
            this.chkbOverwrite.Name = "chkbOverwrite";
            this.chkbOverwrite.UseVisualStyleBackColor = true;
            // 
            // bnDownload
            // 
            resources.ApplyResources(this.bnDownload, "bnDownload");
            this.bnDownload.Name = "bnDownload";
            this.bnDownload.UseVisualStyleBackColor = true;
            this.bnDownload.Click += new System.EventHandler(this.OnDownloadExcelTemplate);
            // 
            // bnStop
            // 
            resources.ApplyResources(this.bnStop, "bnStop");
            this.bnStop.Name = "bnStop";
            this.bnStop.UseVisualStyleBackColor = true;
            this.bnStop.Click += new System.EventHandler(this.OnStopWork);
            // 
            // progressBar
            // 
            resources.ApplyResources(this.progressBar, "progressBar");
            this.progressBar.Name = "progressBar";
            // 
            // bnCancel
            // 
            resources.ApplyResources(this.bnCancel, "bnCancel");
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // lbExcelFilePath
            // 
            resources.ApplyResources(this.lbExcelFilePath, "lbExcelFilePath");
            this.lbExcelFilePath.Name = "lbExcelFilePath";
            // 
            // excelFileSelect
            // 
            resources.ApplyResources(this.excelFileSelect, "excelFileSelect");
            this.excelFileSelect.Filter = "MS Excel work sheet (*.xls;*.xlsx)|*.xls;*.xlsx";
            this.excelFileSelect.Name = "excelFileSelect";
            this.excelFileSelect.FileNameChanged += new System.EventHandler(this.OnFileNameChanged);
            // 
            // bnImport
            // 
            resources.ApplyResources(this.bnImport, "bnImport");
            this.bnImport.Name = "bnImport";
            this.bnImport.UseVisualStyleBackColor = true;
            this.bnImport.Click += new System.EventHandler(this.OnButtonImportClick);
            // 
            // richTextBoxLog
            // 
            resources.ApplyResources(this.richTextBoxLog, "richTextBoxLog");
            this.richTextBoxLog.Name = "richTextBoxLog";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            resources.ApplyResources(this.statusLabel, "statusLabel");
            // 
            // FormImportExcelCatalog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.Controls.Add(this.splitContainerHoriz);
            this.Controls.Add(this.statusStrip1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormImportExcelCatalog";
            this.ShowInTaskbar = false;
            this.splitContainerHoriz.Panel1.ResumeLayout(false);
            this.splitContainerHoriz.Panel1.PerformLayout();
            this.splitContainerHoriz.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz)).EndInit();
            this.splitContainerHoriz.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbExcelFilePath;
        private treeDiM.UserControls.FileSelect excelFileSelect;
        private System.Windows.Forms.Button bnImport;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.SplitContainer splitContainerHoriz;
        private System.Windows.Forms.RichTextBox richTextBoxLog;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Button bnStop;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button bnDownload;
        private System.Windows.Forms.CheckBox chkbOverwrite;
    }
}