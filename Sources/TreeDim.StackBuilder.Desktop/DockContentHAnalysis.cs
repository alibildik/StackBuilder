#region Using directives
using System;
using System.Windows.Forms;
using System.Drawing;
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
    public partial class DockContentHAnalysis : DockContentView, IDrawingContainer
    {
        #region Constructors
        public DockContentHAnalysis()
            : base(null)
        {
            InitializeComponent();
        }
        public DockContentHAnalysis(IDocument document, AnalysisHetero analysis)
            : base(document)
        {
            Analysis = analysis;
            Analysis.AddListener(this);
            Solution = Analysis.Solution;

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

            // initialize drawing container
            graphCtrlSolution.DrawingContainer = this;
            graphCtrlSolution.Invalidate();

            FillGrid();
            UpdateGrid();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Document.RemoveView(this);
        }
        #endregion

        #region IItemListener implementation
        public override void Update(ItemBase item)
        {
        }
        public override void Kill(ItemBase item)
        {
            Close();
            if (null != Analysis)
                Analysis.RemoveListener(this);
        }
        #endregion
        #region IDrawingContainer
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            ViewerHSolution sv = new ViewerHSolution(Solution, SolItemIndex);
            sv.Draw(graphics, Transform3D.Identity);
        }
        #endregion
        #region Public properties
        public AnalysisHetero Analysis { get; set; } = null;
        public HSolution Solution { get; set; }
        #endregion
        #region Protected properties
        protected int GridFontSize => Settings.Default.GridFontSize;
        protected virtual string GridCaption => Resources.ID_LOAD;
        #endregion
        #region Private properties
        private int SolItemIndex { get; set; } = 0;
        #endregion
        #region Grid methods
        public virtual void FillGrid()
        {
            // clear grid
            gridSolutions.Rows.Clear();
            // border
            gridSolutions.BorderStyle = BorderStyle.FixedSingle;
            gridSolutions.ColumnsCount = 2;
            gridSolutions.Columns[0].Width = 100;
            gridSolutions.FixedColumns = 1;
        }
        public virtual void UpdateGrid()
        {
            try
            {
                // sanity check
                if (gridSolutions.ColumnsCount < 2)
                    return;
                // remove all existing rows
                gridSolutions.Rows.Clear();
                // *** IViews
                // caption header
                DevAge.Drawing.VisualElements.RowHeader veHeaderCaption = new DevAge.Drawing.VisualElements.RowHeader()
                {
                    BackColor = Color.SteelBlue,
                    Border = DevAge.Drawing.RectangleBorder.NoBorder
                };
                SourceGrid.Cells.Views.RowHeader captionHeader = new SourceGrid.Cells.Views.RowHeader
                {
                    Background = veHeaderCaption,
                    ForeColor = Color.Black,
                    Font = new Font("Arial", GridFontSize+2, FontStyle.Bold),
                    TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter
                };
                SourceGrid.Cells.Views.RowHeader captionHeader2 = new SourceGrid.Cells.Views.RowHeader
                {
                    Background = veHeaderCaption,
                    ForeColor = Color.Black,
                    Font = new Font("Arial", GridFontSize, FontStyle.Regular),
                    TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter
                };
                // viewRowHeader
                DevAge.Drawing.VisualElements.RowHeader backHeader = new DevAge.Drawing.VisualElements.RowHeader()
                {
                    BackColor = Color.LightGray,
                    Border = DevAge.Drawing.RectangleBorder.NoBorder
                };
                SourceGrid.Cells.Views.RowHeader viewRowHeader = new SourceGrid.Cells.Views.RowHeader
                {
                    Background = backHeader,
                    ForeColor = Color.Black,
                    Font = new Font("Arial", GridFontSize, FontStyle.Regular)
                };
                // viewNormal
                CellBackColorAlternate viewNormal = new CellBackColorAlternate(Color.LightBlue, Color.White);
                // ***

                int iRow = -1;
                // ### sol items : begin
                int solItemIndex = 0;
                foreach (var solItem in Solution.SolItems)
                {
                    gridSolutions.Rows.Insert(++iRow);
                    var rowHeader = new SourceGrid.Cells.RowHeader(string.Format(Resources.ID_PALLET_NUMBER, solItemIndex))
                    {
                        ColumnSpan = 2,
                        View = captionHeader
                    };
                    gridSolutions[iRow, 0] = rowHeader;

                    gridSolutions.Rows.Insert(++iRow);
                    rowHeader = new SourceGrid.Cells.RowHeader(Resources.ID_ITEMS)
                    {
                        ColumnSpan = 2,
                        View = captionHeader2
                    };
                    gridSolutions[iRow, 0] = rowHeader;

                    var dictNameCount = solItem.SolutionItems;
                    foreach (int containedItemIndex in dictNameCount.Keys)
                    {
                        // name
                        string name = string.Empty;
                        if (Analysis.ContentTypeByIndex(containedItemIndex) is Packable packable)
                            name = packable.Name;
                        // count
                        int count = dictNameCount[containedItemIndex];

                        if (count > 0)
                        {
                            gridSolutions.Rows.Insert(++iRow);
                            var itemHeader = new SourceGrid.Cells.RowHeader(name)
                            {
                                View = viewRowHeader
                            };
                            gridSolutions[iRow, 0] = itemHeader;
                            gridSolutions[iRow, 1] = new SourceGrid.Cells.Cell($"{count}");
                        }
                    }
                    // pallet data header
                    gridSolutions.Rows.Insert(++iRow);
                    rowHeader = new SourceGrid.Cells.RowHeader(Resources.ID_PALLETDATA)
                    {
                        ColumnSpan = 2,
                        View = captionHeader2
                    };
                    gridSolutions[iRow, 0] = rowHeader;
                    // ***
                    // outer dimensions
                    BBox3D bboxGlobal = Solution.BBoxGlobal(solItemIndex);
                    // ---
                    gridSolutions.Rows.Insert(++iRow);
                    rowHeader = new SourceGrid.Cells.RowHeader(
                        string.Format(Resources.ID_OUTERDIMENSIONS, UnitsManager.LengthUnitString))
                    {
                        View = viewRowHeader
                    };
                    gridSolutions[iRow, 0] = rowHeader;
                    gridSolutions[iRow, 1] = new SourceGrid.Cells.Cell(
                        string.Format(CultureInfo.InvariantCulture
                        , "{0:0.#} x {1:0.#} x {2:0.#}"
                        , bboxGlobal.Length, bboxGlobal.Width, bboxGlobal.Height));
                    // load dimensions
                    BBox3D bboxLoad = Solution.BBoxLoad(solItemIndex);
                    gridSolutions.Rows.Insert(++iRow);
                    rowHeader = new SourceGrid.Cells.RowHeader(
                        string.Format(Resources.ID_LOADDIMENSIONS, UnitsManager.LengthUnitString))
                    {
                        View = viewRowHeader
                    };
                    gridSolutions[iRow, 0] = rowHeader;
                    gridSolutions[iRow, 1] = new SourceGrid.Cells.Cell(
                        string.Format(CultureInfo.InvariantCulture
                        , "{0:0.#} x {1:0.#} x {2:0.#}"
                        , bboxLoad.Length, bboxLoad.Width, bboxLoad.Height));
                    // ***
                    // ***
                    // load weight
                    gridSolutions.Rows.Insert(++iRow);
                    rowHeader = new SourceGrid.Cells.RowHeader(
                        string.Format(Resources.ID_LOADWEIGHT_WU, UnitsManager.MassUnitString))
                    {
                        View = viewRowHeader
                    };
                    gridSolutions[iRow, 0] = rowHeader;
                    gridSolutions[iRow, 1] = new SourceGrid.Cells.Cell(
                        string.Format(CultureInfo.InvariantCulture, "{0:0.#}", Solution.LoadWeight(solItemIndex)));

                    // total weight
                    gridSolutions.Rows.Insert(++iRow);
                    rowHeader = new SourceGrid.Cells.RowHeader(
                        string.Format(Resources.ID_PALLETWEIGHT_WU, UnitsManager.MassUnitString))
                    {
                        View = viewRowHeader
                    };
                    gridSolutions[iRow, 0] = rowHeader;
                    gridSolutions[iRow, 1] = new SourceGrid.Cells.Cell(
                        string.Format(CultureInfo.InvariantCulture
                        , "{0:0.#}"
                        , Solution.Weight(solItemIndex)));
                    // ***
                    // increment sol item index
                    ++solItemIndex;
                }
                // ### sol items : end
                gridSolutions.AutoSizeCells();
                gridSolutions.Columns.StretchToFit();
                gridSolutions.AutoStretchColumnsToFitWidth = true;
                gridSolutions.Invalidate();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion
        #region Event handlers
        private void OnSolItemIndexUp(object sender, EventArgs e)
        {
            try
            {
                if (null == Solution) return;
                if (SolItemIndex < Solution.SolItemCount - 1)
                    SolItemIndex = SolItemIndex + 1;
                graphCtrlSolution.Invalidate();
                UpdateSolItemIndexButtons();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void OnSolItemIndexDown(object sender, EventArgs e)
        {
            try
            {
                if (null == Solution) return;
                if (SolItemIndex > 0)
                    SolItemIndex = SolItemIndex - 1;
                graphCtrlSolution.Invalidate();
                UpdateSolItemIndexButtons();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void OnBack(object sender, EventArgs e)
        {
            // close this form
            Close();
            // call edit analysis
            if (null != Document && null != Solution)
                Document.EditAnalysis(Solution.Analysis);
            else
                _log.Error("Solution or Document must not be null");


        }
        private void OnGenerateReport(object sender, EventArgs e)
        {
            FormMain.GenerateReport(Analysis);
        }
        private void OnGenerateExport(object sender, EventArgs e)
        {
            ToolStripButton tsb = sender as ToolStripButton;
            string extension = string.Empty;
            switch (tsb.Name)
            {
                case "toolStripButtonExportXML": extension = "xml"; break;
                case "toolStripButtonExportCSV": extension = "csv"; break;
                case "toolStripButtonExportDAE": extension = "dae"; break;
                default: break;
            }
            FormMain.GenerateExport(Analysis, extension);
        }
        #endregion

        #region Helpers
        private void UpdateSolItemIndexButtons()
        {
            bnSolItemIndexDown.Enabled = SolItemIndex > 0;
            bnSolItemIndexUp.Enabled = SolItemIndex < Solution.SolItemCount - 1;
        }
        #endregion

        #region Data members
        private static ILog _log = LogManager.GetLogger(typeof(DockContentHAnalysis));
        #endregion
    }
}
