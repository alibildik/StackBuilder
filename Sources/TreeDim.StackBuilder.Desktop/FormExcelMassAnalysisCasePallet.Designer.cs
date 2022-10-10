
namespace treeDiM.StackBuilder.Desktop
{
    partial class FormExcelMassAnalysisCasePallet
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormExcelMassAnalysisCasePallet));
            this.chkbAllowCombinations = new System.Windows.Forms.CheckBox();
            this.chkbOnlyZOrientation = new System.Windows.Forms.CheckBox();
            this.uCtrlOverhang = new treeDiM.Basics.UCtrlDualDouble();
            this.uCtrlMaxPalletHeight = new treeDiM.Basics.UCtrlDouble();
            this.lbPallets = new System.Windows.Forms.CheckedListBox();
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
            this.tabCtrl = new System.Windows.Forms.TabControl();
            this.tpInputColumns = new System.Windows.Forms.TabPage();
            this.tpOutputs = new System.Windows.Forms.TabPage();
            this.fsFolderReports = new treeDiM.UserControls.FileSelect();
            this.fsFolderImages = new treeDiM.UserControls.FileSelect();
            this.chkbGenerateReportInFolder = new System.Windows.Forms.CheckBox();
            this.chkbGenerateImageInFolder = new System.Windows.Forms.CheckBox();
            this.chkbGenerateImageInRow = new System.Windows.Forms.CheckBox();
            this.nudImageSize = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.cbOutputStart = new System.Windows.Forms.ComboBox();
            this.lbStartOutput = new System.Windows.Forms.Label();
            this.tpPalletsAndConstraints = new System.Windows.Forms.TabPage();
            this.bnUncheckAll = new System.Windows.Forms.Button();
            this.bnCheckAll = new System.Windows.Forms.Button();
            this.chkbPalletAdmissibleLoadWeight = new System.Windows.Forms.CheckBox();
            this.lPallets = new System.Windows.Forms.Label();
            this.tabCtrl.SuspendLayout();
            this.tpInputColumns.SuspendLayout();
            this.tpOutputs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudImageSize)).BeginInit();
            this.tpPalletsAndConstraints.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkbAllowCombinations
            // 
            resources.ApplyResources(this.chkbAllowCombinations, "chkbAllowCombinations");
            this.chkbAllowCombinations.Name = "chkbAllowCombinations";
            this.chkbAllowCombinations.UseVisualStyleBackColor = true;
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
            // 
            // lbPallets
            // 
            this.lbPallets.CheckOnClick = true;
            this.lbPallets.FormattingEnabled = true;
            resources.ApplyResources(this.lbPallets, "lbPallets");
            this.lbPallets.Name = "lbPallets";
            this.lbPallets.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.OnItemChecked);
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
            // tabCtrl
            // 
            this.tabCtrl.Controls.Add(this.tpInputColumns);
            this.tabCtrl.Controls.Add(this.tpOutputs);
            this.tabCtrl.Controls.Add(this.tpPalletsAndConstraints);
            resources.ApplyResources(this.tabCtrl, "tabCtrl");
            this.tabCtrl.Name = "tabCtrl";
            this.tabCtrl.SelectedIndex = 0;
            // 
            // tpInputColumns
            // 
            this.tpInputColumns.Controls.Add(this.cbWeight);
            this.tpInputColumns.Controls.Add(this.lbLength);
            this.tpInputColumns.Controls.Add(this.cbHeight);
            this.tpInputColumns.Controls.Add(this.lbName);
            this.tpInputColumns.Controls.Add(this.cbWidth);
            this.tpInputColumns.Controls.Add(this.lbWidth);
            this.tpInputColumns.Controls.Add(this.cbLength);
            this.tpInputColumns.Controls.Add(this.lbHeight);
            this.tpInputColumns.Controls.Add(this.cbDescription);
            this.tpInputColumns.Controls.Add(this.label5);
            this.tpInputColumns.Controls.Add(this.cbName);
            this.tpInputColumns.Controls.Add(this.chkbDescription);
            resources.ApplyResources(this.tpInputColumns, "tpInputColumns");
            this.tpInputColumns.Name = "tpInputColumns";
            this.tpInputColumns.UseVisualStyleBackColor = true;
            // 
            // tpOutputs
            // 
            this.tpOutputs.Controls.Add(this.fsFolderReports);
            this.tpOutputs.Controls.Add(this.fsFolderImages);
            this.tpOutputs.Controls.Add(this.chkbGenerateReportInFolder);
            this.tpOutputs.Controls.Add(this.chkbGenerateImageInFolder);
            this.tpOutputs.Controls.Add(this.chkbGenerateImageInRow);
            this.tpOutputs.Controls.Add(this.nudImageSize);
            this.tpOutputs.Controls.Add(this.label1);
            this.tpOutputs.Controls.Add(this.cbOutputStart);
            this.tpOutputs.Controls.Add(this.lbStartOutput);
            resources.ApplyResources(this.tpOutputs, "tpOutputs");
            this.tpOutputs.Name = "tpOutputs";
            this.tpOutputs.UseVisualStyleBackColor = true;
            // 
            // fsFolderReports
            // 
            resources.ApplyResources(this.fsFolderReports, "fsFolderReports");
            this.fsFolderReports.Name = "fsFolderReports";
            // 
            // fsFolderImages
            // 
            resources.ApplyResources(this.fsFolderImages, "fsFolderImages");
            this.fsFolderImages.Name = "fsFolderImages";
            // 
            // chkbGenerateReportInFolder
            // 
            resources.ApplyResources(this.chkbGenerateReportInFolder, "chkbGenerateReportInFolder");
            this.chkbGenerateReportInFolder.Name = "chkbGenerateReportInFolder";
            this.chkbGenerateReportInFolder.UseVisualStyleBackColor = true;
            this.chkbGenerateReportInFolder.CheckedChanged += new System.EventHandler(this.OnGenerateReportsInFolderChanged);
            // 
            // chkbGenerateImageInFolder
            // 
            resources.ApplyResources(this.chkbGenerateImageInFolder, "chkbGenerateImageInFolder");
            this.chkbGenerateImageInFolder.Name = "chkbGenerateImageInFolder";
            this.chkbGenerateImageInFolder.UseVisualStyleBackColor = true;
            this.chkbGenerateImageInFolder.CheckStateChanged += new System.EventHandler(this.OnGenerateImagesInFolderChanged);
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
            // tpPalletsAndConstraints
            // 
            this.tpPalletsAndConstraints.Controls.Add(this.bnUncheckAll);
            this.tpPalletsAndConstraints.Controls.Add(this.bnCheckAll);
            this.tpPalletsAndConstraints.Controls.Add(this.chkbPalletAdmissibleLoadWeight);
            this.tpPalletsAndConstraints.Controls.Add(this.lPallets);
            this.tpPalletsAndConstraints.Controls.Add(this.chkbAllowCombinations);
            this.tpPalletsAndConstraints.Controls.Add(this.lbPallets);
            this.tpPalletsAndConstraints.Controls.Add(this.chkbOnlyZOrientation);
            this.tpPalletsAndConstraints.Controls.Add(this.uCtrlMaxPalletHeight);
            this.tpPalletsAndConstraints.Controls.Add(this.uCtrlOverhang);
            resources.ApplyResources(this.tpPalletsAndConstraints, "tpPalletsAndConstraints");
            this.tpPalletsAndConstraints.Name = "tpPalletsAndConstraints";
            this.tpPalletsAndConstraints.UseVisualStyleBackColor = true;
            // 
            // bnUncheckAll
            // 
            resources.ApplyResources(this.bnUncheckAll, "bnUncheckAll");
            this.bnUncheckAll.Name = "bnUncheckAll";
            this.bnUncheckAll.UseVisualStyleBackColor = true;
            this.bnUncheckAll.Click += new System.EventHandler(this.OnUncheckAllPallets);
            // 
            // bnCheckAll
            // 
            resources.ApplyResources(this.bnCheckAll, "bnCheckAll");
            this.bnCheckAll.Name = "bnCheckAll";
            this.bnCheckAll.UseVisualStyleBackColor = true;
            this.bnCheckAll.Click += new System.EventHandler(this.OnCheckAllPallets);
            // 
            // chkbPalletAdmissibleLoadWeight
            // 
            resources.ApplyResources(this.chkbPalletAdmissibleLoadWeight, "chkbPalletAdmissibleLoadWeight");
            this.chkbPalletAdmissibleLoadWeight.Name = "chkbPalletAdmissibleLoadWeight";
            this.chkbPalletAdmissibleLoadWeight.UseVisualStyleBackColor = true;
            // 
            // lPallets
            // 
            resources.ApplyResources(this.lPallets, "lPallets");
            this.lPallets.Name = "lPallets";
            // 
            // FormExcelMassAnalysisCasePallet
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabCtrl);
            this.Name = "FormExcelMassAnalysisCasePallet";
            this.Controls.SetChildIndex(this.tabCtrl, 0);
            this.tabCtrl.ResumeLayout(false);
            this.tpInputColumns.ResumeLayout(false);
            this.tpInputColumns.PerformLayout();
            this.tpOutputs.ResumeLayout(false);
            this.tpOutputs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudImageSize)).EndInit();
            this.tpPalletsAndConstraints.ResumeLayout(false);
            this.tpPalletsAndConstraints.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.CheckedListBox lbPallets;
        private treeDiM.Basics.UCtrlDouble uCtrlMaxPalletHeight;
        private treeDiM.Basics.UCtrlDualDouble uCtrlOverhang;
        private System.Windows.Forms.CheckBox chkbOnlyZOrientation;
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
        private System.Windows.Forms.CheckBox chkbAllowCombinations;
        private System.Windows.Forms.TabControl tabCtrl;
        private System.Windows.Forms.TabPage tpInputColumns;
        private System.Windows.Forms.TabPage tpOutputs;
        private System.Windows.Forms.CheckBox chkbGenerateReportInFolder;
        private System.Windows.Forms.CheckBox chkbGenerateImageInFolder;
        private System.Windows.Forms.CheckBox chkbGenerateImageInRow;
        private System.Windows.Forms.NumericUpDown nudImageSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbOutputStart;
        private System.Windows.Forms.Label lbStartOutput;
        private System.Windows.Forms.TabPage tpPalletsAndConstraints;
        private UserControls.FileSelect fsFolderReports;
        private UserControls.FileSelect fsFolderImages;
        private System.Windows.Forms.Label lPallets;
        private System.Windows.Forms.CheckBox chkbPalletAdmissibleLoadWeight;
        private System.Windows.Forms.Button bnUncheckAll;
        private System.Windows.Forms.Button bnCheckAll;
    }
}