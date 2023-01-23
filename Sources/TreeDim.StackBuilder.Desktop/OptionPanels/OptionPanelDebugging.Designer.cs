namespace treeDiM.StackBuilder.Desktop
{
    partial class OptionPanelDebugging
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionPanelDebugging));
            this.bnShowAppFolder = new System.Windows.Forms.Button();
            this.bnResetDefaultSettings = new System.Windows.Forms.Button();
            this.chkbDisconnected = new System.Windows.Forms.CheckBox();
            this.chkbShowStartPage = new System.Windows.Forms.CheckBox();
            this.chkShowLogConsole = new System.Windows.Forms.CheckBox();
            this.chkbShowBoxNumbers = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // bnShowAppFolder
            // 
            resources.ApplyResources(this.bnShowAppFolder, "bnShowAppFolder");
            this.bnShowAppFolder.Name = "bnShowAppFolder";
            this.bnShowAppFolder.UseVisualStyleBackColor = true;
            this.bnShowAppFolder.Click += new System.EventHandler(this.OnShowApplicationFolder);
            // 
            // bnResetDefaultSettings
            // 
            resources.ApplyResources(this.bnResetDefaultSettings, "bnResetDefaultSettings");
            this.bnResetDefaultSettings.Name = "bnResetDefaultSettings";
            this.bnResetDefaultSettings.UseVisualStyleBackColor = true;
            this.bnResetDefaultSettings.Click += new System.EventHandler(this.OnResetDefaultSettings);
            // 
            // chkbDisconnected
            // 
            resources.ApplyResources(this.chkbDisconnected, "chkbDisconnected");
            this.chkbDisconnected.Checked = global::treeDiM.StackBuilder.Desktop.Properties.Settings.Default.AllowDisconnectedMode;
            this.chkbDisconnected.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::treeDiM.StackBuilder.Desktop.Properties.Settings.Default, "AllowDisconnectedMode", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkbDisconnected.Name = "chkbDisconnected";
            this.chkbDisconnected.UseVisualStyleBackColor = true;
            // 
            // chkbShowStartPage
            // 
            resources.ApplyResources(this.chkbShowStartPage, "chkbShowStartPage");
            this.chkbShowStartPage.Checked = global::treeDiM.StackBuilder.Desktop.Properties.Settings.Default.ShowStartPage;
            this.chkbShowStartPage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkbShowStartPage.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::treeDiM.StackBuilder.Desktop.Properties.Settings.Default, "ShowStartPage", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkbShowStartPage.Name = "chkbShowStartPage";
            this.chkbShowStartPage.UseVisualStyleBackColor = true;
            // 
            // chkShowLogConsole
            // 
            resources.ApplyResources(this.chkShowLogConsole, "chkShowLogConsole");
            this.chkShowLogConsole.Checked = global::treeDiM.StackBuilder.Desktop.Properties.Settings.Default.ShowLogConsole;
            this.chkShowLogConsole.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::treeDiM.StackBuilder.Desktop.Properties.Settings.Default, "ShowLogConsole", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkShowLogConsole.Name = "chkShowLogConsole";
            this.chkShowLogConsole.UseVisualStyleBackColor = true;
            this.chkShowLogConsole.CheckedChanged += new System.EventHandler(this.OnShowLogConsoleCheckChanged);
            // 
            // chkbShowBoxNumbers
            // 
            resources.ApplyResources(this.chkbShowBoxNumbers, "chkbShowBoxNumbers");
            this.chkbShowBoxNumbers.Name = "chkbShowBoxNumbers";
            this.chkbShowBoxNumbers.UseVisualStyleBackColor = true;
            // 
            // OptionPanelDebugging
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CategoryPath = "Options\\\\Debugging";
            this.Controls.Add(this.chkbShowBoxNumbers);
            this.Controls.Add(this.bnResetDefaultSettings);
            this.Controls.Add(this.chkbDisconnected);
            this.Controls.Add(this.chkbShowStartPage);
            this.Controls.Add(this.bnShowAppFolder);
            this.Controls.Add(this.chkShowLogConsole);
            this.DisplayName = "Debugging";
            this.Name = "OptionPanelDebugging";
            this.Click += new System.EventHandler(this.OnShowApplicationFolder);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkShowLogConsole;
        private System.Windows.Forms.Button bnShowAppFolder;
        private System.Windows.Forms.CheckBox chkbShowStartPage;
        private System.Windows.Forms.CheckBox chkbDisconnected;
        private System.Windows.Forms.Button bnResetDefaultSettings;
        private System.Windows.Forms.CheckBox chkbShowBoxNumbers;
    }
}
