
namespace treeDiM.StackBuilder.Desktop
{
    partial class OptionPanelRobot
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionPanelRobot));
            this.pbExporterBrand = new System.Windows.Forms.PictureBox();
            this.cbExporter = new System.Windows.Forms.ComboBox();
            this.lbExporters = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbExporterBrand)).BeginInit();
            this.SuspendLayout();
            // 
            // pbExporterBrand
            // 
            resources.ApplyResources(this.pbExporterBrand, "pbExporterBrand");
            this.pbExporterBrand.Name = "pbExporterBrand";
            this.pbExporterBrand.TabStop = false;
            // 
            // cbExporter
            // 
            this.cbExporter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbExporter.FormattingEnabled = true;
            resources.ApplyResources(this.cbExporter, "cbExporter");
            this.cbExporter.Name = "cbExporter";
            this.cbExporter.SelectedIndexChanged += new System.EventHandler(this.OnLoadedExporterChanged);
            // 
            // lbExporters
            // 
            resources.ApplyResources(this.lbExporters, "lbExporters");
            this.lbExporters.Name = "lbExporters";
            // 
            // OptionPanelRobot
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CategoryPath = "Options\\\\Robot settings";
            this.Controls.Add(this.pbExporterBrand);
            this.Controls.Add(this.cbExporter);
            this.Controls.Add(this.lbExporters);
            this.DisplayName = "Robot";
            this.Name = "OptionPanelRobot";
            ((System.ComponentModel.ISupportInitialize)(this.pbExporterBrand)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbExporters;
        private System.Windows.Forms.ComboBox cbExporter;
        private System.Windows.Forms.PictureBox pbExporterBrand;
    }
}
