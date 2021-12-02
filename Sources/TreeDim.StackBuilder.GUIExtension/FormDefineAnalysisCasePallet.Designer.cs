namespace treeDiM.StackBuilder.GUIExtension
{
    partial class FormDefineAnalysisCasePallet
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDefineAnalysisCasePallet));
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnNext = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelDef = new System.Windows.Forms.ToolStripStatusLabel();
            this.gbCase = new System.Windows.Forms.GroupBox();
            this.uCtrlCase = new treeDiM.StackBuilder.GUIExtension.UCtrlCase();
            this.uCtrlBundle = new treeDiM.StackBuilder.GUIExtension.UCtrlBundle();
            this.uCtrlCaseOrientation = new treeDiM.StackBuilder.Graphics.UCtrlCaseOrientation();
            this.gpPallet = new System.Windows.Forms.GroupBox();
            this.uCtrlOverhang = new treeDiM.Basics.UCtrlDualDouble();
            this.graphCtrlPallet = new treeDiM.StackBuilder.Graphics.Graphics3DControl();
            this.cbPallet = new treeDiM.StackBuilder.GUIExtension.CtrlComboDBPallet();
            this.lbPallets = new System.Windows.Forms.Label();
            this.gpConstraintSet = new System.Windows.Forms.GroupBox();
            this.uCtrlOptMaximumWeight = new treeDiM.Basics.UCtrlOptDouble();
            this.uCtrlMaximumHeight = new treeDiM.Basics.UCtrlDouble();
            this.uCtrlLayerList = new treeDiM.StackBuilder.Graphics.UCtrlLayerList();
            this.statusStrip.SuspendLayout();
            this.gbCase.SuspendLayout();
            this.gpPallet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrlPallet)).BeginInit();
            this.gpConstraintSet.SuspendLayout();
            this.SuspendLayout();
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(553, 4);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 0;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnNext
            // 
            this.bnNext.Location = new System.Drawing.Point(553, 613);
            this.bnNext.Name = "bnNext";
            this.bnNext.Size = new System.Drawing.Size(75, 23);
            this.bnNext.TabIndex = 1;
            this.bnNext.Text = "Next >";
            this.bnNext.UseVisualStyleBackColor = true;
            this.bnNext.Click += new System.EventHandler(this.OnNext);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelDef});
            this.statusStrip.Location = new System.Drawing.Point(0, 639);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(634, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip";
            // 
            // toolStripStatusLabelDef
            // 
            this.toolStripStatusLabelDef.Name = "toolStripStatusLabelDef";
            this.toolStripStatusLabelDef.Size = new System.Drawing.Size(84, 17);
            this.toolStripStatusLabelDef.Text = "statusLabelDef";
            // 
            // gbCase
            // 
            this.gbCase.Controls.Add(this.uCtrlCase);
            this.gbCase.Controls.Add(this.uCtrlBundle);
            this.gbCase.Controls.Add(this.uCtrlCaseOrientation);
            this.gbCase.Location = new System.Drawing.Point(3, 4);
            this.gbCase.Name = "gbCase";
            this.gbCase.Size = new System.Drawing.Size(312, 269);
            this.gbCase.TabIndex = 3;
            this.gbCase.TabStop = false;
            this.gbCase.Text = "Case";
            // 
            // uCtrlCase
            // 
            this.uCtrlCase.Dimensions = new double[] {
        0D,
        0D,
        0D};
            this.uCtrlCase.Location = new System.Drawing.Point(6, 20);
            this.uCtrlCase.Name = "uCtrlCase";
            this.uCtrlCase.Size = new System.Drawing.Size(300, 128);
            this.uCtrlCase.TabIndex = 4;
            this.uCtrlCase.Weight = 0D;
            // 
            // uCtrlBundle
            // 
            this.uCtrlBundle.Dimensions = new double[] {
        0D,
        0D,
        0D};
            this.uCtrlBundle.Location = new System.Drawing.Point(7, 20);
            this.uCtrlBundle.Name = "uCtrlBundle";
            this.uCtrlBundle.NoFlats = 0;
            this.uCtrlBundle.Size = new System.Drawing.Size(300, 110);
            this.uCtrlBundle.TabIndex = 3;
            this.uCtrlBundle.UnitThickness = 0D;
            this.uCtrlBundle.UnitWeight = 0D;
            // 
            // uCtrlCaseOrientation
            // 
            this.uCtrlCaseOrientation.AllowedOrientations = new bool[] {
        false,
        false,
        true};
            this.uCtrlCaseOrientation.Location = new System.Drawing.Point(10, 154);
            this.uCtrlCaseOrientation.Name = "uCtrlCaseOrientation";
            this.uCtrlCaseOrientation.Size = new System.Drawing.Size(280, 110);
            this.uCtrlCaseOrientation.TabIndex = 2;
            this.uCtrlCaseOrientation.CheckedChanged += new treeDiM.StackBuilder.Graphics.UCtrlCaseOrientation.CheckChanged(this.OnInputChanged);
            // 
            // gpPallet
            // 
            this.gpPallet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gpPallet.Controls.Add(this.uCtrlOverhang);
            this.gpPallet.Controls.Add(this.graphCtrlPallet);
            this.gpPallet.Controls.Add(this.cbPallet);
            this.gpPallet.Controls.Add(this.lbPallets);
            this.gpPallet.Location = new System.Drawing.Point(321, 28);
            this.gpPallet.Name = "gpPallet";
            this.gpPallet.Size = new System.Drawing.Size(307, 245);
            this.gpPallet.TabIndex = 4;
            this.gpPallet.TabStop = false;
            this.gpPallet.Text = "Pallet";
            // 
            // uCtrlOverhang
            // 
            this.uCtrlOverhang.Location = new System.Drawing.Point(4, 217);
            this.uCtrlOverhang.Name = "uCtrlOverhang";
            this.uCtrlOverhang.Size = new System.Drawing.Size(243, 20);
            this.uCtrlOverhang.TabIndex = 0;
            this.uCtrlOverhang.Text = "Overhang";
            this.uCtrlOverhang.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlOverhang.ValueX = 0D;
            this.uCtrlOverhang.ValueY = 0D;
            this.uCtrlOverhang.ValueChanged += new treeDiM.Basics.UCtrlDualDouble.ValueChangedDelegate(this.OnInputChanged);
            // 
            // graphCtrlPallet
            // 
            this.graphCtrlPallet.Location = new System.Drawing.Point(89, 40);
            this.graphCtrlPallet.Name = "graphCtrlPallet";
            this.graphCtrlPallet.Size = new System.Drawing.Size(211, 150);
            this.graphCtrlPallet.TabIndex = 2;
            this.graphCtrlPallet.Viewer = null;
            // 
            // cbPallet
            // 
            this.cbPallet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPallet.FormattingEnabled = true;
            this.cbPallet.Location = new System.Drawing.Point(89, 16);
            this.cbPallet.Name = "cbPallet";
            this.cbPallet.Size = new System.Drawing.Size(212, 21);
            this.cbPallet.TabIndex = 1;
            this.cbPallet.SelectedIndexChanged += new System.EventHandler(this.OnPalletChanged);
            // 
            // lbPallets
            // 
            this.lbPallets.AutoSize = true;
            this.lbPallets.Location = new System.Drawing.Point(4, 19);
            this.lbPallets.Name = "lbPallets";
            this.lbPallets.Size = new System.Drawing.Size(33, 13);
            this.lbPallets.TabIndex = 0;
            this.lbPallets.Text = "Pallet";
            // 
            // gpConstraintSet
            // 
            this.gpConstraintSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gpConstraintSet.Controls.Add(this.uCtrlOptMaximumWeight);
            this.gpConstraintSet.Controls.Add(this.uCtrlMaximumHeight);
            this.gpConstraintSet.Location = new System.Drawing.Point(3, 274);
            this.gpConstraintSet.Name = "gpConstraintSet";
            this.gpConstraintSet.Size = new System.Drawing.Size(625, 35);
            this.gpConstraintSet.TabIndex = 5;
            this.gpConstraintSet.TabStop = false;
            this.gpConstraintSet.Text = "Constraint set";
            // 
            // uCtrlOptMaximumWeight
            // 
            this.uCtrlOptMaximumWeight.Location = new System.Drawing.Point(318, 12);
            this.uCtrlOptMaximumWeight.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.uCtrlOptMaximumWeight.MinimumSize = new System.Drawing.Size(100, 20);
            this.uCtrlOptMaximumWeight.Name = "uCtrlOptMaximumWeight";
            this.uCtrlOptMaximumWeight.Size = new System.Drawing.Size(303, 20);
            this.uCtrlOptMaximumWeight.TabIndex = 1;
            this.uCtrlOptMaximumWeight.Text = "Maximum pallet weight";
            this.uCtrlOptMaximumWeight.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_MASS;
            this.uCtrlOptMaximumWeight.ValueChanged += new treeDiM.Basics.UCtrlOptDouble.ValueChangedDelegate(this.OnInputChanged);
            // 
            // uCtrlMaximumHeight
            // 
            this.uCtrlMaximumHeight.Location = new System.Drawing.Point(6, 12);
            this.uCtrlMaximumHeight.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.uCtrlMaximumHeight.MinimumSize = new System.Drawing.Size(100, 20);
            this.uCtrlMaximumHeight.Name = "uCtrlMaximumHeight";
            this.uCtrlMaximumHeight.Size = new System.Drawing.Size(243, 20);
            this.uCtrlMaximumHeight.TabIndex = 0;
            this.uCtrlMaximumHeight.Text = "Maximum pallet height";
            this.uCtrlMaximumHeight.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlMaximumHeight.Value = 0D;
            this.uCtrlMaximumHeight.ValueChanged += new treeDiM.Basics.UCtrlDouble.ValueChangedDelegate(this.OnInputChanged);
            // 
            // uCtrlLayerList
            // 
            this.uCtrlLayerList.AutoScroll = true;
            this.uCtrlLayerList.Location = new System.Drawing.Point(0, 312);
            this.uCtrlLayerList.Name = "uCtrlLayerList";
            this.uCtrlLayerList.SingleSelection = false;
            this.uCtrlLayerList.Size = new System.Drawing.Size(634, 295);
            this.uCtrlLayerList.TabIndex = 6;
            this.uCtrlLayerList.LayerSelected += new treeDiM.StackBuilder.Graphics.UCtrlLayerList.LayerButtonClicked(this.OnLayerSelected);
            // 
            // FormDefineAnalysisCasePallet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(634, 661);
            this.Controls.Add(this.uCtrlLayerList);
            this.Controls.Add(this.gpConstraintSet);
            this.Controls.Add(this.gpPallet);
            this.Controls.Add(this.gbCase);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.bnNext);
            this.Controls.Add(this.bnCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDefineAnalysisCasePallet";
            this.ShowInTaskbar = false;
            this.Text = "Define case/pallet analysis...";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.gbCase.ResumeLayout(false);
            this.gpPallet.ResumeLayout(false);
            this.gpPallet.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrlPallet)).EndInit();
            this.gpConstraintSet.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Button bnNext;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelDef;
        private System.Windows.Forms.GroupBox gbCase;
        private Graphics.UCtrlCaseOrientation uCtrlCaseOrientation;
        private System.Windows.Forms.GroupBox gpPallet;
        private System.Windows.Forms.Label lbPallets;
        private System.Windows.Forms.GroupBox gpConstraintSet;
        private CtrlComboDBPallet cbPallet;
        private Graphics.Graphics3DControl graphCtrlPallet;
        private Graphics.UCtrlLayerList uCtrlLayerList;
        private treeDiM.Basics.UCtrlDualDouble uCtrlOverhang;
        private treeDiM.Basics.UCtrlOptDouble uCtrlOptMaximumWeight;
        private treeDiM.Basics.UCtrlDouble uCtrlMaximumHeight;
        private UCtrlBundle uCtrlBundle;
        private UCtrlCase uCtrlCase;
    }
}