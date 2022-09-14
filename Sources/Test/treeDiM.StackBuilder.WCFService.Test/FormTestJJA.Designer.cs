namespace treeDiM.StackBuilder.WCFService.Test
{
    partial class FormTestJJA
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
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.lbInputDataFile = new System.Windows.Forms.Label();
            this.tbFilePath = new System.Windows.Forms.TextBox();
            this.bnOpenFile = new System.Windows.Forms.Button();
            this.bnLoad1 = new System.Windows.Forms.Button();
            this.splitContainerHoriz = new System.Windows.Forms.SplitContainer();
            this.bnLoad2 = new System.Windows.Forms.Button();
            this.bnEditInputFile = new System.Windows.Forms.Button();
            this.tabCtrlPalletContainer = new System.Windows.Forms.TabControl();
            this.tabPallets = new System.Windows.Forms.TabPage();
            this.gridPallets = new SourceGrid.Grid();
            this.tabContainers = new System.Windows.Forms.TabPage();
            this.gridContainers = new SourceGrid.Grid();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz)).BeginInit();
            this.splitContainerHoriz.Panel1.SuspendLayout();
            this.splitContainerHoriz.Panel2.SuspendLayout();
            this.splitContainerHoriz.SuspendLayout();
            this.tabCtrlPalletContainer.SuspendLayout();
            this.tabPallets.SuspendLayout();
            this.tabContainers.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "xml";
            this.openFileDialog.FileName = "JJA_inputData.xml";
            this.openFileDialog.Filter = "eXtended Mark Language (*.xml)|*.xml|All files (*.*)|*.*";
            this.openFileDialog.FilterIndex = 0;
            // 
            // lbInputDataFile
            // 
            this.lbInputDataFile.AutoSize = true;
            this.lbInputDataFile.Location = new System.Drawing.Point(8, 9);
            this.lbInputDataFile.Name = "lbInputDataFile";
            this.lbInputDataFile.Size = new System.Drawing.Size(71, 13);
            this.lbInputDataFile.TabIndex = 0;
            this.lbInputDataFile.Text = "Input data file";
            // 
            // tbFilePath
            // 
            this.tbFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFilePath.Location = new System.Drawing.Point(101, 6);
            this.tbFilePath.Name = "tbFilePath";
            this.tbFilePath.Size = new System.Drawing.Size(657, 20);
            this.tbFilePath.TabIndex = 1;
            this.tbFilePath.TextChanged += new System.EventHandler(this.OnFileChanged);
            // 
            // bnOpenFile
            // 
            this.bnOpenFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnOpenFile.Location = new System.Drawing.Point(764, 4);
            this.bnOpenFile.Name = "bnOpenFile";
            this.bnOpenFile.Size = new System.Drawing.Size(30, 23);
            this.bnOpenFile.TabIndex = 2;
            this.bnOpenFile.Text = "...";
            this.bnOpenFile.UseVisualStyleBackColor = true;
            this.bnOpenFile.Click += new System.EventHandler(this.OnSelectInputDataFile);
            // 
            // bnLoad1
            // 
            this.bnLoad1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnLoad1.Location = new System.Drawing.Point(356, 33);
            this.bnLoad1.Name = "bnLoad1";
            this.bnLoad1.Size = new System.Drawing.Size(200, 23);
            this.bnLoad1.TabIndex = 3;
            this.bnLoad1.Text = "Call WS multiple times (with images)";
            this.bnLoad1.UseVisualStyleBackColor = true;
            this.bnLoad1.Click += new System.EventHandler(this.OnLoadMultipleCalls);
            // 
            // splitContainerHoriz
            // 
            this.splitContainerHoriz.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerHoriz.Location = new System.Drawing.Point(0, 0);
            this.splitContainerHoriz.Name = "splitContainerHoriz";
            this.splitContainerHoriz.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerHoriz.Panel1
            // 
            this.splitContainerHoriz.Panel1.Controls.Add(this.bnLoad2);
            this.splitContainerHoriz.Panel1.Controls.Add(this.bnEditInputFile);
            this.splitContainerHoriz.Panel1.Controls.Add(this.tbFilePath);
            this.splitContainerHoriz.Panel1.Controls.Add(this.bnLoad1);
            this.splitContainerHoriz.Panel1.Controls.Add(this.lbInputDataFile);
            this.splitContainerHoriz.Panel1.Controls.Add(this.bnOpenFile);
            // 
            // splitContainerHoriz.Panel2
            // 
            this.splitContainerHoriz.Panel2.Controls.Add(this.tabCtrlPalletContainer);
            this.splitContainerHoriz.Size = new System.Drawing.Size(800, 450);
            this.splitContainerHoriz.SplitterDistance = 65;
            this.splitContainerHoriz.TabIndex = 4;
            // 
            // bnLoad2
            // 
            this.bnLoad2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnLoad2.Location = new System.Drawing.Point(573, 33);
            this.bnLoad2.Name = "bnLoad2";
            this.bnLoad2.Size = new System.Drawing.Size(200, 23);
            this.bnLoad2.TabIndex = 5;
            this.bnLoad2.Text = "Call WS once";
            this.bnLoad2.UseVisualStyleBackColor = true;
            this.bnLoad2.Click += new System.EventHandler(this.OnLoadSingleCall);
            // 
            // bnEditInputFile
            // 
            this.bnEditInputFile.Location = new System.Drawing.Point(100, 33);
            this.bnEditInputFile.Name = "bnEditInputFile";
            this.bnEditInputFile.Size = new System.Drawing.Size(117, 23);
            this.bnEditInputFile.TabIndex = 4;
            this.bnEditInputFile.Text = "Edit file";
            this.bnEditInputFile.UseVisualStyleBackColor = true;
            this.bnEditInputFile.Click += new System.EventHandler(this.OnEditFile);
            // 
            // tabCtrlPalletContainer
            // 
            this.tabCtrlPalletContainer.Controls.Add(this.tabPallets);
            this.tabCtrlPalletContainer.Controls.Add(this.tabContainers);
            this.tabCtrlPalletContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCtrlPalletContainer.Location = new System.Drawing.Point(0, 0);
            this.tabCtrlPalletContainer.Name = "tabCtrlPalletContainer";
            this.tabCtrlPalletContainer.SelectedIndex = 0;
            this.tabCtrlPalletContainer.Size = new System.Drawing.Size(800, 381);
            this.tabCtrlPalletContainer.TabIndex = 0;
            // 
            // tabPallets
            // 
            this.tabPallets.Controls.Add(this.gridPallets);
            this.tabPallets.Location = new System.Drawing.Point(4, 22);
            this.tabPallets.Name = "tabPallets";
            this.tabPallets.Padding = new System.Windows.Forms.Padding(3);
            this.tabPallets.Size = new System.Drawing.Size(792, 355);
            this.tabPallets.TabIndex = 0;
            this.tabPallets.Text = "Pallets";
            this.tabPallets.UseVisualStyleBackColor = true;
            // 
            // gridPallets
            // 
            this.gridPallets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPallets.EnableSort = true;
            this.gridPallets.Location = new System.Drawing.Point(3, 3);
            this.gridPallets.Name = "gridPallets";
            this.gridPallets.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridPallets.SelectionMode = SourceGrid.GridSelectionMode.Row;
            this.gridPallets.Size = new System.Drawing.Size(786, 349);
            this.gridPallets.TabIndex = 0;
            this.gridPallets.TabStop = true;
            this.gridPallets.ToolTipText = "";
            // 
            // tabContainers
            // 
            this.tabContainers.Controls.Add(this.gridContainers);
            this.tabContainers.Location = new System.Drawing.Point(4, 22);
            this.tabContainers.Name = "tabContainers";
            this.tabContainers.Padding = new System.Windows.Forms.Padding(3);
            this.tabContainers.Size = new System.Drawing.Size(792, 355);
            this.tabContainers.TabIndex = 1;
            this.tabContainers.Text = "Containers";
            this.tabContainers.UseVisualStyleBackColor = true;
            // 
            // gridContainers
            // 
            this.gridContainers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridContainers.EnableSort = true;
            this.gridContainers.Location = new System.Drawing.Point(3, 3);
            this.gridContainers.Name = "gridContainers";
            this.gridContainers.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridContainers.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.gridContainers.Size = new System.Drawing.Size(786, 349);
            this.gridContainers.TabIndex = 0;
            this.gridContainers.TabStop = true;
            this.gridContainers.ToolTipText = "";
            // 
            // FormTestJJA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainerHoriz);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormTestJJA";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TabText = "Test JJA methods...";
            this.Text = "Test JJA methods...";
            this.splitContainerHoriz.Panel1.ResumeLayout(false);
            this.splitContainerHoriz.Panel1.PerformLayout();
            this.splitContainerHoriz.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz)).EndInit();
            this.splitContainerHoriz.ResumeLayout(false);
            this.tabCtrlPalletContainer.ResumeLayout(false);
            this.tabPallets.ResumeLayout(false);
            this.tabContainers.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label lbInputDataFile;
        private System.Windows.Forms.TextBox tbFilePath;
        private System.Windows.Forms.Button bnOpenFile;
        private System.Windows.Forms.Button bnLoad1;
        private System.Windows.Forms.SplitContainer splitContainerHoriz;
        private System.Windows.Forms.TabControl tabCtrlPalletContainer;
        private System.Windows.Forms.TabPage tabPallets;
        private SourceGrid.Grid gridPallets;
        private System.Windows.Forms.TabPage tabContainers;
        private SourceGrid.Grid gridContainers;
        private System.Windows.Forms.Button bnEditInputFile;
        private System.Windows.Forms.Button bnLoad2;
    }
}