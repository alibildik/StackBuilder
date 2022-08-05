#region Using directives
using System;
using System.Windows.Forms;
using System.Globalization;
using System.Collections.Generic;

using log4net;
using Sharp3D.Math.Core;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class DockContentAnalysisHCylTruck : DockContentView, IDrawingContainer
    {
        #region Constructor
        public DockContentAnalysisHCylTruck(IDocument document, AnalysisHCylTruck analysis)
            : base(document)
        {
           Analysis = analysis;
           Analysis.AddListener(this);

           InitializeComponent();
        }
        #endregion
        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode) return;

            // --- window caption
            if (null != Analysis)
                Text = Analysis.Name + " - " + Analysis.ParentDocument.Name;

            // initialize controls
            uCtrlMaxNumber.ValueChanged += new UCtrlOptInt.ValueChangedDelegate(OnCriterionChanged);

            // initialize drawing container
            graphCtrlSolution.DrawingContainer = this;

            // --- initialize grid control
            FillGrid();
            UpdateGrid();
            // ---
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
            if (null != Analysis)
                Analysis.RemoveListener(this);
        }
        #endregion
        #region IDrawingContainer implementation
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            using (var sv = new ViewerSolutionHCyl(Solution))
                sv.Draw(graphics, Transform3D.Identity);
        }
        #endregion
        #region Virtual functions
        public virtual void FillGrid()
        {
            // clear grid
            gridSolution.Rows.Clear();
            // border
            gridSolution.BorderStyle = BorderStyle.FixedSingle;
            gridSolution.ColumnsCount = 2;
            gridSolution.FixedColumns = 1;
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
            content.InnerContent(ref listContentItems);
            foreach (var item in listContentItems)
            {
                RecurInsertContent(ref iRow, item.first, item.second * number);
            }
        }
        public virtual void UpdateGrid()
        {
            try
            {
                // remove all existing rows
                gridSolution.Rows.Clear();

                // cell visual properties
                var vPropHeader = CellProperties.VisualPropHeader;
                var vPropValue = CellProperties.VisualPropValue;

                SourceGrid.Cells.RowHeader rowHeader;
                int iRow = -1;

                // loading caption
                gridSolution.Rows.Insert(++iRow);
                rowHeader = new SourceGrid.Cells.RowHeader(GridCaption)
                {
                    ColumnSpan = 2,
                    View = vPropHeader
                };
                gridSolution[iRow, 0] = rowHeader;

                var solution = Analysis.Solution;
                // *** Item # (Recursive count)
                RecurInsertContent(ref iRow, Analysis.Content, solution.ItemCount);
                // load dimensions
                BBox3D bboxLoad = solution.BBoxLoad;
                // ---
                gridSolution.Rows.Insert(++iRow);
                rowHeader = new SourceGrid.Cells.RowHeader(
                    string.Format(Resources.ID_LOADDIMENSIONS, UnitsManager.LengthUnitString))
                {
                    View = vPropValue
                };
                gridSolution[iRow, 0] = rowHeader;
                gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(
                    string.Format(CultureInfo.InvariantCulture, "{0:0.#} x {1:0.#} x {2:0.#}", bboxLoad.Length, bboxLoad.Width, bboxLoad.Height));
                // total dimensions
                BBox3D bboxGlobal = solution.BBoxGlobal;
                // ---
                gridSolution.Rows.Insert(++iRow);
                rowHeader = new SourceGrid.Cells.RowHeader(
                    string.Format(Resources.ID_DIMENSIONS, UnitsManager.LengthUnitString))
                {
                    View = vPropValue
                };
                gridSolution[iRow, 0] = rowHeader;
                gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(
                    string.Format(CultureInfo.InvariantCulture, "{0:0.#} x {1:0.#} x {2:0.#}", bboxGlobal.Length, bboxGlobal.Width, bboxGlobal.Height));
                // net weight
                if (Analysis.Solution.HasNetWeight)
                {
                    gridSolution.Rows.Insert(++iRow);
                    rowHeader = new SourceGrid.Cells.RowHeader(
                        string.Format(Resources.ID_NETWEIGHT_WU, UnitsManager.MassUnitString))
                    {
                        View = vPropValue
                    };
                    gridSolution[iRow, 0] = rowHeader;
                    gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(
                        string.Format(CultureInfo.InvariantCulture, "{0:0.#}", solution.NetWeight.Value));
                }
                // load weight
                gridSolution.Rows.Insert(++iRow);
                rowHeader = new SourceGrid.Cells.RowHeader(
                    string.Format(Resources.ID_LOADWEIGHT_WU, UnitsManager.MassUnitString))
                {
                    View = vPropValue
                };
                gridSolution[iRow, 0] = rowHeader;
                gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(
                    string.Format(CultureInfo.InvariantCulture, "{0:0.#}", solution.LoadWeight));
                // total weight
                gridSolution.Rows.Insert(++iRow);
                rowHeader = new SourceGrid.Cells.RowHeader(
                    string.Format(Resources.ID_TOTALWEIGHT_WU, UnitsManager.MassUnitString))
                {
                    View = vPropValue
                };
                gridSolution[iRow, 0] = rowHeader;
                gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(
                    string.Format(CultureInfo.InvariantCulture, "{0:0.#}", solution.Weight));
                // volume efficiency
                gridSolution.Rows.Insert(++iRow);
                rowHeader = new SourceGrid.Cells.RowHeader(Resources.ID_VOLUMEEFFICIENCY)
                {
                    View = vPropValue
                };
                gridSolution[iRow, 0] = rowHeader;
                gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(
                    string.Format(CultureInfo.InvariantCulture, "{0:0.#}", solution.VolumeEfficiency));

                gridSolution.AutoSizeCells();
                gridSolution.AutoStretchColumnsToFitWidth = true;
                gridSolution.Invalidate();

            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private string GridCaption => Resources.ID_LOAD;
        #endregion
        #region Private properties
        private OptInt MaximumNumber => uCtrlMaxNumber.Value;
        private OptDouble MaximumWeight => uCtrlMaxWeight.Value;
        #endregion
        #region Event handlers
        private void OnSizeChanged(object sender, EventArgs args)
        {
            int splitDistance = splitContainerHoriz.Size.Height - 120;
            if (splitDistance > 0)
                splitContainerHoriz.SplitterDistance = splitDistance;
        }
        private void OnCriterionChanged(object sender, EventArgs args)
        {
            try
            {
                var constraintSet = Solution.AnalysisHCyl.ConstraintSet as ConstraintSetCylinderTruck;
                constraintSet.OptMaxWeight = MaximumWeight;
                constraintSet.OptMaxNumber = MaximumNumber;
                Solution.RebuildSolution();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
            // update drawing & grid
            graphCtrlSolution.Invalidate();
            UpdateGrid();
        }
        private void OnGenerateReport(object sender, EventArgs e)
        {
            Close();
            FormMain.GenerateReport(Analysis);
        }
        private void OnBack(object sender, EventArgs e)
        {
            Close();
            Document.EditAnalysis(Analysis);
        }
        private void OnScreenshot(object sender, EventArgs e) => graphCtrlSolution.ScreenshotToClipboard();
        private void OnScreenshotToFile(object sender, EventArgs e) => graphCtrlSolution.ScreenshotToFile();
        #endregion
        #region Data members
        private AnalysisHCylTruck Analysis { get; set; }
        private SolutionHCyl Solution => Analysis.Solution as SolutionHCyl;
        protected static ILog _log = LogManager.GetLogger(typeof(DockContentAnalysisHCylTruck));
        #endregion


    }
}
