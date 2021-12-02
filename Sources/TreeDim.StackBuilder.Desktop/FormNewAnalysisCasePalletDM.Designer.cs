namespace treeDiM.StackBuilder.Desktop
{
    partial class FormNewAnalysisCasePalletDM
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNewAnalysisCasePalletDM));
            this.splitContainerHoriz = new System.Windows.Forms.SplitContainer();
            this.tabCtrlConstraints = new System.Windows.Forms.TabControl();
            this.tabPageStopCriterions = new System.Windows.Forms.TabPage();
            this.uCtrlOptMaximumWeight = new treeDiM.Basics.UCtrlOptDouble();
            this.uCtrlMaximumHeight = new treeDiM.Basics.UCtrlDouble();
            this.tabPageOverhang = new System.Windows.Forms.TabPage();
            this.uCtrlOverhang = new treeDiM.Basics.UCtrlDualDouble();
            this.tabPageSpaces = new System.Windows.Forms.TabPage();
            this.uCtrlOptSpace = new treeDiM.Basics.UCtrlOptDouble();
            this.tabPageLayerFilters = new System.Windows.Forms.TabPage();
            this.uCtrlOptMaximumLayerWeight = new treeDiM.Basics.UCtrlOptDouble();
            this.uCtrlOptMaximumSpace = new treeDiM.Basics.UCtrlOptDouble();
            this.uCtrlCaseOrientation = new treeDiM.StackBuilder.Graphics.UCtrlCaseOrientation();
            this.cbPallets = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered();
            this.cbCases = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered();
            this.lbPallet = new System.Windows.Forms.Label();
            this.lbBox = new System.Windows.Forms.Label();
            this.splitContainerVert = new System.Windows.Forms.SplitContainer();
            this.gridSolutions = new SourceGrid.Grid();
            this.graphCtrl = new treeDiM.StackBuilder.Graphics.Graphics3DControl();
            this.checkBoxBestLayersOnly = new System.Windows.Forms.CheckBox();
            this._timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz)).BeginInit();
            this.splitContainerHoriz.Panel1.SuspendLayout();
            this.splitContainerHoriz.Panel2.SuspendLayout();
            this.splitContainerHoriz.SuspendLayout();
            this.tabCtrlConstraints.SuspendLayout();
            this.tabPageStopCriterions.SuspendLayout();
            this.tabPageOverhang.SuspendLayout();
            this.tabPageSpaces.SuspendLayout();
            this.tabPageLayerFilters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).BeginInit();
            this.splitContainerVert.Panel1.SuspendLayout();
            this.splitContainerVert.Panel2.SuspendLayout();
            this.splitContainerVert.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrl)).BeginInit();
            this.SuspendLayout();
            // 
            // tbDescription
            // 
            this.tbDescription.Size = new System.Drawing.Size(673, 20);
            // 
            // splitContainerHoriz
            // 
            this.splitContainerHoriz.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerHoriz.IsSplitterFixed = true;
            this.splitContainerHoriz.Location = new System.Drawing.Point(0, 60);
            this.splitContainerHoriz.Name = "splitContainerHoriz";
            this.splitContainerHoriz.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerHoriz.Panel1
            // 
            this.splitContainerHoriz.Panel1.Controls.Add(this.tabCtrlConstraints);
            this.splitContainerHoriz.Panel1.Controls.Add(this.uCtrlCaseOrientation);
            this.splitContainerHoriz.Panel1.Controls.Add(this.cbPallets);
            this.splitContainerHoriz.Panel1.Controls.Add(this.cbCases);
            this.splitContainerHoriz.Panel1.Controls.Add(this.lbPallet);
            this.splitContainerHoriz.Panel1.Controls.Add(this.lbBox);
            // 
            // splitContainerHoriz.Panel2
            // 
            this.splitContainerHoriz.Panel2.Controls.Add(this.splitContainerVert);
            this.splitContainerHoriz.Size = new System.Drawing.Size(784, 445);
            this.splitContainerHoriz.SplitterDistance = 148;
            this.splitContainerHoriz.TabIndex = 13;
            // 
            // tabCtrlConstraints
            // 
            this.tabCtrlConstraints.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabCtrlConstraints.Controls.Add(this.tabPageStopCriterions);
            this.tabCtrlConstraints.Controls.Add(this.tabPageOverhang);
            this.tabCtrlConstraints.Controls.Add(this.tabPageSpaces);
            this.tabCtrlConstraints.Controls.Add(this.tabPageLayerFilters);
            this.tabCtrlConstraints.Location = new System.Drawing.Point(310, 32);
            this.tabCtrlConstraints.Name = "tabCtrlConstraints";
            this.tabCtrlConstraints.SelectedIndex = 0;
            this.tabCtrlConstraints.Size = new System.Drawing.Size(471, 110);
            this.tabCtrlConstraints.TabIndex = 29;
            // 
            // tabPageStopCriterions
            // 
            this.tabPageStopCriterions.Controls.Add(this.uCtrlOptMaximumWeight);
            this.tabPageStopCriterions.Controls.Add(this.uCtrlMaximumHeight);
            this.tabPageStopCriterions.Location = new System.Drawing.Point(4, 22);
            this.tabPageStopCriterions.Name = "tabPageStopCriterions";
            this.tabPageStopCriterions.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageStopCriterions.Size = new System.Drawing.Size(463, 84);
            this.tabPageStopCriterions.TabIndex = 1;
            this.tabPageStopCriterions.Text = "Stop criterions";
            this.tabPageStopCriterions.UseVisualStyleBackColor = true;
            // 
            // uCtrlOptMaximumWeight
            // 
            this.uCtrlOptMaximumWeight.Location = new System.Drawing.Point(13, 35);
            this.uCtrlOptMaximumWeight.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.uCtrlOptMaximumWeight.MinimumSize = new System.Drawing.Size(100, 20);
            this.uCtrlOptMaximumWeight.Name = "uCtrlOptMaximumWeight";
            this.uCtrlOptMaximumWeight.Size = new System.Drawing.Size(349, 20);
            this.uCtrlOptMaximumWeight.TabIndex = 24;
            this.uCtrlOptMaximumWeight.Text = "Maximum pallet weight";
            this.uCtrlOptMaximumWeight.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_MASS;
            this.uCtrlOptMaximumWeight.ValueChanged += new treeDiM.Basics.UCtrlOptDouble.ValueChangedDelegate(this.OnInputChanged);
            // 
            // uCtrlMaximumHeight
            // 
            this.uCtrlMaximumHeight.Location = new System.Drawing.Point(13, 9);
            this.uCtrlMaximumHeight.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.uCtrlMaximumHeight.MinimumSize = new System.Drawing.Size(100, 20);
            this.uCtrlMaximumHeight.Name = "uCtrlMaximumHeight";
            this.uCtrlMaximumHeight.Size = new System.Drawing.Size(349, 20);
            this.uCtrlMaximumHeight.TabIndex = 23;
            this.uCtrlMaximumHeight.Text = "Maximum pallet height";
            this.uCtrlMaximumHeight.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlMaximumHeight.Value = 0D;
            this.uCtrlMaximumHeight.ValueChanged += new treeDiM.Basics.UCtrlDouble.ValueChangedDelegate(this.OnInputChanged);
            // 
            // tabPageOverhang
            // 
            this.tabPageOverhang.Controls.Add(this.uCtrlOverhang);
            this.tabPageOverhang.Location = new System.Drawing.Point(4, 22);
            this.tabPageOverhang.Name = "tabPageOverhang";
            this.tabPageOverhang.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageOverhang.Size = new System.Drawing.Size(563, 84);
            this.tabPageOverhang.TabIndex = 0;
            this.tabPageOverhang.Text = "Overhang";
            this.tabPageOverhang.UseVisualStyleBackColor = true;
            // 
            // uCtrlOverhang
            // 
            this.uCtrlOverhang.Location = new System.Drawing.Point(6, 6);
            this.uCtrlOverhang.MinValue = -10000D;
            this.uCtrlOverhang.Name = "uCtrlOverhang";
            this.uCtrlOverhang.Size = new System.Drawing.Size(283, 22);
            this.uCtrlOverhang.TabIndex = 21;
            this.uCtrlOverhang.Text = "Overhang";
            this.uCtrlOverhang.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlOverhang.ValueX = 0D;
            this.uCtrlOverhang.ValueY = 0D;
            this.uCtrlOverhang.ValueChanged += new treeDiM.Basics.UCtrlDualDouble.ValueChangedDelegate(this.OnInputChanged);
            // 
            // tabPageSpaces
            // 
            this.tabPageSpaces.Controls.Add(this.uCtrlOptSpace);
            this.tabPageSpaces.Location = new System.Drawing.Point(4, 22);
            this.tabPageSpaces.Name = "tabPageSpaces";
            this.tabPageSpaces.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSpaces.Size = new System.Drawing.Size(563, 84);
            this.tabPageSpaces.TabIndex = 2;
            this.tabPageSpaces.Text = "Spaces";
            this.tabPageSpaces.UseVisualStyleBackColor = true;
            // 
            // uCtrlOptSpace
            // 
            this.uCtrlOptSpace.Location = new System.Drawing.Point(7, 7);
            this.uCtrlOptSpace.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.uCtrlOptSpace.MinimumSize = new System.Drawing.Size(200, 20);
            this.uCtrlOptSpace.Name = "uCtrlOptSpace";
            this.uCtrlOptSpace.Size = new System.Drawing.Size(220, 20);
            this.uCtrlOptSpace.TabIndex = 0;
            this.uCtrlOptSpace.Text = "Space";
            this.uCtrlOptSpace.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlOptSpace.Load += new System.EventHandler(this.OnInputChanged);
            // 
            // tabPageLayerFilters
            // 
            this.tabPageLayerFilters.Controls.Add(this.uCtrlOptMaximumLayerWeight);
            this.tabPageLayerFilters.Controls.Add(this.uCtrlOptMaximumSpace);
            this.tabPageLayerFilters.Location = new System.Drawing.Point(4, 22);
            this.tabPageLayerFilters.Name = "tabPageLayerFilters";
            this.tabPageLayerFilters.Size = new System.Drawing.Size(563, 84);
            this.tabPageLayerFilters.TabIndex = 3;
            this.tabPageLayerFilters.Text = "Layer filters";
            this.tabPageLayerFilters.UseVisualStyleBackColor = true;
            // 
            // uCtrlOptMaximumLayerWeight
            // 
            this.uCtrlOptMaximumLayerWeight.Location = new System.Drawing.Point(7, 31);
            this.uCtrlOptMaximumLayerWeight.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.uCtrlOptMaximumLayerWeight.MinimumSize = new System.Drawing.Size(100, 20);
            this.uCtrlOptMaximumLayerWeight.Name = "uCtrlOptMaximumLayerWeight";
            this.uCtrlOptMaximumLayerWeight.Size = new System.Drawing.Size(268, 20);
            this.uCtrlOptMaximumLayerWeight.TabIndex = 1;
            this.uCtrlOptMaximumLayerWeight.Text = "Maximum layer weight";
            this.uCtrlOptMaximumLayerWeight.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_MASS;
            this.uCtrlOptMaximumLayerWeight.ValueChanged += new treeDiM.Basics.UCtrlOptDouble.ValueChangedDelegate(this.OnInputChanged);
            // 
            // uCtrlOptMaximumSpace
            // 
            this.uCtrlOptMaximumSpace.Location = new System.Drawing.Point(7, 7);
            this.uCtrlOptMaximumSpace.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.uCtrlOptMaximumSpace.MinimumSize = new System.Drawing.Size(200, 20);
            this.uCtrlOptMaximumSpace.Name = "uCtrlOptMaximumSpace";
            this.uCtrlOptMaximumSpace.Size = new System.Drawing.Size(268, 20);
            this.uCtrlOptMaximumSpace.TabIndex = 0;
            this.uCtrlOptMaximumSpace.Text = "Maximum space";
            this.uCtrlOptMaximumSpace.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlOptMaximumSpace.ValueChanged += new treeDiM.Basics.UCtrlOptDouble.ValueChangedDelegate(this.OnInputChanged);
            // 
            // uCtrlCaseOrientation
            // 
            this.uCtrlCaseOrientation.AllowedOrientations = new bool[] {
        false,
        false,
        true};
            this.uCtrlCaseOrientation.Location = new System.Drawing.Point(12, 33);
            this.uCtrlCaseOrientation.Name = "uCtrlCaseOrientation";
            this.uCtrlCaseOrientation.Size = new System.Drawing.Size(292, 110);
            this.uCtrlCaseOrientation.TabIndex = 25;
            this.uCtrlCaseOrientation.CheckedChanged += new treeDiM.StackBuilder.Graphics.UCtrlCaseOrientation.CheckChanged(this.OnInputChanged);
            // 
            // cbPallets
            // 
            this.cbPallets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPallets.FormattingEnabled = true;
            this.cbPallets.Location = new System.Drawing.Point(428, 5);
            this.cbPallets.Name = "cbPallets";
            this.cbPallets.Size = new System.Drawing.Size(161, 21);
            this.cbPallets.TabIndex = 24;
            this.cbPallets.SelectedIndexChanged += new System.EventHandler(this.OnInputChanged);
            // 
            // cbCases
            // 
            this.cbCases.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCases.FormattingEnabled = true;
            this.cbCases.Location = new System.Drawing.Point(104, 5);
            this.cbCases.Name = "cbCases";
            this.cbCases.Size = new System.Drawing.Size(145, 21);
            this.cbCases.TabIndex = 23;
            this.cbCases.SelectedIndexChanged += new System.EventHandler(this.OnCaseChanged);
            // 
            // lbPallet
            // 
            this.lbPallet.AutoSize = true;
            this.lbPallet.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbPallet.Location = new System.Drawing.Point(306, 8);
            this.lbPallet.Name = "lbPallet";
            this.lbPallet.Size = new System.Drawing.Size(40, 13);
            this.lbPallet.TabIndex = 22;
            this.lbPallet.Text = "Palette";
            // 
            // lbBox
            // 
            this.lbBox.AutoSize = true;
            this.lbBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbBox.Location = new System.Drawing.Point(4, 8);
            this.lbBox.Name = "lbBox";
            this.lbBox.Size = new System.Drawing.Size(38, 13);
            this.lbBox.TabIndex = 21;
            this.lbBox.Text = "Caisse";
            // 
            // splitContainerVert
            // 
            this.splitContainerVert.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerVert.Location = new System.Drawing.Point(0, 0);
            this.splitContainerVert.Name = "splitContainerVert";
            // 
            // splitContainerVert.Panel1
            // 
            this.splitContainerVert.Panel1.Controls.Add(this.gridSolutions);
            // 
            // splitContainerVert.Panel2
            // 
            this.splitContainerVert.Panel2.Controls.Add(this.graphCtrl);
            this.splitContainerVert.Size = new System.Drawing.Size(784, 293);
            this.splitContainerVert.SplitterDistance = 404;
            this.splitContainerVert.TabIndex = 0;
            // 
            // gridSolutions
            // 
            this.gridSolutions.AcceptsInputChar = false;
            this.gridSolutions.DefaultHeight = 50;
            this.gridSolutions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridSolutions.EnableSort = false;
            this.gridSolutions.Location = new System.Drawing.Point(0, 0);
            this.gridSolutions.Name = "gridSolutions";
            this.gridSolutions.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridSolutions.SelectionMode = SourceGrid.GridSelectionMode.Row;
            this.gridSolutions.Size = new System.Drawing.Size(404, 293);
            this.gridSolutions.TabIndex = 0;
            this.gridSolutions.TabStop = true;
            this.gridSolutions.ToolTipText = "";
            // 
            // graphCtrl
            // 
            this.graphCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphCtrl.Location = new System.Drawing.Point(0, 0);
            this.graphCtrl.Name = "graphCtrl";
            this.graphCtrl.Size = new System.Drawing.Size(376, 293);
            this.graphCtrl.TabIndex = 0;
            this.graphCtrl.Viewer = null;
            // 
            // checkBoxBestLayersOnly
            // 
            this.checkBoxBestLayersOnly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxBestLayersOnly.AutoSize = true;
            this.checkBoxBestLayersOnly.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.checkBoxBestLayersOnly.Location = new System.Drawing.Point(8, 516);
            this.checkBoxBestLayersOnly.Name = "checkBoxBestLayersOnly";
            this.checkBoxBestLayersOnly.Size = new System.Drawing.Size(128, 17);
            this.checkBoxBestLayersOnly.TabIndex = 26;
            this.checkBoxBestLayersOnly.Text = "Show best layers only";
            this.checkBoxBestLayersOnly.UseVisualStyleBackColor = true;
            this.checkBoxBestLayersOnly.CheckedChanged += new System.EventHandler(this.OnInputChanged);
            // 
            // _timer
            // 
            this._timer.Interval = 2;
            this._timer.Tick += new System.EventHandler(this.OnTimerTick);
            // 
            // FormNewAnalysisCasePalletDM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.checkBoxBestLayersOnly);
            this.Controls.Add(this.splitContainerHoriz);
            this.Name = "FormNewAnalysisCasePalletDM";
            this.Text = "Create new case/pallet  analysis... (Dummy mode)";
            this.Controls.SetChildIndex(this.lbName, 0);
            this.Controls.SetChildIndex(this.lbDescription, 0);
            this.Controls.SetChildIndex(this.tbName, 0);
            this.Controls.SetChildIndex(this.tbDescription, 0);
            this.Controls.SetChildIndex(this.splitContainerHoriz, 0);
            this.Controls.SetChildIndex(this.checkBoxBestLayersOnly, 0);
            this.splitContainerHoriz.Panel1.ResumeLayout(false);
            this.splitContainerHoriz.Panel1.PerformLayout();
            this.splitContainerHoriz.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz)).EndInit();
            this.splitContainerHoriz.ResumeLayout(false);
            this.tabCtrlConstraints.ResumeLayout(false);
            this.tabPageStopCriterions.ResumeLayout(false);
            this.tabPageOverhang.ResumeLayout(false);
            this.tabPageSpaces.ResumeLayout(false);
            this.tabPageLayerFilters.ResumeLayout(false);
            this.splitContainerVert.Panel1.ResumeLayout(false);
            this.splitContainerVert.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).EndInit();
            this.splitContainerVert.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerHoriz;
        private System.Windows.Forms.SplitContainer splitContainerVert;
        private Graphics.Graphics3DControl graphCtrl;
        private SourceGrid.Grid gridSolutions;
        private System.Windows.Forms.CheckBox checkBoxBestLayersOnly;
        private Graphics.Controls.CCtrlComboFiltered cbPallets;
        private Graphics.Controls.CCtrlComboFiltered cbCases;
        private System.Windows.Forms.Label lbPallet;
        private System.Windows.Forms.Label lbBox;
        private Graphics.UCtrlCaseOrientation uCtrlCaseOrientation;
        private System.Windows.Forms.TabControl tabCtrlConstraints;
        private System.Windows.Forms.TabPage tabPageStopCriterions;
        private treeDiM.Basics.UCtrlOptDouble uCtrlOptMaximumWeight;
        private treeDiM.Basics.UCtrlDouble uCtrlMaximumHeight;
        private System.Windows.Forms.TabPage tabPageOverhang;
        private treeDiM.Basics.UCtrlDualDouble uCtrlOverhang;
        private System.Windows.Forms.TabPage tabPageSpaces;
        private treeDiM.Basics.UCtrlOptDouble uCtrlOptSpace;
        private System.Windows.Forms.TabPage tabPageLayerFilters;
        private treeDiM.Basics.UCtrlOptDouble uCtrlOptMaximumLayerWeight;
        private treeDiM.Basics.UCtrlOptDouble uCtrlOptMaximumSpace;
        private System.Windows.Forms.Timer _timer;
    }
}