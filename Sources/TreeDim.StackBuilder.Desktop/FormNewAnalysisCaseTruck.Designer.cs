namespace treeDiM.StackBuilder.Desktop
{
    partial class FormNewAnalysisCaseTruck
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNewAnalysisCaseTruck));
            this.cbCases = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered();
            this.lbBox = new System.Windows.Forms.Label();
            this.uCtrlCaseOrientation = new treeDiM.StackBuilder.Graphics.UCtrlCaseOrientation();
            this.lbSelect = new System.Windows.Forms.Label();
            this.uCtrlLayerList = new treeDiM.StackBuilder.Graphics.UCtrlLayerList();
            this.checkBoxBestLayersOnly = new System.Windows.Forms.CheckBox();
            this.cbTrucks = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered();
            this.lbTrucks = new System.Windows.Forms.Label();
            this.uCtrlMinDistanceLoadWall = new treeDiM.Basics.UCtrlDualDouble();
            this.uCtrlMinDistanceLoadRoof = new treeDiM.Basics.UCtrlDouble();
            this.SuspendLayout();
            // 
            // tbDescription
            // 
            resources.ApplyResources(this.tbDescription, "tbDescription");
            // 
            // tbName
            // 
            resources.ApplyResources(this.tbName, "tbName");
            // 
            // lbDescription
            // 
            resources.ApplyResources(this.lbDescription, "lbDescription");
            // 
            // lbName
            // 
            resources.ApplyResources(this.lbName, "lbName");
            // 
            // cbCases
            // 
            resources.ApplyResources(this.cbCases, "cbCases");
            this.cbCases.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCases.FormattingEnabled = true;
            this.cbCases.Name = "cbCases";
            this.cbCases.SelectedIndexChanged += new System.EventHandler(this.OnCaseChanged);
            // 
            // lbBox
            // 
            resources.ApplyResources(this.lbBox, "lbBox");
            this.lbBox.Name = "lbBox";
            // 
            // uCtrlCaseOrientation
            // 
            resources.ApplyResources(this.uCtrlCaseOrientation, "uCtrlCaseOrientation");
            this.uCtrlCaseOrientation.AllowedOrientations = new bool[] {
        false,
        false,
        true};
            this.uCtrlCaseOrientation.Name = "uCtrlCaseOrientation";
            this.uCtrlCaseOrientation.CheckedChanged += new treeDiM.StackBuilder.Graphics.UCtrlCaseOrientation.CheckChanged(this.OnInputChanged);
            // 
            // lbSelect
            // 
            resources.ApplyResources(this.lbSelect, "lbSelect");
            this.lbSelect.Name = "lbSelect";
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
            // cbTrucks
            // 
            resources.ApplyResources(this.cbTrucks, "cbTrucks");
            this.cbTrucks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTrucks.FormattingEnabled = true;
            this.cbTrucks.Name = "cbTrucks";
            this.cbTrucks.SelectedIndexChanged += new System.EventHandler(this.OnInputChanged);
            // 
            // lbTrucks
            // 
            resources.ApplyResources(this.lbTrucks, "lbTrucks");
            this.lbTrucks.Name = "lbTrucks";
            // 
            // uCtrlMinDistanceLoadWall
            // 
            resources.ApplyResources(this.uCtrlMinDistanceLoadWall, "uCtrlMinDistanceLoadWall");
            this.uCtrlMinDistanceLoadWall.MinValue = -10000D;
            this.uCtrlMinDistanceLoadWall.Name = "uCtrlMinDistanceLoadWall";
            this.uCtrlMinDistanceLoadWall.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlMinDistanceLoadWall.ValueX = 0D;
            this.uCtrlMinDistanceLoadWall.ValueY = 0D;
            this.uCtrlMinDistanceLoadWall.ValueChanged += new treeDiM.Basics.UCtrlDualDouble.ValueChangedDelegate(this.OnInputChanged);
            // 
            // uCtrlMinDistanceLoadRoof
            // 
            resources.ApplyResources(this.uCtrlMinDistanceLoadRoof, "uCtrlMinDistanceLoadRoof");
            this.uCtrlMinDistanceLoadRoof.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.uCtrlMinDistanceLoadRoof.Name = "uCtrlMinDistanceLoadRoof";
            this.uCtrlMinDistanceLoadRoof.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlMinDistanceLoadRoof.Value = 0D;
            this.uCtrlMinDistanceLoadRoof.ValueChanged += new treeDiM.Basics.UCtrlDouble.ValueChangedDelegate(this.OnInputChanged);
            // 
            // FormNewAnalysisCaseTruck
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uCtrlMinDistanceLoadRoof);
            this.Controls.Add(this.uCtrlMinDistanceLoadWall);
            this.Controls.Add(this.cbTrucks);
            this.Controls.Add(this.lbTrucks);
            this.Controls.Add(this.checkBoxBestLayersOnly);
            this.Controls.Add(this.uCtrlLayerList);
            this.Controls.Add(this.lbSelect);
            this.Controls.Add(this.uCtrlCaseOrientation);
            this.Controls.Add(this.cbCases);
            this.Controls.Add(this.lbBox);
            this.Name = "FormNewAnalysisCaseTruck";
            this.Controls.SetChildIndex(this.lbName, 0);
            this.Controls.SetChildIndex(this.lbDescription, 0);
            this.Controls.SetChildIndex(this.tbName, 0);
            this.Controls.SetChildIndex(this.tbDescription, 0);
            this.Controls.SetChildIndex(this.lbBox, 0);
            this.Controls.SetChildIndex(this.cbCases, 0);
            this.Controls.SetChildIndex(this.uCtrlCaseOrientation, 0);
            this.Controls.SetChildIndex(this.lbSelect, 0);
            this.Controls.SetChildIndex(this.uCtrlLayerList, 0);
            this.Controls.SetChildIndex(this.checkBoxBestLayersOnly, 0);
            this.Controls.SetChildIndex(this.lbTrucks, 0);
            this.Controls.SetChildIndex(this.cbTrucks, 0);
            this.Controls.SetChildIndex(this.uCtrlMinDistanceLoadWall, 0);
            this.Controls.SetChildIndex(this.uCtrlMinDistanceLoadRoof, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Graphics.Controls.CCtrlComboFiltered cbCases;
        private System.Windows.Forms.Label lbBox;
        private Graphics.UCtrlCaseOrientation uCtrlCaseOrientation;
        private System.Windows.Forms.Label lbSelect;
        private Graphics.UCtrlLayerList uCtrlLayerList;
        private System.Windows.Forms.CheckBox checkBoxBestLayersOnly;
        private Graphics.Controls.CCtrlComboFiltered cbTrucks;
        private System.Windows.Forms.Label lbTrucks;
        private treeDiM.Basics.UCtrlDualDouble uCtrlMinDistanceLoadWall;
        private treeDiM.Basics.UCtrlDouble uCtrlMinDistanceLoadRoof;
    }
}