
namespace treeDiM.StackBuilder.BestCaseFinder
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.labelCasesLoaded = new System.Windows.Forms.Label();
            this.lbItemsLoaded = new System.Windows.Forms.Label();
            this.fsCrates = new treeDiM.UserControls.FileSelect();
            this.lbCaseFile = new System.Windows.Forms.Label();
            this.fsItems = new treeDiM.UserControls.FileSelect();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.uCtrlNumber = new treeDiM.Basics.UCtrlOptInt();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lbFilterByName = new System.Windows.Forms.Label();
            this.chkbFilterOutProcessedItems = new System.Windows.Forms.CheckBox();
            this.cbItem = new System.Windows.Forms.ComboBox();
            this.lbItem = new System.Windows.Forms.Label();
            this.bnSelect = new System.Windows.Forms.Button();
            this.uCtrlCaseOrientation1 = new treeDiM.StackBuilder.Graphics.UCtrlCaseOrientation();
            this.gridSolutions = new SourceGrid.Grid();
            this.pbSolution = new System.Windows.Forms.PictureBox();
            this.lbSelectedCase = new System.Windows.Forms.Label();
            this.lbNumberOfItems = new System.Windows.Forms.Label();
            this.lbEfficiency = new System.Windows.Forms.Label();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSolution)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 529);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(841, 22);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(39, 17);
            this.statusLabel.Text = "Ready";
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.labelCasesLoaded);
            this.splitContainerMain.Panel1.Controls.Add(this.lbItemsLoaded);
            this.splitContainerMain.Panel1.Controls.Add(this.fsCrates);
            this.splitContainerMain.Panel1.Controls.Add(this.lbCaseFile);
            this.splitContainerMain.Panel1.Controls.Add(this.fsItems);
            this.splitContainerMain.Panel1.Controls.Add(this.label1);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainerMain.Size = new System.Drawing.Size(841, 529);
            this.splitContainerMain.SplitterDistance = 120;
            this.splitContainerMain.TabIndex = 1;
            // 
            // labelCasesLoaded
            // 
            this.labelCasesLoaded.AutoSize = true;
            this.labelCasesLoaded.Location = new System.Drawing.Point(650, 43);
            this.labelCasesLoaded.Name = "labelCasesLoaded";
            this.labelCasesLoaded.Size = new System.Drawing.Size(76, 13);
            this.labelCasesLoaded.TabIndex = 5;
            this.labelCasesLoaded.Text = "(cases loaded)";
            // 
            // lbItemsLoaded
            // 
            this.lbItemsLoaded.AutoSize = true;
            this.lbItemsLoaded.Location = new System.Drawing.Point(650, 16);
            this.lbItemsLoaded.Name = "lbItemsLoaded";
            this.lbItemsLoaded.Size = new System.Drawing.Size(72, 13);
            this.lbItemsLoaded.TabIndex = 4;
            this.lbItemsLoaded.Text = "(items loaded)";
            // 
            // fsCrates
            // 
            this.fsCrates.Location = new System.Drawing.Point(110, 43);
            this.fsCrates.Name = "fsCrates";
            this.fsCrates.Size = new System.Drawing.Size(534, 20);
            this.fsCrates.TabIndex = 3;
            // 
            // lbCaseFile
            // 
            this.lbCaseFile.AutoSize = true;
            this.lbCaseFile.Location = new System.Drawing.Point(12, 43);
            this.lbCaseFile.Name = "lbCaseFile";
            this.lbCaseFile.Size = new System.Drawing.Size(47, 13);
            this.lbCaseFile.TabIndex = 2;
            this.lbCaseFile.Text = "Case file";
            // 
            // fsItems
            // 
            this.fsItems.Location = new System.Drawing.Point(110, 13);
            this.fsItems.Name = "fsItems";
            this.fsItems.Size = new System.Drawing.Size(534, 20);
            this.fsItems.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Items file";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.uCtrlCaseOrientation1);
            this.splitContainer1.Panel1.Controls.Add(this.uCtrlNumber);
            this.splitContainer1.Panel1.Controls.Add(this.textBox1);
            this.splitContainer1.Panel1.Controls.Add(this.lbFilterByName);
            this.splitContainer1.Panel1.Controls.Add(this.chkbFilterOutProcessedItems);
            this.splitContainer1.Panel1.Controls.Add(this.cbItem);
            this.splitContainer1.Panel1.Controls.Add(this.lbItem);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lbEfficiency);
            this.splitContainer1.Panel2.Controls.Add(this.lbNumberOfItems);
            this.splitContainer1.Panel2.Controls.Add(this.lbSelectedCase);
            this.splitContainer1.Panel2.Controls.Add(this.pbSolution);
            this.splitContainer1.Panel2.Controls.Add(this.gridSolutions);
            this.splitContainer1.Panel2.Controls.Add(this.bnSelect);
            this.splitContainer1.Size = new System.Drawing.Size(841, 405);
            this.splitContainer1.SplitterDistance = 417;
            this.splitContainer1.TabIndex = 0;
            // 
            // uCtrlNumber
            // 
            this.uCtrlNumber.Location = new System.Drawing.Point(15, 96);
            this.uCtrlNumber.Minimum = -10000;
            this.uCtrlNumber.MinimumSize = new System.Drawing.Size(100, 20);
            this.uCtrlNumber.Name = "uCtrlNumber";
            this.uCtrlNumber.Size = new System.Drawing.Size(207, 20);
            this.uCtrlNumber.TabIndex = 5;
            this.uCtrlNumber.Text = "Max. number";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(122, 42);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 4;
            // 
            // lbFilterByName
            // 
            this.lbFilterByName.AutoSize = true;
            this.lbFilterByName.Location = new System.Drawing.Point(12, 45);
            this.lbFilterByName.Name = "lbFilterByName";
            this.lbFilterByName.Size = new System.Drawing.Size(72, 13);
            this.lbFilterByName.TabIndex = 3;
            this.lbFilterByName.Text = "Filter by name";
            // 
            // chkbFilterOutProcessedItems
            // 
            this.chkbFilterOutProcessedItems.AutoSize = true;
            this.chkbFilterOutProcessedItems.Location = new System.Drawing.Point(15, 19);
            this.chkbFilterOutProcessedItems.Name = "chkbFilterOutProcessedItems";
            this.chkbFilterOutProcessedItems.Size = new System.Drawing.Size(182, 17);
            this.chkbFilterOutProcessedItems.TabIndex = 2;
            this.chkbFilterOutProcessedItems.Text = "Filter out already processed items";
            this.chkbFilterOutProcessedItems.UseVisualStyleBackColor = true;
            // 
            // cbItem
            // 
            this.cbItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbItem.FormattingEnabled = true;
            this.cbItem.Location = new System.Drawing.Point(122, 68);
            this.cbItem.Name = "cbItem";
            this.cbItem.Size = new System.Drawing.Size(258, 21);
            this.cbItem.TabIndex = 1;
            // 
            // lbItem
            // 
            this.lbItem.AutoSize = true;
            this.lbItem.Location = new System.Drawing.Point(12, 72);
            this.lbItem.Name = "lbItem";
            this.lbItem.Size = new System.Drawing.Size(27, 13);
            this.lbItem.TabIndex = 0;
            this.lbItem.Text = "Item";
            // 
            // bnSelect
            // 
            this.bnSelect.Location = new System.Drawing.Point(342, 378);
            this.bnSelect.Name = "bnSelect";
            this.bnSelect.Size = new System.Drawing.Size(75, 23);
            this.bnSelect.TabIndex = 0;
            this.bnSelect.Text = "Select solution";
            this.bnSelect.UseVisualStyleBackColor = true;
            // 
            // uCtrlCaseOrientation1
            // 
            this.uCtrlCaseOrientation1.AllowedOrientations = new bool[] {
        false,
        false,
        true};
            this.uCtrlCaseOrientation1.Location = new System.Drawing.Point(15, 180);
            this.uCtrlCaseOrientation1.Name = "uCtrlCaseOrientation1";
            this.uCtrlCaseOrientation1.Size = new System.Drawing.Size(280, 110);
            this.uCtrlCaseOrientation1.TabIndex = 6;
            // 
            // gridSolutions
            // 
            this.gridSolutions.EnableSort = true;
            this.gridSolutions.Location = new System.Drawing.Point(3, 3);
            this.gridSolutions.Name = "gridSolutions";
            this.gridSolutions.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridSolutions.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.gridSolutions.Size = new System.Drawing.Size(414, 168);
            this.gridSolutions.TabIndex = 1;
            this.gridSolutions.TabStop = true;
            this.gridSolutions.ToolTipText = "";
            // 
            // pbSolution
            // 
            this.pbSolution.Location = new System.Drawing.Point(194, 180);
            this.pbSolution.Name = "pbSolution";
            this.pbSolution.Size = new System.Drawing.Size(214, 192);
            this.pbSolution.TabIndex = 2;
            this.pbSolution.TabStop = false;
            // 
            // lbSelectedCase
            // 
            this.lbSelectedCase.AutoSize = true;
            this.lbSelectedCase.Location = new System.Drawing.Point(3, 192);
            this.lbSelectedCase.Name = "lbSelectedCase";
            this.lbSelectedCase.Size = new System.Drawing.Size(31, 13);
            this.lbSelectedCase.TabIndex = 3;
            this.lbSelectedCase.Text = "Case";
            // 
            // lbNumberOfItems
            // 
            this.lbNumberOfItems.AutoSize = true;
            this.lbNumberOfItems.Location = new System.Drawing.Point(3, 215);
            this.lbNumberOfItems.Name = "lbNumberOfItems";
            this.lbNumberOfItems.Size = new System.Drawing.Size(83, 13);
            this.lbNumberOfItems.TabIndex = 4;
            this.lbNumberOfItems.Text = "Number of items";
            // 
            // lbEfficiency
            // 
            this.lbEfficiency.AutoSize = true;
            this.lbEfficiency.Location = new System.Drawing.Point(3, 241);
            this.lbEfficiency.Name = "lbEfficiency";
            this.lbEfficiency.Size = new System.Drawing.Size(53, 13);
            this.lbEfficiency.TabIndex = 5;
            this.lbEfficiency.Text = "Efficiency";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 551);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "BelgianTrain Best Crate Finder";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel1.PerformLayout();
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbSolution)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Label labelCasesLoaded;
        private System.Windows.Forms.Label lbItemsLoaded;
        private UserControls.FileSelect fsCrates;
        private System.Windows.Forms.Label lbCaseFile;
        private UserControls.FileSelect fsItems;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Basics.UCtrlOptInt uCtrlNumber;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lbFilterByName;
        private System.Windows.Forms.CheckBox chkbFilterOutProcessedItems;
        private System.Windows.Forms.ComboBox cbItem;
        private System.Windows.Forms.Label lbItem;
        private System.Windows.Forms.Button bnSelect;
        private Graphics.UCtrlCaseOrientation uCtrlCaseOrientation1;
        private System.Windows.Forms.Label lbEfficiency;
        private System.Windows.Forms.Label lbNumberOfItems;
        private System.Windows.Forms.Label lbSelectedCase;
        private System.Windows.Forms.PictureBox pbSolution;
        private SourceGrid.Grid gridSolutions;
    }
}

