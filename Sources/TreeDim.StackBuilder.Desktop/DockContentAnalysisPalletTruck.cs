#region Using directives
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Collections.Generic;

using log4net;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class DockContentAnalysisPalletTruck : DockContentView
    {
        #region Constructor
        public DockContentAnalysisPalletTruck(IDocument document, AnalysisPalletTruck analysis)
            : base(document)
        {
            _analysis = analysis;
            _analysis.AddListener(this);
            _solution = analysis.SolutionLay;

            InitializeComponent();
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

        #region Public properties
        public AnalysisPalletTruck Analysis
        {
            get { return _analysis; }  
        }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // --- window caption
            Text = _analysis.Name + " _ " + _analysis.ParentDocument.Name;
            // --- initialize drawing container
            graphCtrlSolution.Viewer = new ViewerSolution(_solution);
            graphCtrlSolution.Invalidate();

            uCtrlMaxNoPallets.Value = _analysis.ConstraintSet.OptMaxNumber;

            // --- initialize grid control
            FillGrid();
            UpdateGrid();
            // ---
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Document.RemoveView(this);
        }
        #endregion

        #region Grid filling
        private void FillGrid()
        {
            // clear grid
            gridSolution.Rows.Clear();
            // border
            gridSolution.BorderStyle = BorderStyle.FixedSingle;
            gridSolution.ColumnsCount = 2;
            gridSolution.FixedColumns = 1;
            gridSolution.FixedRows = 1;
        }
        public void RecurInsertContent(ref int iRow, Packable content, int number)
        {
            gridSolution.Rows.Insert(++iRow);
            SourceGrid.Cells.RowHeader rowHeader = new SourceGrid.Cells.RowHeader($"{content.DetailedName} #")
            {
                View = CellProperties.VisualPropValue
            };
            gridSolution[iRow, 0] = rowHeader;
            gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(number);

            List<Pair<Packable, int>> listContentItems = new List<Pair<Packable, int>>();
            if (content.InnerContent(ref listContentItems))
            {
                foreach (var item in listContentItems)
                {
                    RecurInsertContent(ref iRow, item.first, item.second * number);
                }
            }
        }
        private void UpdateGrid()
        {
            try
            {
                // sanity check
                if (gridSolution.ColumnsCount < 2)
                    return;
                // remove all existing rows
                gridSolution.Rows.Clear();
                // cell visual properties
                var vPropHeader = CellProperties.VisualPropHeader;
                var vPropValue = CellProperties.VisualPropValue;

                int iRow = -1;
                // pallet caption
                gridSolution.Rows.Insert(++iRow);
                gridSolution[iRow, 0] = new ColumnHeaderSolution(Resources.ID_TRUCK) { ColumnSpan = 1 };
                gridSolution[iRow, 1] = new ColumnHeaderSolution(string.Empty) { ColumnSpan = 1 };
                // layer #
                gridSolution.Rows.Insert(++iRow);
                gridSolution[iRow, 0] = new SourceGrid.Cells.RowHeader(Resources.ID_LAYERNUMBER) { View = vPropValue };
                gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(_solution.LayerCount);
                // *** Item # (Recursive count)
                Packable content = _analysis.Content;
                RecurInsertContent(ref iRow, content, _solution.ItemCount);
                // ***
                // load dimensions
                BBox3D bboxLoad = _solution.BBoxLoad;
                // ---
                gridSolution.Rows.Insert(++iRow);
                gridSolution[iRow, 0] = new SourceGrid.Cells.RowHeader(
                    string.Format(Resources.ID_LOADDIMENSIONS, UnitsManager.LengthUnitString)) { View = vPropValue };
                gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(
                    string.Format(CultureInfo.InvariantCulture, "{0:0.#} x {1:0.#} x {2:0.#}", bboxLoad.Length, bboxLoad.Width, bboxLoad.Height));
                // net weight
                if (_solution.HasNetWeight)
                {
                    gridSolution.Rows.Insert(++iRow);
                    gridSolution[iRow, 0] = new SourceGrid.Cells.RowHeader(
                        string.Format(Resources.ID_NETWEIGHT_WU, UnitsManager.MassUnitString)) { View = vPropValue };
                    gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(
                        string.Format(CultureInfo.InvariantCulture, "{0:0.#}", _solution.NetWeight.Value));
                }
                // load weight
                gridSolution.Rows.Insert(++iRow);
                gridSolution[iRow, 0] = new SourceGrid.Cells.RowHeader(
                    string.Format(Resources.ID_LOADWEIGHT_WU, UnitsManager.MassUnitString)) { View = vPropValue };
                gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(
                    string.Format(CultureInfo.InvariantCulture, "{0:0.#}", _solution.LoadWeight));
                // total weight
                gridSolution.Rows.Insert(++iRow);
                gridSolution[iRow, 0] = new SourceGrid.Cells.RowHeader(string.Format(Resources.ID_TOTALWEIGHT_WU, UnitsManager.MassUnitString)) {View = vPropValue };
                gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(
                    string.Format(CultureInfo.InvariantCulture, "{0:0.#}", _solution.Weight));
                // volume efficiency
                gridSolution.Rows.Insert(++iRow);
                gridSolution[iRow, 0] = new SourceGrid.Cells.RowHeader(Resources.ID_VOLUMEEFFICIENCY) { View = vPropValue };
                gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(
                    string.Format(CultureInfo.InvariantCulture, "{0:0.#}", _solution.VolumeEfficiency));
                // ### layers : begin
                for (int i = 0; i < _solution.Layers.Count; ++i)
                {
                }
                // ### layers : end
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
        #endregion

        #region Event handlers
        private void OnCriterionChanged(object sender, EventArgs e)
        {
            try
            {
                var constraintSet = _solution.Analysis.ConstraintSet as ConstraintSetPalletTruck;
                constraintSet.OptMaxNumber = uCtrlMaxNoPallets.Value;
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
        private void OnScreenshot(object sender, EventArgs e) => graphCtrlSolution.ScreenshotToClipboard();
        private void OnScreenshotToFile(object sender, EventArgs e) => graphCtrlSolution.ScreenshotToFile();
        #endregion

        #region Data members
        /// <summary>
        /// analysis
        /// </summary>
        private AnalysisPalletTruck _analysis;
        /// <summary>
        /// solution
        /// </summary>
        private SolutionLayered _solution;
        /// <summary>
        /// logger
        /// </summary>
        static readonly ILog _log = LogManager.GetLogger(typeof(DockContentAnalysisPalletTruck));
        #endregion
    }
}
