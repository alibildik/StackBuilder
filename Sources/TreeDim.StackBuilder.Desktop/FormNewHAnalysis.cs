#region Using directives
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

using log4net;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Engine;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormNewHAnalysis : Form, IDrawingContainer
    {
        #region Constructor
        public FormNewHAnalysis()
        {
            InitializeComponent();
        }
        public FormNewHAnalysis(Document doc, AnalysisHetero analysis)
        {
            InitializeComponent();
            _document = doc;
            HAnalysis = analysis;
            if (null == HAnalysis)
            {
                HAnalysis = IntantiateTempAnalysis();
                UseTempAnalysis = true;
                HAnalysis.ID.SetNameDesc(doc.GetValidNewAnalysisName("HAnalysis"), string.Empty);
            }
        }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode || null == HAnalysis)
                return;

            // initialize name / description
            ItemName = HAnalysis.ID.Name;
            ItemDescription = HAnalysis.ID.Description;

            // initialize graphic control 
            graphCtrl.DrawingContainer = this;

            // handling content grid events
            _checkBoxEvent.Click += new EventHandler(OnDataModified);
            _numUpDownEvent.ValueChanged += new EventHandler(OnDataModified);
            FillContentGrid();

            // handling row change in solution grid
            gridSolutions.Selection.SelectionChanged += OnSolutionChanged;

            OnDataModified(this, null);
        }
        private void OnSolutionChanged(object sender, SourceGrid.RangeRegionChangedEventArgs e)
        {
            if (PreventUpdate)
                return;

            if (SelectedSolutionIndex != _selectedSolutionIndex)
            {
                _selectedSolutionIndex = SelectedSolutionIndex;
                OnSolutionChangedActual();
            }
        }
        private void OnSolutionChangedActual()
        {
            SolItemIndex = 0;
            UpdateSolItemIndexButtons();
            graphCtrl.Invalidate();
        }

        private bool PreventUpdate { get; set; } = false;
        #endregion

        #region IDrawingContainer
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            try
            {
                HSolution sol = SelectedSolution;
                if (null != sol)
                {
                    ViewerHSolution sv = new ViewerHSolution(sol, SolItemIndex);
                    sv.Draw(graphics, Transform3D.Identity);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Grids
        private void FillContentGrid()
        {
            try
            {
                // remove existing rows
                gridContent.Rows.Clear();
                // viewColumnHeader
                SourceGrid.Cells.Views.ColumnHeader viewColumnHeader = new SourceGrid.Cells.Views.ColumnHeader()
                {
                    Background = new DevAge.Drawing.VisualElements.ColumnHeader()
                    {
                        BackColor = Color.LightGray,
                        Border = DevAge.Drawing.RectangleBorder.NoBorder
                    },
                    ForeColor = Color.Black,
                    Font = new Font("Arial", 10, FontStyle.Regular)
                };
                viewColumnHeader.ElementSort.SortStyle = DevAge.Drawing.HeaderSortStyle.None;
                // viewNormal
                CellBackColorAlternate viewNormal = new CellBackColorAlternate(Color.LightBlue, Color.White);
                // ***
                // set first row
                gridContent.BorderStyle = BorderStyle.FixedSingle;
                gridContent.ColumnsCount = 5;
                gridContent.FixedRows = 1;

                // header
                int iCol = 0;
                gridContent.Rows.Insert(0);
                gridContent[0, iCol] = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_NAME) { AutomaticSortEnabled = false, View = viewColumnHeader };
                gridContent[0, ++iCol] = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_NUMBER) { AutomaticSortEnabled = false, View = viewColumnHeader };
                gridContent[0, ++iCol] = new SourceGrid.Cells.ColumnHeader("X") { AutomaticSortEnabled = false, View = viewColumnHeader };
                gridContent[0, ++iCol] = new SourceGrid.Cells.ColumnHeader("Y") { AutomaticSortEnabled = false, View = viewColumnHeader };
                gridContent[0, ++iCol] = new SourceGrid.Cells.ColumnHeader("Z") { AutomaticSortEnabled = false, View = viewColumnHeader };

                // content
                int iIndex = 0;
                foreach (ContentItem ci in ContentItems)
                {
                    // insert row
                    gridContent.Rows.Insert(++iIndex);
                    iCol = 0;
                    // name
                    gridContent[iIndex, iCol] = new SourceGrid.Cells.Cell(ci.Pack.Name) { View = viewNormal, Tag = ci.Pack };
                    // number
                    gridContent[iIndex, ++iCol] = new SourceGrid.Cells.Cell((int)ci.Number) { View = viewNormal };
                    SourceGrid.Cells.Editors.NumericUpDown l_NumericUpDownEditor = new SourceGrid.Cells.Editors.NumericUpDown(typeof(int), 10000, 0, 1);
                    l_NumericUpDownEditor.SetEditValue((int)ci.Number);
                    gridContent[iIndex, iCol].Editor = l_NumericUpDownEditor;
                    gridContent[iIndex, iCol].AddController(_numUpDownEvent);
                    // orientation X
                    gridContent[iIndex, ++iCol] = new SourceGrid.Cells.CheckBox(null, ci.AllowOrientX);
                    gridContent[iIndex, iCol].AddController(_checkBoxEvent);
                    // orientation Y
                    gridContent[iIndex, ++iCol] = new SourceGrid.Cells.CheckBox(null, ci.AllowOrientY);
                    gridContent[iIndex, iCol].AddController(_checkBoxEvent);
                    // orientation Z
                    gridContent[iIndex, ++iCol] = new SourceGrid.Cells.CheckBox(null, ci.AllowOrientZ);
                    gridContent[iIndex, iCol].AddController(_checkBoxEvent);
                }
                gridContent.AutoSizeCells();
                gridContent.Columns.StretchToFit();
                gridContent.AutoStretchColumnsToFitWidth = true;
                gridContent.Invalidate();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        private void FillResultGrid()
        {
            try
            {
                _selectedSolutionIndex = -1;

                SourceGrid.Cells.Controllers.ToolTipText toolTipController = new SourceGrid.Cells.Controllers.ToolTipText { IsBalloon = true };

                // remove existing rows
                gridSolutions.Rows.Clear();
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
                // viewNormal
                CellBackColorAlternate viewNormal = new CellBackColorAlternate(Color.LightBlue, Color.White);
                // set first row
                gridSolutions.BorderStyle = BorderStyle.FixedSingle;
                gridSolutions.ColumnsCount = 6;
                gridSolutions.FixedRows = 1;

                // header
                int iCol = 0;
                gridSolutions.Rows.Insert(0);
                gridSolutions[0, iCol] = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_INDEX) { View = viewColumnHeader };
                gridSolutions[0, ++iCol] = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_LOADEDCASES) { View = viewColumnHeader };
                gridSolutions[0, ++iCol] = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_NUMBEROFPALLETS) { View = viewColumnHeader };
                gridSolutions[0, ++iCol] = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_LOADEDVOLUMEPERCENTAGE) { View = viewColumnHeader };
                gridSolutions[0, ++iCol] = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_ALGORITHM) { View = viewColumnHeader };

                // solutions
                int iIndex = 0;
                foreach (HSolution sol in Solutions)
                {
                    // insert row
                    gridSolutions.Rows.Insert(++iIndex);
                    iCol = 0;
                    // name
                    gridSolutions[iIndex, iCol] = new SourceGrid.Cells.Cell($"{iIndex}") { View = viewNormal, ToolTipText = sol.Algorithm };
                    gridSolutions[iIndex, iCol].AddController( toolTipController );
                    gridSolutions[iIndex, ++iCol] = new SourceGrid.Cells.Cell(sol.LoadedCasesCountString) { View = viewNormal };
                    gridSolutions[iIndex, ++iCol] = new SourceGrid.Cells.Cell(sol.SolItemCount) { View = viewNormal };
                    gridSolutions[iIndex, ++iCol] = new SourceGrid.Cells.Cell(sol.LoadedVolumePercentage) { View = viewNormal };
                    gridSolutions[iIndex, ++iCol] = new SourceGrid.Cells.Cell(sol.Algorithm) { View = viewNormal };
                }

                gridSolutions.AutoSizeCells();
                gridSolutions.Columns.StretchToFit();
                gridSolutions.AutoStretchColumnsToFitWidth = true;
                gridSolutions.Invalidate();

                // select first solution
                if (gridSolutions.RowsCount > 1)
                    gridSolutions.Selection.SelectRow(1, true);
                else
                    graphCtrl.Invalidate();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Computation
        protected virtual Vector3D DimContainer { get; }
        protected virtual HConstraintSet ConstraintSet { get; }
        protected List<ContentItem> ListContentItems
        {
            get
            {
                List<ContentItem> contentItems = new List<ContentItem>();
                for (int iRow = 1; iRow < gridContent.RowsCount; ++iRow)
                {
                    try
                    {
                        // get packable
                        Packable p = gridContent[iRow, 0].Tag as Packable;
                        // get number
                        SourceGrid.Cells.Editors.NumericUpDown upDownEditor = gridContent[iRow, 1].Editor as SourceGrid.Cells.Editors.NumericUpDown;
                        int number = (int)upDownEditor.GetEditedValue();
                        SourceGrid.Cells.CheckBox checkBoxX = gridContent[iRow, 2] as SourceGrid.Cells.CheckBox;
                        SourceGrid.Cells.CheckBox checkBoxY = gridContent[iRow, 3] as SourceGrid.Cells.CheckBox;
                        SourceGrid.Cells.CheckBox checkBoxZ = gridContent[iRow, 4] as SourceGrid.Cells.CheckBox;
                        // get orientation
                        bool[] orientations = new bool[3] { (bool)checkBoxX.Value, (bool)checkBoxY.Value, (bool)checkBoxZ.Value };
                        contentItems.Add(new ContentItem(p, (uint)number, orientations));
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.ToString());
                    }
                }
                return contentItems;
            }
        }

        protected virtual void LoadContainer() {}

        protected void Compute()
        {
            if (null == HAnalysis) return;
            HAnalysis.ClearContent();
            HAnalysis.AddContent(ListContentItems);
            LoadContainer();
            HAnalysis.ConstraintSet = ConstraintSet;
            if (!HAnalysis.IsValid)
                return;
            try
            {
                HSolver solver = new HSolver();
                Solutions = solver.BuildSolutions(HAnalysis);
            }
            catch (InvalidOperationException ex)
            {
                _log.WarnFormat("Solver -> {0}", ex.Message);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            Compute();
            FillResultGrid();
            graphCtrl.Invalidate();
        }

        #endregion

        #region Event handlers
        protected void OnDataModified(object sender, EventArgs e)
        {
            try
            {
                _timer.Stop();
                Solutions.Clear();
                FillResultGrid();
                _timer.Start();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        
        private void OnSolItemIndexUp(object sender, EventArgs e)
        {
            try
            {
                if (null == SelectedSolution) return;
                PreventUpdate = true;
                if (SolItemIndex < SelectedSolution.SolItemCount - 1)
                    SolItemIndex = SolItemIndex + 1;
                graphCtrl.Invalidate();
                UpdateSolItemIndexButtons();
                PreventUpdate = false;
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
                if (null == SelectedSolution) return;
                PreventUpdate = true;
                if (SolItemIndex > 0)
                    SolItemIndex = SolItemIndex - 1;
                graphCtrl.Invalidate();
                UpdateSolItemIndexButtons();
                PreventUpdate = false;
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void OnNext(object sender, EventArgs e)
        {
            if (null == SelectedSolution)
                return;
            try
            {
                CreateNewAnalysis();
                Close();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Selected solution
        private int SelectedSolutionIndex
        {
            get
            {
                if (Solutions.Count < 1) return -1;
                SourceGrid.RangeRegion region = gridSolutions.Selection.GetSelectionRegion();
                int[] indexes = region.GetRowsIndex();
                // no selection -> exit
                return indexes.Length > 0 ? indexes[0] - 1 : -1;
            }
        }
        protected HSolution SelectedSolution
        {
            get
            {
                int iSel = SelectedSolutionIndex;
                if (-1 == iSel) return null;
                else return Solutions[iSel];
            }
        }
        #endregion

        #region Helpers
        protected string ItemName
        {
            get => tbName.Text;
            set => tbName.Text = value;
        }
        protected string ItemDescription
        {
            get => tbDescription.Text;
            set => tbDescription.Text = value;
        }
        private List<ContentItem> ContentItems
        {
            get
            {
                List<ContentItem> contentItems = new List<ContentItem>();
                foreach (var boxProperties in _document.Bricks)
                {
                    bool[] orientations = new bool[] { true, true, true };
                    contentItems.Add(new ContentItem(boxProperties, GetNoItems(boxProperties), orientations));
                }
                return contentItems;
            }
        }
        private uint GetNoItems(Packable p)
        {
            uint noItems = 0;
            if (null == HAnalysis)
                noItems = Properties.Settings.Default.NoItemsHeterogeneous;
            else
                noItems = HAnalysis.GetNoContent(p);
            return noItems;
        }
        private void UpdateSolItemIndexButtons()
        {
            if (null != SelectedSolution)
            {
                bnSolItemIndexDown.Enabled = SolItemIndex > 0;
                bnSolItemIndexUp.Enabled = SolItemIndex < SelectedSolution.SolItemCount - 1;
            }
            else
            {
                bnSolItemIndexDown.Enabled = false;
                bnSolItemIndexUp.Enabled = false;
            }
        }
        private int SolItemIndex
        {
            get => _solItemIndex;
            set
            {
                _solItemIndex = value;
                lbSolItemIndex.Text = $"{_solItemIndex}";
                UpdateSolItemIndexButtons();
            }
        }
        protected virtual void CreateNewAnalysis() {}
        protected virtual AnalysisHetero IntantiateTempAnalysis() { return null; }

        private bool UseTempAnalysis { get; set; }
        protected List<HSolution> Solutions { get; set; } = new List<HSolution>();
        protected AnalysisHetero HAnalysis { get; set; }
        #endregion

        #region Data members
        protected Document _document;
        protected List<BoxProperties> lBoxes = new List<BoxProperties>();
        protected int _selectedSolutionIndex = -1, _solItemIndex = 0;
        protected SourceGrid.Cells.Controllers.CustomEvents _checkBoxEvent = new SourceGrid.Cells.Controllers.CustomEvents();
        protected SourceGrid.Cells.Controllers.CustomEvents _numUpDownEvent = new SourceGrid.Cells.Controllers.CustomEvents();
        protected static ILog _log = LogManager.GetLogger(typeof(FormNewHAnalysis));
        #endregion
    }
}