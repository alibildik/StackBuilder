
namespace treeDiM.StackBuilder.Desktop
{
    partial class FormNewAnalysisPalletsOnPallet
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
            this.label1 = new System.Windows.Forms.Label();
            this.rbHalf = new System.Windows.Forms.RadioButton();
            this.rbQuarter = new System.Windows.Forms.RadioButton();
            this.lbInputPallet1 = new System.Windows.Forms.Label();
            this.lbInputPallet2 = new System.Windows.Forms.Label();
            this.lbInputPallet3 = new System.Windows.Forms.Label();
            this.lbInputPallet4 = new System.Windows.Forms.Label();
            this.cbMasterSplit = new System.Windows.Forms.ComboBox();
            this.lbMasterPalletSplit = new System.Windows.Forms.Label();
            this.lbLoadedPalletOrientation = new System.Windows.Forms.Label();
            this.cbPalletOrientation = new System.Windows.Forms.ComboBox();
            this.cbDestinationPallet = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered();
            this.cbInputPallet4 = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered();
            this.cbInputPallet3 = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered();
            this.cbInputPallet2 = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered();
            this.cbInputPallet1 = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered();
            this.graphCtrl = new treeDiM.StackBuilder.Graphics.Graphics3DControl();
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrl)).BeginInit();
            this.SuspendLayout();
            // 
            // tbDescription
            // 
            this.tbDescription.Size = new System.Drawing.Size(791, 20);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(286, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Destination pallet";
            // 
            // rbHalf
            // 
            this.rbHalf.AutoSize = true;
            this.rbHalf.Location = new System.Drawing.Point(8, 68);
            this.rbHalf.Name = "rbHalf";
            this.rbHalf.Size = new System.Drawing.Size(111, 17);
            this.rbHalf.TabIndex = 14;
            this.rbHalf.TabStop = true;
            this.rbHalf.Text = "Load 2 half pallets";
            this.rbHalf.UseVisualStyleBackColor = true;
            this.rbHalf.CheckedChanged += new System.EventHandler(this.OnPalletLayoutChanged);
            // 
            // rbQuarter
            // 
            this.rbQuarter.AutoSize = true;
            this.rbQuarter.Location = new System.Drawing.Point(8, 91);
            this.rbQuarter.Name = "rbQuarter";
            this.rbQuarter.Size = new System.Drawing.Size(127, 17);
            this.rbQuarter.TabIndex = 15;
            this.rbQuarter.TabStop = true;
            this.rbQuarter.Text = "Load 4 quarter pallets";
            this.rbQuarter.UseVisualStyleBackColor = true;
            this.rbQuarter.CheckedChanged += new System.EventHandler(this.OnPalletLayoutChanged);
            // 
            // lbInputPallet1
            // 
            this.lbInputPallet1.AutoSize = true;
            this.lbInputPallet1.Location = new System.Drawing.Point(8, 130);
            this.lbInputPallet1.Name = "lbInputPallet1";
            this.lbInputPallet1.Size = new System.Drawing.Size(68, 13);
            this.lbInputPallet1.TabIndex = 16;
            this.lbInputPallet1.Text = "Input pallet 1";
            // 
            // lbInputPallet2
            // 
            this.lbInputPallet2.AutoSize = true;
            this.lbInputPallet2.Location = new System.Drawing.Point(8, 156);
            this.lbInputPallet2.Name = "lbInputPallet2";
            this.lbInputPallet2.Size = new System.Drawing.Size(68, 13);
            this.lbInputPallet2.TabIndex = 17;
            this.lbInputPallet2.Text = "Input pallet 2";
            // 
            // lbInputPallet3
            // 
            this.lbInputPallet3.AutoSize = true;
            this.lbInputPallet3.Location = new System.Drawing.Point(8, 182);
            this.lbInputPallet3.Name = "lbInputPallet3";
            this.lbInputPallet3.Size = new System.Drawing.Size(68, 13);
            this.lbInputPallet3.TabIndex = 18;
            this.lbInputPallet3.Text = "Input pallet 3";
            // 
            // lbInputPallet4
            // 
            this.lbInputPallet4.AutoSize = true;
            this.lbInputPallet4.Location = new System.Drawing.Point(8, 208);
            this.lbInputPallet4.Name = "lbInputPallet4";
            this.lbInputPallet4.Size = new System.Drawing.Size(68, 13);
            this.lbInputPallet4.TabIndex = 19;
            this.lbInputPallet4.Text = "Input pallet 4";
            // 
            // cbMasterSplit
            // 
            this.cbMasterSplit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMasterSplit.FormattingEnabled = true;
            this.cbMasterSplit.Items.AddRange(new object[] {
            "Horizontal",
            "Vertical"});
            this.cbMasterSplit.Location = new System.Drawing.Point(162, 259);
            this.cbMasterSplit.Name = "cbMasterSplit";
            this.cbMasterSplit.Size = new System.Drawing.Size(121, 21);
            this.cbMasterSplit.TabIndex = 26;
            this.cbMasterSplit.SelectedIndexChanged += new System.EventHandler(this.OnInputChanged);
            // 
            // lbMasterPalletSplit
            // 
            this.lbMasterPalletSplit.AutoSize = true;
            this.lbMasterPalletSplit.Location = new System.Drawing.Point(8, 262);
            this.lbMasterPalletSplit.Name = "lbMasterPalletSplit";
            this.lbMasterPalletSplit.Size = new System.Drawing.Size(88, 13);
            this.lbMasterPalletSplit.TabIndex = 27;
            this.lbMasterPalletSplit.Text = "Master pallet split";
            // 
            // lbLoadedPalletOrientation
            // 
            this.lbLoadedPalletOrientation.AutoSize = true;
            this.lbLoadedPalletOrientation.Location = new System.Drawing.Point(8, 289);
            this.lbLoadedPalletOrientation.Name = "lbLoadedPalletOrientation";
            this.lbLoadedPalletOrientation.Size = new System.Drawing.Size(123, 13);
            this.lbLoadedPalletOrientation.TabIndex = 28;
            this.lbLoadedPalletOrientation.Text = "Loaded pallet orientation";
            // 
            // cbPalletOrientation
            // 
            this.cbPalletOrientation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPalletOrientation.FormattingEnabled = true;
            this.cbPalletOrientation.Items.AddRange(new object[] {
            "Normal",
            "Rotated"});
            this.cbPalletOrientation.Location = new System.Drawing.Point(162, 286);
            this.cbPalletOrientation.Name = "cbPalletOrientation";
            this.cbPalletOrientation.Size = new System.Drawing.Size(121, 21);
            this.cbPalletOrientation.TabIndex = 29;
            this.cbPalletOrientation.SelectedIndexChanged += new System.EventHandler(this.OnInputChanged);
            // 
            // cbDestinationPallet
            // 
            this.cbDestinationPallet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDestinationPallet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDestinationPallet.FormattingEnabled = true;
            this.cbDestinationPallet.Location = new System.Drawing.Point(549, 67);
            this.cbDestinationPallet.Name = "cbDestinationPallet";
            this.cbDestinationPallet.Size = new System.Drawing.Size(138, 21);
            this.cbDestinationPallet.TabIndex = 25;
            this.cbDestinationPallet.SelectedIndexChanged += new System.EventHandler(this.OnInputChanged);
            // 
            // cbInputPallet4
            // 
            this.cbInputPallet4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInputPallet4.FormattingEnabled = true;
            this.cbInputPallet4.Location = new System.Drawing.Point(125, 205);
            this.cbInputPallet4.Name = "cbInputPallet4";
            this.cbInputPallet4.Size = new System.Drawing.Size(158, 21);
            this.cbInputPallet4.TabIndex = 24;
            this.cbInputPallet4.SelectedIndexChanged += new System.EventHandler(this.OnLoadedPalletChanged);
            // 
            // cbInputPallet3
            // 
            this.cbInputPallet3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInputPallet3.FormattingEnabled = true;
            this.cbInputPallet3.Location = new System.Drawing.Point(125, 179);
            this.cbInputPallet3.Name = "cbInputPallet3";
            this.cbInputPallet3.Size = new System.Drawing.Size(158, 21);
            this.cbInputPallet3.TabIndex = 23;
            this.cbInputPallet3.SelectedIndexChanged += new System.EventHandler(this.OnLoadedPalletChanged);
            // 
            // cbInputPallet2
            // 
            this.cbInputPallet2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInputPallet2.FormattingEnabled = true;
            this.cbInputPallet2.Location = new System.Drawing.Point(125, 153);
            this.cbInputPallet2.Name = "cbInputPallet2";
            this.cbInputPallet2.Size = new System.Drawing.Size(158, 21);
            this.cbInputPallet2.TabIndex = 22;
            this.cbInputPallet2.SelectedIndexChanged += new System.EventHandler(this.OnLoadedPalletChanged);
            // 
            // cbInputPallet1
            // 
            this.cbInputPallet1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInputPallet1.FormattingEnabled = true;
            this.cbInputPallet1.Location = new System.Drawing.Point(125, 127);
            this.cbInputPallet1.Name = "cbInputPallet1";
            this.cbInputPallet1.Size = new System.Drawing.Size(158, 21);
            this.cbInputPallet1.TabIndex = 21;
            this.cbInputPallet1.SelectedIndexChanged += new System.EventHandler(this.OnLoadedPalletChanged);
            // 
            // graphCtrl
            // 
            this.graphCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.graphCtrl.AngleHoriz = 45D;
            this.graphCtrl.Location = new System.Drawing.Point(289, 125);
            this.graphCtrl.Name = "graphCtrl";
            this.graphCtrl.Size = new System.Drawing.Size(483, 383);
            this.graphCtrl.TabIndex = 20;
            this.graphCtrl.Viewer = null;
            // 
            // FormNewAnalysisPalletsOnPallet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.cbPalletOrientation);
            this.Controls.Add(this.lbLoadedPalletOrientation);
            this.Controls.Add(this.lbMasterPalletSplit);
            this.Controls.Add(this.cbMasterSplit);
            this.Controls.Add(this.cbDestinationPallet);
            this.Controls.Add(this.cbInputPallet4);
            this.Controls.Add(this.cbInputPallet3);
            this.Controls.Add(this.cbInputPallet2);
            this.Controls.Add(this.cbInputPallet1);
            this.Controls.Add(this.graphCtrl);
            this.Controls.Add(this.lbInputPallet4);
            this.Controls.Add(this.lbInputPallet3);
            this.Controls.Add(this.lbInputPallet2);
            this.Controls.Add(this.lbInputPallet1);
            this.Controls.Add(this.rbQuarter);
            this.Controls.Add(this.rbHalf);
            this.Controls.Add(this.label1);
            this.Name = "FormNewAnalysisPalletsOnPallet";
            this.Text = "Pallets on pallet analysis...";
            this.Controls.SetChildIndex(this.lbName, 0);
            this.Controls.SetChildIndex(this.lbDescription, 0);
            this.Controls.SetChildIndex(this.tbName, 0);
            this.Controls.SetChildIndex(this.tbDescription, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.rbHalf, 0);
            this.Controls.SetChildIndex(this.rbQuarter, 0);
            this.Controls.SetChildIndex(this.lbInputPallet1, 0);
            this.Controls.SetChildIndex(this.lbInputPallet2, 0);
            this.Controls.SetChildIndex(this.lbInputPallet3, 0);
            this.Controls.SetChildIndex(this.lbInputPallet4, 0);
            this.Controls.SetChildIndex(this.graphCtrl, 0);
            this.Controls.SetChildIndex(this.cbInputPallet1, 0);
            this.Controls.SetChildIndex(this.cbInputPallet2, 0);
            this.Controls.SetChildIndex(this.cbInputPallet3, 0);
            this.Controls.SetChildIndex(this.cbInputPallet4, 0);
            this.Controls.SetChildIndex(this.cbDestinationPallet, 0);
            this.Controls.SetChildIndex(this.cbMasterSplit, 0);
            this.Controls.SetChildIndex(this.lbMasterPalletSplit, 0);
            this.Controls.SetChildIndex(this.lbLoadedPalletOrientation, 0);
            this.Controls.SetChildIndex(this.cbPalletOrientation, 0);
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbHalf;
        private System.Windows.Forms.RadioButton rbQuarter;
        private System.Windows.Forms.Label lbInputPallet1;
        private System.Windows.Forms.Label lbInputPallet2;
        private System.Windows.Forms.Label lbInputPallet3;
        private System.Windows.Forms.Label lbInputPallet4;
        private Graphics.Graphics3DControl graphCtrl;
        private treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered cbInputPallet1;
        private treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered cbInputPallet2;
        private treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered cbInputPallet3;
        private treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered cbInputPallet4;
        private treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered cbDestinationPallet;
        private System.Windows.Forms.ComboBox cbMasterSplit;
        private System.Windows.Forms.Label lbMasterPalletSplit;
        private System.Windows.Forms.Label lbLoadedPalletOrientation;
        private System.Windows.Forms.ComboBox cbPalletOrientation;
    }
}