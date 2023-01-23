#region Using directives
using System;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing;
using System.Collections.Generic;
// Sharp3D
using Sharp3D.Math.Core;
// log4net
using log4net;
// treeDiM
using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class DockContentAnalysisCasePallet : DockContentAnalysisEdit
    {
        #region Constructor
        public DockContentAnalysisCasePallet(IDocument document, AnalysisCasePallet analysis)
            : base(document, analysis)
        {
            InitializeComponent();
        }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // --- initialize drawing container
            graphCtrlSolution.Viewer = new ViewerSolution(_solution);
            graphCtrlSolution.Invalidate();
            graphCtrlSolution.VolumeSelected += OnLayerSelected;
            graphCtrlSolution.ShowBoxIds = Settings.Default.ShowBoxIds;
            // ---

            // --- initialize layer controls
            UpdateControls();

            uCtrlMaxPalletHeight.Value = _analysis.ConstraintSet.OptMaxHeight.Value;
            uCtrlOptMaximumWeight.Value = _analysis.ConstraintSet.OptMaxWeight;
            uCtrlOptMaxNumber.Value = _analysis.ConstraintSet.OptMaxNumber;

            uCtrlMaxPalletHeight.ValueChanged += new UCtrlDouble.ValueChangedDelegate(OnCriterionChanged);
            uCtrlOptMaximumWeight.ValueChanged += new UCtrlOptDouble.ValueChangedDelegate(OnCriterionChanged);
            uCtrlOptMaxNumber.ValueChanged += new UCtrlOptInt.ValueChangedDelegate(OnCriterionChanged);

            chkbPalletCap.CheckedChanged += new EventHandler(OnCriterionChanged);
            chkbPalletFilm.CheckedChanged += new EventHandler(OnCriterionChanged);
            chkbPalletSleeve.CheckedChanged += new EventHandler(OnCriterionChanged);
            chkbPalletCornersTopX.CheckedChanged += new EventHandler(OnCriterionChanged);
            chkbPalletCornersTopY.CheckedChanged += new EventHandler(OnCriterionChanged);
            chkbPalletSleeve.CheckedChanged += new EventHandler(OnCriterionChanged);

            AnalysisCasePallet analysisCasePallet = _analysis as AnalysisCasePallet;


            cbTopInterlayer.Initialize(Document as Document, this, analysisCasePallet?.TopInterlayerProperties);
            chkbTopInterlayer.Enabled = (cbTopInterlayer.Items.Count > 0);

            ComboBoxHelpers.FillCombo(PalletCorners, cbPalletCorners, analysisCasePallet?.PalletCornerProperties);
            chkbPalletCorners.Enabled = (cbPalletCorners.Items.Count > 0);
            ComboBoxHelpers.FillCombo(PalletCorners, cbPalletCornersTop, analysisCasePallet?.PalletCornerTopProperties);
            chkbPalletCornersTopX.Enabled = (cbPalletCornersTop.Items.Count > 0);
            ComboBoxHelpers.FillCombo(PalletCaps, cbPalletCap, analysisCasePallet?.PalletCapProperties);
            chkbPalletCap.Enabled = (cbPalletCap.Items.Count > 0);
            ComboBoxHelpers.FillCombo(PalletFilms, cbPalletFilm, analysisCasePallet?.PalletFilmProperties);
            chkbPalletFilm.Enabled = (cbPalletFilm.Items.Count > 0);
            ComboBoxHelpers.FillCombo(PalletLabels, cbPalletLabels, null);
            bnAdd.Enabled = (cbPalletLabels.Items.Count > 0);

            chkbPalletCornersTopX.Enabled = (cbPalletCornersTop.Items.Count > 0);
            chkbPalletCornersTopY.Enabled = (cbPalletCornersTop.Items.Count > 0);

            if (null != analysisCasePallet)
            {
                chkbPalletCorners.Checked = null != analysisCasePallet.PalletCornerProperties;
                chkbPalletCornersTopX.Checked = null != analysisCasePallet.PalletCornerTopProperties && analysisCasePallet.PalletCornersTopX;
                chkbPalletCornersTopY.Checked = null != analysisCasePallet.PalletCornerTopProperties && analysisCasePallet.PalletCornersTopY;

                chkbPalletCap.Checked = null != analysisCasePallet.PalletCapProperties;
                chkbPalletFilm.Checked = null != analysisCasePallet.PalletFilmProperties;
                ctrlStrapperSet.StrapperSet = analysisCasePallet.StrapperSet;

                PalletFilmTopCovering = analysisCasePallet.PalletFilmTopCovering;

                HasPalletSleeve = analysisCasePallet.HasPalletSleeve;
                PalletSleeveColor = analysisCasePallet.PalletSleeveColor;
            }
            // ---
            // --- initialize grid control
            FillGrid();
            UpdateGrid();
            FillGridLabel();
            // ---

            // handling row change in label grids
            _gripLabelsControllerEvent.ValueChanged += new EventHandler(OnPalletProtectionChanged);
            _initialized = true;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Document.RemoveView(this);
        }
        #endregion

        #region Override DockContentAnalysisEdit
        protected override string GridCaption => Resources.ID_PALLET;
        protected override bool AllowExport3D => true;
        #endregion

        public override bool  Accept(Control ctrl, ItemBase itemBase)
        {
            // base implementation
            if (base.Accept(ctrl, itemBase)) return true;
            // interlayer
            if (ctrl == cbTopInterlayer && itemBase is InterlayerProperties interlayer)
                return _analysis.AllowInterlayer(interlayer);
            return false;
        }

        #region Private properties (Pallet corners, pallet caps, pallet film)
        private PalletCornerProperties SelectedPalletCorners
        {
            get
            {
                if (cbPalletCorners.Items.Count > 0 && chkbPalletCorners.Checked)
                {
                    if (cbPalletCorners.SelectedItem is ItemBaseCB item)
                        return  item.Item as PalletCornerProperties;
                }
                return null;
            }
        }
        private PalletCornerProperties SelectedPalletCornersTop
        {
            get
            {
                if (cbPalletCornersTop.Items.Count > 0
                    && (chkbPalletCornersTopX.Checked || chkbPalletCornersTopY.Checked) )
                {
                    if (cbPalletCornersTop.SelectedItem is ItemBaseCB item)
                        return item.Item as PalletCornerProperties;
                }
                return null;
            }
        }
        private PalletCapProperties SelectedPalletCap
        {
            get
            {
                if (cbPalletCap.Items.Count > 0 && chkbPalletCap.Checked)
                {
                    if (cbPalletCap.SelectedItem is ItemBaseCB item)
                        return item.Item as PalletCapProperties;
                }
                return null;
            }
        }
        private InterlayerProperties SelectedTopInterlayer
        {
            get
            {
                if (cbTopInterlayer.Items.Count > 0 && chkbTopInterlayer.Checked)
                {
                    if (cbTopInterlayer.SelectedType is InterlayerProperties interlayer)
                        return interlayer;
                }
                return null;
            }
        }

        private PalletFilmProperties SelectedPalletFilm
        {
            get
            {
                if (cbPalletFilm.Items.Count > 0 && chkbPalletFilm.Checked)
                {
                    if (cbPalletFilm.SelectedItem is ItemBaseCB item)
                        return item.Item as PalletFilmProperties;
                }
                return null;
            }
        }
        private PalletLabelProperties SelectedPalletLabel
        {
            get
            {
                if (cbPalletLabels.Items.Count > 0)
                {
                    if (cbPalletLabels.SelectedItem is ItemBaseCB item)
                        return item.Item as PalletLabelProperties;
                }
                return null;
            }
        }

        private double PalletFilmTopCovering
        {
            get => uCtrlPalletFilmCovering.Value;
            set => uCtrlPalletFilmCovering.Value = value;
        }
        private bool PalletCornerTopX
        {
            get => chkbPalletCornersTopX.Checked;
            set => chkbPalletCornersTopX.Checked = value;
        }
        private bool PalletCornerTopY
        {
            get => chkbPalletCornersTopY.Checked;
            set => chkbPalletCornersTopY.Checked = value;
        }
        private bool HasPalletSleeve
        {
            get => chkbPalletSleeve.Checked;
            set => chkbPalletSleeve.Checked = value;
        }
        private bool HasTopInterlayer
        {
            get => chkbTopInterlayer.Checked;
            set => chkbTopInterlayer.Checked = value;
        }
        private Color PalletSleeveColor
        {
            get => cbPalletSleeveColor.Color;
            set => cbPalletSleeveColor.Color = value;
        }
        #endregion

        #region IItemListener implementation
        public override void Update(ItemBase item)
        {
            base.Update(item);
            graphCtrlSolution.Invalidate();
        }
        public override void Kill(ItemBase item)
        {
            base.Kill(item);
            if (null != _analysis)
                _analysis.RemoveListener(this);
        }
        #endregion

        #region IDrawingContainer
        #endregion

        #region Private properties / Helpers
        private ItemBase[] PalletCorners
        {
            get
            {
                Document doc = Document as Document;
                return doc.GetByType(typeof(PalletCornerProperties)).ToArray(); 
            }
        }
        private ItemBase[] PalletCaps
        {
            get
            {
                Document doc = Document as Document;
                return doc.GetByType(typeof(PalletCapProperties)).ToArray(); 
            }
        }
        private ItemBase[] PalletFilms
        {
            get
            {
                Document doc = Document as Document;
                return doc.GetByType(typeof(PalletFilmProperties)).ToArray(); 
            }
        }
        private ItemBase[] PalletLabels
        {
            get
            {
                Document doc = Document as Document;
                return doc.GetByType(typeof(PalletLabelProperties)).ToArray();
            }
        }
        #endregion

        #region Grid filling virtual method
        public override void FillGrid()
        { 
            // clear grid
            gridSolution.Rows.Clear();
            // border
            gridSolution.BorderStyle = BorderStyle.FixedSingle;
            gridSolution.ColumnsCount = 2;
            gridSolution.FixedColumns = 1;
            gridSolution.FixedRows = 1;
        }
        public override void UpdateGrid()
        {
            try
            {
                // sanity check
                if (gridSolution.ColumnsCount < 2)  return;
                // remove all existing rows
                gridSolution.Rows.Clear();

                // cell visual properties
                var vPropHeader = CellProperties.VisualPropHeader;
                var vPropValue = CellProperties.VisualPropValue;

                int iRow = -1;
                // pallet caption
                gridSolution.Rows.Insert(++iRow);
                gridSolution[iRow, 0] = new ColumnHeaderSolution(Resources.ID_PALLET) { ColumnSpan = 1 };
                gridSolution[iRow, 1] = new ColumnHeaderSolution(string.Empty) { ColumnSpan = 1 };

                // layer #
                gridSolution.Rows.Insert(++iRow);
                gridSolution[iRow, 0] = new SourceGrid.Cells.RowHeader(Resources.ID_LAYERNUMBER) { View = vPropValue };
                gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(_solution.LayerCount);
                // pallet
                gridSolution.Rows.Insert(++iRow);
                gridSolution[iRow, 0] = new SourceGrid.Cells.RowHeader($"{Resources.ID_PALLET} ({Analysis.Container.Name})") { View = vPropValue };
                gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(1);
                // interlayers #
                foreach (var interPair in _solution.InterlayerCounts)
                {
                    gridSolution.Rows.Insert(++iRow);
                    gridSolution[iRow, 0] = new SourceGrid.Cells.RowHeader($"{Resources.ID_INTERLAYER} ({interPair.first.Name})") { View = vPropValue };
                    gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(interPair.second);
                }
                // *** Item # (Recursive count)
                RecurInsertContent(ref iRow, _analysis.Content, _solution.ItemCount);
                // ***
                // outer dimensions
                BBox3D bboxGlobal = _solution.BBoxGlobal;
                // ---
                gridSolution.Rows.Insert(++iRow);
                gridSolution[iRow, 0] = new RowHeaderPropSolution(string.Format(Resources.ID_OUTERDIMENSIONS, UnitsManager.LengthUnitString));
                gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(string.Format(CultureInfo.InvariantCulture, "{0:0.#} x {1:0.#} x {2:0.#}", bboxGlobal.Length, bboxGlobal.Width, bboxGlobal.Height));
                // load dimensions
                BBox3D bboxLoad = _solution.BBoxLoad;
                // ---
                gridSolution.Rows.Insert(++iRow);
                gridSolution[iRow, 0] = new RowHeaderPropSolution(string.Format(Resources.ID_LOADDIMENSIONS, UnitsManager.LengthUnitString));
                gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(string.Format(CultureInfo.InvariantCulture, "{0:0.#} x {1:0.#} x {2:0.#}", bboxLoad.Length, bboxLoad.Width, bboxLoad.Height));
                // net weight
                if (_solution.HasNetWeight)
                {
                    gridSolution.Rows.Insert(++iRow);
                    gridSolution[iRow, 0] = new RowHeaderPropSolution(string.Format(Resources.ID_NETWEIGHT_WU, UnitsManager.MassUnitString));
                    gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(string.Format(CultureInfo.InvariantCulture, "{0:0.#}", _solution.NetWeight.Value));
                }
                // load weight
                gridSolution.Rows.Insert(++iRow);
                gridSolution[iRow, 0] = new RowHeaderPropSolution(string.Format(Resources.ID_LOADWEIGHT_WU, UnitsManager.MassUnitString));
                gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(
                    string.Format(CultureInfo.InvariantCulture, "{0:0.#}", _solution.LoadWeight));
                // total weight
                gridSolution.Rows.Insert(++iRow);
                gridSolution[iRow, 0] = new RowHeaderPropSolution(string.Format(Resources.ID_TOTALWEIGHT_WU, UnitsManager.MassUnitString));
                gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(
                    string.Format(CultureInfo.InvariantCulture, "{0:0.#}", _solution.Weight));
                // volume efficiency
                gridSolution.Rows.Insert(++iRow);
                gridSolution[iRow, 0] = new RowHeaderPropSolution(Resources.ID_VOLUMEEFFICIENCY);
                gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(
                    string.Format(CultureInfo.InvariantCulture, "{0:0.#}", _solution.VolumeEfficiency));

                // ### layers : begin
                for (int i = 0; i < _solution.NoLayerTypesUsed; ++i)
                {
                    // layer caption
                    gridSolution.Rows.Insert(++iRow);
                    gridSolution[iRow, 0] = new SourceGrid.Cells.RowHeader(_solution.LayerCaption(i)) { ColumnSpan = 2, View = vPropHeader };

                    // *** Item # (recursive count)
                    RecurInsertContent(ref iRow, _analysis.Content, _solution.LayerBoxCount(i));
                    // ***
                    // layer weight
                    gridSolution.Rows.Insert(++iRow);
                    gridSolution[iRow, 0] = new SourceGrid.Cells.RowHeader(string.Format(Resources.ID_WEIGHT_WU, UnitsManager.MassUnitString)) { View = vPropValue };
                    gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(
                        string.Format(CultureInfo.InvariantCulture, "{0:0.#}", _solution.LayerWeight(i)));
                    // layer space
                    gridSolution.Rows.Insert(++iRow);
                    gridSolution[iRow, 0] = new SourceGrid.Cells.RowHeader(Resources.ID_SPACES) { View = vPropValue };
                    gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(
                        string.Format(CultureInfo.InvariantCulture, "{0:0.#}", _solution.LayerMaximumSpace(i)));
                }
                // ### layers : end
                gridSolution.AutoSizeMode = AutoSizeMode.GrowAndShrink;

                gridSolution.AutoSizeCells();
                gridSolution.Columns.StretchToFit();
                gridSolution.AutoStretchColumnsToFitWidth = true;
                gridSolution.Invalidate();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void OnLayerSelected(int id)
        {
            try
            {
                _solution.SelectLayer(id);
                UpdateControls();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Event handlers
        private void OnSizeChanged(object sender, EventArgs e)
        {
            int splitDistance = splitContainerHoriz.Size.Height - 120;
            if (splitDistance > 0)
                splitContainerHoriz.SplitterDistance = splitDistance;
        }
        private void OnPalletProtectionChanged(object sender, EventArgs e)
        {
            if (!_initialized)  return;

            cbPalletCorners.Enabled = chkbPalletCorners.Checked;
            cbPalletCornersTop.Enabled = cbPalletCornersTop.Items.Count > 0;
            cbPalletCap.Enabled = chkbPalletCap.Checked;
            cbPalletFilm.Enabled = chkbPalletFilm.Checked;
            uCtrlPalletFilmCovering.Enabled = chkbPalletFilm.Checked;

            if (_solution.Analysis is AnalysisCasePallet analysisCasePallet)
            {
                analysisCasePallet.PalletCornerProperties = SelectedPalletCorners;
                analysisCasePallet.PalletCornerTopProperties = SelectedPalletCornersTop;
                analysisCasePallet.PalletCornersTopX = PalletCornerTopX;
                analysisCasePallet.PalletCornersTopY = PalletCornerTopY;
                analysisCasePallet.TopInterlayerProperties = SelectedTopInterlayer;
                analysisCasePallet.PalletCapProperties = SelectedPalletCap;
                analysisCasePallet.PalletFilmProperties = SelectedPalletFilm;
                analysisCasePallet.StrapperSet = ctrlStrapperSet.StrapperSet;

                analysisCasePallet.PalletFilmTopCovering = PalletFilmTopCovering;

                analysisCasePallet.HasPalletSleeve = HasPalletSleeve;
                analysisCasePallet.PalletSleeveColor = PalletSleeveColor;

                analysisCasePallet.PalletLabels = LoadPalletLabelInst();
            }
            OnCriterionChanged(sender, e);
        }
        private void OnCriterionChanged(object sender, EventArgs args)
        {
            try
            {
                ConstraintSetCasePallet constraintSet = _solution.Analysis.ConstraintSet as ConstraintSetCasePallet;
                constraintSet.SetMaxHeight(new OptDouble(true, uCtrlMaxPalletHeight.Value));
                constraintSet.OptMaxWeight = uCtrlOptMaximumWeight.Value;
                constraintSet.OptMaxNumber = uCtrlOptMaxNumber.Value;
                _solution.RebuildSolutionItemList();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }

            // update drawing & grid
            graphCtrlSolution.Invalidate();
            UpdateGrid();
        }
        #endregion

        #region Labels
        private void OnBnAddLabel(object sender, EventArgs e)
        {
            try
            {
                var selectedPalletLabel = SelectedPalletLabel;

                if (_solution.Analysis is AnalysisCasePallet analysisCasePallet)
                {
                    analysisCasePallet.PalletLabels.Add(
                        new PalletLabelInst(selectedPalletLabel, new Vector2D(500.0, 500.0), HalfAxis.HAxis.AXIS_Y_P));
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
            FillGridLabel();
            OnPalletProtectionChanged(sender, e);
        }
        private void FillGridLabel()
        {
            try
            {
                // remove all existing rows
                gridLabels.Rows.Clear();

                // *** IViews
                // viewColumnHeader
                SourceGrid.Cells.Views.ColumnHeader viewColumnHeader = new SourceGrid.Cells.Views.ColumnHeader()
                {
                    Background = new DevAge.Drawing.VisualElements.ColumnHeader()
                    {
                        BackColor = Color.LightGray,
                        Border = DevAge.Drawing.RectangleBorder.NoBorder
                    },
                    ForeColor = Color.Black,
                    Font = new Font("Arial", 8, FontStyle.Regular),
                };
                viewColumnHeader.ElementSort.SortStyle = DevAge.Drawing.HeaderSortStyle.None;
                // initialize
                gridLabels.BorderStyle = BorderStyle.FixedSingle;
                gridLabels.ColumnsCount = 5;
                gridLabels.FixedRows = 1;
                // viewNormal
                CellBackColorAlternate viewNormal = new CellBackColorAlternate(Color.LightBlue, Color.White);
                // ***
                int iRow = -1;

                // header
                int iCol = 0;
                gridLabels.Rows.Insert(++iRow);
                gridLabels[iRow, iCol] = new SourceGrid.Cells.RowHeader(Resources.ID_LABEL) { View = viewColumnHeader };
                gridLabels[iRow, ++iCol] = new SourceGrid.Cells.RowHeader(Resources.ID_PALLETSIDE) { View = viewColumnHeader };
                gridLabels[iRow, ++iCol] = new SourceGrid.Cells.RowHeader(Resources.ID_POSX) { View = viewColumnHeader };
                gridLabels[iRow, ++iCol] = new SourceGrid.Cells.RowHeader(Resources.ID_POSY) { View = viewColumnHeader };
                gridLabels[iRow, ++iCol] = new SourceGrid.Cells.RowHeader(Resources.ID_DELETE) { View = viewColumnHeader };

                if (_solution.Analysis is AnalysisCasePallet analysisCasePallet)
                {
                    // handling delete event
                    SourceGrid.Cells.Controllers.CustomEvents buttonDelete = new SourceGrid.Cells.Controllers.CustomEvents();
                    buttonDelete.Click += new EventHandler(OnDeleteLabel);                    

                    foreach (var pli in analysisCasePallet.PalletLabels)
                    {
                        gridLabels.Rows.Insert(++iRow);
                        iCol = 0;
                        // name                        
                        gridLabels[iRow, iCol] = new SourceGrid.Cells.Cell(pli.PalletLabelProperties.Name)
                        {
                            Tag = pli.PalletLabelProperties
                        };
                        // pallet side
                        SourceGrid.Cells.Editors.ComboBox cbPalletSide = new SourceGrid.Cells.Editors.ComboBox(typeof(string), new string[] { "X+", "X-", "Y+", "Y-" }, false);
                        cbPalletSide.EditableMode = SourceGrid.EditableMode.Focus | SourceGrid.EditableMode.SingleClick | SourceGrid.EditableMode.AnyKey;
                        cbPalletSide.SetEditValue(HalfAxis.ToAbbrev(pli.Side));
                        gridLabels[iRow, ++iCol] = new SourceGrid.Cells.Cell(HalfAxis.ToAbbrev(pli.Side), cbPalletSide) { View = viewNormal };
                        gridLabels[iRow, iCol].Editor = cbPalletSide;
                        gridLabels[iRow, iCol].AddController(_gripLabelsControllerEvent);
                        // position.X 
                        gridLabels[iRow, ++iCol] = new SourceGrid.Cells.Cell((decimal)pli.Position.X) { View = viewNormal };
                        SourceGrid.Cells.Editors.NumericUpDown l_NumericUpDownEditorX = new SourceGrid.Cells.Editors.NumericUpDown(typeof(int), 10000, 0, 1);
                        l_NumericUpDownEditorX.SetEditValue((decimal)pli.Position.X);
                        gridLabels[iRow, iCol].Editor = l_NumericUpDownEditorX;
                        gridLabels[iRow, iCol].AddController(_gripLabelsControllerEvent);
                        // position.Y
                        gridLabels[iRow, ++iCol] = new SourceGrid.Cells.Cell((decimal)pli.Position.Y) { View = viewNormal };
                        SourceGrid.Cells.Editors.NumericUpDown l_NumericUpDownEditorY = new SourceGrid.Cells.Editors.NumericUpDown(typeof(int), 10000, 0, 1);
                        l_NumericUpDownEditorY.SetEditValue((decimal)pli.Position.Y);
                        gridLabels[iRow, iCol].Editor = l_NumericUpDownEditorY;
                        gridLabels[iRow, iCol].AddController(_gripLabelsControllerEvent);
                        // delete button
                        gridLabels[iRow, ++iCol] = new SourceGrid.Cells.Button("") { Image = Resources.Delete };
                        gridLabels[iRow, iCol].AddController(buttonDelete);
                    }

                    gridLabels.AutoSizeCells();
                    gridLabels.Columns.StretchToFit();
                    gridLabels.AutoStretchColumnsToFitWidth = true;
                    gridLabels.Invalidate();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private List<PalletLabelInst> LoadPalletLabelInst()
        {
            List<PalletLabelInst> palletLabelInst = new List<PalletLabelInst>();

            for (int iRow = 1; iRow < gridLabels.RowsCount; ++iRow)
            {
                try
                {
                    // PalletLabelProperties from tag
                    PalletLabelProperties pl = gridLabels[iRow, 0].Tag as PalletLabelProperties;
                    // pallet side
                    SourceGrid.Cells.Editors.ComboBox cbSide = gridLabels[iRow, 1].Editor as SourceGrid.Cells.Editors.ComboBox;
                    string sSide = (string)cbSide.GetEditedValue();
                    if (string.IsNullOrEmpty(sSide))
                        continue;
                    // position
                    Vector2D position = new Vector2D();
                    SourceGrid.Cells.Editors.NumericUpDown upDownEditorX = gridLabels[iRow, 2].Editor as SourceGrid.Cells.Editors.NumericUpDown;
                    position.X = (int)upDownEditorX.GetEditedValue();
                    SourceGrid.Cells.Editors.NumericUpDown upDownEditorY = gridLabels[iRow, 3].Editor as SourceGrid.Cells.Editors.NumericUpDown;
                    position.Y = (int)upDownEditorY.GetEditedValue();
                    // add pallet label instance
                    palletLabelInst.Add(new PalletLabelInst(pl, position, HalfAxis.ParseAbbrev(sSide)));
                }
                catch (Exception ex)
                {
                    _log.Error(ex.ToString());
                }
            }
            return palletLabelInst;
        }

        protected int GridFontSize => Settings.Default.GridFontSize;

        private void OnDeleteLabel(object sender, EventArgs e)
        {
            SourceGrid.CellContext context = (SourceGrid.CellContext)sender;
            int iSel = context.Position.Row - 1;
            try
            {
                if ((_solution.Analysis is AnalysisCasePallet analysisCasePallet) && (iSel < analysisCasePallet.PalletLabels.Count))
                    analysisCasePallet.PalletLabels.RemoveAt(iSel);
             }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
            FillGridLabel();
            OnPalletProtectionChanged(sender, e);
        }
        #endregion

        #region Toolbar event handlers
        private void OnBack(object sender, EventArgs e)
        {
            // close this form
            Close();
            // call edit analysis
            Document.EditAnalysis(_analysis);
        }
        private void OnGenerateReport(object sender, EventArgs e)
        {
            FormMain.GenerateReport(_analysis);
        }
        #endregion

        #region Data members
        private bool _initialized = false;
        // grids
        protected SourceGrid.Cells.Controllers.CustomEvents _gripLabelsControllerEvent = new SourceGrid.Cells.Controllers.CustomEvents();
        protected SourceGrid.Cells.Controllers.CustomEvents _deleteLabelEvent = new SourceGrid.Cells.Controllers.CustomEvents();
        // logging
        protected static readonly new ILog _log = LogManager.GetLogger(typeof(DockContentAnalysisCasePallet));
        #endregion
    }
}
