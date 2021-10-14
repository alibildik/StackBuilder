#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;

using log4net;
using Sharp3D.Math.Core;

using treeDiM.ThreadCallback;
using treeDiM.UserControls;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Graphics.Controls;
using treeDiM.StackBuilder.Engine;
using treeDiM.PLMPack.DBClient.PLMPackSR;
using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormOptimiseMultiCase : Form, IItemBaseFilter, IDrawingContainer
    {
        #region Constructor
        public FormOptimiseMultiCase()
        {
            InitializeComponent();
        }
        #endregion
        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // fill combo boxes
            cbBoxes.Initialize(Document, this, null);
            // initialize orientation
            uCtrlCaseOrient.AllowedOrientations = new bool[] { Settings.Default.AllowVerticalX, Settings.Default.AllowVerticalY, Settings.Default.AllowVerticalZ };
            // initialize grid
            gridSolutions.Selection.SelectionChanged += new SourceGrid.RangeRegionChangedEventHandler(OnSelChangeGrid);
            // load values
            uCtrlCaseDimensionsMin.Checked = Settings.Default.MinCaseDimChecked;
            uCtrlCaseDimensionsMin.X = Settings.Default.MinCaseDimX;
            uCtrlCaseDimensionsMin.Y = Settings.Default.MinCaseDimY;
            uCtrlCaseDimensionsMin.Z = Settings.Default.MinCaseDimZ;
            uCtrlCaseDimensionsMax.Checked = Settings.Default.MaxCaseDimChecked;
            uCtrlCaseDimensionsMax.X = Settings.Default.MaxCaseDimX;
            uCtrlCaseDimensionsMax.Y = Settings.Default.MaxCaseDimY;
            uCtrlCaseDimensionsMax.Z = Settings.Default.MaxCaseDimZ;
            chkbAllowCombinations.Checked = Settings.Default.AllowCombinations;

            // fill list of cases
            OnFillListCases(this, null);
            // Mode
            uCtrlNumberPerCase.Value = new OptInt(false, 2);
            // update graph control
            graphCtrl.DrawingContainer = this;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
 	        base.OnClosing(e);
            Settings.Default.MinCaseDimChecked = uCtrlCaseDimensionsMin.Checked;
            Settings.Default.MinCaseDimX = uCtrlCaseDimensionsMin.X;
            Settings.Default.MinCaseDimY = uCtrlCaseDimensionsMin.Y;
            Settings.Default.MinCaseDimZ = uCtrlCaseDimensionsMin.Z;
            Settings.Default.MaxCaseDimChecked = uCtrlCaseDimensionsMax.Checked;
            Settings.Default.MaxCaseDimX = uCtrlCaseDimensionsMax.X;
            Settings.Default.MaxCaseDimY = uCtrlCaseDimensionsMax.Y;
            Settings.Default.MaxCaseDimZ = uCtrlCaseDimensionsMax.Z;
            Settings.Default.AllowCombinations = chkbAllowCombinations.Checked;
        }
        #endregion
        #region Status toolstrip updating
        public virtual void UpdateStatus(string message)
        {
            // status + message
            if (!uCtrlCaseOrient.HasOrientationSelected)
                message = Resources.ID_NOORIENTATIONSELECTED;
            else if (CheckedIndices.Count < 1)
                message = Resources.ID_NOCASESELECTED;
            else if (null == SelectedAnalysis)
                message = Resources.ID_NOANALYSISSELECTED;
            else if (string.IsNullOrEmpty(AnalysisName))
                message = Resources.ID_FIELDNAMEEMPTY;
            else if (string.IsNullOrEmpty(AnalysisDescription))
                message = Resources.ID_FIELDDESCRIPTIONEMPTY;

            toolStripStatusLabelDef.ForeColor = string.IsNullOrEmpty(message) ? Color.Black : Color.Red;
            toolStripStatusLabelDef.Text = string.IsNullOrEmpty(message) ? Resources.ID_READY : message;

            bnCreateAnalysis.Enabled = string.IsNullOrEmpty(message);
        }
        #endregion
        #region IItemBaseFilter
        public bool Accept(Control ctrl, ItemBase itemBase)
        {
            if (ctrl == cbBoxes)
                return itemBase is PackableBrick;
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
                ViewerSolution sv = new ViewerSolution(SelectedAnalysis.SolutionLay);
                sv.Draw(graphics, Transform3D.Identity);
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
            if (0 == cbBoxes.Items.Count) return;
            if (cbBoxes.SelectedType is PackableBrick packable)
                uCtrlCaseOrient.BProperties = packable;
            
            UpdateStatus(string.Empty);
            timer.Stop();
            timer.Start();
        }
        private void OnConstraintsChanged(object sender, EventArgs e)
        {
            UpdateStatus(string.Empty);
            timer.Stop();
            timer.Start();
        }
        private void OnCaseChecked(object sender, ItemCheckEventArgs e)
        {
            // build list of checked items in chklbCases
            CheckedIndices.Clear();
            foreach (int index in chklbCases.CheckedIndices)
                CheckedIndices.Add(index);
            if (null != e)
            {
                if (e.NewValue == CheckState.Checked)
                    CheckedIndices.Add(e.Index);
                else if (e.NewValue == CheckState.Unchecked)
                    CheckedIndices.Remove(e.Index);
            }
            UpdateStatus(string.Empty);
            timer.Stop();
            timer.Start();
        }
        private void OnCreateAnalysis(object sender, EventArgs e)
        {
            try
            {
                // selected analysis -> get case 
                AnalysisBoxCase analysisSel = SelectedAnalysis;
                BoxProperties caseSel = analysisSel.CaseProperties;
                // create case
                BoxProperties caseProperties = Document.CreateNewCase(caseSel);
                // create analysis
                List<InterlayerProperties> interlayers = new List<InterlayerProperties>();
                Document.CreateNewAnalysisBoxCase(
                    AnalysisName, AnalysisDescription,
                    analysisSel.Content, caseProperties,
                    interlayers,
                    analysisSel.ConstraintSet as ConstraintSetBoxCase,
                    analysisSel.SolutionLay.LayerEncaps);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        private void OnSelChangeGrid(object sender, SourceGrid.RangeRegionChangedEventArgs e)
        {
            try
            {
                // analysis name/description
                if (null != SelectedAnalysis)
                {
                    AnalysisName = string.Format("Analysis_{0}_in_{1}", SelectedAnalysis.Content.Name, SelectedAnalysis.Container.Name);
                    AnalysisDescription = string.Format(" Packing {0} in {1}", SelectedAnalysis.Content.Name, SelectedAnalysis.Container.Name);
                }
                else
                {
                    AnalysisName = string.Empty;
                    AnalysisDescription = string.Empty;
                }
                // update drawing
                graphCtrl.Invalidate();

                UpdateStatus(string.Empty);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        private AnalysisBoxCase SelectedAnalysis
        {
            get
            {
                SourceGrid.RangeRegion region = gridSolutions.Selection.GetSelectionRegion();
                int[] indexes = region.GetRowsIndex();
                if (indexes.Length < 1 || indexes[0] < 1)
                    return null;
                return indexes[0] <= Analyses.Count ? (Analyses[indexes[0]-1] as AnalysisBoxCase) : null;
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            timer.Stop();
            if (!ThreadRunning)
                ComputeSolutions();
        }
        #endregion
        #region Compute solutions
        protected void ComputeSolutions()
        {
            // sanity checks 
            if (cbBoxes.Items.Count == 0 || chklbCases.Items.Count == 0)
                return;
            // clear existing analyses
            Analyses.Clear();

            int expectedCount = uCtrlNumberPerCase.Value.Activated ? uCtrlNumberPerCase.Value.Value : -1;

            try
            {
                PackableBrick packable = cbBoxes.SelectedType as PackableBrick;

                ThreadRunning = true;

                ProgressWindow progress = new ProgressWindow();
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(BuildAnalyses), progress);
                progress.ShowDialog();

                // fill grid
                FillGrid();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
            finally
            {
                ThreadRunning = false;
            }
        }
        private void BuildAnalyses(object status)
        {
            try
            {
                IProgressCallback callback = status as IProgressCallback;
                callback.SetRange(0, CheckedIndices.Count - 1);
                callback.Begin();

                int expectedCount = uCtrlNumberPerCase.Value.Activated ? uCtrlNumberPerCase.Value.Value : -1;
                PackableBrick packable = cbBoxes.SelectedType as PackableBrick;

                // build list of analyses
                for (int i = 0; i < CheckedIndices.Count; ++i)
                {
                    try
                    {
                        if (callback.IsAborting)
                            break;
                        callback.StepTo(i);
                        callback.SetText(string.Format(Resources.ID_EVALUATINGCASE, i + 1, CheckedIndices.Count));

                        if ((chklbCases.Items[CheckedIndices[i]] as ItemBaseCB).Item is BoxProperties caseProperties)
                        {
                            // build constraint set
                            ConstraintSetBoxCase constraintSet = new ConstraintSetBoxCase(caseProperties);
                            constraintSet.SetAllowedOrientations(uCtrlCaseOrient.AllowedOrientations);
                            if (uCtrlNumberPerCase.Value.Activated)
                                constraintSet.SetMaxNumber(uCtrlNumberPerCase.Value.Value);
                            // build solver + get analyses
                            SolverBoxCase solver = new SolverBoxCase(packable, caseProperties, constraintSet);
                            var listAnalyses = solver.BuildAnalyses(chkbAllowCombinations.Checked);
                            foreach (var analysis in listAnalyses)
                            {
                                if ((-1 == expectedCount) || (expectedCount == analysis.Solution.ItemCount))
                                    Analyses.Add(analysis);
                            }
                        }
                    }
                    catch (Exception) {}
                }
                callback.SetText(Resources.ID_SORTINGSOLUTIONS);
                // sort analysis
                Analyses.Sort(new AnalysisComparer());
                callback.End();
            }
            catch (Exception) {}
        }

        #endregion
        #region Grid
        private int GridFontSize
        {
            get { return Settings.Default.GridFontSize; }
        }
        private void FillGrid()
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
            gridSolutions.ColumnsCount = 5;
            gridSolutions.FixedRows = 1;
            gridSolutions.Rows.Insert(0);
            // header
            int iCol = 0;
            SourceGrid.Cells.ColumnHeader columnHeader;
            // name
            columnHeader = new SourceGrid.Cells.ColumnHeader(Resources.ID_CASENAME)
            {
                AutomaticSortEnabled = false,
                View = viewColumnHeader
            };
            gridSolutions[0, iCol++] = columnHeader;
            // dimensions
            columnHeader = new SourceGrid.Cells.ColumnHeader(string.Format(Resources.ID_DIMENSIONS, UnitsManager.LengthUnitString))
            {
                AutomaticSortEnabled = false,
                View = viewColumnHeader
            };
            gridSolutions[0, iCol++] = columnHeader;
            // #items
            columnHeader = new SourceGrid.Cells.ColumnHeader("#")
            {
                AutomaticSortEnabled = false,
                View = viewColumnHeader
            };
            gridSolutions[0, iCol++] = columnHeader;
            // efficiency
            columnHeader = new SourceGrid.Cells.ColumnHeader(Resources.ID_EFFICIENCYPERCENTAGE)
            {
                AutomaticSortEnabled = false,
                View = viewColumnHeader
            };
            gridSolutions[0, iCol++] = columnHeader;
            // weight
            columnHeader = new SourceGrid.Cells.ColumnHeader(string.Format(Resources.ID_WEIGHT_WU, UnitsManager.MassUnitString))
            {
                AutomaticSortEnabled = false,
                View = viewColumnHeader
            };
            gridSolutions[0, iCol++] = columnHeader;

            int iRow = 0;
            foreach (AnalysisLayered analysis in Analyses)
            {
                AnalysisBoxCase analysisBoxCase = analysis as AnalysisBoxCase;
                BoxProperties caseProperties = analysisBoxCase.CaseProperties;

                gridSolutions.Rows.Insert(++iRow);
                iCol = 0;
                gridSolutions[iRow, iCol++] = new SourceGrid.Cells.Cell(analysis.Container.ID.Name);
                gridSolutions[iRow, iCol++] = new SourceGrid.Cells.Cell(string.Format("{0} x {1} x {2} / {3} x {4} x {5}",
                    caseProperties.Length, caseProperties.Width, caseProperties.Height,
                    caseProperties.InsideLength, caseProperties.InsideWidth, caseProperties.InsideHeight));
                gridSolutions[iRow, iCol++] = new SourceGrid.Cells.Cell(analysis.Solution.ItemCount);
                gridSolutions[iRow, iCol++] = new SourceGrid.Cells.Cell(string.Format(CultureInfo.InvariantCulture, "{0:0.#}", analysis.Solution.VolumeEfficiency));
                gridSolutions[iRow, iCol++] = new SourceGrid.Cells.Cell(string.Format(CultureInfo.InvariantCulture, "{0:0.#}", analysis.Solution.Weight));
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
                graphCtrl.Invalidate();
            }
        }
        #endregion
        #region Public properties
        public DocumentSB Document { get; set; }
        private string AnalysisName
        {
            get { return tbAnalysisName.Text; }
            set { tbAnalysisName.Text = value; }
        }
        private string AnalysisDescription
        {
            get { return tbAnalysisDescription.Text; }
            set { tbAnalysisDescription.Text = value; }
        }
        #endregion
        #region Private helpers
        private void OnFillListCases(object sender, EventArgs e)
        {
            // sanity check
            if (null == ListCases || ListCases.Count < 1) return;
            // clear check list
            chklbCases.Items.Clear();
            // fill list of cases
            foreach (DCSBCase sbCase in ListCases)
            {
                try
                {
                    // get values from database
                    if (!sbCase.HasInnerDims) continue;
                    UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)sbCase.UnitSystem;
                    double boxLength = UnitsManager.ConvertLengthFrom(sbCase.DimensionsOuter.M0, us);
                    double boxWidth = UnitsManager.ConvertLengthFrom(sbCase.DimensionsOuter.M1, us);
                    double boxHeight = UnitsManager.ConvertLengthFrom(sbCase.DimensionsOuter.M2, us);
                    double innerLength = UnitsManager.ConvertLengthFrom(sbCase.DimensionsInner.M0, us);
                    double innerWidth = UnitsManager.ConvertLengthFrom(sbCase.DimensionsInner.M1, us);
                    double innerHeight = UnitsManager.ConvertLengthFrom(sbCase.DimensionsInner.M2, us);

                    if (uCtrlCaseDimensionsMin.Checked
                        && (innerLength < uCtrlCaseDimensionsMin.X
                        || innerWidth < uCtrlCaseDimensionsMin.Y
                        || innerHeight < uCtrlCaseDimensionsMin.Z))
                        continue;
                    if (uCtrlCaseDimensionsMax.Checked
                        && (innerLength > uCtrlCaseDimensionsMax.X
                        || innerWidth > uCtrlCaseDimensionsMax.Y
                        || innerHeight > uCtrlCaseDimensionsMax.Z))
                        continue;

                    double weight = UnitsManager.ConvertMassFrom(sbCase.Weight, us);
                    Color[] colors = new Color[6];
                    for (int i = 0; i < 6; ++i)
                        colors[i] = Color.FromArgb(sbCase.Colors[i]);

                    // instantiate box properties
                    BoxProperties bProperties = new BoxProperties(null
                        , boxLength, boxWidth, boxHeight
                        , innerLength, innerWidth, innerHeight);
                    bProperties.TapeWidth = new OptDouble(sbCase.ShowTape, sbCase.TapeWidth);
                    bProperties.TapeColor = Color.FromArgb(sbCase.TapeColor);
                    bProperties.ID.SetNameDesc(sbCase.Name, sbCase.Description);
                    bProperties.SetWeight(weight);
                    bProperties.SetAllColors(colors);
                    // insert in listbox control
                    chklbCases.Items.Add(new ItemBaseCB(bProperties), true);
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message);
                }
            }
            OnCaseChecked(this, null);
        }
        #endregion
        #region Public properties
        public List<DCSBCase> ListCases { get; set; }
        #endregion
        #region Data members
        private List<AnalysisLayered> Analyses { get; set; } = new List<AnalysisLayered>();
        private List<int> CheckedIndices { get; set; } = new List<int>();
        private bool ThreadRunning { get; set; } = false;
        protected static ILog _log = LogManager.GetLogger(typeof(FormOptimiseMultiCase));
        #endregion
    }
}
