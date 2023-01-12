#region Using directives
using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

using Excel = Microsoft.Office.Interop.Excel;

using Sharp3D.Math.Core;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Reporting;
using treeDiM.StackBuilder.Engine;
using treeDiM.StackBuilder.Desktop.Properties;
using treeDiM.StackBuilder.Graphics.Controls;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormExcelMassAnalysisCasePallet : FormExcelMassAnalysis
    {
        #region Constructor
        public FormExcelMassAnalysisCasePallet()
        {
            InitializeComponent();
        }
        #endregion
        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            uCtrlMaxPalletHeight.Value = Settings.Default.MaximumPalletHeight;
            uCtrlOverhang.ValueX = Settings.Default.OverhangX;
            uCtrlOverhang.ValueY = Settings.Default.OverhangY;

            OnGenerateImagesInFolderChanged(this, null);
            OnGenerateReportsInFolderChanged(this, null);
            UpdateStatus(this, null);            
        }
        #endregion
        #region Override FormExcelMassAnalysis
        protected override string SampleFileURL => Settings.Default.MassExcelTestSheetURLCasePallet;
        protected override void InitializeInputFields()
        {
            FillPalletList(lbPallets);

            ComboBox[] comboBoxes = new ComboBox[]
            {
                cbName,
                cbDescription,
                cbLength,
                cbWidth,
                cbHeight,
                cbWeight,
                cbOutputStart
            };
            foreach (var cb in comboBoxes)
                ExcelHelpers.FillComboWithColumnName(cb);

            // ---
            cbName.SelectedIndex = 0;
            chkbDescription.Checked = true;
            cbDescription.SelectedIndex = 1;
            cbLength.SelectedIndex = 2;
            cbWidth.SelectedIndex = 3;
            cbHeight.SelectedIndex = 4;
            cbWeight.SelectedIndex = 5;
            cbOutputStart.SelectedIndex = 6;
            // ---
        }
        protected override void SaveSettings()
        {
            Settings.Default.MassExcelColLetterName = ColumnLetterName;
            Settings.Default.MassExcelColLetterDescription = ColumnLetterDescription;
            Settings.Default.MassExcelColLetterLength = ColumnLetterLength;
            Settings.Default.MassExcelColLetterWidth = ColumnLetterWidth;
            Settings.Default.MassExcelColLetterHeight = ColumnLetterHeight;
            Settings.Default.MassExcelColLetterWeight = ColumnLetterWeight;
            Settings.Default.MassExcelImageSize = ImageSize;
            Settings.Default.MassExcelGenerateImageInRow = GenerateImage;
            Settings.Default.MassExcelGenerateImagesInFolder = GenerateImageInFolder;
            Settings.Default.MassExcelGenerateReportsInFolder = GenerateReport;
            Settings.Default.MassExcelImageFolderPath = DirectoryPathImages;
            Settings.Default.MassExcelReportFolderPath = DirectoryPathReports;
            Settings.Default.AllowCombinations = AllowCombinations;
            Settings.Default.MassExcelUseAdmissibleLoadWeight = UseAdmissibleLoadWeight;
        }
        protected override void LoadSettings()
        {
            cbName.SelectedIndex = ExcelHelpers.ColumnLetterToColumnIndex(Settings.Default.MassExcelColLetterName) - 1;
            cbDescription.SelectedIndex = ExcelHelpers.ColumnLetterToColumnIndex(
                string.IsNullOrEmpty(Settings.Default.MassExcelColLetterDescription) ? Settings.Default.MassExcelColLetterName : Settings.Default.MassExcelColLetterDescription) - 1;
            cbLength.SelectedIndex = ExcelHelpers.ColumnLetterToColumnIndex(Settings.Default.MassExcelColLetterLength) - 1;
            cbWidth.SelectedIndex = ExcelHelpers.ColumnLetterToColumnIndex(Settings.Default.MassExcelColLetterWidth) - 1;
            cbHeight.SelectedIndex = ExcelHelpers.ColumnLetterToColumnIndex(Settings.Default.MassExcelColLetterHeight) - 1;
            cbWeight.SelectedIndex = ExcelHelpers.ColumnLetterToColumnIndex(Settings.Default.MassExcelColLetterWeight) - 1;
            cbOutputStart.SelectedIndex = ExcelHelpers.ColumnLetterToColumnIndex(Settings.Default.MassExcelColLetterOutputStart) - 1;
            ImageSize = Settings.Default.MassExcelImageSize;
            GenerateImage = Settings.Default.MassExcelGenerateImageInRow;
            GenerateImageInFolder = Settings.Default.MassExcelGenerateImagesInFolder;
            GenerateReport = Settings.Default.MassExcelGenerateReportsInFolder;
            DirectoryPathImages = Settings.Default.MassExcelImageFolderPath;
            DirectoryPathReports = Settings.Default.MassExcelReportFolderPath;
            AllowCombinations = Settings.Default.AllowCombinations;
            UseAdmissibleLoadWeight = Settings.Default.MassExcelUseAdmissibleLoadWeight;
        }
        #endregion
        #region Computation
        protected override void Compute(Excel.Worksheet xlSheet, StringBuilder sbErrors)
        {
            string colName = ColumnLetterName;
            string colDescription = ColumnLetterDescription;
            string colLength = ColumnLetterLength;
            string colWidth = ColumnLetterWidth;
            string colHeight = ColumnLetterHeight;
            string colWeight = ColumnLetterWeight;

            // get the collection of work sheets
            Excel.Range range = xlSheet.UsedRange;
            int rowCount = range.Rows.Count;
            int colStartIndex = ExcelHelpers.ColumnLetterToColumnIndex(ColumnLetterOutputStart);
            int palletColStartIndex = colStartIndex;
            // pallet loop
            var pallets = SelectedPallets(lbPallets);
            foreach (var palletProperties in pallets)
            {
                _log.Info($"Pallet : {palletProperties.Name}");
                int iOutputFieldCount = palletColStartIndex;
                int iNoCols = 0;
                // ### header : begin
                // count
                Excel.Range countHeaderCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + 1];
                countHeaderCell.Value = Resources.ID_RESULT_NOCASES;
                ++iNoCols;
                // tiCount
                Excel.Range tiCountHeader = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + 1];
                tiCountHeader.Value = Resources.ID_RESULT_PERLAYERCOUNT;
                ++iNoCols;
                // hiCount
                Excel.Range hiCountHeader = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + 1];
                hiCountHeader.Value = Resources.ID_RESULT_LAYERCOUNT;
                ++iNoCols;
                // load dimensions
                Excel.Range loadDimensionsHeaderCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + 1];
                loadDimensionsHeaderCell.Value = $"{Resources.ID_RESULT_LOADDIMENSIONS} ({UnitsManager.LengthUnitString}x{UnitsManager.LengthUnitString}x{UnitsManager.LengthUnitString})";
                ++iNoCols;
                // pallet dimensions
                Excel.Range palletDimensionsHeaderCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + 1];
                palletDimensionsHeaderCell.Value = $"{Resources.ID_RESULT_PALLETDIMENSIONS} ({UnitsManager.LengthUnitString}x{UnitsManager.LengthUnitString}x{UnitsManager.LengthUnitString})";
                ++iNoCols;
                // load weight
                Excel.Range loadWeightHeaderCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + 1];
                loadWeightHeaderCell.Value = Resources.ID_RESULT_LOADWEIGHT + " (" + UnitsManager.MassUnitString + ")";
                ++iNoCols;
                // total pallet weight
                Excel.Range totalPalletWeightHeaderCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + 1];
                totalPalletWeightHeaderCell.Value = Resources.ID_RESULT_TOTALPALLETWEIGHT + " (" + UnitsManager.MassUnitString + ")";
                ++iNoCols;
                // efficiency
                Excel.Range efficiencyHeaderCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + 1];
                efficiencyHeaderCell.Value = Resources.ID_RESULT_EFFICIENCY;
                ++iNoCols;
                // image
                if (GenerateImage)
                {
                    Excel.Range imageHeaderCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount) + 1];
                    imageHeaderCell.Value = Resources.ID_RESULT_IMAGE;
                    ++iNoCols;
                }
                // set bold font for all header row
                Excel.Range headerRange = xlSheet.Range["a" + 1, ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount) + 1];
                headerRange.Font.Bold = true;
                // modify range for images
                if (GenerateImage)
                {
                    Excel.Range dataRange = xlSheet.Range["a" + 2,
                        ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount) + rowCount];
                    dataRange.RowHeight = 128;
                    Excel.Range imageRange = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount) + 2,
                        ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount) + rowCount];
                    imageRange.ColumnWidth = 24;
                }
                // ### header : end
                // ### rows : begin
                for (var iRow = 2; iRow <= rowCount; ++iRow)
                {
                    iOutputFieldCount = palletColStartIndex;
                    try
                    {
                        // free version should exit after MaxNumberRowFree
                        if (!UserIsSubscribed && iRow > MaxNumberRowFree + 1)
                        {
                            var cell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow];
                            cell.Value = string.Format(Resources.ID_MASSEXCEL_FREEVERSIONLIMITEDNUMBER, MaxNumberRowFree);
                            break;
                        }

                        string[] colHeaders = new string[] { colName, colLength, colWidth, colHeight };
                        bool readValues = true;
                        foreach (var s in colHeaders)
                        {
                            if (null == xlSheet.Range[s + iRow, s + iRow].Value)
                            {
                                readValues = false;
                                break;
                            }
                        }
                        if (!readValues)
                            continue;

                        // get name
                        string name = (xlSheet.Range[colName + iRow, colName + iRow].Value).ToString();
                        // get description
                        string description = string.Empty;
                        try
                        {
                            description = string.IsNullOrEmpty(colDescription) ? string.Empty : (xlSheet.Range[colDescription + iRow, colDescription + iRow].Value).ToString();
                        }
                        catch (Exception ex)
                        {
                            _log.Error(ex.ToString());
                        }
                        // get length
                        double length = (double)xlSheet.Range[colLength + iRow, colLength + iRow].Value;
                        // get width
                        double width = (double)xlSheet.Range[colWidth + iRow, colWidth + iRow].Value;
                        // get height
                        double height = (double)xlSheet.Range[colHeight + iRow, colHeight + iRow].Value;

                        double maxDimension = Math.Max(Math.Max(length, width), height);
                        if (maxDimension < LargestDimensionMinimum)
                        {
                            _log.Warn($"Ignoring row {iRow} -> ({length},{width},{height})");
                            continue;
                        }
                        else
                            _log.Info($"Processing row {iRow} -> ({length},{width},{height})");
                        // get weight
                        double? weight = null;
                        if (!string.IsNullOrEmpty(colWeight)
                            && null != xlSheet.Range[colWeight + iRow, colWeight + iRow].Value)
                            weight = (double)xlSheet.Range[colWeight + iRow, colWeight + iRow].Value;
                        // compute stacking
                        int stackCount = 0, layerCount = 0;
                        double palletLength = 0.0, palletWidth = 0.0, palletHeight = 0.0;
                        double loadLength = 0.0, loadWidth = 0.0, loadHeight = 0.0;
                        double loadWeight = 0.0, totalPalletWeight = 0.0, stackEfficiency = 0.0;
                        string stackImagePath = string.Empty;
                        int iTIcount = 0;
                        string sTiHi = string.Empty;
                        // generate result
                        GenerateResult(name, description
                            , length, width, height, weight
                            , palletProperties, Overhang
                            , AllowCombinations
                            , ref stackCount
                            , ref layerCount, ref iTIcount, ref sTiHi
                            , ref loadWeight, ref totalPalletWeight
                            , ref palletLength, ref palletWidth, ref palletHeight
                            , ref loadLength, ref loadWidth, ref loadHeight
                            , ref stackEfficiency
                            , ref stackImagePath);

                        // insert count
                        var countCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow];
                        countCell.Value = stackCount;
                        // insert TI
                        var TIcountCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow];
                        if (0 != iTIcount)
                            TIcountCell.Value = iTIcount;
                        else
                            TIcountCell.Value = sTiHi;
                        // insert HI
                        var HIcountCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow];
                        HIcountCell.Value = layerCount;
                        // insert load dimensions
                        var loadDimCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow];
                        loadDimCell.Value = $"{loadLength}x{loadWidth}x{loadHeight}";
                        // insert pallet dimensions
                        var palletDimCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow];
                        palletDimCell.Value = $"{palletLength}x{palletWidth}x{palletHeight}";
                        // insert load weight
                        var loadWeightCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow];
                        loadWeightCell.Value = loadWeight;
                        // insert total weight
                        var totalWeightCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow];
                        totalWeightCell.Value = totalPalletWeight;
                        // efficiency
                        var efficiencyCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow];
                        efficiencyCell.Value = Math.Round(stackEfficiency, 2);
                        // insert image 
                        if (GenerateImage)
                        {
                            var imageCell = xlSheet.Range[
                                ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount) + iRow,
                                ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow];
                            xlSheet.Shapes.AddPicture(
                                stackImagePath,
                                Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue,
                                (float)Convert.ToDecimal(imageCell.Left) + 1.0f,
                                (float)Convert.ToDecimal(imageCell.Top) + 1.0f,
                                (float)Convert.ToDecimal(imageCell.Width) - 2.0f,
                                (float)Convert.ToDecimal(imageCell.Height) - 2.0f
                                );
                        }
                    }
                    catch (OutOfMemoryException ex)
                    {
                        _log.Error($"{ex.Message} (row={iRow})");
                        sbErrors.Append($"{ex.Message}"); 
                    }
                    catch (EngineException ex)
                    {
                        _log.Error($"{ex.Message} (row={iRow})");
                        sbErrors.Append($"{ex.Message} (row={iRow})"); 
                    }
                    catch (InvalidCastException ex)
                    {
                        _log.Error( $"{ex.Message} (row={iRow})" );
                        sbErrors.Append($"Invalid cast exception (row={iRow})"); 
                    }
                    catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
                    {
                        _log.Error($"{ex.Message}");
                        iOutputFieldCount = ExcelHelpers.ColumnLetterToColumnIndex(ColumnLetterOutputStart) - 1; ;
                        var countCel = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow];
                        countCel.Value = string.Format($"ERROR : Invalid input data!");
                    }
                    catch (Exception ex)
                    {
                        _log.Error( $"{ex} (row={iRow})" );
                        sbErrors.Append($"{ex.Message} (row={iRow})"); 
                    }
                } // loop row
                  // ### rows : end
                  // increment palletColStartIndex
                palletColStartIndex += iNoCols;

                _log.Info($"End pallet: {palletProperties.Name}");
            } // loop pallets
        }
        private void GenerateResult(
            string name, string description
            , double length, double width, double height, double? weight
            , PalletProperties palletProperties, Vector2D overhang
            , bool allowCombinations
            , ref int stackCount
            , ref int layerCount, ref int TICount, ref string sTiHi
            , ref double loadWeight, ref double totalWeight
            , ref double palletLength, ref double palletWidth, ref double palletHeight
            , ref double loadLength, ref double loadWidth, ref double loadHeight
            , ref double stackEfficiency
            , ref string stackImagePath)
        {
            stackCount = 0;
            totalWeight = 0.0;
            stackImagePath = string.Empty;

            // generate case
            var bProperties = new BoxProperties(null, length, width, height);
            bProperties.ID.SetNameDesc(name, description);
            if (weight.HasValue) bProperties.SetWeight(weight.Value);
            bProperties.SetColor(Color.Chocolate);
            bProperties.TapeWidth = new OptDouble(true, Math.Min(UnitsManager.ConvertLengthFrom(50.0, UnitsManager.UnitSystem.UNIT_METRIC1), 0.5 * width));
            bProperties.TapeColor = Color.Beige;

            // generate image path
            if (GenerateImage)
                stackImagePath = Path.Combine(Path.ChangeExtension(Path.GetTempFileName(), "png"));
            if (GenerateImageInFolder)
                stackImagePath = Path.ChangeExtension(Path.Combine(DirectoryPathImages, $"{name}_on_{palletProperties.Name}"), "png");

            Graphics3DImage graphics = null;
            if (GenerateImage || GenerateImageInFolder)
            {
                graphics = new Graphics3DImage(new Size(ImageSize, ImageSize))
                {
                    FontSizeRatio = 0.01F,
                    CameraPosition = Graphics3D.Corner_0
                };
            }

            // compute analysis
            ConstraintSetCasePallet constraintSet = new ConstraintSetCasePallet();
            constraintSet.SetAllowedOrientations(new[] { !AllowOnlyZOrientation, !AllowOnlyZOrientation, true });
            constraintSet.SetMaxHeight(new OptDouble(true, MaxPalletHeight));
            constraintSet.Overhang = overhang;
            constraintSet.OptMaxWeight = new OptDouble(
                UseAdmissibleLoadWeight && palletProperties.AdmissibleLoadWeight > 0
                , palletProperties.AdmissibleLoadWeight + palletProperties.Weight
                );

            List<AnalysisLayered> analyzes = new List<AnalysisLayered>();
            if (!allowCombinations)
            {
                SolverCasePallet solver = new SolverCasePallet(bProperties, palletProperties, constraintSet);
                analyzes = solver.BuildAnalyses(false);
            }
            else
            {
                List<KeyValuePair<LayerEncap, int>> listLayers = new List<KeyValuePair<LayerEncap, int>>();
                LayerSolver.GetBestCombination(
                    bProperties.OuterDimensions,
                    Vector3D.Zero,
                    palletProperties.GetStackingDimensions(constraintSet),
                    constraintSet,
                    ref listLayers);

                AnalysisCasePallet analysis = new AnalysisCasePallet(bProperties, palletProperties, constraintSet);
                analysis.ID.SetNameDesc(name, description);
                analysis.AddSolution(listLayers);
                analyzes.Add(analysis);
            }

            if (analyzes.Any())
            {
                var analysis = analyzes[0];
                stackCount = analysis.Solution.ItemCount;
                loadWeight = analysis.Solution.LoadWeight;
                totalWeight = analysis.Solution.Weight;
                stackEfficiency = analysis.Solution.VolumeEfficiency;

                if (analysis.Solution is SolutionLayered solutionLayered)
                {
                    if (solutionLayered.HasConstantTI)
                        TICount = solutionLayered.ConstantTI;
                    else
                        TICount = 0;
                    layerCount = solutionLayered.LayerCount;
                    sTiHi = solutionLayered.TiHiString;


                    palletLength = solutionLayered.BBoxGlobal.Length;
                    palletWidth = solutionLayered.BBoxGlobal.Width;
                    palletHeight = solutionLayered.BBoxGlobal.Height;

                    loadLength = solutionLayered.BBoxLoad.Length;
                    loadWidth = solutionLayered.BBoxLoad.Width;
                    loadHeight = solutionLayered.BBoxLoad.Height;
                }

                if (stackCount <= StackCountMax)
                {
                    if (GenerateImage || GenerateImageInFolder)
                    {
                        var sv = new ViewerSolution(analysis.SolutionLay);
                        sv.Draw(graphics, Transform3D.Identity);
                        graphics.Flush();
                    }
                    if (GenerateReport)
                    {
                        var inputData = new ReportDataAnalysis(analysis);
                        string outputFilePath = Path.ChangeExtension(Path.Combine(DirectoryPathReports,
                            $"Report_{analysis.Content.Name}_on_{analysis.Container.Name}"), "pdf");

                        var rnRoot = new ReportNode(Resources.ID_REPORT);
                        Reporter.SetFontSizeRatios(0.015F, 0.05F);
                        Reporter reporter = new ReporterPDF(inputData, ref rnRoot, Reporter.TemplatePath, outputFilePath);
                    }
                }
            }
            if (GenerateImage || GenerateImageInFolder)
            {
                var bmp = graphics.Bitmap;
                bmp.Save(stackImagePath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }
        #endregion
        #region Status
        protected override string GetStatusMessage()
        {
            if (SelectedPallets(lbPallets).Count < 1)
                return Resources.ID_ERROR_NOPALLETSELECTED;
            else
                return base.GetStatusMessage();
        }
        #endregion
        #region Private properties
        private bool GenerateImage { get => chkbGenerateImageInRow.Checked; set => chkbGenerateImageInRow.Checked = value; }
        private bool GenerateImageInFolder { get => chkbGenerateImageInFolder.Checked; set => chkbGenerateImageInFolder.Checked = value; }
        private bool GenerateReport { get => chkbGenerateReportInFolder.Checked; set => chkbGenerateReportInFolder.Checked = value; }
        private string DirectoryPathImages { get => fsFolderImages.FileName; set => fsFolderImages.FileName = value; } 
        private string DirectoryPathReports { get => fsFolderReports.FileName; set => fsFolderReports.FileName = value; }
        private int ImageSize { get => (int)nudImageSize.Value; set => nudImageSize.Value = value; }
        private bool AllowOnlyZOrientation { get => chkbOnlyZOrientation.Checked; }
        private int StackCountMax => Settings.Default.MassExcelStackCountMax;
        private Vector2D Overhang => new Vector2D(uCtrlOverhang.ValueX, uCtrlOverhang.ValueY);
        private double MaxPalletHeight => uCtrlMaxPalletHeight.Value;
        private string ColumnLetterName => cbName.SelectedItem.ToString();
        private string ColumnLetterDescription => cbDescription.SelectedItem.ToString();
        private string ColumnLetterLength => cbLength.SelectedItem.ToString();
        private string ColumnLetterWidth => cbWidth.SelectedItem.ToString();
        private string ColumnLetterHeight => cbHeight.SelectedItem.ToString();
        private string ColumnLetterWeight => cbWeight.SelectedItem.ToString();
        private string ColumnLetterOutputStart => cbOutputStart.SelectedItem.ToString();
        private bool UseAdmissibleLoadWeight { get => chkbPalletAdmissibleLoadWeight.Checked; set => chkbPalletAdmissibleLoadWeight.Checked = value; }
        
        private readonly double LargestDimensionMinimum = 10.0;
        private bool AllowCombinations
        {
            get => chkbAllowCombinations.Checked;
            set => chkbAllowCombinations.Checked = value;
        }
        #endregion
        #region Event handlers
        private void OnGenerateImagesInFolderChanged(object sender, EventArgs e) => fsFolderImages.Enabled = chkbGenerateImageInFolder.Checked;
        private void OnGenerateReportsInFolderChanged(object sender, EventArgs e) => fsFolderReports.Enabled = chkbGenerateReportInFolder.Checked;
        private void OnItemChecked(object sender, ItemCheckEventArgs e)
        {
            if (!AuthorizeCheck)
                e.NewValue = e.CurrentValue; //check state change was not through authorized actions
            UpdateStatus(sender, null);
        }
        private void OnLBPalletsMouseDown(object sender, MouseEventArgs e)
        {
            Point loc = lbPallets.PointToClient(Cursor.Position);
            for (int i = 0; i < lbPallets.Items.Count; i++)
            {
                Rectangle rec = lbPallets.GetItemRectangle(i);
                rec.Width = 16; //checkbox itself has a default width of about 16 pixels

                if (rec.Contains(loc))
                {
                    AuthorizeCheck = true;
                    bool newValue = !lbPallets.GetItemChecked(i);
                    lbPallets.SetItemChecked(i, newValue);//check 
                    AuthorizeCheck = false;
                    return;
                }
            }
        }
        protected bool AuthorizeCheck { get; set; } = true;
        private void OnCheckAllPallets(object sender, EventArgs e) => CheckAll(sender, true);
        private void OnUncheckAllPallets(object sender, EventArgs e) => CheckAll(sender, false);
        #endregion
        #region Helpers
        private void CheckAll(object sender, bool bChecked)
        {
            AuthorizeCheck = true;
            for (int i=0; i<lbPallets.Items.Count; ++i)
                lbPallets.SetItemChecked(i, bChecked);
            AuthorizeCheck= false;
            UpdateStatus(sender, null);
        }
        #endregion

    }
}
