namespace treeDiM.StackBuilder.Desktop
{
    partial class FormNewAnalysisCasePallet
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNewAnalysisCasePallet));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.uCtrlLayerList = new treeDiM.StackBuilder.Graphics.UCtrlLayerList();
            this.checkBoxBestLayersOnly = new System.Windows.Forms.CheckBox();
            this.bnEditLayerRight = new System.Windows.Forms.Button();
            this.bnEditLayer = new System.Windows.Forms.Button();
            this.uCtrlLayerListEdited = new treeDiM.StackBuilder.Graphics.UCtrlLayerList();
            this.cbPallets = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered();
            this.cbCases = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered();
            this.lbBox = new System.Windows.Forms.Label();
            this.uCtrlOverhang = new treeDiM.Basics.UCtrlDualDouble();
            this.lbPallet = new System.Windows.Forms.Label();
            this.bnBestCombination = new System.Windows.Forms.Button();
            this.uCtrlCaseOrientation = new treeDiM.StackBuilder.Graphics.UCtrlCaseOrientation();
            this.tabCtrlConstraints = new System.Windows.Forms.TabControl();
            this.tabPageStopCriterions = new System.Windows.Forms.TabPage();
            this.uCtrlOptMaxNumber = new treeDiM.Basics.UCtrlOptInt();
            this.uCtrlOptMaximumWeight = new treeDiM.Basics.UCtrlOptDouble();
            this.uCtrlMaximumHeight = new treeDiM.Basics.UCtrlDouble();
            this.tabPageOverhang = new System.Windows.Forms.TabPage();
            this.tabPageSpaces = new System.Windows.Forms.TabPage();
            this.uCtrlOptSpace = new treeDiM.Basics.UCtrlOptDouble();
            this.lbSelect = new System.Windows.Forms.Label();
            this.lbSuggestion = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabCtrlConstraints.SuspendLayout();
            this.tabPageStopCriterions.SuspendLayout();
            this.tabPageOverhang.SuspendLayout();
            this.tabPageSpaces.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbDescription
            // 
            resources.ApplyResources(this.tbDescription, "tbDescription");
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.uCtrlLayerList);
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxBestLayersOnly);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.bnEditLayerRight);
            this.splitContainer1.Panel2.Controls.Add(this.bnEditLayer);
            this.splitContainer1.Panel2.Controls.Add(this.uCtrlLayerListEdited);
            // 
            // uCtrlLayerList
            // 
            resources.ApplyResources(this.uCtrlLayerList, "uCtrlLayerList");
            this.uCtrlLayerList.ButtonSizes = new System.Drawing.Size(150, 150);
            this.uCtrlLayerList.Name = "uCtrlLayerList";
            this.uCtrlLayerList.Show3D = true;
            this.uCtrlLayerList.SingleSelection = false;
            // 
            // checkBoxBestLayersOnly
            // 
            resources.ApplyResources(this.checkBoxBestLayersOnly, "checkBoxBestLayersOnly");
            this.checkBoxBestLayersOnly.Name = "checkBoxBestLayersOnly";
            this.checkBoxBestLayersOnly.UseVisualStyleBackColor = true;
            this.checkBoxBestLayersOnly.CheckedChanged += new System.EventHandler(this.OnInputChanged);
            // 
            // bnEditLayerRight
            // 
            resources.ApplyResources(this.bnEditLayerRight, "bnEditLayerRight");
            this.bnEditLayerRight.Name = "bnEditLayerRight";
            this.bnEditLayerRight.UseVisualStyleBackColor = true;
            this.bnEditLayerRight.Click += new System.EventHandler(this.OnEditLayerRight);
            // 
            // bnEditLayer
            // 
            resources.ApplyResources(this.bnEditLayer, "bnEditLayer");
            this.bnEditLayer.Name = "bnEditLayer";
            this.bnEditLayer.UseVisualStyleBackColor = true;
            this.bnEditLayer.Click += new System.EventHandler(this.OnEditLayer);
            // 
            // uCtrlLayerListEdited
            // 
            resources.ApplyResources(this.uCtrlLayerListEdited, "uCtrlLayerListEdited");
            this.uCtrlLayerListEdited.ButtonSizes = new System.Drawing.Size(150, 150);
            this.uCtrlLayerListEdited.Name = "uCtrlLayerListEdited";
            this.uCtrlLayerListEdited.Show3D = true;
            this.uCtrlLayerListEdited.SingleSelection = false;
            // 
            // cbPallets
            // 
            this.cbPallets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPallets.FormattingEnabled = true;
            resources.ApplyResources(this.cbPallets, "cbPallets");
            this.cbPallets.Name = "cbPallets";
            this.cbPallets.SelectedIndexChanged += new System.EventHandler(this.OnInputChanged);
            // 
            // cbCases
            // 
            this.cbCases.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCases.FormattingEnabled = true;
            resources.ApplyResources(this.cbCases, "cbCases");
            this.cbCases.Name = "cbCases";
            this.cbCases.SelectedIndexChanged += new System.EventHandler(this.OnCaseChanged);
            // 
            // lbBox
            // 
            resources.ApplyResources(this.lbBox, "lbBox");
            this.lbBox.Name = "lbBox";
            // 
            // uCtrlOverhang
            // 
            resources.ApplyResources(this.uCtrlOverhang, "uCtrlOverhang");
            this.uCtrlOverhang.MinValue = -10000D;
            this.uCtrlOverhang.Name = "uCtrlOverhang";
            this.uCtrlOverhang.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlOverhang.ValueX = 0D;
            this.uCtrlOverhang.ValueY = 0D;
            this.uCtrlOverhang.ValueChanged += new treeDiM.Basics.UCtrlDualDouble.ValueChangedDelegate(this.OnInputChanged);
            // 
            // lbPallet
            // 
            resources.ApplyResources(this.lbPallet, "lbPallet");
            this.lbPallet.Name = "lbPallet";
            // 
            // bnBestCombination
            // 
            resources.ApplyResources(this.bnBestCombination, "bnBestCombination");
            this.bnBestCombination.Name = "bnBestCombination";
            this.bnBestCombination.UseVisualStyleBackColor = true;
            this.bnBestCombination.Click += new System.EventHandler(this.OnBestCombinationClicked);
            // 
            // uCtrlCaseOrientation
            // 
            this.uCtrlCaseOrientation.AllowedOrientations = new bool[] {
        false,
        false,
        true};
            resources.ApplyResources(this.uCtrlCaseOrientation, "uCtrlCaseOrientation");
            this.uCtrlCaseOrientation.Name = "uCtrlCaseOrientation";
            this.uCtrlCaseOrientation.CheckedChanged += new treeDiM.StackBuilder.Graphics.UCtrlCaseOrientation.CheckChanged(this.OnInputChanged);
            // 
            // tabCtrlConstraints
            // 
            this.tabCtrlConstraints.Controls.Add(this.tabPageStopCriterions);
            this.tabCtrlConstraints.Controls.Add(this.tabPageOverhang);
            this.tabCtrlConstraints.Controls.Add(this.tabPageSpaces);
            resources.ApplyResources(this.tabCtrlConstraints, "tabCtrlConstraints");
            this.tabCtrlConstraints.Name = "tabCtrlConstraints";
            this.tabCtrlConstraints.SelectedIndex = 0;
            // 
            // tabPageStopCriterions
            // 
            this.tabPageStopCriterions.Controls.Add(this.uCtrlOptMaxNumber);
            this.tabPageStopCriterions.Controls.Add(this.uCtrlOptMaximumWeight);
            this.tabPageStopCriterions.Controls.Add(this.uCtrlMaximumHeight);
            resources.ApplyResources(this.tabPageStopCriterions, "tabPageStopCriterions");
            this.tabPageStopCriterions.Name = "tabPageStopCriterions";
            this.tabPageStopCriterions.UseVisualStyleBackColor = true;
            // 
            // uCtrlOptMaxNumber
            // 
            resources.ApplyResources(this.uCtrlOptMaxNumber, "uCtrlOptMaxNumber");
            this.uCtrlOptMaxNumber.Minimum = 0;
            this.uCtrlOptMaxNumber.Name = "uCtrlOptMaxNumber";
            this.uCtrlOptMaxNumber.ValueChanged += new treeDiM.Basics.UCtrlOptInt.ValueChangedDelegate(this.OnInputChanged);
            // 
            // uCtrlOptMaximumWeight
            // 
            resources.ApplyResources(this.uCtrlOptMaximumWeight, "uCtrlOptMaximumWeight");
            this.uCtrlOptMaximumWeight.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.uCtrlOptMaximumWeight.Name = "uCtrlOptMaximumWeight";
            this.uCtrlOptMaximumWeight.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_MASS;
            this.uCtrlOptMaximumWeight.ValueChanged += new treeDiM.Basics.UCtrlOptDouble.ValueChangedDelegate(this.OnInputChanged);
            // 
            // uCtrlMaximumHeight
            // 
            resources.ApplyResources(this.uCtrlMaximumHeight, "uCtrlMaximumHeight");
            this.uCtrlMaximumHeight.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.uCtrlMaximumHeight.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.uCtrlMaximumHeight.Name = "uCtrlMaximumHeight";
            this.uCtrlMaximumHeight.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlMaximumHeight.ValueChanged += new treeDiM.Basics.UCtrlDouble.ValueChangedDelegate(this.OnInputChanged);
            // 
            // tabPageOverhang
            // 
            this.tabPageOverhang.Controls.Add(this.lbSuggestion);
            this.tabPageOverhang.Controls.Add(this.uCtrlOverhang);
            resources.ApplyResources(this.tabPageOverhang, "tabPageOverhang");
            this.tabPageOverhang.Name = "tabPageOverhang";
            this.tabPageOverhang.UseVisualStyleBackColor = true;
            // 
            // tabPageSpaces
            // 
            this.tabPageSpaces.Controls.Add(this.uCtrlOptSpace);
            resources.ApplyResources(this.tabPageSpaces, "tabPageSpaces");
            this.tabPageSpaces.Name = "tabPageSpaces";
            this.tabPageSpaces.UseVisualStyleBackColor = true;
            // 
            // uCtrlOptSpace
            // 
            resources.ApplyResources(this.uCtrlOptSpace, "uCtrlOptSpace");
            this.uCtrlOptSpace.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.uCtrlOptSpace.Name = "uCtrlOptSpace";
            this.uCtrlOptSpace.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlOptSpace.ValueChanged += new treeDiM.Basics.UCtrlOptDouble.ValueChangedDelegate(this.OnInputChanged);
            // 
            // lbSelect
            // 
            resources.ApplyResources(this.lbSelect, "lbSelect");
            this.lbSelect.Name = "lbSelect";
            // 
            // lbSuggestion
            // 
            resources.ApplyResources(this.lbSuggestion, "lbSuggestion");
            this.lbSuggestion.Name = "lbSuggestion";
            // 
            // FormNewAnalysisCasePallet
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.lbSelect);
            this.Controls.Add(this.tabCtrlConstraints);
            this.Controls.Add(this.bnBestCombination);
            this.Controls.Add(this.uCtrlCaseOrientation);
            this.Controls.Add(this.cbPallets);
            this.Controls.Add(this.cbCases);
            this.Controls.Add(this.lbPallet);
            this.Controls.Add(this.lbBox);
            this.Name = "FormNewAnalysisCasePallet";
            this.TopMost = true;
            this.Controls.SetChildIndex(this.lbName, 0);
            this.Controls.SetChildIndex(this.lbDescription, 0);
            this.Controls.SetChildIndex(this.tbName, 0);
            this.Controls.SetChildIndex(this.tbDescription, 0);
            this.Controls.SetChildIndex(this.lbBox, 0);
            this.Controls.SetChildIndex(this.lbPallet, 0);
            this.Controls.SetChildIndex(this.cbCases, 0);
            this.Controls.SetChildIndex(this.cbPallets, 0);
            this.Controls.SetChildIndex(this.uCtrlCaseOrientation, 0);
            this.Controls.SetChildIndex(this.bnBestCombination, 0);
            this.Controls.SetChildIndex(this.tabCtrlConstraints, 0);
            this.Controls.SetChildIndex(this.lbSelect, 0);
            this.Controls.SetChildIndex(this.splitContainer1, 0);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabCtrlConstraints.ResumeLayout(false);
            this.tabPageStopCriterions.ResumeLayout(false);
            this.tabPageOverhang.ResumeLayout(false);
            this.tabPageOverhang.PerformLayout();
            this.tabPageSpaces.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered cbPallets;
        private treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered cbCases;
        private Graphics.UCtrlCaseOrientation uCtrlCaseOrientation;
        private Graphics.UCtrlLayerList uCtrlLayerList;
        private treeDiM.Basics.UCtrlOptDouble uCtrlOptSpace;
        private treeDiM.Basics.UCtrlDualDouble uCtrlOverhang;
        private treeDiM.Basics.UCtrlOptDouble uCtrlOptMaximumWeight;
        private treeDiM.Basics.UCtrlDouble uCtrlMaximumHeight;
        private treeDiM.Basics.UCtrlOptInt uCtrlOptMaxNumber;
        private System.Windows.Forms.Label lbBox;
        private System.Windows.Forms.Label lbPallet;
        private System.Windows.Forms.CheckBox checkBoxBestLayersOnly;
        private System.Windows.Forms.Button bnBestCombination;
        private System.Windows.Forms.TabControl tabCtrlConstraints;
        private System.Windows.Forms.TabPage tabPageOverhang;
        private System.Windows.Forms.TabPage tabPageStopCriterions;
        private System.Windows.Forms.TabPage tabPageSpaces;
        private System.Windows.Forms.Label lbSelect;
        private Graphics.UCtrlLayerList uCtrlLayerListEdited;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button bnEditLayer;
        private System.Windows.Forms.Button bnEditLayerRight;
        private System.Windows.Forms.Label lbSuggestion;
    }
}