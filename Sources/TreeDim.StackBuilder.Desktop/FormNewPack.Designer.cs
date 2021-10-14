namespace treeDiM.StackBuilder.Desktop
{
    partial class FormNewPack
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNewPack));
            this.graphCtrl = new treeDiM.StackBuilder.Graphics.Graphics3DControl();
            this.lbContent = new System.Windows.Forms.Label();
            this.cbInnerPackable = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered();
            this.lbDir = new System.Windows.Forms.Label();
            this.cbDir = new System.Windows.Forms.ComboBox();
            this.uCtrlOuterDimensions = new treeDiM.Basics.UCtrlOptTriDouble();
            this.uCtrlLayout = new treeDiM.Basics.UCtrlTriInt();
            this.uCtrlWrapperWalls = new treeDiM.Basics.UCtrlTriInt();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.uCtrlWrapperWeight = new treeDiM.Basics.UCtrlDouble();
            this.uCtrlWrapperThickness = new treeDiM.Basics.UCtrlDouble();
            this.lbWrapperColor = new System.Windows.Forms.Label();
            this.cbWrapperColor = new OfficePickers.ColorPicker.ComboBoxColorPicker();
            this.tabCtrl = new System.Windows.Forms.TabControl();
            this.tabWrapper = new System.Windows.Forms.TabPage();
            this.chkbWrapper = new System.Windows.Forms.CheckBox();
            this.tabTray = new System.Windows.Forms.TabPage();
            this.uCtrlTrayWalls = new treeDiM.Basics.UCtrlTriInt();
            this.uCtrlTrayUnitThickness = new treeDiM.Basics.UCtrlDouble();
            this.uCtrlTrayWeight = new treeDiM.Basics.UCtrlDouble();
            this.chkbTray = new System.Windows.Forms.CheckBox();
            this.cbTrayColor = new OfficePickers.ColorPicker.ComboBoxColorPicker();
            this.uCtrlTrayHeight = new treeDiM.Basics.UCtrlDouble();
            this.tabStrappers = new System.Windows.Forms.TabPage();
            this.ctrlStrapperSet = new treeDiM.StackBuilder.Basics.Controls.CtrlStrapperSet();
            this.tabPageBulge = new System.Windows.Forms.TabPage();
            this.uCtrlOptBulge = new treeDiM.Basics.UCtrlOptTriDouble();
            this.lbCylLayoutType = new System.Windows.Forms.Label();
            this.cbCylLayoutType = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrl)).BeginInit();
            this.tabCtrl.SuspendLayout();
            this.tabWrapper.SuspendLayout();
            this.tabTray.SuspendLayout();
            this.tabStrappers.SuspendLayout();
            this.tabPageBulge.SuspendLayout();
            this.SuspendLayout();
            // 
            // bnOk
            // 
            resources.ApplyResources(this.bnOk, "bnOk");
            // 
            // bnCancel
            // 
            resources.ApplyResources(this.bnCancel, "bnCancel");
            // 
            // lbName
            // 
            resources.ApplyResources(this.lbName, "lbName");
            // 
            // lbDescription
            // 
            resources.ApplyResources(this.lbDescription, "lbDescription");
            // 
            // tbName
            // 
            resources.ApplyResources(this.tbName, "tbName");
            // 
            // tbDescription
            // 
            resources.ApplyResources(this.tbDescription, "tbDescription");
            // 
            // graphCtrl
            // 
            resources.ApplyResources(this.graphCtrl, "graphCtrl");
            this.graphCtrl.AngleHoriz = 45D;
            this.graphCtrl.Name = "graphCtrl";
            this.graphCtrl.Viewer = null;
            // 
            // lbContent
            // 
            resources.ApplyResources(this.lbContent, "lbContent");
            this.lbContent.Name = "lbContent";
            // 
            // cbInnerPackable
            // 
            resources.ApplyResources(this.cbInnerPackable, "cbInnerPackable");
            this.cbInnerPackable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInnerPackable.FormattingEnabled = true;
            this.cbInnerPackable.Name = "cbInnerPackable";
            this.cbInnerPackable.SelectedIndexChanged += new System.EventHandler(this.OnContentChanged);
            // 
            // lbDir
            // 
            resources.ApplyResources(this.lbDir, "lbDir");
            this.lbDir.Name = "lbDir";
            // 
            // cbDir
            // 
            resources.ApplyResources(this.cbDir, "cbDir");
            this.cbDir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDir.FormattingEnabled = true;
            this.cbDir.Items.AddRange(new object[] {
            resources.GetString("cbDir.Items"),
            resources.GetString("cbDir.Items1"),
            resources.GetString("cbDir.Items2"),
            resources.GetString("cbDir.Items3"),
            resources.GetString("cbDir.Items4"),
            resources.GetString("cbDir.Items5")});
            this.cbDir.Name = "cbDir";
            this.cbDir.SelectedIndexChanged += new System.EventHandler(this.OnPackChanged);
            // 
            // uCtrlOuterDimensions
            // 
            resources.ApplyResources(this.uCtrlOuterDimensions, "uCtrlOuterDimensions");
            this.uCtrlOuterDimensions.Checked = false;
            this.uCtrlOuterDimensions.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.uCtrlOuterDimensions.Name = "uCtrlOuterDimensions";
            this.uCtrlOuterDimensions.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlOuterDimensions.X = 0D;
            this.uCtrlOuterDimensions.Y = 0D;
            this.uCtrlOuterDimensions.Z = 0D;
            this.uCtrlOuterDimensions.ValueChanged += new treeDiM.Basics.UCtrlOptTriDouble.ValueChangedDelegate(this.OnPackChanged);
            // 
            // uCtrlLayout
            // 
            resources.ApplyResources(this.uCtrlLayout, "uCtrlLayout");
            this.uCtrlLayout.Name = "uCtrlLayout";
            this.uCtrlLayout.NoX = 1;
            this.uCtrlLayout.NoY = 1;
            this.uCtrlLayout.NoZ = 1;
            this.uCtrlLayout.ValueChanged += new treeDiM.Basics.UCtrlTriInt.ValueChangedDelegate(this.OnPackChanged);
            // 
            // uCtrlWrapperWalls
            // 
            resources.ApplyResources(this.uCtrlWrapperWalls, "uCtrlWrapperWalls");
            this.uCtrlWrapperWalls.Name = "uCtrlWrapperWalls";
            this.uCtrlWrapperWalls.NoX = 1;
            this.uCtrlWrapperWalls.NoY = 1;
            this.uCtrlWrapperWalls.NoZ = 1;
            this.uCtrlWrapperWalls.ValueChanged += new treeDiM.Basics.UCtrlTriInt.ValueChangedDelegate(this.OnPackChanged);
            // 
            // cbType
            // 
            resources.ApplyResources(this.cbType, "cbType");
            this.cbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbType.FormattingEnabled = true;
            this.cbType.Items.AddRange(new object[] {
            resources.GetString("cbType.Items"),
            resources.GetString("cbType.Items1"),
            resources.GetString("cbType.Items2")});
            this.cbType.Name = "cbType";
            this.cbType.SelectedIndexChanged += new System.EventHandler(this.OnWrapperTypeChanged);
            // 
            // uCtrlWrapperWeight
            // 
            resources.ApplyResources(this.uCtrlWrapperWeight, "uCtrlWrapperWeight");
            this.uCtrlWrapperWeight.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.uCtrlWrapperWeight.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.uCtrlWrapperWeight.Name = "uCtrlWrapperWeight";
            this.uCtrlWrapperWeight.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_MASS;
            this.uCtrlWrapperWeight.ValueChanged += new treeDiM.Basics.UCtrlDouble.ValueChangedDelegate(this.OnPackChanged);
            // 
            // uCtrlWrapperThickness
            // 
            resources.ApplyResources(this.uCtrlWrapperThickness, "uCtrlWrapperThickness");
            this.uCtrlWrapperThickness.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.uCtrlWrapperThickness.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.uCtrlWrapperThickness.Name = "uCtrlWrapperThickness";
            this.uCtrlWrapperThickness.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlWrapperThickness.ValueChanged += new treeDiM.Basics.UCtrlDouble.ValueChangedDelegate(this.OnPackChanged);
            // 
            // lbWrapperColor
            // 
            resources.ApplyResources(this.lbWrapperColor, "lbWrapperColor");
            this.lbWrapperColor.Name = "lbWrapperColor";
            // 
            // cbWrapperColor
            // 
            resources.ApplyResources(this.cbWrapperColor, "cbWrapperColor");
            this.cbWrapperColor.Color = System.Drawing.Color.LightGray;
            this.cbWrapperColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbWrapperColor.DropDownHeight = 1;
            this.cbWrapperColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWrapperColor.DropDownWidth = 1;
            this.cbWrapperColor.FormattingEnabled = true;
            this.cbWrapperColor.Items.AddRange(new object[] {
            resources.GetString("cbWrapperColor.Items"),
            resources.GetString("cbWrapperColor.Items1"),
            resources.GetString("cbWrapperColor.Items2"),
            resources.GetString("cbWrapperColor.Items3"),
            resources.GetString("cbWrapperColor.Items4"),
            resources.GetString("cbWrapperColor.Items5"),
            resources.GetString("cbWrapperColor.Items6"),
            resources.GetString("cbWrapperColor.Items7"),
            resources.GetString("cbWrapperColor.Items8"),
            resources.GetString("cbWrapperColor.Items9"),
            resources.GetString("cbWrapperColor.Items10"),
            resources.GetString("cbWrapperColor.Items11"),
            resources.GetString("cbWrapperColor.Items12"),
            resources.GetString("cbWrapperColor.Items13"),
            resources.GetString("cbWrapperColor.Items14"),
            resources.GetString("cbWrapperColor.Items15"),
            resources.GetString("cbWrapperColor.Items16"),
            resources.GetString("cbWrapperColor.Items17"),
            resources.GetString("cbWrapperColor.Items18"),
            resources.GetString("cbWrapperColor.Items19"),
            resources.GetString("cbWrapperColor.Items20"),
            resources.GetString("cbWrapperColor.Items21"),
            resources.GetString("cbWrapperColor.Items22"),
            resources.GetString("cbWrapperColor.Items23"),
            resources.GetString("cbWrapperColor.Items24"),
            resources.GetString("cbWrapperColor.Items25"),
            resources.GetString("cbWrapperColor.Items26"),
            resources.GetString("cbWrapperColor.Items27"),
            resources.GetString("cbWrapperColor.Items28"),
            resources.GetString("cbWrapperColor.Items29"),
            resources.GetString("cbWrapperColor.Items30"),
            resources.GetString("cbWrapperColor.Items31"),
            resources.GetString("cbWrapperColor.Items32"),
            resources.GetString("cbWrapperColor.Items33"),
            resources.GetString("cbWrapperColor.Items34"),
            resources.GetString("cbWrapperColor.Items35"),
            resources.GetString("cbWrapperColor.Items36"),
            resources.GetString("cbWrapperColor.Items37"),
            resources.GetString("cbWrapperColor.Items38"),
            resources.GetString("cbWrapperColor.Items39"),
            resources.GetString("cbWrapperColor.Items40"),
            resources.GetString("cbWrapperColor.Items41"),
            resources.GetString("cbWrapperColor.Items42"),
            resources.GetString("cbWrapperColor.Items43"),
            resources.GetString("cbWrapperColor.Items44"),
            resources.GetString("cbWrapperColor.Items45"),
            resources.GetString("cbWrapperColor.Items46")});
            this.cbWrapperColor.Name = "cbWrapperColor";
            this.cbWrapperColor.SelectedColorChanged += new System.EventHandler(this.OnPackChanged);
            // 
            // tabCtrl
            // 
            resources.ApplyResources(this.tabCtrl, "tabCtrl");
            this.tabCtrl.Controls.Add(this.tabWrapper);
            this.tabCtrl.Controls.Add(this.tabTray);
            this.tabCtrl.Controls.Add(this.tabStrappers);
            this.tabCtrl.Controls.Add(this.tabPageBulge);
            this.tabCtrl.Name = "tabCtrl";
            this.tabCtrl.SelectedIndex = 0;
            // 
            // tabWrapper
            // 
            resources.ApplyResources(this.tabWrapper, "tabWrapper");
            this.tabWrapper.Controls.Add(this.chkbWrapper);
            this.tabWrapper.Controls.Add(this.uCtrlWrapperWalls);
            this.tabWrapper.Controls.Add(this.cbWrapperColor);
            this.tabWrapper.Controls.Add(this.lbWrapperColor);
            this.tabWrapper.Controls.Add(this.uCtrlWrapperWeight);
            this.tabWrapper.Controls.Add(this.uCtrlWrapperThickness);
            this.tabWrapper.Controls.Add(this.cbType);
            this.tabWrapper.Name = "tabWrapper";
            this.tabWrapper.UseVisualStyleBackColor = true;
            // 
            // chkbWrapper
            // 
            resources.ApplyResources(this.chkbWrapper, "chkbWrapper");
            this.chkbWrapper.Name = "chkbWrapper";
            this.chkbWrapper.UseVisualStyleBackColor = true;
            this.chkbWrapper.CheckedChanged += new System.EventHandler(this.OnWrapperChecked);
            // 
            // tabTray
            // 
            resources.ApplyResources(this.tabTray, "tabTray");
            this.tabTray.Controls.Add(this.uCtrlTrayWalls);
            this.tabTray.Controls.Add(this.uCtrlTrayUnitThickness);
            this.tabTray.Controls.Add(this.uCtrlTrayWeight);
            this.tabTray.Controls.Add(this.chkbTray);
            this.tabTray.Controls.Add(this.cbTrayColor);
            this.tabTray.Controls.Add(this.uCtrlTrayHeight);
            this.tabTray.Name = "tabTray";
            this.tabTray.UseVisualStyleBackColor = true;
            // 
            // uCtrlTrayWalls
            // 
            resources.ApplyResources(this.uCtrlTrayWalls, "uCtrlTrayWalls");
            this.uCtrlTrayWalls.Name = "uCtrlTrayWalls";
            this.uCtrlTrayWalls.NoX = 1;
            this.uCtrlTrayWalls.NoY = 1;
            this.uCtrlTrayWalls.NoZ = 1;
            this.uCtrlTrayWalls.ValueChanged += new treeDiM.Basics.UCtrlTriInt.ValueChangedDelegate(this.OnPackChanged);
            // 
            // uCtrlTrayUnitThickness
            // 
            resources.ApplyResources(this.uCtrlTrayUnitThickness, "uCtrlTrayUnitThickness");
            this.uCtrlTrayUnitThickness.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.uCtrlTrayUnitThickness.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.uCtrlTrayUnitThickness.Name = "uCtrlTrayUnitThickness";
            this.uCtrlTrayUnitThickness.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlTrayUnitThickness.ValueChanged += new treeDiM.Basics.UCtrlDouble.ValueChangedDelegate(this.OnPackChanged);
            // 
            // uCtrlTrayWeight
            // 
            resources.ApplyResources(this.uCtrlTrayWeight, "uCtrlTrayWeight");
            this.uCtrlTrayWeight.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.uCtrlTrayWeight.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.uCtrlTrayWeight.Name = "uCtrlTrayWeight";
            this.uCtrlTrayWeight.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_MASS;
            this.uCtrlTrayWeight.ValueChanged += new treeDiM.Basics.UCtrlDouble.ValueChangedDelegate(this.OnPackChanged);
            // 
            // chkbTray
            // 
            resources.ApplyResources(this.chkbTray, "chkbTray");
            this.chkbTray.Name = "chkbTray";
            this.chkbTray.UseVisualStyleBackColor = true;
            this.chkbTray.CheckedChanged += new System.EventHandler(this.OnTrayChecked);
            // 
            // cbTrayColor
            // 
            resources.ApplyResources(this.cbTrayColor, "cbTrayColor");
            this.cbTrayColor.Color = System.Drawing.Color.LightGray;
            this.cbTrayColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbTrayColor.DropDownHeight = 1;
            this.cbTrayColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTrayColor.DropDownWidth = 1;
            this.cbTrayColor.FormattingEnabled = true;
            this.cbTrayColor.Items.AddRange(new object[] {
            resources.GetString("cbTrayColor.Items"),
            resources.GetString("cbTrayColor.Items1"),
            resources.GetString("cbTrayColor.Items2"),
            resources.GetString("cbTrayColor.Items3"),
            resources.GetString("cbTrayColor.Items4"),
            resources.GetString("cbTrayColor.Items5"),
            resources.GetString("cbTrayColor.Items6"),
            resources.GetString("cbTrayColor.Items7"),
            resources.GetString("cbTrayColor.Items8"),
            resources.GetString("cbTrayColor.Items9"),
            resources.GetString("cbTrayColor.Items10"),
            resources.GetString("cbTrayColor.Items11"),
            resources.GetString("cbTrayColor.Items12"),
            resources.GetString("cbTrayColor.Items13"),
            resources.GetString("cbTrayColor.Items14"),
            resources.GetString("cbTrayColor.Items15"),
            resources.GetString("cbTrayColor.Items16"),
            resources.GetString("cbTrayColor.Items17"),
            resources.GetString("cbTrayColor.Items18"),
            resources.GetString("cbTrayColor.Items19"),
            resources.GetString("cbTrayColor.Items20"),
            resources.GetString("cbTrayColor.Items21"),
            resources.GetString("cbTrayColor.Items22"),
            resources.GetString("cbTrayColor.Items23"),
            resources.GetString("cbTrayColor.Items24"),
            resources.GetString("cbTrayColor.Items25"),
            resources.GetString("cbTrayColor.Items26"),
            resources.GetString("cbTrayColor.Items27"),
            resources.GetString("cbTrayColor.Items28"),
            resources.GetString("cbTrayColor.Items29"),
            resources.GetString("cbTrayColor.Items30"),
            resources.GetString("cbTrayColor.Items31"),
            resources.GetString("cbTrayColor.Items32"),
            resources.GetString("cbTrayColor.Items33"),
            resources.GetString("cbTrayColor.Items34"),
            resources.GetString("cbTrayColor.Items35"),
            resources.GetString("cbTrayColor.Items36"),
            resources.GetString("cbTrayColor.Items37"),
            resources.GetString("cbTrayColor.Items38"),
            resources.GetString("cbTrayColor.Items39"),
            resources.GetString("cbTrayColor.Items40"),
            resources.GetString("cbTrayColor.Items41"),
            resources.GetString("cbTrayColor.Items42"),
            resources.GetString("cbTrayColor.Items43"),
            resources.GetString("cbTrayColor.Items44"),
            resources.GetString("cbTrayColor.Items45"),
            resources.GetString("cbTrayColor.Items46"),
            resources.GetString("cbTrayColor.Items47"),
            resources.GetString("cbTrayColor.Items48")});
            this.cbTrayColor.Name = "cbTrayColor";
            this.cbTrayColor.SelectedColorChanged += new System.EventHandler(this.OnPackChanged);
            // 
            // uCtrlTrayHeight
            // 
            resources.ApplyResources(this.uCtrlTrayHeight, "uCtrlTrayHeight");
            this.uCtrlTrayHeight.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.uCtrlTrayHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.uCtrlTrayHeight.Name = "uCtrlTrayHeight";
            this.uCtrlTrayHeight.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlTrayHeight.ValueChanged += new treeDiM.Basics.UCtrlDouble.ValueChangedDelegate(this.OnPackChanged);
            // 
            // tabStrappers
            // 
            resources.ApplyResources(this.tabStrappers, "tabStrappers");
            this.tabStrappers.Controls.Add(this.ctrlStrapperSet);
            this.tabStrappers.Name = "tabStrappers";
            this.tabStrappers.UseVisualStyleBackColor = true;
            // 
            // ctrlStrapperSet
            // 
            resources.ApplyResources(this.ctrlStrapperSet, "ctrlStrapperSet");
            this.ctrlStrapperSet.Name = "ctrlStrapperSet";
            this.ctrlStrapperSet.Number = 0;
            this.ctrlStrapperSet.StrapperSet = null;
            this.ctrlStrapperSet.ValueChanged += new treeDiM.StackBuilder.Basics.Controls.CtrlStrapperSet.OnValueChanged(this.OnPackChanged);
            // 
            // tabPageBulge
            // 
            resources.ApplyResources(this.tabPageBulge, "tabPageBulge");
            this.tabPageBulge.Controls.Add(this.uCtrlOptBulge);
            this.tabPageBulge.Name = "tabPageBulge";
            this.tabPageBulge.UseVisualStyleBackColor = true;
            // 
            // uCtrlOptBulge
            // 
            resources.ApplyResources(this.uCtrlOptBulge, "uCtrlOptBulge");
            this.uCtrlOptBulge.Checked = false;
            this.uCtrlOptBulge.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.uCtrlOptBulge.Name = "uCtrlOptBulge";
            this.uCtrlOptBulge.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlOptBulge.X = 0D;
            this.uCtrlOptBulge.Y = 0D;
            this.uCtrlOptBulge.Z = 0D;
            // 
            // lbCylLayoutType
            // 
            resources.ApplyResources(this.lbCylLayoutType, "lbCylLayoutType");
            this.lbCylLayoutType.Name = "lbCylLayoutType";
            // 
            // cbCylLayoutType
            // 
            resources.ApplyResources(this.cbCylLayoutType, "cbCylLayoutType");
            this.cbCylLayoutType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCylLayoutType.FormattingEnabled = true;
            this.cbCylLayoutType.Items.AddRange(new object[] {
            resources.GetString("cbCylLayoutType.Items"),
            resources.GetString("cbCylLayoutType.Items1"),
            resources.GetString("cbCylLayoutType.Items2"),
            resources.GetString("cbCylLayoutType.Items3")});
            this.cbCylLayoutType.Name = "cbCylLayoutType";
            this.cbCylLayoutType.SelectedIndexChanged += new System.EventHandler(this.OnPackChanged);
            // 
            // FormNewPack
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbCylLayoutType);
            this.Controls.Add(this.lbCylLayoutType);
            this.Controls.Add(this.tabCtrl);
            this.Controls.Add(this.uCtrlLayout);
            this.Controls.Add(this.uCtrlOuterDimensions);
            this.Controls.Add(this.cbDir);
            this.Controls.Add(this.lbDir);
            this.Controls.Add(this.cbInnerPackable);
            this.Controls.Add(this.lbContent);
            this.Controls.Add(this.graphCtrl);
            this.Name = "FormNewPack";
            this.Controls.SetChildIndex(this.bnOk, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.lbName, 0);
            this.Controls.SetChildIndex(this.lbDescription, 0);
            this.Controls.SetChildIndex(this.tbName, 0);
            this.Controls.SetChildIndex(this.tbDescription, 0);
            this.Controls.SetChildIndex(this.graphCtrl, 0);
            this.Controls.SetChildIndex(this.lbContent, 0);
            this.Controls.SetChildIndex(this.cbInnerPackable, 0);
            this.Controls.SetChildIndex(this.lbDir, 0);
            this.Controls.SetChildIndex(this.cbDir, 0);
            this.Controls.SetChildIndex(this.uCtrlOuterDimensions, 0);
            this.Controls.SetChildIndex(this.uCtrlLayout, 0);
            this.Controls.SetChildIndex(this.tabCtrl, 0);
            this.Controls.SetChildIndex(this.lbCylLayoutType, 0);
            this.Controls.SetChildIndex(this.cbCylLayoutType, 0);
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrl)).EndInit();
            this.tabCtrl.ResumeLayout(false);
            this.tabWrapper.ResumeLayout(false);
            this.tabWrapper.PerformLayout();
            this.tabTray.ResumeLayout(false);
            this.tabTray.PerformLayout();
            this.tabStrappers.ResumeLayout(false);
            this.tabPageBulge.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Graphics.Graphics3DControl graphCtrl;
        private System.Windows.Forms.Label lbContent;
        private treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered cbInnerPackable;
        private System.Windows.Forms.Label lbDir;
        private System.Windows.Forms.ComboBox cbDir;
        private treeDiM.Basics.UCtrlOptTriDouble uCtrlOuterDimensions;
        private treeDiM.Basics.UCtrlTriInt uCtrlLayout;
        private treeDiM.Basics.UCtrlDouble uCtrlWrapperWeight;
        private treeDiM.Basics.UCtrlDouble uCtrlWrapperThickness;
        private System.Windows.Forms.Label lbWrapperColor;
        private OfficePickers.ColorPicker.ComboBoxColorPicker cbWrapperColor;
        private System.Windows.Forms.ComboBox cbType;
        private treeDiM.Basics.UCtrlTriInt uCtrlWrapperWalls;
        private System.Windows.Forms.TabControl tabCtrl;
        private System.Windows.Forms.TabPage tabWrapper;
        private System.Windows.Forms.TabPage tabStrappers;
        private Basics.Controls.CtrlStrapperSet ctrlStrapperSet;
        private System.Windows.Forms.TabPage tabTray;
        private System.Windows.Forms.CheckBox chkbTray;
        private OfficePickers.ColorPicker.ComboBoxColorPicker cbTrayColor;
        private treeDiM.Basics.UCtrlDouble uCtrlTrayHeight;
        private treeDiM.Basics.UCtrlDouble uCtrlTrayWeight;
        private System.Windows.Forms.CheckBox chkbWrapper;
        private treeDiM.Basics.UCtrlTriInt uCtrlTrayWalls;
        private treeDiM.Basics.UCtrlDouble uCtrlTrayUnitThickness;
        private System.Windows.Forms.Label lbCylLayoutType;
        private System.Windows.Forms.ComboBox cbCylLayoutType;
        private System.Windows.Forms.TabPage tabPageBulge;
        private treeDiM.Basics.UCtrlOptTriDouble uCtrlOptBulge;
    }
}