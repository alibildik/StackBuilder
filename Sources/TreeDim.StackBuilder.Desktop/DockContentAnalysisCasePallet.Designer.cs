namespace treeDiM.StackBuilder.Desktop
{
    partial class DockContentAnalysisCasePallet
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockContentAnalysisCasePallet));
            this.gbStopCriterions = new System.Windows.Forms.GroupBox();
            this.uCtrlOptMaxNumber = new treeDiM.Basics.UCtrlOptInt();
            this.uCtrlOptMaximumWeight = new treeDiM.Basics.UCtrlOptDouble();
            this.uCtrlMaxPalletHeight = new treeDiM.Basics.UCtrlDouble();
            this.tabCtrl = new System.Windows.Forms.TabControl();
            this.tabPageStrappers = new System.Windows.Forms.TabPage();
            this.ctrlStrapperSet = new treeDiM.StackBuilder.Basics.Controls.CtrlStrapperSet();
            this.tabPagePalletCorners = new System.Windows.Forms.TabPage();
            this.cbPalletCorners = new System.Windows.Forms.ComboBox();
            this.chkbPalletCorners = new System.Windows.Forms.CheckBox();
            this.tabPagePalletCornersTop = new System.Windows.Forms.TabPage();
            this.chkbPalletCornersTopY = new System.Windows.Forms.CheckBox();
            this.cbPalletCornersTop = new System.Windows.Forms.ComboBox();
            this.chkbPalletCornersTopX = new System.Windows.Forms.CheckBox();
            this.tabPagePalletCap = new System.Windows.Forms.TabPage();
            this.cbTopInterlayer = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered();
            this.chkbTopInterlayer = new System.Windows.Forms.CheckBox();
            this.cbPalletCap = new System.Windows.Forms.ComboBox();
            this.chkbPalletCap = new System.Windows.Forms.CheckBox();
            this.tabPagePalletFilm = new System.Windows.Forms.TabPage();
            this.uCtrlPalletFilmCovering = new treeDiM.Basics.UCtrlDouble();
            this.cbPalletFilm = new System.Windows.Forms.ComboBox();
            this.chkbPalletFilm = new System.Windows.Forms.CheckBox();
            this.tabPagePalletLabels = new System.Windows.Forms.TabPage();
            this.cbPalletLabels = new System.Windows.Forms.ComboBox();
            this.bnAdd = new System.Windows.Forms.Button();
            this.gridLabels = new SourceGrid.Grid();
            this.tabPagePalletSleeve = new System.Windows.Forms.TabPage();
            this.cbPalletSleeveColor = new OfficePickers.ColorPicker.ComboBoxColorPicker();
            this.chkbPalletSleeve = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrlSolution)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz)).BeginInit();
            this.splitContainerHoriz.Panel1.SuspendLayout();
            this.splitContainerHoriz.Panel2.SuspendLayout();
            this.splitContainerHoriz.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).BeginInit();
            this.splitContainerVert.Panel1.SuspendLayout();
            this.splitContainerVert.Panel2.SuspendLayout();
            this.splitContainerVert.SuspendLayout();
            this.gbStopCriterions.SuspendLayout();
            this.tabCtrl.SuspendLayout();
            this.tabPageStrappers.SuspendLayout();
            this.tabPagePalletCorners.SuspendLayout();
            this.tabPagePalletCornersTop.SuspendLayout();
            this.tabPagePalletCap.SuspendLayout();
            this.tabPagePalletFilm.SuspendLayout();
            this.tabPagePalletLabels.SuspendLayout();
            this.tabPagePalletSleeve.SuspendLayout();
            this.SuspendLayout();
            // 
            // graphCtrlSolution
            // 
            resources.ApplyResources(this.graphCtrlSolution, "graphCtrlSolution");
            // 
            // splitContainerHoriz
            // 
            resources.ApplyResources(this.splitContainerHoriz, "splitContainerHoriz");
            // 
            // splitContainerHoriz.Panel1
            // 
            resources.ApplyResources(this.splitContainerHoriz.Panel1, "splitContainerHoriz.Panel1");
            // 
            // splitContainerHoriz.Panel2
            // 
            resources.ApplyResources(this.splitContainerHoriz.Panel2, "splitContainerHoriz.Panel2");
            this.splitContainerHoriz.Panel2.Controls.Add(this.gbStopCriterions);
            this.splitContainerHoriz.Panel2.Controls.Add(this.tabCtrl);
            // 
            // splitContainerVert
            // 
            resources.ApplyResources(this.splitContainerVert, "splitContainerVert");
            // 
            // splitContainerVert.Panel1
            // 
            resources.ApplyResources(this.splitContainerVert.Panel1, "splitContainerVert.Panel1");
            // 
            // splitContainerVert.Panel2
            // 
            resources.ApplyResources(this.splitContainerVert.Panel2, "splitContainerVert.Panel2");
            // 
            // cbInterlayer
            // 
            resources.ApplyResources(this.cbInterlayer, "cbInterlayer");
            // 
            // gridSolution
            // 
            resources.ApplyResources(this.gridSolution, "gridSolution");
            // 
            // gbStopCriterions
            // 
            resources.ApplyResources(this.gbStopCriterions, "gbStopCriterions");
            this.gbStopCriterions.Controls.Add(this.uCtrlOptMaxNumber);
            this.gbStopCriterions.Controls.Add(this.uCtrlOptMaximumWeight);
            this.gbStopCriterions.Controls.Add(this.uCtrlMaxPalletHeight);
            this.gbStopCriterions.Name = "gbStopCriterions";
            this.gbStopCriterions.TabStop = false;
            // 
            // uCtrlOptMaxNumber
            // 
            resources.ApplyResources(this.uCtrlOptMaxNumber, "uCtrlOptMaxNumber");
            this.uCtrlOptMaxNumber.Minimum = 0;
            this.uCtrlOptMaxNumber.Name = "uCtrlOptMaxNumber";
            this.uCtrlOptMaxNumber.ValueChanged += new treeDiM.Basics.UCtrlOptInt.ValueChangedDelegate(this.OnCriterionChanged);
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
            // tabCtrl
            // 
            resources.ApplyResources(this.tabCtrl, "tabCtrl");
            this.tabCtrl.Controls.Add(this.tabPageStrappers);
            this.tabCtrl.Controls.Add(this.tabPagePalletCorners);
            this.tabCtrl.Controls.Add(this.tabPagePalletCornersTop);
            this.tabCtrl.Controls.Add(this.tabPagePalletCap);
            this.tabCtrl.Controls.Add(this.tabPagePalletFilm);
            this.tabCtrl.Controls.Add(this.tabPagePalletLabels);
            this.tabCtrl.Controls.Add(this.tabPagePalletSleeve);
            this.tabCtrl.Multiline = true;
            this.tabCtrl.Name = "tabCtrl";
            this.tabCtrl.SelectedIndex = 0;
            // 
            // tabPageStrappers
            // 
            resources.ApplyResources(this.tabPageStrappers, "tabPageStrappers");
            this.tabPageStrappers.Controls.Add(this.ctrlStrapperSet);
            this.tabPageStrappers.Name = "tabPageStrappers";
            this.tabPageStrappers.UseVisualStyleBackColor = true;
            // 
            // ctrlStrapperSet
            // 
            resources.ApplyResources(this.ctrlStrapperSet, "ctrlStrapperSet");
            this.ctrlStrapperSet.Name = "ctrlStrapperSet";
            this.ctrlStrapperSet.Number = 0;
            this.ctrlStrapperSet.StrapperSet = null;
            this.ctrlStrapperSet.ValueChanged += new treeDiM.StackBuilder.Basics.Controls.CtrlStrapperSet.OnValueChanged(this.OnPalletProtectionChanged);
            // 
            // tabPagePalletCorners
            // 
            resources.ApplyResources(this.tabPagePalletCorners, "tabPagePalletCorners");
            this.tabPagePalletCorners.Controls.Add(this.cbPalletCorners);
            this.tabPagePalletCorners.Controls.Add(this.chkbPalletCorners);
            this.tabPagePalletCorners.Name = "tabPagePalletCorners";
            this.tabPagePalletCorners.UseVisualStyleBackColor = true;
            // 
            // cbPalletCorners
            // 
            resources.ApplyResources(this.cbPalletCorners, "cbPalletCorners");
            this.cbPalletCorners.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPalletCorners.FormattingEnabled = true;
            this.cbPalletCorners.Name = "cbPalletCorners";
            this.cbPalletCorners.SelectedIndexChanged += new System.EventHandler(this.OnPalletProtectionChanged);
            // 
            // chkbPalletCorners
            // 
            resources.ApplyResources(this.chkbPalletCorners, "chkbPalletCorners");
            this.chkbPalletCorners.Name = "chkbPalletCorners";
            this.chkbPalletCorners.UseVisualStyleBackColor = true;
            this.chkbPalletCorners.CheckedChanged += new System.EventHandler(this.OnPalletProtectionChanged);
            // 
            // tabPagePalletCornersTop
            // 
            resources.ApplyResources(this.tabPagePalletCornersTop, "tabPagePalletCornersTop");
            this.tabPagePalletCornersTop.Controls.Add(this.chkbPalletCornersTopY);
            this.tabPagePalletCornersTop.Controls.Add(this.cbPalletCornersTop);
            this.tabPagePalletCornersTop.Controls.Add(this.chkbPalletCornersTopX);
            this.tabPagePalletCornersTop.Name = "tabPagePalletCornersTop";
            this.tabPagePalletCornersTop.UseVisualStyleBackColor = true;
            // 
            // chkbPalletCornersTopY
            // 
            resources.ApplyResources(this.chkbPalletCornersTopY, "chkbPalletCornersTopY");
            this.chkbPalletCornersTopY.Name = "chkbPalletCornersTopY";
            this.chkbPalletCornersTopY.UseVisualStyleBackColor = true;
            this.chkbPalletCornersTopY.CheckedChanged += new System.EventHandler(this.OnPalletProtectionChanged);
            // 
            // cbPalletCornersTop
            // 
            resources.ApplyResources(this.cbPalletCornersTop, "cbPalletCornersTop");
            this.cbPalletCornersTop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPalletCornersTop.FormattingEnabled = true;
            this.cbPalletCornersTop.Name = "cbPalletCornersTop";
            this.cbPalletCornersTop.SelectedIndexChanged += new System.EventHandler(this.OnPalletProtectionChanged);
            // 
            // chkbPalletCornersTopX
            // 
            resources.ApplyResources(this.chkbPalletCornersTopX, "chkbPalletCornersTopX");
            this.chkbPalletCornersTopX.Name = "chkbPalletCornersTopX";
            this.chkbPalletCornersTopX.UseVisualStyleBackColor = true;
            this.chkbPalletCornersTopX.CheckedChanged += new System.EventHandler(this.OnPalletProtectionChanged);
            // 
            // tabPagePalletCap
            // 
            resources.ApplyResources(this.tabPagePalletCap, "tabPagePalletCap");
            this.tabPagePalletCap.Controls.Add(this.cbTopInterlayer);
            this.tabPagePalletCap.Controls.Add(this.chkbTopInterlayer);
            this.tabPagePalletCap.Controls.Add(this.cbPalletCap);
            this.tabPagePalletCap.Controls.Add(this.chkbPalletCap);
            this.tabPagePalletCap.Name = "tabPagePalletCap";
            this.tabPagePalletCap.UseVisualStyleBackColor = true;
            // 
            // cbTopInterlayer
            // 
            resources.ApplyResources(this.cbTopInterlayer, "cbTopInterlayer");
            this.cbTopInterlayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTopInterlayer.FormattingEnabled = true;
            this.cbTopInterlayer.Name = "cbTopInterlayer";
            this.cbTopInterlayer.SelectedIndexChanged += new System.EventHandler(this.OnPalletProtectionChanged);
            // 
            // chkbTopInterlayer
            // 
            resources.ApplyResources(this.chkbTopInterlayer, "chkbTopInterlayer");
            this.chkbTopInterlayer.Name = "chkbTopInterlayer";
            this.chkbTopInterlayer.UseVisualStyleBackColor = true;
            this.chkbTopInterlayer.CheckedChanged += new System.EventHandler(this.OnPalletProtectionChanged);
            // 
            // cbPalletCap
            // 
            resources.ApplyResources(this.cbPalletCap, "cbPalletCap");
            this.cbPalletCap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPalletCap.FormattingEnabled = true;
            this.cbPalletCap.Name = "cbPalletCap";
            this.cbPalletCap.SelectedIndexChanged += new System.EventHandler(this.OnPalletProtectionChanged);
            // 
            // chkbPalletCap
            // 
            resources.ApplyResources(this.chkbPalletCap, "chkbPalletCap");
            this.chkbPalletCap.Name = "chkbPalletCap";
            this.chkbPalletCap.UseVisualStyleBackColor = true;
            this.chkbPalletCap.CheckedChanged += new System.EventHandler(this.OnPalletProtectionChanged);
            // 
            // tabPagePalletFilm
            // 
            resources.ApplyResources(this.tabPagePalletFilm, "tabPagePalletFilm");
            this.tabPagePalletFilm.Controls.Add(this.uCtrlPalletFilmCovering);
            this.tabPagePalletFilm.Controls.Add(this.cbPalletFilm);
            this.tabPagePalletFilm.Controls.Add(this.chkbPalletFilm);
            this.tabPagePalletFilm.Name = "tabPagePalletFilm";
            this.tabPagePalletFilm.UseVisualStyleBackColor = true;
            // 
            // uCtrlPalletFilmCovering
            // 
            resources.ApplyResources(this.uCtrlPalletFilmCovering, "uCtrlPalletFilmCovering");
            this.uCtrlPalletFilmCovering.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.uCtrlPalletFilmCovering.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.uCtrlPalletFilmCovering.Name = "uCtrlPalletFilmCovering";
            this.uCtrlPalletFilmCovering.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlPalletFilmCovering.ValueChanged += new treeDiM.Basics.UCtrlDouble.ValueChangedDelegate(this.OnPalletProtectionChanged);
            // 
            // cbPalletFilm
            // 
            resources.ApplyResources(this.cbPalletFilm, "cbPalletFilm");
            this.cbPalletFilm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPalletFilm.FormattingEnabled = true;
            this.cbPalletFilm.Name = "cbPalletFilm";
            this.cbPalletFilm.SelectedIndexChanged += new System.EventHandler(this.OnCriterionChanged);
            // 
            // chkbPalletFilm
            // 
            resources.ApplyResources(this.chkbPalletFilm, "chkbPalletFilm");
            this.chkbPalletFilm.Name = "chkbPalletFilm";
            this.chkbPalletFilm.UseVisualStyleBackColor = true;
            this.chkbPalletFilm.CheckedChanged += new System.EventHandler(this.OnPalletProtectionChanged);
            // 
            // tabPagePalletLabels
            // 
            resources.ApplyResources(this.tabPagePalletLabels, "tabPagePalletLabels");
            this.tabPagePalletLabels.Controls.Add(this.cbPalletLabels);
            this.tabPagePalletLabels.Controls.Add(this.bnAdd);
            this.tabPagePalletLabels.Controls.Add(this.gridLabels);
            this.tabPagePalletLabels.Name = "tabPagePalletLabels";
            this.tabPagePalletLabels.UseVisualStyleBackColor = true;
            // 
            // cbPalletLabels
            // 
            resources.ApplyResources(this.cbPalletLabels, "cbPalletLabels");
            this.cbPalletLabels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPalletLabels.FormattingEnabled = true;
            this.cbPalletLabels.Name = "cbPalletLabels";
            // 
            // bnAdd
            // 
            resources.ApplyResources(this.bnAdd, "bnAdd");
            this.bnAdd.Name = "bnAdd";
            this.bnAdd.UseVisualStyleBackColor = true;
            this.bnAdd.Click += new System.EventHandler(this.OnBnAddLabel);
            // 
            // gridLabels
            // 
            resources.ApplyResources(this.gridLabels, "gridLabels");
            this.gridLabels.EnableSort = true;
            this.gridLabels.Name = "gridLabels";
            this.gridLabels.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridLabels.SelectionMode = SourceGrid.GridSelectionMode.Row;
            this.gridLabels.TabStop = true;
            this.gridLabels.ToolTipText = "";
            // 
            // tabPagePalletSleeve
            // 
            resources.ApplyResources(this.tabPagePalletSleeve, "tabPagePalletSleeve");
            this.tabPagePalletSleeve.Controls.Add(this.cbPalletSleeveColor);
            this.tabPagePalletSleeve.Controls.Add(this.chkbPalletSleeve);
            this.tabPagePalletSleeve.Name = "tabPagePalletSleeve";
            this.tabPagePalletSleeve.UseVisualStyleBackColor = true;
            // 
            // cbPalletSleeveColor
            // 
            resources.ApplyResources(this.cbPalletSleeveColor, "cbPalletSleeveColor");
            this.cbPalletSleeveColor.Color = System.Drawing.Color.Beige;
            this.cbPalletSleeveColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbPalletSleeveColor.DropDownHeight = 1;
            this.cbPalletSleeveColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPalletSleeveColor.DropDownWidth = 1;
            this.cbPalletSleeveColor.FormattingEnabled = true;
            this.cbPalletSleeveColor.Items.AddRange(new object[] {
            resources.GetString("cbPalletSleeveColor.Items"),
            resources.GetString("cbPalletSleeveColor.Items1"),
            resources.GetString("cbPalletSleeveColor.Items2"),
            resources.GetString("cbPalletSleeveColor.Items3"),
            resources.GetString("cbPalletSleeveColor.Items4"),
            resources.GetString("cbPalletSleeveColor.Items5"),
            resources.GetString("cbPalletSleeveColor.Items6"),
            resources.GetString("cbPalletSleeveColor.Items7"),
            resources.GetString("cbPalletSleeveColor.Items8"),
            resources.GetString("cbPalletSleeveColor.Items9"),
            resources.GetString("cbPalletSleeveColor.Items10"),
            resources.GetString("cbPalletSleeveColor.Items11"),
            resources.GetString("cbPalletSleeveColor.Items12"),
            resources.GetString("cbPalletSleeveColor.Items13"),
            resources.GetString("cbPalletSleeveColor.Items14"),
            resources.GetString("cbPalletSleeveColor.Items15"),
            resources.GetString("cbPalletSleeveColor.Items16"),
            resources.GetString("cbPalletSleeveColor.Items17"),
            resources.GetString("cbPalletSleeveColor.Items18"),
            resources.GetString("cbPalletSleeveColor.Items19")});
            this.cbPalletSleeveColor.Name = "cbPalletSleeveColor";
            this.cbPalletSleeveColor.SelectedIndexChanged += new System.EventHandler(this.OnPalletProtectionChanged);
            // 
            // chkbPalletSleeve
            // 
            resources.ApplyResources(this.chkbPalletSleeve, "chkbPalletSleeve");
            this.chkbPalletSleeve.Name = "chkbPalletSleeve";
            this.chkbPalletSleeve.UseVisualStyleBackColor = true;
            this.chkbPalletSleeve.CheckedChanged += new System.EventHandler(this.OnPalletProtectionChanged);
            // 
            // DockContentAnalysisCasePallet
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "DockContentAnalysisCasePallet";
            this.SizeChanged += new System.EventHandler(this.OnSizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrlSolution)).EndInit();
            this.splitContainerHoriz.Panel1.ResumeLayout(false);
            this.splitContainerHoriz.Panel2.ResumeLayout(false);
            this.splitContainerHoriz.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz)).EndInit();
            this.splitContainerHoriz.ResumeLayout(false);
            this.splitContainerVert.Panel1.ResumeLayout(false);
            this.splitContainerVert.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).EndInit();
            this.splitContainerVert.ResumeLayout(false);
            this.gbStopCriterions.ResumeLayout(false);
            this.tabCtrl.ResumeLayout(false);
            this.tabPageStrappers.ResumeLayout(false);
            this.tabPagePalletCorners.ResumeLayout(false);
            this.tabPagePalletCorners.PerformLayout();
            this.tabPagePalletCornersTop.ResumeLayout(false);
            this.tabPagePalletCornersTop.PerformLayout();
            this.tabPagePalletCap.ResumeLayout(false);
            this.tabPagePalletCap.PerformLayout();
            this.tabPagePalletFilm.ResumeLayout(false);
            this.tabPagePalletFilm.PerformLayout();
            this.tabPagePalletLabels.ResumeLayout(false);
            this.tabPagePalletSleeve.ResumeLayout(false);
            this.tabPagePalletSleeve.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.TabControl tabCtrl;
        private System.Windows.Forms.TabPage tabPagePalletCorners;
        private System.Windows.Forms.ComboBox cbPalletCorners;
        private System.Windows.Forms.CheckBox chkbPalletCorners;
        private System.Windows.Forms.TabPage tabPagePalletCap;
        private System.Windows.Forms.ComboBox cbPalletCap;
        private System.Windows.Forms.CheckBox chkbPalletCap;
        private System.Windows.Forms.TabPage tabPagePalletFilm;
        private System.Windows.Forms.ComboBox cbPalletFilm;
        private System.Windows.Forms.CheckBox chkbPalletFilm;
        private System.Windows.Forms.GroupBox gbStopCriterions;
        private treeDiM.Basics.UCtrlDouble uCtrlMaxPalletHeight;
        private treeDiM.Basics.UCtrlOptDouble uCtrlOptMaximumWeight;
        private treeDiM.Basics.UCtrlOptInt uCtrlOptMaxNumber;
        private System.Windows.Forms.TabPage tabPageStrappers;
        private Basics.Controls.CtrlStrapperSet ctrlStrapperSet;
        private System.Windows.Forms.TabPage tabPagePalletCornersTop;
        private System.Windows.Forms.ComboBox cbPalletCornersTop;
        private System.Windows.Forms.CheckBox chkbPalletCornersTopX;
        private System.Windows.Forms.CheckBox chkbPalletCornersTopY;
        private System.Windows.Forms.TabPage tabPagePalletSleeve;
        private System.Windows.Forms.CheckBox chkbPalletSleeve;
        private System.Windows.Forms.TabPage tabPagePalletLabels;
        private OfficePickers.ColorPicker.ComboBoxColorPicker cbPalletSleeveColor;
        private System.Windows.Forms.ComboBox cbPalletLabels;
        private treeDiM.Basics.UCtrlDouble uCtrlPalletFilmCovering;
        private System.Windows.Forms.Button bnAdd;
        private SourceGrid.Grid gridLabels;
        private Graphics.Controls.CCtrlComboFiltered cbTopInterlayer;
        private System.Windows.Forms.CheckBox chkbTopInterlayer;
    }
}