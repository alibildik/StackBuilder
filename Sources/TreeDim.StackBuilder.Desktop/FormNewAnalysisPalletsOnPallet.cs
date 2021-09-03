#region Using directives
using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

using log4net;
using Sharp3D.Math.Core;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Graphics.Controls;
using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormNewAnalysisPalletsOnPallet : FormNewAnalysis, IDrawingContainer, IItemBaseFilter
    {
        #region Constructor
        public FormNewAnalysisPalletsOnPallet(Document doc, AnalysisPalletsOnPallet analysisPalletsOnPallet)
            : base(doc, analysisPalletsOnPallet)
        {
            InitializeComponent();
        }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (null != _item)
            {
                tbName.Text = _item.Name;
                tbDescription.Text = _item.Description;
            }
            else if (null != _document)
            {
                tbName.Text = _document.GetValidNewAnalysisName(ItemDefaultName);
                tbDescription.Text = tbName.Text;
            }
            // graphics3D control
            graphCtrl.DrawingContainer = this;
            // list of pallets
            cbDestinationPallet.Initialize(_document, this, null);
            cbInputPallet1.Initialize(_document, this, null);
            cbInputPallet2.Initialize(_document, this, null);
            cbInputPallet3.Initialize(_document, this, null);
            cbInputPallet4.Initialize(_document, this, null);
            // MasterPallet
            cbMasterSplit.SelectedIndex = 0;
            cbPalletOrientation.SelectedIndex = 0;

            rbHalf.Checked = true;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }
        public override void UpdateStatus(string message)
        {
            if (!Program.IsSubscribed)
                message = Resources.ID_GOPREMIUMORUNSELECT;
            base.UpdateStatus(message);
        }
        public override void OnNext()
        {
            try
            {
                AnalysisPalletsOnPallet analysis = AnalysisCast;
                if (null == analysis)
                {
                    _item = _document.CreateNewAnalysisPalletsOnPallet(
                        ItemName, ItemDescription
                        , MasterPalletSplit, LoadedPalletOrientation
                        , MasterPallet
                        , LoadedPallet0
                        , LoadedPallet1
                        , 1 == Mode ? LoadedPallet2 : null
                        , 1 == Mode ? LoadedPallet3 : null);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
            base.OnNext();
            Close();
        }
        #endregion

        #region FormNewBase override
        public override string ItemDefaultName => Resources.ID_PALLET;
        #endregion

        #region IItemBaseFilter
        public bool Accept(Control ctrl, ItemBase itemBase)
        {
            if (ctrl == cbDestinationPallet)
                return itemBase is PalletProperties;
            else if (ctrl == cbInputPallet1
                || ctrl == cbInputPallet2
                || ctrl == cbInputPallet3
                || ctrl == cbInputPallet4)
                return itemBase is LoadedPallet;
            return false;
        }
        #endregion

        #region Public properties
        public AnalysisPalletsOnPallet AnalysisCast
        { get => _item as AnalysisPalletsOnPallet;  }
        #endregion

        #region Handlers
        private void OnPalletLayoutChanged(object sender, EventArgs e)
        {
            bool quarter = rbQuarter.Checked;
            lbInputPallet3.Visible = quarter;
            lbInputPallet4.Visible = quarter;
        }
        private void OnLoadedPalletChanged(object sender, EventArgs e)
        {
            double masterLength = MasterPallet.Length;
            double masterWidth = MasterPallet.Width;

            double loadedLength = LoadedPallet0.Length;
            double loadedWidth = LoadedPallet0.Width;
            int indexSplit = -1;
            int indexOrientation = -1;

            switch (Mode)
            {
                case 0:
                    {
                        double[] diff = new double[4];
                        diff[0] = Math.Abs(masterLength - loadedLength) + Math.Abs(masterWidth - 2.0 * loadedWidth);
                        diff[1] = Math.Abs(masterLength - loadedWidth) + Math.Abs(masterWidth - 2.0 * loadedLength);
                        diff[2] = Math.Abs(masterLength - 2.0 * loadedLength) + Math.Abs(masterWidth - loadedWidth);
                        diff[3] = Math.Abs(masterLength - 2.0 * loadedWidth) + Math.Abs(masterWidth - loadedLength);

                        int minIndex = -1;
                        double minValue = double.MaxValue;
                        for (int i = 0; i < 4; ++i)
                        {
                            if (diff[i] < minValue)
                            {
                                minIndex = i;
                                minValue = diff[i];
                            }
                        }
                        switch (minIndex)
                        {
                            case 0: indexSplit = 0; indexOrientation = 0; break;
                            case 1: indexSplit = 0; indexOrientation = 1; break;
                            case 2: indexSplit = 1; indexOrientation = 0; break;
                            case 3: indexSplit = 1; indexOrientation = 1; break;
                            default: break;
                                
                        }
                    }
                    break;
                case 1:
                    {
                        double[] diff = new double[2];
                        diff[0] = Math.Abs(masterLength - 2.0 * loadedLength) + Math.Abs(masterWidth - 2.0 * loadedWidth);
                        diff[0] = Math.Abs(masterLength - 2.0 * loadedWidth) + Math.Abs(masterWidth - 2.0 * loadedLength);

                        int minIndex = -1;
                        double minValue = double.MaxValue;
                        for (int i = 0; i < 2; ++i)
                        {
                            if (diff[i] < minValue)
                            {
                                minIndex = i;
                                minValue = diff[i];
                            }
                        }
                        switch (minIndex)
                        {
                            case 0: indexOrientation = 0; break;
                            case 1: indexOrientation = 1; break;
                            default: break;
                        }
                    }
                    break;
                default: break;
            }
            cbMasterSplit.SelectedIndex = indexSplit;
            cbPalletOrientation.SelectedIndex = indexOrientation;

            OnInputChanged(sender, e);
        }
        private void OnInputChanged(object sender, EventArgs e)
        {
            graphCtrl.Invalidate();
        }
        #endregion

        #region IDrawingContainer
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            if (null == MasterPallet || null == LoadedPallet0 || null == LoadedPallet1)
                return;

            var analysis = new AnalysisPalletsOnPallet(null,
                MasterPalletSplit,
                LoadedPalletOrientation,
                MasterPallet,
                LoadedPallet0,
                LoadedPallet1,
                1 == Mode ? LoadedPallet2 : null,
                1 == Mode ? LoadedPallet3 : null);

            if (!analysis.HasValidSolution) return;

            var viewer = new ViewerSolutionPalletsOnPallet(analysis.Solution);
            viewer.Draw(graphics, Transform3D.Identity);
        }
        #endregion

        #region Accessors
        private PalletProperties MasterPallet => cbDestinationPallet.SelectedType as PalletProperties;
        private LoadedPallet LoadedPallet0 => cbInputPallet1.SelectedType as LoadedPallet;
        private LoadedPallet LoadedPallet1 => cbInputPallet2.SelectedType as LoadedPallet;
        private LoadedPallet LoadedPallet2 => cbInputPallet3.SelectedType as LoadedPallet;
        private LoadedPallet LoadedPallet3 => cbInputPallet4.SelectedType as LoadedPallet;
        private int Mode => rbHalf.Checked ? 0 : 1;
        private AnalysisPalletsOnPallet.EMasterPalletSplit MasterPalletSplit => (AnalysisPalletsOnPallet.EMasterPalletSplit)cbMasterSplit.SelectedIndex;
        private AnalysisPalletsOnPallet.ELoadedPalletOrientation LoadedPalletOrientation => (AnalysisPalletsOnPallet.ELoadedPalletOrientation)cbPalletOrientation.SelectedIndex;

        #endregion

        #region Data members
        private static readonly ILog _log = LogManager.GetLogger(typeof(FormNewAnalysisPalletsOnPallet));
        #endregion
    }
}
