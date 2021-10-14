
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
            this.bnCompute = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.gbPallet = new System.Windows.Forms.GroupBox();
            this.chkbOnlyZOrientation = new System.Windows.Forms.CheckBox();
            this.uCtrlOverhang = new treeDiM.Basics.UCtrlDualDouble();
            this.uCtrlMaxPalletHeight = new treeDiM.Basics.UCtrlDouble();
            this.lbPallets = new System.Windows.Forms.CheckedListBox();
            this.fileSelectExcel = new treeDiM.UserControls.FileSelect();
            this.lbExcelFile = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusStripLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.gbInputColumns = new System.Windows.Forms.GroupBox();
            this.cbWeight = new System.Windows.Forms.ComboBox();
            this.cbHeight = new System.Windows.Forms.ComboBox();
            this.cbWidth = new System.Windows.Forms.ComboBox();
            this.cbLength = new System.Windows.Forms.ComboBox();
            this.cbDescription = new System.Windows.Forms.ComboBox();
            this.cbName = new System.Windows.Forms.ComboBox();
            this.chkbDescription = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lbHeight = new System.Windows.Forms.Label();
            this.lbWidth = new System.Windows.Forms.Label();
            this.lbLength = new System.Windows.Forms.Label();
            this.lbName = new System.Windows.Forms.Label();
            this.gbOutput = new System.Windows.Forms.GroupBox();
            this.bnFolderReports = new System.Windows.Forms.Button();
            this.tbFolderReports = new System.Windows.Forms.TextBox();
            this.chkbGenerateReportInFolder = new System.Windows.Forms.CheckBox();
            this.bnFolderImages = new System.Windows.Forms.Button();
            this.tbFolderImages = new System.Windows.Forms.TextBox();
            this.chkbGenerateImageInFolder = new System.Windows.Forms.CheckBox();
            this.chkbGenerateImageInRow = new System.Windows.Forms.CheckBox();
            this.nudImageSize = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.cbOutputStart = new System.Windows.Forms.ComboBox();
            this.lbStartOutput = new System.Windows.Forms.Label();
            this.lbSheet = new System.Windows.Forms.Label();
            this.cbSheets = new System.Windows.Forms.ComboBox();
            this.bnDownloadSampleSheet = new System.Windows.Forms.Button();
            this.chkbAllowCombinations = new System.Windows.Forms.CheckBox();
            this.gbPallet.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.gbInputColumns.SuspendLayout();
            this.gbOutput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudImageSize)).BeginInit();
            this.SuspendLayout();
            // 
            // bnCompute
            // 
            resources.ApplyResources(this.bnCompute, "bnCompute");
            this.bnCompute.Name = "bnCompute";
            this.bnCompute.UseVisualStyleBackColor = true;
            this.bnCompute.Click += new System.EventHandler(this.OnCompute);
            // 
            // bnCancel
            // 
            resources.ApplyResources(this.bnCancel, "bnCancel");
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // gbPallet
            // 
            this.gbPallet.Controls.Add(this.chkbAllowCombinations);
            this.gbPallet.Controls.Add(this.chkbOnlyZOrientation);
            this.gbPallet.Controls.Add(this.uCtrlOverhang);
            this.gbPallet.Controls.Add(this.uCtrlMaxPalletHeight);
            this.gbPallet.Controls.Add(this.lbPallets);
            resources.ApplyResources(this.gbPallet, "gbPallet");
            this.gbPallet.Name = "gbPallet";
            this.gbPallet.TabStop = false;
            // 
            // chkbOnlyZOrientation
            // 
            resources.ApplyResources(this.chkbOnlyZOrientation, "chkbOnlyZOrientation");
            this.chkbOnlyZOrientation.Name = "chkbOnlyZOrientation";
            this.chkbOnlyZOrientation.UseVisualStyleBackColor = true;
            // 
            // uCtrlOverhang
            // 
            resources.ApplyResources(this.uCtrlOverhang, "uCtrlOverhang");
            this.uCtrlOverhang.MinValue = -10000D;
            this.uCtrlOverhang.Name = "uCtrlOverhang";
            this.uCtrlOverhang.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlOverhang.ValueX = 0D;
            this.uCtrlOverhang.ValueY = 0D;
            // 
            // uCtrlMaxPalletHeight
            // 
            resources.ApplyResources(this.uCtrlMaxPalletHeight, "uCtrlMaxPalletHeight");
            this.uCtrlMaxPalletHeight.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.uCtrlMaxPalletHeight.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.uCtrlMaxPalletHeight.Name = "uCtrlMaxPalletHeight";
            this.uCtrlMaxPalletHeight.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlMaxPalletHeight.ValueChanged += new treeDiM.Basics.UCtrlDouble.ValueChangedDelegate(this.UpdateStatus);
            // 
            // lbPallets
            // 
            this.lbPallets.CheckOnClick = true;
            this.lbPallets.FormattingEnabled = true;
            resources.ApplyResources(this.lbPallets, "lbPallets");
            this.lbPallets.Name = "lbPallets";
            this.lbPallets.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.OnItemChecked);
            // 
            // fileSelectExcel
            // 
            resources.ApplyResources(this.fileSelectExcel, "fileSelectExcel");
            this.fileSelectExcel.Name = "fileSelectExcel";
            this.fileSelectExcel.FileNameChanged += new System.EventHandler(this.OnFilePathChanged);
            // 
            // lbExcelFile
            // 
            resources.ApplyResources(this.lbExcelFile, "lbExcelFile");
            this.lbExcelFile.Name = "lbExcelFile";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusStripLabel});
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Name = "statusStrip";
            // 
            // statusStripLabel
            // 
            this.statusStripLabel.Name = "statusStripLabel";
            resources.ApplyResources(this.statusStripLabel, "statusStripLabel");
            // 
            // gbInputColumns
            // 
            this.gbInputColumns.Controls.Add(this.cbWeight);
            this.gbInputColumns.Controls.Add(this.cbHeight);
            this.gbInputColumns.Controls.Add(this.cbWidth);
            this.gbInputColumns.Controls.Add(this.cbLength);
            this.gbInputColumns.Controls.Add(this.cbDescription);
            this.gbInputColumns.Controls.Add(this.cbName);
            this.gbInputColumns.Controls.Add(this.chkbDescription);
            this.gbInputColumns.Controls.Add(this.label5);
            this.gbInputColumns.Controls.Add(this.lbHeight);
            this.gbInputColumns.Controls.Add(this.lbWidth);
            this.gbInputColumns.Controls.Add(this.lbLength);
            this.gbInputColumns.Controls.Add(this.lbName);
            resources.ApplyResources(this.gbInputColumns, "gbInputColumns");
            this.gbInputColumns.Name = "gbInputColumns";
            this.gbInputColumns.TabStop = false;
            // 
            // cbWeight
            // 
            this.cbWeight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWeight.FormattingEnabled = true;
            resources.ApplyResources(this.cbWeight, "cbWeight");
            this.cbWeight.Name = "cbWeight";
            // 
            // cbHeight
            // 
            this.cbHeight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHeight.FormattingEnabled = true;
            resources.ApplyResources(this.cbHeight, "cbHeight");
            this.cbHeight.Name = "cbHeight";
            // 
            // cbWidth
            // 
            this.cbWidth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWidth.FormattingEnabled = true;
            resources.ApplyResources(this.cbWidth, "cbWidth");
            this.cbWidth.Name = "cbWidth";
            // 
            // cbLength
            // 
            this.cbLength.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLength.FormattingEnabled = true;
            resources.ApplyResources(this.cbLength, "cbLength");
            this.cbLength.Name = "cbLength";
            // 
            // cbDescription
            // 
            this.cbDescription.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDescription.FormattingEnabled = true;
            resources.ApplyResources(this.cbDescription, "cbDescription");
            this.cbDescription.Name = "cbDescription";
            // 
            // cbName
            // 
            this.cbName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbName.FormattingEnabled = true;
            resources.ApplyResources(this.cbName, "cbName");
            this.cbName.Name = "cbName";
            // 
            // chkbDescription
            // 
            resources.ApplyResources(this.chkbDescription, "chkbDescription");
            this.chkbDescription.Name = "chkbDescription";
            this.chkbDescription.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // lbHeight
            // 
            resources.ApplyResources(this.lbHeight, "lbHeight");
            this.lbHeight.Name = "lbHeight";
            // 
            // lbWidth
            // 
            resources.ApplyResources(this.lbWidth, "lbWidth");
            this.lbWidth.Name = "lbWidth";
            // 
            // lbLength
            // 
            resources.ApplyResources(this.lbLength, "lbLength");
            this.lbLength.Name = "lbLength";
            // 
            // lbName
            // 
            resources.ApplyResources(this.lbName, "lbName");
            this.lbName.Name = "lbName";
            // 
            // gbOutput
            // 
            this.gbOutput.Controls.Add(this.bnFolderReports);
            this.gbOutput.Controls.Add(this.tbFolderReports);
            this.gbOutput.Controls.Add(this.chkbGenerateReportInFolder);
            this.gbOutput.Controls.Add(this.bnFolderImages);
            this.gbOutput.Controls.Add(this.tbFolderImages);
            this.gbOutput.Controls.Add(this.chkbGenerateImageInFolder);
            this.gbOutput.Controls.Add(this.chkbGenerateImageInRow);
            this.gbOutput.Controls.Add(this.nudImageSize);
            this.gbOutput.Controls.Add(this.label1);
            this.gbOutput.Controls.Add(this.cbOutputStart);
            this.gbOutput.Controls.Add(this.lbStartOutput);
            resources.ApplyResources(this.gbOutput, "gbOutput");
            this.gbOutput.Name = "gbOutput";
            this.gbOutput.TabStop = false;
            // 
            // bnFolderReports
            // 
            resources.ApplyResources(this.bnFolderReports, "bnFolderReports");
            this.bnFolderReports.Name = "bnFolderReports";
            this.bnFolderReports.UseVisualStyleBackColor = true;
            // 
            // tbFolderReports
            // 
            resources.ApplyResources(this.tbFolderReports, "tbFolderReports");
            this.tbFolderReports.Name = "tbFolderReports";
            // 
            // chkbGenerateReportInFolder
            // 
            resources.ApplyResources(this.chkbGenerateReportInFolder, "chkbGenerateReportInFolder");
            this.chkbGenerateReportInFolder.Name = "chkbGenerateReportInFolder";
            this.chkbGenerateReportInFolder.UseVisualStyleBackColor = true;
            this.chkbGenerateReportInFolder.CheckedChanged += new System.EventHandler(this.OnGenerateReportsInFolderChanged);
            // 
            // bnFolderImages
            // 
            resources.ApplyResources(this.bnFolderImages, "bnFolderImages");
            this.bnFolderImages.Name = "bnFolderImages";
            this.bnFolderImages.UseVisualStyleBackColor = true;
            // 
            // tbFolderImages
            // 
            resources.ApplyResources(this.tbFolderImages, "tbFolderImages");
            this.tbFolderImages.Name = "tbFolderImages";
            // 
            // chkbGenerateImageInFolder
            // 
            resources.ApplyResources(this.chkbGenerateImageInFolder, "chkbGenerateImageInFolder");
            this.chkbGenerateImageInFolder.Name = "chkbGenerateImageInFolder";
            this.chkbGenerateImageInFolder.UseVisualStyleBackColor = true;
            this.chkbGenerateImageInFolder.CheckedChanged += new System.EventHandler(this.OnGenerateImagesInFolderChanged);
            // 
            // chkbGenerateImageInRow
            // 
            resources.ApplyResources(this.chkbGenerateImageInRow, "chkbGenerateImageInRow");
            this.chkbGenerateImageInRow.Name = "chkbGenerateImageInRow";
            this.chkbGenerateImageInRow.UseVisualStyleBackColor = true;
            // 
            // nudImageSize
            // 
            resources.ApplyResources(this.nudImageSize, "nudImageSize");
            this.nudImageSize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudImageSize.Name = "nudImageSize";
            this.nudImageSize.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cbOutputStart
            // 
            this.cbOutputStart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOutputStart.FormattingEnabled = true;
            resources.ApplyResources(this.cbOutputStart, "cbOutputStart");
            this.cbOutputStart.Name = "cbOutputStart";
            // 
            // lbStartOutput
            // 
            resources.ApplyResources(this.lbStartOutput, "lbStartOutput");
            this.lbStartOutput.Name = "lbStartOutput";
            // 
            // lbSheet
            // 
            resources.ApplyResources(this.lbSheet, "lbSheet");
            this.lbSheet.Name = "lbSheet";
            // 
            // cbSheets
            // 
            this.cbSheets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSheets.FormattingEnabled = true;
            resources.ApplyResources(this.cbSheets, "cbSheets");
            this.cbSheets.Name = "cbSheets";
            // 
            // bnDownloadSampleSheet
            // 
            resources.ApplyResources(this.bnDownloadSampleSheet, "bnDownloadSampleSheet");
            this.bnDownloadSampleSheet.Name = "bnDownloadSampleSheet";
            this.bnDownloadSampleSheet.UseVisualStyleBackColor = true;
            this.bnDownloadSampleSheet.Click += new System.EventHandler(this.OnDownloadSampleSheet);
            // 
            // chkbAllowCombinations
            // 
            resources.ApplyResources(this.chkbAllowCombinations, "chkbAllowCombinations");
            this.chkbAllowCombinations.Name = "chkbAllowCombinations";
            this.chkbAllowCombinations.UseVisualStyleBackColor = true;
            // 
            // FormExcelMassAnalysis
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bnDownloadSampleSheet);
            this.Controls.Add(this.cbSheets);
            this.Controls.Add(this.lbSheet);
            this.Controls.Add(this.gbOutput);
            this.Controls.Add(this.gbInputColumns);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.lbExcelFile);
            this.Controls.Add(this.fileSelectExcel);
            this.Controls.Add(this.gbPallet);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnCompute);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormExcelMassAnalysis";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.gbPallet.ResumeLayout(false);
            this.gbPallet.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.gbInputColumns.ResumeLayout(false);
            this.gbInputColumns.PerformLayout();
            this.gbOutput.ResumeLayout(false);
            this.gbOutput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudImageSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bnCompute;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.GroupBox gbPallet;
        private System.Windows.Forms.CheckedListBox lbPallets;
        private treeDiM.Basics.UCtrlDouble uCtrlMaxPalletHeight;
        private treeDiM.Basics.UCtrlDualDouble uCtrlOverhang;
        private System.Windows.Forms.CheckBox chkbOnlyZOrientation;
        private UserControls.FileSelect fileSelectExcel;
        private System.Windows.Forms.Label lbExcelFile;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusStripLabel;
        private System.Windows.Forms.GroupBox gbInputColumns;
        private System.Windows.Forms.ComboBox cbWeight;
        private System.Windows.Forms.ComboBox cbHeight;
        private System.Windows.Forms.ComboBox cbWidth;
        private System.Windows.Forms.ComboBox cbLength;
        private System.Windows.Forms.ComboBox cbDescription;
        private System.Windows.Forms.ComboBox cbName;
        private System.Windows.Forms.CheckBox chkbDescription;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbHeight;
        private System.Windows.Forms.Label lbWidth;
        private System.Windows.Forms.Label lbLength;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.GroupBox gbOutput;
        private System.Windows.Forms.ComboBox cbOutputStart;
        private System.Windows.Forms.Label lbStartOutput;
        private System.Windows.Forms.NumericUpDown nudImageSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkbGenerateImageInRow;
        private System.Windows.Forms.CheckBox chkbGenerateImageInFolder;
        private System.Windows.Forms.Button bnFolderImages;
        private System.Windows.Forms.TextBox tbFolderImages;
        private System.Windows.Forms.CheckBox chkbGenerateReportInFolder;
        private System.Windows.Forms.TextBox tbFolderReports;
        private System.Windows.Forms.Button bnFolderReports;
        private System.Windows.Forms.Label lbSheet;
        private System.Windows.Forms.ComboBox cbSheets;
        private System.Windows.Forms.Button bnDownloadSampleSheet;
        private System.Windows.Forms.CheckBox chkbAllowCombinations;
    }
}