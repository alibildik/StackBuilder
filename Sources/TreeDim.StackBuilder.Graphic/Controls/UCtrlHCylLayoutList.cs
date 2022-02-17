#region Using directives
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using log4net;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics.Controls
{
    public partial class UCtrlHCylLayoutList : UserControl
    {
        #region Constants
        private Size szButtons = new Size(150, 150);
        #endregion
        #region Delegate
        public delegate void LayoutButtonClicked(object sender, EventArgs e);
        public delegate void RefreshEnded(object sender, EventArgs e);
        #endregion
        #region Event handlers
        public event LayoutButtonClicked LayoutSelected;
        public event RefreshEnded RefreshFinished;
        #endregion
        #region Constructor
        public UCtrlHCylLayoutList()
        {
            InitializeComponent();
            AutoScroll = true;
            // set default thumbnail size from settings
            switch (Properties.Settings.Default.LayerViewThumbSizeIndex)
            {
                case 0: ButtonSizes = new Size(75, 75); break;
                case 1: ButtonSizes = new Size(100, 100); break;
                case 2: ButtonSizes = new Size(150, 150); break;
                case 3: ButtonSizes = new Size(200, 200); break;
                default: break;
            }
        }
        #endregion
        #region Public properties
        [Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool FirstLayoutSelected { get; set; } = false;
        [Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<HCylLayout> HCylLayouts
        {
            get { return _hCylLayouts; }
            set { _hCylLayouts = value; Start(); }
        }
        [Browsable(false)]
        public Size ButtonSizes
        {
            get { return szButtons; }
            set { szButtons = value; OnButtonSizeChange(null, null); Start(); }
        }
        public HCylLayout Selected
        {
            get
            {
                foreach (Control ctrl in Controls)
                {
                    if (ctrl is Button button)
                    {
                        HCylLayoutItem item = button.Tag as HCylLayoutItem;
                        if (item.Selected)
                            return item.Layout;
                    }
                }
                return null;
            }
        }
        [Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CylinderProperties Packable { get; set; }
        #endregion
        #region Event handlers
        private void Start()
        {
            if (DesignMode) return;
            // clear all
            Controls.Clear();
            // initialize
            _index = 0; _x = 0; _y = 0;
            // timer
            timer.Start();
        }
        private void OnTimerTick(object sender, EventArgs e)
        {
            if (_index == HCylLayouts.Count)
            {
                timer.Stop();
                if (Controls.Count > 0)
                {
                    var bt = Controls[0] as Button;
                    HCylLayoutItem btItem = bt.Tag as HCylLayoutItem;
                    btItem.Selected = true;
                    OnLayoutSelected(bt, null);
                }
                RefreshFinished?.Invoke(this, null);
                return;
            }
            bool selected = (0 == Controls.Count) ? FirstLayoutSelected : false;
            var layout = HCylLayouts[_index];

            // create pattern and add to panel
            var btn = new Button
            {
                Image = TryGeneratePatternImage(HCylLayouts[_index], szButtons, selected),
                Location = new Point(_x, _y) + (Size)AutoScrollPosition,
                Size = new Size(szButtons.Width, szButtons.Height),
                Tag = new HCylLayoutItem(layout, selected)
            };
            btn.Click += OnLayoutSelected;
            Controls.Add(btn);
            // give button a tooltip
            tooltip.SetToolTip(btn, layout.Tooltip);
            // adjust i, x and y for next image
            AdjustXY(ref _x, ref _y);
            ++_index;
        }
        private void OnLayoutSelected(object sender, EventArgs e)
        {
            Button bnSender = sender as Button;
            foreach (var ctrl in Controls)
            {
                if (ctrl is Button bt && bt != bnSender)
                {
                    if (bt.Tag is HCylLayoutItem btItem && btItem.Selected)
                    {
                        btItem.Selected = false;
                        bt.Image = TryGeneratePatternImage(btItem.Layout, szButtons, btItem.Selected);
                    }
                }
            }
            // ***
            HCylLayoutItem lItem = bnSender.Tag as HCylLayoutItem;
            bool selected = true;
            bnSender.Image = TryGeneratePatternImage(lItem.Layout, szButtons, selected);
            bnSender.Tag = new HCylLayoutItem(lItem.Layout, selected);
            LayoutSelected?.Invoke(this, e);
        }
        #endregion
        #region Context menu
        private void OnButtonSizeChange(object sender, EventArgs e)
        {
            if (sender == toolStripMenuItemX75)
                ButtonSizes = new Size(75, 75);
            else if (sender == toolStripMenuItemX100)
                ButtonSizes = new Size(100, 100);
            else if (sender == toolStripMenuItemX150)
                ButtonSizes = new Size(150, 150);
            else if (sender == toolStripMenuItemX200)
                ButtonSizes = new Size(200, 200);

            toolStripMenuItemX75.Checked = ButtonSizes.Height == 75;
            toolStripMenuItemX100.Checked = ButtonSizes.Height == 100;
            toolStripMenuItemX150.Checked = ButtonSizes.Height == 150;
            toolStripMenuItemX200.Checked = ButtonSizes.Height == 200;
        }
        #endregion
        #region Helpers
        private Image TryGeneratePatternImage(HCylLayout cylLayout, Size szButtons, bool selected)
        {
            return HCylLayoutToImage.Draw(cylLayout, Packable as CylinderProperties, cylLayout.DimContainer, szButtons, selected, true);
        }
        private void AdjustXY(ref int x, ref int y)
        {
            x += szButtons.Width;
            if (x + szButtons.Width > Width - SystemInformation.VerticalScrollBarWidth)
            {
                x = 0;
                y += szButtons.Height;
            }
        }
        #endregion
        #region Data members
        protected static readonly ILog _log = LogManager.GetLogger(typeof(UCtrlHCylLayoutList));
        private List<HCylLayout> _hCylLayouts = new List<HCylLayout>();
        private int _index;
        private int _x, _y;
        private readonly ToolTip tooltip = new ToolTip();
        #endregion
    }

    #region HCylLayoutItem
    internal class HCylLayoutItem
    {
        public HCylLayoutItem(HCylLayout layout, bool selected) { Layout = layout; Selected = selected; }
        public HCylLayout Layout { get; set; }
        public bool Selected { get; set; }
    }
    #endregion
}
