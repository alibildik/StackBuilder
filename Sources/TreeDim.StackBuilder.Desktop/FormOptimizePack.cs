#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Sharp3D.Math.Core;
using log4net;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Engine;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Graphics.Controls;
using treeDiM.StackBuilder.Desktop.Properties;

using ColumnHeader = SourceGrid.Cells.ColumnHeader;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormOptimizePack : Form, IItemBaseFilter, IDrawingContainer
    {
        #region Constructor
        public FormOptimizePack(DocumentSB document)
        {
            InitializeComponent();
            // set  unit labels
            UnitsManager.AdaptUnitLabels(this);
            // document 
            _doc = document;
        }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // font size
            GridFontSize = Settings.Default.GridFontSize;
            // initialize combo boxes
            cbBoxes.Initialize(_doc, this, null);
            cbPallets.Initialize(_doc, this, null);
            // initialize graph containers
            graphCtrlPack.DrawingContainer = this;
            graphCtrlSolution.DrawingContainer = this;
            // set default pallet height
            MaximumPalletHeight = UnitsManager.ConvertLengthFrom(Settings.Default.PalletHeight, UnitsManager.UnitSystem.UNIT_METRIC1);
            // set default wall numbers and thickness
            HasWrapper = Settings.Default.OptHasWrapper;
            uCtrlWrapperWalls.NoX = Settings.Default.NumberWallsLength;
            uCtrlWrapperWalls.NoY = Settings.Default.NumberWallsWidth;
            uCtrlWrapperWalls.NoZ = Settings.Default.NumberWallsHeight;
            WrapperWallThickness = UnitsManager.ConvertLengthFrom(Settings.Default.WrapperThickness, UnitsManager.UnitSystem.UNIT_METRIC1);
            WrapperWallSurfMass = UnitsManager.ConvertSurfaceMassFrom(Settings.Default.WrapperSurfMass, UnitsManager.UnitSystem.UNIT_METRIC1);

            HasTray = Settings.Default.OptHasTray;
            uCtrlTrayWalls.NoX = Settings.Default.NumberWallsLength;
            uCtrlTrayWalls.NoY = Settings.Default.NumberWallsWidth;
            uCtrlTrayWalls.NoZ = Settings.Default.NumberWallsHeight;
            uCtrlTrayHeight.Value = UnitsManager.ConvertLengthFrom(Settings.Default.TrayHeight, UnitsManager.UnitSystem.UNIT_METRIC1);
            uCtrlTrayThickness.Value = UnitsManager.ConvertLengthFrom(Settings.Default.TrayThickness, UnitsManager.UnitSystem.UNIT_METRIC1);
            uCtrlTraySurfMass.Value = UnitsManager.ConvertSurfaceMassFrom(Settings.Default.TraySurfMass, UnitsManager.UnitSystem.UNIT_METRIC1);


            // set default number of boxes
            nudNumber.Value = Settings.Default.NumberBoxesPerCase;
            // set default wrapper type
            cbWrapperType.SelectedIndex = Settings.Default.WrapperType;
            // initialize grid
            gridSolutions.Selection.SelectionChanged += OnSelChangeGrid;

            UpdateStatus(string.Empty);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            // save settings
            // default pallet height
            Settings.Default.PalletHeight = MaximumPalletHeight;
            // default wall numbers and thickness
            Settings.Default.OptHasWrapper = HasWrapper;
            Settings.Default.NumberWallsLength = uCtrlWrapperWalls.NoX;
            Settings.Default.NumberWallsWidth = uCtrlWrapperWalls.NoY;
            Settings.Default.NumberWallsHeight = uCtrlWrapperWalls.NoZ;
            Settings.Default.WrapperThickness = WrapperWallThickness;
            Settings.Default.WrapperSurfMass = WrapperWallSurfMass;

            Settings.Default.OptHasTray = HasTray;
            Settings.Default.TrayThickness = UnitsManager.ConvertLengthTo(TrayWallThickness, UnitsManager.UnitSystem.UNIT_METRIC1);
            Settings.Default.TrayHeight = UnitsManager.ConvertLengthTo(TrayHeight, UnitsManager.UnitSystem.UNIT_METRIC1);

            // default number of boxes
            Settings.Default.NumberBoxesPerCase = (int)nudNumber.Value;
            // default wrapper type
            Settings.Default.WrapperType = cbWrapperType.SelectedIndex;
            // window position
            Settings.Default.FormOptimizeCasePosition ??= new WindowSettings();
            Settings.Default.FormOptimizeCasePosition.Record(this);
        }
        #endregion

        #region Status toolstrip updating
        public virtual void UpdateStatus(string message)
        {
            if (null == SelectedAnalysis)
                message = Resources.ID_ANALYSISHASNOVALIDSOLUTION;

            toolStripStatusLabelDef.ForeColor = string.IsNullOrEmpty(message) ? Color.Black : Color.Red;
            toolStripStatusLabelDef.Text = string.IsNullOrEmpty(message) ? Resources.ID_READY : message;

            bnCreateAnalysis.Enabled = (null != SelectedAnalysis);
        }
        #endregion

        #region IItemBaseFilter
        public bool Accept(Control ctrl, ItemBase itemBase)
        {
            if (ctrl == cbBoxes)
                return itemBase is BoxProperties || itemBase is BagProperties;
            if (ctrl == cbPallets)
                return itemBase is PalletProperties;
            return false; 
        }
        #endregion

        #region IDrawingContainer
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            if (null == SelectedAnalysis)
                return;
            try
            {
                AnalysisCasePallet analysis = SelectedAnalysis;
                if (graphCtrlPack == ctrl)
                {
                    bool showDimensions = true;
                    // build pack
                    if (analysis.Content is PackProperties packProperties)
                    {
                        var pack = new Pack(0, packProperties) { ForceTransparency = true };
                        graphics.AddBox(pack);
                        if (showDimensions)
                        {
                            graphics.AddDimensions(new DimensionCube(Vector3D.Zero, pack.Length, pack.Width,
                                pack.Height, Color.Black, true));
                            if (null != packProperties.Wrap && packProperties.Wrap.Transparent)
                                graphics.AddDimensions(
                                    new DimensionCube(
                                        packProperties.InnerOffset
                                        , packProperties.InnerLength, packProperties.InnerWidth,
                                        packProperties.InnerHeight
                                        , Color.Red, false));
                        }
                    }
                }
                else if (graphCtrlSolution == ctrl)
                {
                    var sv = new ViewerSolution(SelectedAnalysis.SolutionLay);
                    sv.Draw(graphics, Transform3D.Identity);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        #endregion

        #region Grid
        private void FillGrid()
        {
            try
            {
                // remove all existing rows
                gridSolutions.Rows.Clear();
                // *** IViews
                // captionHeader
                SourceGrid.Cells.Views.RowHeader captionHeader = new SourceGrid.Cells.Views.RowHeader();
                DevAge.Drawing.VisualElements.RowHeader veHeaderCaption = new DevAge.Drawing.VisualElements.RowHeader()
                {
                    BackColor = Color.SteelBlue,
                    Border = DevAge.Drawing.RectangleBorder.NoBorder
                };
                captionHeader.Background = veHeaderCaption;
                captionHeader.ForeColor = Color.Black;
                captionHeader.Font = new Font("Arial", GridFontSize, FontStyle.Bold);
                captionHeader.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                // viewRowHeader
                SourceGrid.Cells.Views.ColumnHeader viewColumnHeader = new SourceGrid.Cells.Views.ColumnHeader();
                DevAge.Drawing.VisualElements.ColumnHeader backHeader = new DevAge.Drawing.VisualElements.ColumnHeader()
                {
                    BackColor = Color.LightGray,
                    Border = DevAge.Drawing.RectangleBorder.NoBorder
                };
                viewColumnHeader.Background = backHeader;
                viewColumnHeader.ForeColor = Color.Black;
                viewColumnHeader.Font = new Font("Arial", GridFontSize, FontStyle.Regular);
                viewColumnHeader.ElementSort.SortStyle = DevAge.Drawing.HeaderSortStyle.None;
                // viewNormal
                CellBackColorAlternate viewNormal = new CellBackColorAlternate(Color.LightBlue, Color.White);
                // ***
                // set first row
                gridSolutions.BorderStyle = BorderStyle.FixedSingle;
                gridSolutions.ColumnsCount = 7;
                gridSolutions.FixedRows = 1;
                gridSolutions.Rows.Insert(0);
                // header
                int iCol = 0;
                // A1xA2xA3
                var columnHeader = new ColumnHeader("A1 x A2 x A3")
                {
                    AutomaticSortEnabled = false,
                    View = viewColumnHeader
                };
                gridSolutions[0, iCol++] = columnHeader;
                // dimensions
                columnHeader = new ColumnHeader(string.Format(Properties.Resources.ID_DIMENSIONS, UnitsManager.LengthUnitString))
                {
                    AutomaticSortEnabled = false,
                    View = viewColumnHeader
                };
                gridSolutions[0, iCol++] = columnHeader;
                // weight
                columnHeader = new ColumnHeader(string.Format(Properties.Resources.ID_WEIGHT_WU, UnitsManager.MassUnitString))
                {
                    AutomaticSortEnabled = false,
                    View = viewColumnHeader
                };
                gridSolutions[0, iCol++] = columnHeader;
                // #packs 
                columnHeader = new ColumnHeader("#")
                {
                    AutomaticSortEnabled = false,
                    View = viewColumnHeader
                };
                gridSolutions[0, iCol++] = columnHeader;
                // weight
                columnHeader = new SourceGrid.Cells.ColumnHeader(string.Format(Properties.Resources.ID_PALLETWEIGHT, UnitsManager.MassUnitString))
                {
                    AutomaticSortEnabled = false,
                    View = viewColumnHeader
                };
                gridSolutions[0, iCol++] = columnHeader;
                // efficiency
                columnHeader = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_EFFICIENCYPERCENTAGE)
                {
                    AutomaticSortEnabled = false,
                    View = viewColumnHeader
                };
                gridSolutions[0, iCol++] = columnHeader;
                // maximum space
                columnHeader = new SourceGrid.Cells.ColumnHeader(string.Format(Resources.ID_MAXIMUMSPACE, UnitsManager.LengthUnitString))
                {
                    AutomaticSortEnabled = false,
                    View = viewColumnHeader
                };
                gridSolutions[0, iCol++] = columnHeader;

                int iRow = 0;
                foreach (AnalysisLayered analysis in _analyses)
                {
                    if (analysis is AnalysisCasePallet analysisCasePallet && analysis.Content is PackProperties pack)
                    {
                        int layerCount = analysisCasePallet.SolutionLay.Layers.Count;
                        if (layerCount < 1) continue;
                        int packPerLayerCount = analysisCasePallet.SolutionLay.Layers[0].BoxCount;
                        int itemCount = analysisCasePallet.Solution.ItemCount;
                        double palletWeight = analysisCasePallet.Solution.Weight;
                        double volumeEfficiency = analysisCasePallet.Solution.VolumeEfficiency;
                        double maximumSpace = analysisCasePallet.SolutionLay.LayerCount > 0
                            ? analysisCasePallet.SolutionLay.LayerMaximumSpace(0)
                            : 0;


                        gridSolutions.Rows.Insert(++iRow);
                        iCol = 0;
                        gridSolutions[iRow, iCol++] = new SourceGrid.Cells.Cell($"{pack.Arrangement.Length} x {pack.Arrangement.Width} x {pack.Arrangement.Height}");
                        gridSolutions[iRow, iCol++] = new SourceGrid.Cells.Cell($"{pack.OuterDimensions.X:0.#} x {pack.OuterDimensions.Y:0.#} x {pack.OuterDimensions.Z:0.#}");
                        gridSolutions[iRow, iCol++] = new SourceGrid.Cells.Cell($"{pack.Weight:0.###}");
                        gridSolutions[iRow, iCol++] = new SourceGrid.Cells.Cell($"{itemCount} = {packPerLayerCount} x {layerCount}");
                        gridSolutions[iRow, iCol++] = new SourceGrid.Cells.Cell($"{palletWeight:0.###}");
                        gridSolutions[iRow, iCol++] = new SourceGrid.Cells.Cell($"{volumeEfficiency:0.#}");
                        gridSolutions[iRow, iCol++] = new SourceGrid.Cells.Cell($"{maximumSpace:0.#}");
                    }
                }

                gridSolutions.AutoStretchColumnsToFitWidth = true;
                gridSolutions.AutoSizeCells();
                gridSolutions.Columns.StretchToFit();

                // select first solution
                if (gridSolutions.RowsCount > 1)
                    gridSolutions.Selection.SelectRow(1, true);
                else
                {
                    // grid empty -> clear drawing
                    _selectedAnalysis = null;
                    graphCtrlPack.Invalidate();
                    graphCtrlSolution.Invalidate();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        #endregion

        #region Event handlers
        private void OnBoxChanged(object sender, EventArgs e)
        {
            SetMinCaseDimensions();
            OnDataChanged(sender, e);
        }
        private void OnPalletChanged(object sender, EventArgs e)
        {
            SetMaxCaseDimensions();
            OnDataChanged(sender, e);
        }
        public void OnDataChanged(object sender, EventArgs e)
        {
            // stop timer
            _timer.Stop();
            // clear grid
            _analyses.Clear();
            FillGrid();
            // restart timer
            _timer.Start();
        }
        private void OnWrapperCheckChanged(object sender, EventArgs e)
        {
            var enabled = chkbWrapper.Checked;

            lbWrapperColor.Enabled = enabled;
            cbWrapperColor.Enabled = enabled;
            uCtrlWrapperWalls.Enabled = enabled;
            uCtrlWrapperThickness.Enabled = enabled;
            uCtrlWrapperSurfMass.Enabled = enabled;
            cbWrapperType.Enabled = enabled;

            OnDataChanged(sender, e); 
        }
        private void OnTrayCheckChanged(object sender, EventArgs e)
        {
            bool enabled = chkbTray.Checked;

            cbTrayColor.Enabled = enabled;
            uCtrlTrayWalls.Enabled = enabled;
            uCtrlTrayThickness.Enabled = enabled;
            uCtrlTraySurfMass.Enabled = enabled;
            uCtrlTrayHeight.Enabled = enabled;

            OnDataChanged(sender, e);
        }
        private void OnTimerTick(object sender, EventArgs e)
        {
            try
            {
                _timer.Stop();
                // recompute optimisation
                var packOptimizer = new PackOptimizer(
                    SelectedBox, SelectedPallet, BuildConstraintSet(),
                    BuildParamSetPackOptim()                
                    );
                _analyses = packOptimizer.BuildAnalyses( false );

                // refill solution grid
                FillGrid();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void OnSelChangeGrid(object sender, SourceGrid.RangeRegionChangedEventArgs e)
        {
            try
            {
                if (sender is SourceGrid.Selection.RowSelection select &&
                    select.Grid is SourceGrid.Grid g)
                {

                    var region = g.Selection.GetSelectionRegion();
                    var indexes = region.GetRowsIndex();
                    if (indexes.Length < 1 || indexes[0] < 1)
                        _selectedAnalysis = null;
                    else
                    {
                        _selectedAnalysis = _analyses[indexes[0] - 1];
                        // analysis name/description
                        if (null != _selectedAnalysis)
                        {
                            var box = SelectedBox;
                            if (_selectedAnalysis.Content is PackProperties pack)
                            {
                                AnalysisName = $"Analysis_{pack.Dim0}x{pack.Dim1}x{pack.Dim2}_{box.Name}_on_{_selectedAnalysis.Container.Name}";
                                AnalysisDescription = $"Packing {pack.Dim0}x{pack.Dim1}x{pack.Dim2} {box.Name} on {_selectedAnalysis.Container.Name}";
                            }
                            UpdateStatus(string.Empty);
                        }
                        else
                        {
                            AnalysisName = string.Empty;
                            AnalysisDescription = string.Empty;
                        }
                    }
                }

                graphCtrlPack.Invalidate();
                graphCtrlSolution.Invalidate();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        private void OnCreateAnalysis(object sender, EventArgs e)
        {
            try
            {
                // selected analysis -> get pack
                var analysisSel = SelectedAnalysis;
                if (analysisSel.Content is PackProperties packSel)
                {
                    packSel.ID.SetNameDesc(AnalysisName, AnalysisName);

                    // create pack
                    var packProperties = _doc.CreateNewPack(packSel);
                    // create analysis
                    _doc.CreateNewAnalysisCasePallet(
                        AnalysisName, AnalysisDescription,
                        packProperties, SelectedPallet,
                        new List<InterlayerProperties>(), null, null, null,
                        BuildConstraintSet(),
                        analysisSel.SolutionLay.LayerEncaps,
                        new List<ConveyorSetting>());
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        #endregion

        #region Private properties
        private PackableBrick SelectedBox => cbBoxes.SelectedType as PackableBrick;
        private PalletProperties SelectedPallet => cbPallets.SelectedType as PalletProperties;
        private AnalysisCasePallet SelectedAnalysis => _selectedAnalysis as AnalysisCasePallet;
        private Vector2D Overhang => new Vector2D(uCtrlOverhang.ValueX, uCtrlOverhang.ValueY);
        private Vector3D DimensionsMin=> new Vector3D(uCtrlPackDimensionsMin.ValueX, uCtrlPackDimensionsMin.ValueY, uCtrlPackDimensionsMin.ValueZ);
        private Vector3D DimensionsMax => new Vector3D(uCtrlPackDimensionsMax.ValueX, uCtrlPackDimensionsMax.ValueY, uCtrlPackDimensionsMax.ValueZ);
        private int BoxPerCase => (int)nudNumber.Value;
        private bool ForceVerticalBoxOrientation
        {
            get => chkVerticalOrientationOnly.Checked;
            set => chkVerticalOrientationOnly.Checked = value;
        }
        private double MaximumPalletHeight
        {
            get => uCtrlPalletHeight.Value;
            set => uCtrlPalletHeight.Value = value;
        }
        private int[] NoWrapperWalls => new[] {uCtrlWrapperWalls.NoX, uCtrlWrapperWalls.NoY, uCtrlWrapperWalls.NoZ};
        private int[] NoTrayWalls => new[] { uCtrlTrayWalls.NoX, uCtrlTrayWalls.NoY, uCtrlTrayWalls.NoZ };
        private double WrapperWallThickness
        {
            get => uCtrlWrapperThickness.Value;
            set => uCtrlWrapperThickness.Value = value;
        }
        private double TrayWallThickness
        {
            get => uCtrlTrayThickness.Value;
            set => uCtrlTrayThickness.Value = value;
        }
        private double WrapperWallSurfMass
        {
            get => uCtrlWrapperSurfMass.Value;
            set => uCtrlWrapperSurfMass.Value = value;
        }

        private double TrayWallSurfaceMass => uCtrlTraySurfMass.Value;
        private bool HasWrapper
        {
            get => chkbWrapper.Checked;
            set => chkbWrapper.Checked = value;
        }
        private bool HasTray
        {
            get =>chkbTray. Checked;
            set => chkbTray.Checked = value;
        }
        private Color WrapperColor => cbWrapperColor.Color;
        private Color TrayColor => cbTrayColor.Color;
        private PackWrapper.WType WrapperType => (PackWrapper.WType)cbWrapperType.SelectedIndex;
        private double TrayHeight => uCtrlTrayHeight.Value;

        private string AnalysisName
        {
            get => tbAnalysisName.Text;
            set => tbAnalysisName.Text = value;
        }
        private string AnalysisDescription
        {
            get => tbAnalysisDescription.Text;
            set => tbAnalysisDescription.Text = value;
        }
        private int GridFontSize { get; set; }
        #endregion

        #region Helpers
        private void SetMinCaseDimensions()
        {
            var boxProperties = SelectedBox;
            if (null == boxProperties) return;
            double minDim = Math.Min(boxProperties.Length, Math.Min(boxProperties.Width, boxProperties.Height));
            if ((int)nudNumber.Value > 8)
                minDim *= 2;
            // set min dimension
            uCtrlPackDimensionsMin.ValueX = minDim;
            uCtrlPackDimensionsMin.ValueY = minDim;
            uCtrlPackDimensionsMin.ValueZ = minDim;
        }
        private void SetMaxCaseDimensions()
        { 
            var palletProperties = SelectedPallet;
            if (null == palletProperties) return;
            if (MaximumPalletHeight <= palletProperties.Height)
                MaximumPalletHeight = palletProperties.Height;

            double length = 0.5 * palletProperties.Length;
            uCtrlPackDimensionsMax.ValueX = length;
            uCtrlPackDimensionsMax.ValueY = length;
            uCtrlPackDimensionsMax.ValueZ = length;
        }
        private ConstraintSetCasePallet BuildConstraintSet()
        {
            ConstraintSetCasePallet constraintSet = new ConstraintSetCasePallet();
            constraintSet.SetAllowedOrientations(new[] { false, false, true });
            constraintSet.SetMaxHeight( new OptDouble(true, MaximumPalletHeight));
            constraintSet.Overhang = Overhang;
            return constraintSet;
        }
        private ParamSetPackOptim BuildParamSetPackOptim()
        {
            return new ParamSetPackOptim(
                BoxPerCase,
                DimensionsMin,
                DimensionsMax,
                ForceVerticalBoxOrientation,
                HasWrapper, WrapperColor,  NoWrapperWalls, WrapperWallThickness, WrapperWallSurfMass, WrapperType,
                HasTray, TrayColor, NoTrayWalls, TrayWallThickness, TrayWallSurfaceMass, TrayHeight);
        }
        #endregion

        #region Data members
        private DocumentSB _doc;
        private List<AnalysisLayered> _analyses = new List<AnalysisLayered>();
        private AnalysisLayered _selectedAnalysis;
        private static ILog _log = LogManager.GetLogger(typeof(FormOptimizePack));
        #endregion
    }
}
