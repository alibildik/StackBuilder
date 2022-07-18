#region Using directives
using System;
using System.Linq;
using System.ComponentModel;
using System.Drawing;

using WeifenLuo.WinFormsUI.Docking;
using log4net;

using treeDiM.StackBuilder.WCFService.Test.SB_SR;
#endregion

namespace treeDiM.StackBuilder.WCFService.Test
{
    public partial class FormTestHomogeneous : DockContent
    {
        #region Constructor
        public FormTestHomogeneous()
        {
            InitializeComponent();
        }
        #endregion

        #region Form overrides
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // case dimensions
            CaseLength = 400.0;
            CaseWidth = 300.0;
            CaseHeight = 200.0;
            CaseWeight = 4.5;
            AllowOrientX = false;
            AllowOrientY = false;
            AllowOrientZ = true;
            // bundle
            FlatLength = 400.0;
            FlatWidth = 300.0;
            FlatHeight = 10.0;
            FlatWeight = 0.050;
            NumberOfFlats = 10;
            // pallet
            PalletLength = 1200.0;
            PalletWidth = 1000.0;
            PalletHeight = 145.0;
            PalletWeight = 24.0;
            MaxPalletHeight = 1200;
            MaxPalletWeight = 1000.0;
            // case 
            CaseInnerLength = 395.0;
            CaseInnerWidth = 295.0;
            CaseInnerHeight = 195.0;
            CaseEmptyWeight = 0.25;
            // allow multiple layer orientations
            AllowMultipleLayerOrientations = true;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }
        #endregion

        #region Handlers
        private void OnCompute(object sender, EventArgs e)
        {
            try
            {
                DateTime dt0 = DateTime.Now;
                using (StackBuilderClient client = new StackBuilderClient())
                {
                    DCSBSolution sol = null;
                    if (tabCtrlContent.SelectedIndex == 0 && tabCtrlContainer.SelectedIndex == 0)
                    {
                        _log.Info($"Calling web service method \"SB_GetCasePalletBestSolution\"...");
                        sol = client.SB_GetCasePalletBestSolution(
                                new DCSBCase()
                                {
                                    Name = "Case",
                                    Description = "Default case",
                                    DimensionsOuter = OuterDimensions,
                                    HasInnerDims = false,
                                    DimensionsInner = null,
                                    Weight = CaseWeight,
                                    MaxWeight = 100.0,
                                    NetWeight = 0.9 * CaseWeight,
                                    ShowTape = true,
                                    TapeWidth = 50.0,
                                    TapeColor = Color.Beige.ToArgb(),
                                    Colors = Enumerable.Repeat(Color.Chocolate.ToArgb(), 6).ToArray()
                                }
                                , new DCSBPallet()
                                {
                                    Name = "EUR2",
                                    Description = "EUR2",
                                    PalletType = "EUR2",
                                    Color = Color.Yellow.ToArgb(),
                                    Dimensions = PalletDimensions,
                                    Weight = PalletWeight
                                }
                                , null
                                , new DCSBConstraintSet()
                                {
                                    Overhang = PalletOverhang,
                                    Orientation = new DCSBBool3()
                                    {
                                        X = AllowOrientX,
                                        Y = AllowOrientY,
                                        Z = AllowOrientZ
                                    },
                                    MaxHeight = new DCSBConstraintDouble() { Active = true, Value_d = MaxPalletHeight },
                                    MaxWeight = new DCSBConstraintDouble() { Active = true, Value_d = MaxPalletWeight },
                                    MaxNumber = new DCSBConstraintInt() { Active = false, Value_i = 100 },
                                    AllowMultipleLayerOrientations = this.AllowMultipleLayerOrientations
                                }
                                , new DCCompFormat()
                                {
                                    Size = new DCCompSize()
                                    {
                                        CX = pbStackbuilder.Size.Width,
                                        CY = pbStackbuilder.Size.Height
                                    },
                                    Format = OutFormat.IMAGE
                                }
                                , true
                            );
                    }
                    else if (tabCtrlContent.SelectedIndex == 1 && tabCtrlContainer.SelectedIndex == 0)
                    {
                        _log.Info($"Calling web service method \"SB_GetBundlePalletBestSolution\"...");
                        sol = client.SB_GetBundlePalletBestSolution(
                            new DCSBBundle()
                            {
                                Name = "Bundle",
                                Description = "Bundle",
                                DimensionsUnit = FlatDimensions,
                                UnitWeight = FlatWeight,
                                Number = NumberOfFlats,
                                Color = Color.Beige.ToArgb()
                            }
                            , new DCSBPallet()
                            {
                                Name = "EUR2",
                                Description = "EUR2",
                                PalletType = "EUR2",
                                Color = Color.Yellow.ToArgb(),
                                Dimensions = PalletDimensions,
                                Weight = PalletWeight,
                            }
                            , null
                            , new DCSBConstraintSet()
                            {
                                Overhang = PalletOverhang,
                                Orientation = new DCSBBool3() { X = false, Y = false, Z = true },
                                MaxHeight = new DCSBConstraintDouble() { Active = true, Value_d = MaxPalletHeight },
                                MaxWeight = new DCSBConstraintDouble() { Active = false, Value_d = 1000.0 },
                                MaxNumber = new DCSBConstraintInt() { Active = false, Value_i = 100 }
                            }
                            , new DCCompFormat()
                            {
                                Size = new DCCompSize()
                                {
                                    CX = pbStackbuilder.Size.Width,
                                    CY = pbStackbuilder.Size.Height
                                },
                                Format = OutFormat.IMAGE
                            }
                            , true
                            );
                    }
                    else if (tabCtrlContent.SelectedIndex == 0 && tabCtrlContainer.SelectedIndex == 1)
                    {
                        _log.Info($"Calling web service method \"SB_GetBoxCaseBestSolution\"...");
                        sol = client.SB_GetBoxCaseBestSolution(
                            new DCSBCase()
                            {
                                Name = "Box",
                                Description = "Box",
                                DimensionsOuter = OuterDimensions,
                                HasInnerDims = false,
                                DimensionsInner = null,
                                Weight = CaseWeight,
                                MaxWeight = 1.0,
                                NetWeight = 0.9 * CaseWeight,
                                ShowTape = false,
                                Colors = Enumerable.Repeat<int>(Color.Turquoise.ToArgb(), 6).ToArray()
                            }
                            , new DCSBCase()
                            {
                                Name = "Case",
                                Description = "Case",
                                HasInnerDims = true,
                                DimensionsOuter = null,
                                DimensionsInner = InnerDimensions,
                                Colors = Enumerable.Repeat(Color.Chocolate.ToArgb(), 6).ToArray()
                            }
                            , null
                            , new DCSBConstraintSet()
                            {
                                Orientation = new DCSBBool3() { X = AllowOrientX, Y = AllowOrientY, Z = AllowOrientZ },
                                MaxWeight = new DCSBConstraintDouble() { Active = false, Value_d = 1000.0 },
                                MaxNumber = new DCSBConstraintInt() { Active = false, Value_i = 100 },
                                AllowMultipleLayerOrientations = AllowMultipleLayerOrientations
                            }
                            , new DCCompFormat()
                            {
                                Size = new DCCompSize()
                                {
                                    CX = pbStackbuilder.Size.Width,
                                    CY = pbStackbuilder.Size.Height
                                },
                                Format = OutFormat.IMAGE
                            }
                            , true
                            );
                    }
                    else if (tabCtrlContent.SelectedIndex == 1 && tabCtrlContainer.SelectedIndex == 1)
                    {
                        _log.Info($"Calling web service method \"SB_GetBundleCaseBestSolution\"...");
                        sol = client.SB_GetBundleCaseBestSolution(
                                new DCSBBundle()
                                {
                                    Name = "Bundle",
                                    Description = "Bundle",
                                    DimensionsUnit = FlatDimensions,
                                    UnitWeight = FlatWeight,
                                    Number = NumberOfFlats,
                                    Color = Color.Beige.ToArgb()
                                }
                                , new DCSBCase()
                                {
                                    Name = "Case",
                                    Description = "Case",
                                    HasInnerDims = true,
                                    DimensionsOuter = null,
                                    DimensionsInner = InnerDimensions,
                                    Colors = Enumerable.Repeat<int>(Color.Chocolate.ToArgb(), 6).ToArray()
                                }
                                , new DCSBConstraintSet()
                                {
                                    Orientation = new DCSBBool3() { X = false, Y = false, Z = true },
                                    MaxWeight = new DCSBConstraintDouble() { Active = false, Value_d = 1000.0 },
                                    MaxNumber = new DCSBConstraintInt() { Active = false, Value_i = 100 }
                                }
                                , new DCCompFormat()
                                {
                                    Size = new DCCompSize()
                                    {
                                        CX = pbStackbuilder.Size.Width,
                                        CY = pbStackbuilder.Size.Height
                                    },
                                    Format = OutFormat.IMAGE
                                }
                                , true
                            );
                    }
                    if (null != sol)
                    {
                        foreach (string err in sol.Errors)
                            _log.Warn(err);
                        if (sol.Errors.Length > 0)
                            return;
                    }
                    else
                    {
                        _log.Error("Call to SB_GetBestSolution failed... : sol == null");
                        return;
                    }
                    DateTime dt1 = DateTime.Now;
                    _log.Info($"Web service answered in {(dt1 - dt0).TotalMilliseconds} ms");

                    // image
                    pbStackbuilder.Image = null;
                    if (null != sol.OutFile)
                        using (var ms = new System.IO.MemoryStream(sol.OutFile.Bytes))
                        {
                            Image img = Image.FromStream(ms);
                            pbStackbuilder.Image = img;
                        }
                    // case count
                    CaseCount = sol.CaseCount;
                    TotalPalletWeight = sol.WeightTotal;
                    PalletEfficiency = sol.Efficiency;
                    BBoxTotal = sol.BBoxTotal;
                    BBoxLoad = sol.BBoxLoad;
                    PalletMapPhrase = sol.PalletMapPhrase;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void OnExit(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        #region Helpers
        private bool AllowMultipleLayerOrientations { get => chkbAllowMultipleLayerOrientations.Checked; set => chkbAllowMultipleLayerOrientations.Checked = value; }
        // case (content)
        private double CaseLength { get => (double)nudCaseDimX.Value; set => nudCaseDimX.Value = (decimal)value; }
        private double CaseWidth {  get => (double)nudCaseDimY.Value; set => nudCaseDimY.Value = (decimal)value; }
        private double CaseHeight { get => (double)nudCaseDimZ.Value; set => nudCaseDimZ.Value = (decimal)value; }
        private double CaseWeight { get => (double)nudCaseWeight.Value; set => nudCaseWeight.Value = (decimal)value; }
        private DCSBDim3D OuterDimensions => new DCSBDim3D() { M0 = CaseLength, M1 = CaseWidth, M2 = CaseHeight };
        private bool AllowOrientX { get => chkbAllowX.Checked; set => chkbAllowX.Checked = value; }
        private bool AllowOrientY { get => chkbAllowY.Checked; set => chkbAllowY.Checked = value; }
        private bool AllowOrientZ { get => chkbAllowZ.Checked; set => chkbAllowZ.Checked = value; }
        // bundle (content)
        private double FlatLength { get => (double)nudFlatX.Value; set => nudFlatX.Value = (decimal)value; }
        private double FlatWidth { get => (double)nudFlatY.Value; set => nudFlatY.Value = (decimal)value; }
        private double FlatHeight { get => (double)nudFlatZ.Value; set => nudFlatZ.Value = (decimal)value; }
        private double FlatWeight { get => (double)nudFlatWeight.Value; set => nudFlatWeight.Value = (decimal)value; }
        private DCSBDim3D FlatDimensions => new DCSBDim3D() { M0 = FlatLength, M1 = FlatWidth, M2 = FlatHeight }; 
        private int NumberOfFlats { get => (int)nudFlatNumber.Value; set => nudFlatNumber.Value = value; }
        // case (container)
        private double CaseInnerLength { get => (double)nudInnerX.Value; set => nudInnerX.Value = (decimal)value; }
        private double CaseInnerWidth { get => (double)nudInnerY.Value; set => nudInnerY.Value = (decimal)value; }
        private double CaseInnerHeight { get => (double)nudInnerZ.Value; set => nudInnerZ.Value = (decimal)value; }
        private DCSBDim3D InnerDimensions => new DCSBDim3D() { M0 = CaseInnerLength, M1 = CaseInnerWidth, M2 = CaseInnerHeight };
        private double CaseEmptyWeight { get => (double)nudCaseEmptyWeight.Value; set => nudCaseEmptyWeight.Value = (decimal)value; }
        // pallet
        private double PalletLength { get => (double)nudPalletDimX.Value; set => nudPalletDimX.Value = (decimal)value; }
        private double PalletWidth  { get => (double)nudPalletDimY.Value; set => nudPalletDimY.Value = (decimal)value; }
        private double PalletHeight { get => (double)nudPalletDimZ.Value; set => nudPalletDimZ.Value = (decimal)value; }
        private double PalletWeight { get => (double)nudPalletWeight.Value; set => nudPalletWeight.Value = (decimal)value; }
        private double MaxPalletHeight { get => (double)nudMaxPalletHeight.Value; set => nudMaxPalletHeight.Value = (decimal)value; }
        private double MaxPalletWeight { get => (double)nudMaxPalletWeight.Value; set => nudMaxPalletWeight.Value = (decimal)value; }
        private double OverhangX { get => (double)nudOverhangX.Value; }
        private double OverhangY { get => (double)nudOverhangY.Value; }
        private DCSBDim3D PalletDimensions => new DCSBDim3D() { M0 = PalletLength, M1 = PalletWidth, M2 = PalletHeight };
        private DCSBDim2D PalletOverhang => new DCSBDim2D() { M0 = OverhangX, M1 = OverhangY };
        // solution        
        private int CaseCount               { set => lbLoadedPalletCaseCountValue.Text     = $": {value}"; }
        private double TotalPalletWeight    { set => lbLoadedPalletWeightValue.Text        = $": {value:0.#} kg"; }
        private double PalletEfficiency     { set => lbLoadedPalletEfficiencyValue.Text    = $": {value:0.#} %"; }
        private DCSBDim3D BBoxTotal         { set => lbTotalPalletDimValues.Text           = null != value ? $": {value.M0:0.#} x{value.M1:0.#} x{value.M2:0.#} mm" : string.Empty; }
        private DCSBDim3D BBoxLoad          { set => lbLoadDimValues.Text                  = null != value ? $": {value.M0:0.#} x{value.M1:0.#} x{value.M2:0.#} mm" : string.Empty; }
        private string PalletMapPhrase      { set => lbPalletMapPhrase.Text                = null != value ? $": {value}": string.Empty; }

        private ILog _log = LogManager.GetLogger(typeof(FormTestHomogeneous));
        #endregion
    }
}
