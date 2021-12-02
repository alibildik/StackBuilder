namespace treeDiM.StackBuilder.Desktop
{
    partial class FormOptimiseMultiCase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOptimiseMultiCase));
            this.splitContainerHoriz = new System.Windows.Forms.SplitContainer();
            this.uCtrlNumberPerCase = new treeDiM.Basics.UCtrlOptInt();
            this.uCtrlCaseDimensionsMax = new treeDiM.Basics.UCtrlOptTriDouble();
            this.uCtrlCaseDimensionsMin = new treeDiM.Basics.UCtrlOptTriDouble();
            this.gridSolutions = new SourceGrid.Grid();
            this.cbBoxes = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered();
            this.uCtrlCaseOrient = new treeDiM.StackBuilder.Graphics.UCtrlCaseOrientation();
            this.chklbCases = new System.Windows.Forms.CheckedListBox();
            this.bnClose = new System.Windows.Forms.Button();
            this.lbCases = new System.Windows.Forms.Label();
            this.lbBox = new System.Windows.Forms.Label();
            this.bnCreateAnalysis = new System.Windows.Forms.Button();
            this.tbAnalysisDescription = new System.Windows.Forms.TextBox();
            this.tbAnalysisName = new System.Windows.Forms.TextBox();
            this.lbAnalysisDescription = new System.Windows.Forms.Label();
            this.lbAnalysisName = new System.Windows.Forms.Label();
            this.graphCtrl = new treeDiM.StackBuilder.Graphics.Graphics3DControl();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelDef = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.chkbAllowCombinations = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz)).BeginInit();
            this.splitContainerHoriz.Panel1.SuspendLayout();
            this.splitContainerHoriz.Panel2.SuspendLayout();
            this.splitContainerHoriz.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrl)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerHoriz
            // 
            resources.ApplyResources(this.splitContainerHoriz, "splitContainerHoriz");
            this.splitContainerHoriz.Name = "splitContainerHoriz";
            // 
            // splitContainerHoriz.Panel1
            // 
            this.splitContainerHoriz.Panel1.Controls.Add(this.chkbAllowCombinations);
            this.splitContainerHoriz.Panel1.Controls.Add(this.uCtrlNumberPerCase);
            this.splitContainerHoriz.Panel1.Controls.Add(this.uCtrlCaseDimensionsMax);
            this.splitContainerHoriz.Panel1.Controls.Add(this.uCtrlCaseDimensionsMin);
            this.splitContainerHoriz.Panel1.Controls.Add(this.gridSolutions);
            this.splitContainerHoriz.Panel1.Controls.Add(this.cbBoxes);
            this.splitContainerHoriz.Panel1.Controls.Add(this.uCtrlCaseOrient);
            this.splitContainerHoriz.Panel1.Controls.Add(this.chklbCases);
            this.splitContainerHoriz.Panel1.Controls.Add(this.bnClose);
            this.splitContainerHoriz.Panel1.Controls.Add(this.lbCases);
            this.splitContainerHoriz.Panel1.Controls.Add(this.lbBox);
            // 
            // splitContainerHoriz.Panel2
            // 
            this.splitContainerHoriz.Panel2.Controls.Add(this.bnCreateAnalysis);
            this.splitContainerHoriz.Panel2.Controls.Add(this.tbAnalysisDescription);
            this.splitContainerHoriz.Panel2.Controls.Add(this.tbAnalysisName);
            this.splitContainerHoriz.Panel2.Controls.Add(this.lbAnalysisDescription);
            this.splitContainerHoriz.Panel2.Controls.Add(this.lbAnalysisName);
            this.splitContainerHoriz.Panel2.Controls.Add(this.graphCtrl);
            this.splitContainerHoriz.Panel2.Controls.Add(this.statusStrip);
            // 
            // uCtrlNumberPerCase
            // 
            resources.ApplyResources(this.uCtrlNumberPerCase, "uCtrlNumberPerCase");
            this.uCtrlNumberPerCase.Minimum = -10000;
            this.uCtrlNumberPerCase.Name = "uCtrlNumberPerCase";
            this.uCtrlNumberPerCase.ValueChanged += new treeDiM.Basics.UCtrlOptInt.ValueChangedDelegate(this.OnConstraintsChanged);
            // 
            // uCtrlCaseDimensionsMax
            // 
            this.uCtrlCaseDimensionsMax.Checked = false;
            resources.ApplyResources(this.uCtrlCaseDimensionsMax, "uCtrlCaseDimensionsMax");
            this.uCtrlCaseDimensionsMax.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.uCtrlCaseDimensionsMax.Name = "uCtrlCaseDimensionsMax";
            this.uCtrlCaseDimensionsMax.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlCaseDimensionsMax.X = 0D;
            this.uCtrlCaseDimensionsMax.Y = 0D;
            this.uCtrlCaseDimensionsMax.Z = 0D;
            this.uCtrlCaseDimensionsMax.ValueChanged += new treeDiM.Basics.UCtrlOptTriDouble.ValueChangedDelegate(this.OnFillListCases);
            // 
            // uCtrlCaseDimensionsMin
            // 
            this.uCtrlCaseDimensionsMin.Checked = false;
            resources.ApplyResources(this.uCtrlCaseDimensionsMin, "uCtrlCaseDimensionsMin");
            this.uCtrlCaseDimensionsMin.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.uCtrlCaseDimensionsMin.Name = "uCtrlCaseDimensionsMin";
            this.uCtrlCaseDimensionsMin.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlCaseDimensionsMin.X = 0D;
            this.uCtrlCaseDimensionsMin.Y = 0D;
            this.uCtrlCaseDimensionsMin.Z = 0D;
            this.uCtrlCaseDimensionsMin.ValueChanged += new treeDiM.Basics.UCtrlOptTriDouble.ValueChangedDelegate(this.OnFillListCases);
            // 
            // gridSolutions
            // 
            resources.ApplyResources(this.gridSolutions, "gridSolutions");
            this.gridSolutions.EnableSort = true;
            this.gridSolutions.Name = "gridSolutions";
            this.gridSolutions.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridSolutions.SelectionMode = SourceGrid.GridSelectionMode.Row;
            this.gridSolutions.TabStop = true;
            this.gridSolutions.ToolTipText = "";
            // 
            // cbBoxes
            // 
            this.cbBoxes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBoxes.FormattingEnabled = true;
            resources.ApplyResources(this.cbBoxes, "cbBoxes");
            this.cbBoxes.Name = "cbBoxes";
            this.cbBoxes.SelectedIndexChanged += new System.EventHandler(this.OnBoxChanged);
            // 
            // uCtrlCaseOrient
            // 
            this.uCtrlCaseOrient.AllowedOrientations = new bool[] {
        false,
        false,
        true};
            resources.ApplyResources(this.uCtrlCaseOrient, "uCtrlCaseOrient");
            this.uCtrlCaseOrient.Name = "uCtrlCaseOrient";
            this.uCtrlCaseOrient.CheckedChanged += new treeDiM.StackBuilder.Graphics.UCtrlCaseOrientation.CheckChanged(this.OnConstraintsChanged);
            // 
            // chklbCases
            // 
            this.chklbCases.CheckOnClick = true;
            this.chklbCases.FormattingEnabled = true;
            resources.ApplyResources(this.chklbCases, "chklbCases");
            this.chklbCases.Name = "chklbCases";
            this.chklbCases.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.OnCaseChecked);
            // 
            // bnClose
            // 
            resources.ApplyResources(this.bnClose, "bnClose");
            this.bnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnClose.Name = "bnClose";
            this.bnClose.UseVisualStyleBackColor = true;
            // 
            // lbCases
            // 
            resources.ApplyResources(this.lbCases, "lbCases");
            this.lbCases.Name = "lbCases";
            // 
            // lbBox
            // 
            resources.ApplyResources(this.lbBox, "lbBox");
            this.lbBox.Name = "lbBox";
            // 
            // bnCreateAnalysis
            // 
            resources.ApplyResources(this.bnCreateAnalysis, "bnCreateAnalysis");
            this.bnCreateAnalysis.Name = "bnCreateAnalysis";
            this.bnCreateAnalysis.UseVisualStyleBackColor = true;
            this.bnCreateAnalysis.Click += new System.EventHandler(this.OnCreateAnalysis);
            // 
            // tbAnalysisDescription
            // 
            resources.ApplyResources(this.tbAnalysisDescription, "tbAnalysisDescription");
            this.tbAnalysisDescription.Name = "tbAnalysisDescription";
            // 
            // tbAnalysisName
            // 
            resources.ApplyResources(this.tbAnalysisName, "tbAnalysisName");
            this.tbAnalysisName.Name = "tbAnalysisName";
            // 
            // lbAnalysisDescription
            // 
            resources.ApplyResources(this.lbAnalysisDescription, "lbAnalysisDescription");
            this.lbAnalysisDescription.Name = "lbAnalysisDescription";
            // 
            // lbAnalysisName
            // 
            resources.ApplyResources(this.lbAnalysisName, "lbAnalysisName");
            this.lbAnalysisName.Name = "lbAnalysisName";
            // 
            // graphCtrl
            // 
            resources.ApplyResources(this.graphCtrl, "graphCtrl");
            this.graphCtrl.AngleHoriz = 45D;
            this.graphCtrl.Name = "graphCtrl";
            this.graphCtrl.Viewer = null;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelDef});
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Name = "statusStrip";
            // 
            // toolStripStatusLabelDef
            // 
            this.toolStripStatusLabelDef.Name = "toolStripStatusLabelDef";
            resources.ApplyResources(this.toolStripStatusLabelDef, "toolStripStatusLabelDef");
            // 
            // timer
            // 
            this.timer.Interval = 3000;
            this.timer.Tick += new System.EventHandler(this.OnTimerTick);
            // 
            // chkbAllowCombinations
            // 
            resources.ApplyResources(this.chkbAllowCombinations, "chkbAllowCombinations");
            this.chkbAllowCombinations.Name = "chkbAllowCombinations";
            this.chkbAllowCombinations.UseVisualStyleBackColor = true;
            this.chkbAllowCombinations.CheckedChanged += new System.EventHandler(this.OnConstraintsChanged);
            // 
            // FormOptimiseMultiCase
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnClose;
            this.Controls.Add(this.splitContainerHoriz);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormOptimiseMultiCase";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.splitContainerHoriz.Panel1.ResumeLayout(false);
            this.splitContainerHoriz.Panel1.PerformLayout();
            this.splitContainerHoriz.Panel2.ResumeLayout(false);
            this.splitContainerHoriz.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz)).EndInit();
            this.splitContainerHoriz.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrl)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bnClose;
        private System.Windows.Forms.Label lbBox;
        private System.Windows.Forms.Label lbCases;
        private System.Windows.Forms.CheckedListBox chklbCases;
        private System.Windows.Forms.SplitContainer splitContainerHoriz;
        private Graphics.UCtrlCaseOrientation uCtrlCaseOrient;
        private Graphics.Controls.CCtrlComboFiltered cbBoxes;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelDef;
        private SourceGrid.Grid gridSolutions;
        private Graphics.Graphics3DControl graphCtrl;
        private System.Windows.Forms.Button bnCreateAnalysis;
        private System.Windows.Forms.TextBox tbAnalysisDescription;
        private System.Windows.Forms.TextBox tbAnalysisName;
        private System.Windows.Forms.Label lbAnalysisDescription;
        private System.Windows.Forms.Label lbAnalysisName;
        private treeDiM.Basics.UCtrlOptTriDouble uCtrlCaseDimensionsMax;
        private treeDiM.Basics.UCtrlOptTriDouble uCtrlCaseDimensionsMin;
        private treeDiM.Basics.UCtrlOptInt uCtrlNumberPerCase;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.CheckBox chkbAllowCombinations;
    }
}