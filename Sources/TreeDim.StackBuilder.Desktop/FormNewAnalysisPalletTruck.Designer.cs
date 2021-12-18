namespace treeDiM.StackBuilder.Desktop
{
    partial class FormNewAnalysisPalletTruck
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNewAnalysisPalletTruck));
            this.lbPallets = new System.Windows.Forms.Label();
            this.lbTrucks = new System.Windows.Forms.Label();
            this.checkBoxBestLayersOnly = new System.Windows.Forms.CheckBox();
            this.chkbAllowMultipleLayers = new System.Windows.Forms.CheckBox();
            this.uCtrlMinDistanceLoadWall = new treeDiM.Basics.UCtrlDualDouble();
            this.uCtrlMinDistanceLoadRoof = new treeDiM.Basics.UCtrlDouble();
            this.uCtrlLayerList = new treeDiM.StackBuilder.Graphics.UCtrlLayerList();
            this.cbTrucks = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered();
            this.cbPallets = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered();
            this.uCtrlMaxNoPallets = new treeDiM.Basics.UCtrlOptInt();
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
            // lbPallets
            // 
            resources.ApplyResources(this.lbPallets, "lbPallets");
            this.lbPallets.Name = "lbPallets";
            // 
            // lbTrucks
            // 
            resources.ApplyResources(this.lbTrucks, "lbTrucks");
            this.lbTrucks.Name = "lbTrucks";
            // 
            // checkBoxBestLayersOnly
            // 
            resources.ApplyResources(this.checkBoxBestLayersOnly, "checkBoxBestLayersOnly");
            this.checkBoxBestLayersOnly.Name = "checkBoxBestLayersOnly";
            this.checkBoxBestLayersOnly.UseVisualStyleBackColor = true;
            this.checkBoxBestLayersOnly.CheckedChanged += new System.EventHandler(this.OnInputChanged);
            // 
            // chkbAllowMultipleLayers
            // 
            resources.ApplyResources(this.chkbAllowMultipleLayers, "chkbAllowMultipleLayers");
            this.chkbAllowMultipleLayers.Name = "chkbAllowMultipleLayers";
            this.chkbAllowMultipleLayers.UseVisualStyleBackColor = true;
            this.chkbAllowMultipleLayers.CheckedChanged += new System.EventHandler(this.OnInputChanged);
            // 
            // uCtrlMinDistanceLoadWall
            // 
            resources.ApplyResources(this.uCtrlMinDistanceLoadWall, "uCtrlMinDistanceLoadWall");
            this.uCtrlMinDistanceLoadWall.MinValue = 0D;
            this.uCtrlMinDistanceLoadWall.Name = "uCtrlMinDistanceLoadWall";
            this.uCtrlMinDistanceLoadWall.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlMinDistanceLoadWall.ValueX = 0D;
            this.uCtrlMinDistanceLoadWall.ValueY = 0D;
            this.uCtrlMinDistanceLoadWall.ValueChanged += new treeDiM.Basics.UCtrlDualDouble.ValueChangedDelegate(this.OnInputChanged);
            // 
            // uCtrlMinDistanceLoadRoof
            // 
            resources.ApplyResources(this.uCtrlMinDistanceLoadRoof, "uCtrlMinDistanceLoadRoof");
            this.uCtrlMinDistanceLoadRoof.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.uCtrlMinDistanceLoadRoof.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.uCtrlMinDistanceLoadRoof.Name = "uCtrlMinDistanceLoadRoof";
            this.uCtrlMinDistanceLoadRoof.Unit = treeDiM.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlMinDistanceLoadRoof.ValueChanged += new treeDiM.Basics.UCtrlDouble.ValueChangedDelegate(this.OnInputChanged);
            // 
            // uCtrlLayerList
            // 
            resources.ApplyResources(this.uCtrlLayerList, "uCtrlLayerList");
            this.uCtrlLayerList.ButtonSizes = new System.Drawing.Size(150, 150);
            this.uCtrlLayerList.Name = "uCtrlLayerList";
            this.uCtrlLayerList.Show3D = true;
            this.uCtrlLayerList.SingleSelection = false;
            // 
            // cbTrucks
            // 
            resources.ApplyResources(this.cbTrucks, "cbTrucks");
            this.cbTrucks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTrucks.FormattingEnabled = true;
            this.cbTrucks.Name = "cbTrucks";
            this.cbTrucks.SelectedIndexChanged += new System.EventHandler(this.OnInputChanged);
            // 
            // cbPallets
            // 
            resources.ApplyResources(this.cbPallets, "cbPallets");
            this.cbPallets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPallets.FormattingEnabled = true;
            this.cbPallets.Name = "cbPallets";
            this.cbPallets.SelectedIndexChanged += new System.EventHandler(this.OnInputChanged);
            // 
            // uCtrlMaxNoPallets
            // 
            resources.ApplyResources(this.uCtrlMaxNoPallets, "uCtrlMaxNoPallets");
            this.uCtrlMaxNoPallets.Minimum = 0;
            this.uCtrlMaxNoPallets.Name = "uCtrlMaxNoPallets";
            this.uCtrlMaxNoPallets.ValueChanged += new treeDiM.Basics.UCtrlOptInt.ValueChangedDelegate(this.OnInputChanged);
            // 
            // FormNewAnalysisPalletTruck
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uCtrlMaxNoPallets);
            this.Controls.Add(this.chkbAllowMultipleLayers);
            this.Controls.Add(this.uCtrlMinDistanceLoadWall);
            this.Controls.Add(this.uCtrlMinDistanceLoadRoof);
            this.Controls.Add(this.checkBoxBestLayersOnly);
            this.Controls.Add(this.uCtrlLayerList);
            this.Controls.Add(this.lbTrucks);
            this.Controls.Add(this.lbPallets);
            this.Controls.Add(this.cbTrucks);
            this.Controls.Add(this.cbPallets);
            this.Name = "FormNewAnalysisPalletTruck";
            this.Controls.SetChildIndex(this.lbName, 0);
            this.Controls.SetChildIndex(this.lbDescription, 0);
            this.Controls.SetChildIndex(this.tbName, 0);
            this.Controls.SetChildIndex(this.tbDescription, 0);
            this.Controls.SetChildIndex(this.cbPallets, 0);
            this.Controls.SetChildIndex(this.cbTrucks, 0);
            this.Controls.SetChildIndex(this.lbPallets, 0);
            this.Controls.SetChildIndex(this.lbTrucks, 0);
            this.Controls.SetChildIndex(this.uCtrlLayerList, 0);
            this.Controls.SetChildIndex(this.checkBoxBestLayersOnly, 0);
            this.Controls.SetChildIndex(this.uCtrlMinDistanceLoadRoof, 0);
            this.Controls.SetChildIndex(this.uCtrlMinDistanceLoadWall, 0);
            this.Controls.SetChildIndex(this.chkbAllowMultipleLayers, 0);
            this.Controls.SetChildIndex(this.uCtrlMaxNoPallets, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Graphics.Controls.CCtrlComboFiltered cbPallets;
        private Graphics.Controls.CCtrlComboFiltered cbTrucks;
        private System.Windows.Forms.Label lbPallets;
        private System.Windows.Forms.Label lbTrucks;
        private Graphics.UCtrlLayerList uCtrlLayerList;
        private System.Windows.Forms.CheckBox checkBoxBestLayersOnly;
        private treeDiM.Basics.UCtrlDouble uCtrlMinDistanceLoadRoof;
        private treeDiM.Basics.UCtrlDualDouble uCtrlMinDistanceLoadWall;
        private System.Windows.Forms.CheckBox chkbAllowMultipleLayers;
        private treeDiM.Basics.UCtrlOptInt uCtrlMaxNoPallets;
    }
}