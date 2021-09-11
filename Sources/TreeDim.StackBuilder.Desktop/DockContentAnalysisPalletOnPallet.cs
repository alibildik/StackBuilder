#region Using directives
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Globalization;

using log4net;
using Sharp3D.Math.Core;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class DockContentAnalysisPalletsOnPallet : DockContentView, IDrawingContainer
    {
        #region Constructor
        public DockContentAnalysisPalletsOnPallet(IDocument document, AnalysisPalletsOnPallet analysis)
            : base(document)
        {
            InitializeComponent();
            Analysis = analysis;
            Analysis.AddListener(this);
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
            Close();
            Analysis?.RemoveListener(this);
        }
        #endregion
        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // --- window caption
            if (null != Analysis)
                Text = Analysis.Name + "_" + Analysis.ParentDocument.Name;

            // --- initialize drawing container
            graphCtrlSolution.DrawingContainer = this;
            graphCtrlSolution.Viewer = new ViewerSolutionPalletsOnPallet(Analysis.Solution);
            graphCtrlSolution.Invalidate();

            FillGrid();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Document.RemoveView(this);
        }
        #endregion
        #region IDrawingContainer
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            ctrl.Viewer.Draw(graphics, Transform3D.Identity);
        }
        #endregion
        #region Grid
        private void RecurInsertContent(ref int iRow, Packable content, int number)
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
            if (null != listContentItems)
            {
                foreach (var item in listContentItems)
                {
                    RecurInsertContent(ref iRow, item.first, item.second * number);
                }
            }
        }
        private void FillGrid()
        {
            // clear grid
            gridSolution.Rows.Clear();
            // border
            gridSolution.BorderStyle = BorderStyle.FixedSingle;
            gridSolution.ColumnsCount = 2;
            gridSolution.FixedColumns = 1;
            gridSolution.FixedRows = 1;

            // cell visual properties
            var vPropHeader = CellProperties.VisualPropHeader;
            var vPropValue = CellProperties.VisualPropValue;

            int iRow = -1;
            try
            {
                // pallet caption
                gridSolution.Rows.Insert(++iRow);
                gridSolution[iRow, 0] = new ColumnHeaderSolution(Resources.ID_PALLET) { ColumnSpan = 1 };
                gridSolution[iRow, 1] = new ColumnHeaderSolution(string.Empty) { ColumnSpan = 1 };

                // *** Item # (Recursive count)
                List<Pair<Packable, int>> listInnerPackables = new List<Pair<Packable, int>>();
                Analysis.InnerContent(ref listInnerPackables);
                foreach (var pa in listInnerPackables)
                    RecurInsertContent(ref iRow, pa.first, pa.second);
                // ***
                // outer dimensions
                BBox3D bboxGlobal = Analysis.Solution.BBoxGlobal;
                gridSolution.Rows.Insert(++iRow);
                gridSolution[iRow, 0] = new SourceGrid.Cells.RowHeader(
                    string.Format(Resources.ID_OUTERDIMENSIONS, UnitsManager.LengthUnitString)) { View = vPropValue };
                gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(
                    string.Format(CultureInfo.InvariantCulture, "{0:0.#} x {1:0.#} x {2:0.#}", bboxGlobal.Length, bboxGlobal.Width, bboxGlobal.Height)
                    );
                // load dimensions
                BBox3D bboxLoad = Analysis.Solution.BBoxLoad;
                gridSolution.Rows.Insert(++iRow);
                gridSolution[iRow, 0] = new SourceGrid.Cells.RowHeader(
                    string.Format(Resources.ID_LOADDIMENSIONS, UnitsManager.LengthUnitString)) { View = vPropValue };
                gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(
                    string.Format(CultureInfo.InvariantCulture, "{0:0.#} x {1:0.#} x {2:0.#}", bboxLoad.Length, bboxLoad.Width, bboxLoad.Height)
                    );
                // total weight
                gridSolution.Rows.Insert(++iRow);
                gridSolution[iRow, 0] = new SourceGrid.Cells.RowHeader(
                    string.Format(Resources.ID_TOTALWEIGHT_WU, UnitsManager.MassUnitString)) { View = vPropValue };
                gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(string.Format(CultureInfo.InvariantCulture, "{0:0.#}", Analysis.Solution.Weight)
                    );
                // load weight
                gridSolution.Rows.Insert(++iRow);
                gridSolution[iRow, 0] = new SourceGrid.Cells.RowHeader(
                    string.Format(Resources.ID_LOADWEIGHT_WU, UnitsManager.MassUnitString)) { View = vPropValue };
                gridSolution[iRow, 1] = new SourceGrid.Cells.Cell(string.Format(CultureInfo.InvariantCulture, "{0:0.#}", Analysis.Solution.LoadWeight)
                    );
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }

            gridSolution.AutoSizeCells();
            gridSolution.Columns.StretchToFit();
            gridSolution.AutoStretchColumnsToFitWidth = true;
            gridSolution.Invalidate();
        }
        #endregion
        #region Toolbar event handlers
        private void OnBack(object sender, EventArgs e)
        {
            // close this form
            Close();
            // call edit analysis
            Document.EditAnalysis(Analysis);
        }
        private void OnGenerateReport(object sender, EventArgs e)
        {
            FormMain.GenerateReport(Analysis);
        }
        private void OnScreenshot(object sender, EventArgs e)
        {
            graphCtrlSolution.ScreenShotToClipboard();
        }

        #endregion
        #region Data members
        /// <summary>
        /// analysis
        /// </summary>
        public AnalysisPalletsOnPallet Analysis { get; set; }
        /// <summary>
        ///  logger
        /// </summary>
        static readonly ILog _log = LogManager.GetLogger(typeof(DockContentAnalysisPalletsOnPallet));
        #endregion
    }
}
