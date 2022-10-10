#region Using directives
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Sharp3D.Math.Core;
using log4net;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Desktop.Properties;

using treeDiM.PLMPack.DBClient;
using treeDiM.PLMPack.DBClient.PLMPackSR;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormNewPallet : FormNewBase, IDrawingContainer
    {
        #region Constructor
        public FormNewPallet(Document document, PalletProperties palletProperties)
            : base(document, palletProperties)
        {
            InitializeComponent();

            // fill type combo
            cbType.Items.AddRange(PalletData.TypeNames);
            // initialize data
            if (null != palletProperties)
            {
                PalletTypeName = palletProperties.TypeName;
                PalletLength = palletProperties.Length;
                PalletWidth = palletProperties.Width;
                PalletHeight = palletProperties.Height;
                Weight = palletProperties.Weight;
                AdmissibleLoad = palletProperties.AdmissibleLoadWeight;
                PalletColor = palletProperties.Color;
            }
            else
            {
                // set selected item
                PalletTypeName = Settings.Default.PalletTypeName;
                OnPalletTypeChanged(this, null);
            }
            UpdateStatus(string.Empty);
            // set unit labels
            UnitsManager.AdaptUnitLabels(this);
        }
        #endregion

        #region FormNewBase override
        public override string ItemDefaultName => Resources.ID_PALLET;
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            graphCtrl.DrawingContainer = this;
            // windows settings
            if (null != Settings.Default.FormNewPalletPosition)
                Settings.Default.FormNewPalletPosition.Restore(this);
            // enable / disable database button
            bnSendToDB.Enabled = WCFClient.IsConnected;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            // form position
            if (null == Settings.Default.FormNewPalletPosition)
                Settings.Default.FormNewPalletPosition = new WindowSettings();
            Settings.Default.FormNewPalletPosition.Record(this);

            // pallet type name
            Settings.Default.PalletTypeName = PalletTypeName;
        }
        public override void UpdateStatus(string message)
        {
            base.UpdateStatus(message);
        }
        #endregion

        #region Public properties
        public double PalletLength { get => uCtrlDimensions.ValueX; set => uCtrlDimensions.ValueX = value; }
        public double PalletWidth { get => uCtrlDimensions.ValueY; set => uCtrlDimensions.ValueY = value; }
        public double PalletHeight { get => uCtrlDimensions.ValueZ; set => uCtrlDimensions.ValueZ = value; }
        public double Weight { get => uCtrlWeight.Value; set => uCtrlWeight.Value = value; }
        public double AdmissibleLoad { get => uCtrlAdmissibleLoad.Value; set => uCtrlAdmissibleLoad.Value = value; }
        public Color PalletColor { get => cbColor.Color; set => cbColor.Color = value; }
        public string PalletTypeName
        {
            get { return cbType.Items[cbType.SelectedIndex].ToString(); }
            set
            {
                int index = 0;
                foreach (string item in cbType.Items)
                {
                    if (string.Equals(item, value))
                        break;
                    ++index;
                }
                if (cbType.Items.Count > index)
                    cbType.SelectedIndex = index;
            }
        }
        #endregion

        #region Implement IDrawingContainer
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            if (0 == cbType.Items.Count)
                return;
            PalletProperties palletProperties = new PalletProperties(null, PalletTypeName, PalletLength, PalletWidth, PalletHeight)
            {
                Color = PalletColor
            };
            Pallet pallet = new Pallet(palletProperties);
            pallet.Draw(graphics, Transform3D.Identity);
            graphics.AddDimensions(new DimensionCube(PalletLength, PalletWidth, PalletHeight));
        }
        #endregion

        #region Handlers
        private void OnPalletPropertyChanged(object sender, EventArgs e)
        {
            graphCtrl.Invalidate();
            UpdateStatus(string.Empty);
        }
        private void OnPalletTypeChanged(object sender, EventArgs e)
        {
            PalletData palletData = PalletData.GetByName(PalletTypeName);
            if (null == palletData) return;

            // set name / description / length / width / height / weight
            ItemName = palletData.Name;
            ItemDescription = palletData.Description;
            PalletLength = UnitsManager.ConvertLengthFrom(palletData.Length, UnitsManager.UnitSystem.UNIT_METRIC1);
            PalletWidth = UnitsManager.ConvertLengthFrom(palletData.Width,  UnitsManager.UnitSystem.UNIT_METRIC1);
            PalletHeight = UnitsManager.ConvertLengthFrom(palletData.Height,  UnitsManager.UnitSystem.UNIT_METRIC1);
            Weight = UnitsManager.ConvertMassFrom(palletData.Weight, UnitsManager.UnitSystem.UNIT_METRIC1);
            PalletColor = palletData.Color;

            graphCtrl.Invalidate();
        }
        #endregion

        #region Send to database
        private void OnSendToDatabase(object sender, EventArgs e)
        {
            try
            {
                FormSetItemName form = new FormSetItemName() {  ItemName = ItemName };
                if (DialogResult.OK == form.ShowDialog())
                {
                    using (WCFClient wcfClient = new WCFClient())
                    {
                        wcfClient.Client?.CreateNewPallet(new DCSBPallet()
                        {
                            Name = form.ItemName,
                            Description = ItemDescription,
                            UnitSystem = (int)UnitsManager.CurrentUnitSystem,
                            PalletType = PalletTypeName,
                            Dimensions = new DCSBDim3D() { M0 = PalletLength, M1 = PalletWidth, M2 = PalletHeight },
                            Weight = Weight,
                            AdmissibleLoad = AdmissibleLoad,
                            Color = PalletColor.ToArgb(),
                            AutoInsert = false
                        }
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        #endregion

        #region Data members
        private static readonly ILog _log = LogManager.GetLogger(typeof(FormNewPallet));
        #endregion
    }
}