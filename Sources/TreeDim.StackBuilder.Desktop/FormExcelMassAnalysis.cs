#region Using directives
using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;

using Syroot.Windows.IO;

using Excel = Microsoft.Office.Interop.Excel;

using Sharp3D.Math.Core;
using log4net;

using treeDiM.PLMPack.DBClient.PLMPackSR;
using treeDiM.PLMPack.DBClient;

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
    public partial class FormExcelMassAnalysis : Form
    {
        #region Constructor
        public FormExcelMassAnalysis()
        {
            InitializeComponent();
        }
        #endregion
        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            FillPalletList();

            ExcelHelpers.FillComboWithColumnName(cbName);
            ExcelHelpers.FillComboWithColumnName(cbDescription);
            ExcelHelpers.FillComboWithColumnName(cbLength);
            ExcelHelpers.FillComboWithColumnName(cbWidth);
            ExcelHelpers.FillComboWithColumnName(cbHeight);
            ExcelHelpers.FillComboWithColumnName(cbWeight);
            ExcelHelpers.FillComboWithColumnName(cbOutputStart);

            uCtrlMaxPalletHeight.Value = Settings.Default.MaximumPalletHeight;
            uCtrlOverhang.ValueX = Settings.Default.OverhangX;
            uCtrlOverhang.ValueY = Settings.Default.OverhangY;

            LoadSettings();
            OnGenerateImagesInFolderChanged(this, null);
            OnGenerateReportsInFolderChanged(this, null);

            UpdateStatus(this, null);
        }
        #endregion
        #region Load sheets
        private void OnFilePathChanged(object sender, EventArgs e)
        {
            if (File.Exists(InputFilePath))
            {
                try
                {
                    cbSheets.Items.Clear();
                    Excel.Application xlApp = new Excel.Application() { Visible = false, DisplayAlerts = false };
                    Excel.Workbook xlWbk = xlApp.Workbooks.Open(InputFilePath, Type.Missing, false, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    foreach (Excel.Worksheet worksheet in xlWbk.Worksheets)
                    {
                        cbSheets.Items.Add(worksheet.Name);
                    }
                    xlWbk.Close();

                    if (cbSheets.Items.Count > 0)
                        cbSheets.SelectedIndex = 0;
                    xlApp.Quit();
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message);
                }
            }
            UpdateStatus(sender, e);
        }
        #endregion
        #region Pallet listbox
        private void FillPalletList()
        {
            int rangeIndex = 0;
            int numberOfItems = 0;

            using (WCFClient wcfClient = new WCFClient())
            {
                do
                {

                    DCSBPallet[] dcsbPallets = wcfClient.Client.GetAllPallets(rangeIndex++, ref numberOfItems);
                    foreach (var dcsbPallet in dcsbPallets)
                    {
                        UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)dcsbPallet.UnitSystem;
                        var palletProperties = new PalletProperties(null,
                            dcsbPallet.PalletType,
                            UnitsManager.ConvertLengthFrom(dcsbPallet.Dimensions.M0, us),
                            UnitsManager.ConvertLengthFrom(dcsbPallet.Dimensions.M1, us),
                            UnitsManager.ConvertLengthFrom(dcsbPallet.Dimensions.M2, us)
                            )
                        {
                            Color = Color.FromArgb(dcsbPallet.Color),
                            Weight = UnitsManager.ConvertMassFrom(dcsbPallet.Weight, us)
                        };
                        palletProperties.ID.SetNameDesc(dcsbPallet.Name, dcsbPallet.Description);

                        lbPallets.Items.Add(new ItemBaseWrapper(palletProperties), true);
                    }
                }
                while ((rangeIndex + 1) * 20 < numberOfItems);
            }
        }
        #endregion
        #region Computation
        private void OnCompute(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            SaveSettings();
            StringBuilder sbErrors = new StringBuilder();
            try
            {
                string colName = ColumnLetterName;
                string colDescription = ColumnLetterDescription;
                string colLength = ColumnLetterLength;
                string colWidth = ColumnLetterWidth;
                string colHeight = ColumnLetterHeight;
                string colWeight = ColumnLetterWeight;

                string filePath = InputFilePath;

                string outputPath = Path.Combine(Path.GetDirectoryName(InputFilePath), Path.GetFileNameWithoutExtension(InputFilePath) + "_output");
                string filePathCopy = Path.ChangeExtension(outputPath, Path.GetExtension(InputFilePath));

                File.Copy(filePath, filePathCopy, true);

                // get the collection of work sheets
                Excel.Application xlApp = new Excel.Application() { Visible = true, DisplayAlerts = false };
                Excel.Workbook xlWbk = xlApp.Workbooks.Open(filePathCopy, Type.Missing, false, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                Excel.Worksheet xlSheet = (Excel.Worksheet)xlApp.Sheets[SheetName];
                Excel.Range range = xlSheet.UsedRange;
                int rowCount = range.Rows.Count;
                int colStartIndex = ExcelHelpers.ColumnLetterToColumnIndex(ColumnLetterOutputStart);
                int palletColStartIndex = colStartIndex;
                // pallet loop
                var pallets = SelectedPallets;
                foreach (var palletProperties in pallets)
                {
                    int iOutputFieldCount = palletColStartIndex;
                    int iNoCols = 0;
                    // ### header : begin
                    // count
                    Excel.Range countHeaderCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + 1];
                    countHeaderCell.Value = Resources.ID_RESULT_NOCASES;
                    ++iNoCols;
                    // byLayerCount
                    Excel.Range layerCountHeader = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + 1];
                    layerCountHeader.Value = Resources.ID_RESULT_LAYERCOUNT;
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
                    efficiencyHeaderCell.Value = Resources.ID_RESULT_EFFICIENCY + " (%)";
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
                        try
                        {
                            iOutputFieldCount = palletColStartIndex;

                            // free version should exit after MaxNumberRowFree
                            if (!Program.IsSubscribed && iRow > MaxNumberRowFree + 1)
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
                            string description = string.IsNullOrEmpty(colDescription) ? string.Empty : (xlSheet.Range[colDescription + iRow, colDescription + iRow].Value).ToString();
                            // get length
                            double length = (double)xlSheet.Range[colLength + iRow, colLength + iRow].Value;
                            // get width
                            double width = (double)xlSheet.Range[colWidth + iRow, colWidth + iRow].Value;
                            // get height
                            double height = (double)xlSheet.Range[colHeight + iRow, colHeight + iRow].Value;

                            double maxDimension = Math.Max(Math.Max(length, width), height);
                            if (maxDimension < LargestDimensionMinimum) continue;

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
                            string noLayersPerNoCasesString = string.Empty;
                            // generate result
                            GenerateResult(name, description
                                , length, width, height, weight
                                , palletProperties, Overhang
                                , AllowCombinations
                                , ref stackCount
                                , ref layerCount, ref noLayersPerNoCasesString
                                , ref loadWeight, ref totalPalletWeight
                                , ref palletLength, ref palletWidth, ref palletHeight
                                , ref loadLength, ref loadWidth, ref loadHeight
                                , ref stackEfficiency
                                , ref stackImagePath);
                            // insert count
                            var countCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow];
                            countCell.Value = stackCount;
                            // insert layer count
                            var layerCountCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow];
                            layerCountCell.Value = $"{noLayersPerNoCasesString}";
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
                        catch (OutOfMemoryException ex) { sbErrors.Append($"{ex.Message}"); }
                        catch (EngineException ex) { sbErrors.Append($"{ex.Message} (row={iRow})"); }
                        catch (InvalidCastException /*ex*/) { sbErrors.Append($"Invalid cast exception (row={iRow})"); }
                        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException /*ex*/)
                        {
                            iOutputFieldCount = ExcelHelpers.ColumnLetterToColumnIndex(ColumnLetterOutputStart) - 1; ;
                            var countCel = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow];
                            countCel.Value = string.Format($"ERROR : Invalid input data!");
                        }
                        catch (Exception ex) { sbErrors.Append($"{ex.Message} (row={iRow})"); }
                    } // loop row
                    // ### rows : end
                    // increment palletColStartIndex
                    palletColStartIndex += iNoCols;
                } // loop pallets
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                switch ((uint)ex.ErrorCode)
                {
                    case 0x800A03EC:
                        MessageBox.Show("NAME_NOT_FOUND : Could not find cell with given name!");
                        break;
                    default:
                        MessageBox.Show(ex.Message);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Arrow;
            }
        }
        private void GenerateResult(
            string name, string description
            , double length, double width, double height, double? weight
            , PalletProperties palletProperties, Vector2D overhang
            , bool allowCombinations
            , ref int stackCount
            , ref int layerCount, ref string noLayersPerNoCasesString
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

            if (analyzes.Count > 0)
            {
                var analysis = analyzes[0];
                stackCount = analysis.Solution.ItemCount;
                loadWeight = analysis.Solution.LoadWeight;
                totalWeight = analysis.Solution.Weight;
                stackEfficiency = analysis.Solution.VolumeEfficiency;

                if (analysis.Solution is SolutionLayered solutionLayered)
                {
                    layerCount = solutionLayered.LayerCount;
                    noLayersPerNoCasesString = solutionLayered.NoLayersPerNoCasesString;

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
        #region Settings
        private void LoadSettings()
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
        }
        private void SaveSettings()
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
        }
        #endregion
        #region Status
        private void UpdateStatus(object sender, EventArgs e)
        {
            string message = string.Empty;
            if (!File.Exists(InputFilePath))
                message = $"Invalid input file path {InputFilePath}.";
            else if (SelectedPallets.Count < 1)
                message = $"No pallet selected. Select one at least.";
            else if (cbSheets.Items.Count < 1)
                message = $"No sheet loaded";

            statusStripLabel.ForeColor = string.IsNullOrEmpty(message) ? Color.Black : Color.Red;
            statusStripLabel.Text = string.IsNullOrEmpty(message) ? Resources.ID_READY : message;

            bnCompute.Enabled = string.IsNullOrEmpty(message);
        }
        #endregion
        #region Private properties
        private string InputFilePath
        {
            get => fileSelectExcel.FileName;
            set => fileSelectExcel.FileName = value; 
        }
        private string SheetName => cbSheets.SelectedItem.ToString();
        private bool GenerateImage { get => chkbGenerateImageInRow.Checked; set => chkbGenerateImageInRow.Checked = value; }
        private bool GenerateImageInFolder { get => chkbGenerateImageInFolder.Checked; set => chkbGenerateImageInFolder.Checked = value; }
        private bool GenerateReport { get => chkbGenerateReportInFolder.Checked; set => chkbGenerateReportInFolder.Checked = value; }
        private string DirectoryPathImages { get => tbFolderImages.Text; set => tbFolderImages.Text = value; } 
        private string DirectoryPathReports { get => tbFolderReports.Text; set => tbFolderReports.Text = value; }
        private int ImageSize { get => (int)nudImageSize.Value; set => nudImageSize.Value = value; }
        private bool AllowOnlyZOrientation { get => chkbOnlyZOrientation.Checked; }
        private int StackCountMax => Settings.Default.MassExcelStackCountMax;
        private Vector2D Overhang => new Vector2D(uCtrlOverhang.ValueX, uCtrlOverhang.ValueY);
        private double MaxPalletHeight => uCtrlMaxPalletHeight.Value;
        private List<PalletProperties> SelectedPallets
        {
            get
            {
                List<PalletProperties> list = new List<PalletProperties>();
                foreach (var item in lbPallets.CheckedItems)
                {
                    if (item is ItemBaseWrapper ibw)
                    {
                        if (ibw.ItemBase is PalletProperties palletProp)
                            list.Add(palletProp);
                    }
                }
                return list;
            }
        }
        private string ColumnLetterName => cbName.SelectedItem.ToString();
        private string ColumnLetterDescription => cbDescription.SelectedItem.ToString();
        private string ColumnLetterLength => cbLength.SelectedItem.ToString();
        private string ColumnLetterWidth => cbWidth.SelectedItem.ToString();
        private string ColumnLetterHeight => cbHeight.SelectedItem.ToString();
        private string ColumnLetterWeight => cbWeight.SelectedItem.ToString();
        private string ColumnLetterOutputStart => cbOutputStart.SelectedItem.ToString();
        private int MaxNumberRowFree { get; set; } = 5;
        private readonly double LargestDimensionMinimum = 10.0;
        private bool AllowCombinations
        {
            get => chkbAllowCombinations.Checked;
            set => chkbAllowCombinations.Checked = value;
        }
        #endregion
        #region Event handlers
        private void OnGenerateImagesInFolderChanged(object sender, EventArgs e)
        {
            tbFolderImages.Enabled = chkbGenerateImageInFolder.Checked;
            bnFolderImages.Enabled = chkbGenerateImageInFolder.Checked;
        }
        private void OnGenerateReportsInFolderChanged(object sender, EventArgs e)
        {
            tbFolderReports.Enabled = chkbGenerateReportInFolder.Checked;
            bnFolderReports.Enabled = chkbGenerateReportInFolder.Checked;
        }
        private void OnItemChecked(object sender, ItemCheckEventArgs e)
        {
            UpdateStatus(sender, null);
        }
        private void OnDownloadSampleSheet(object sender, EventArgs e)
        {
            string fileURL = Settings.Default.MassExcelTestSheetURL;
            try
            {
                var knownFolder = new KnownFolder(KnownFolderType.Downloads);
                string downloadPath = Path.Combine(knownFolder.Path, Path.GetFileName(fileURL));

                using (var client = new WebClient())
                { client.DownloadFile(fileURL, downloadPath); }

                if (File.Exists(downloadPath))
                    fileSelectExcel.FileName = downloadPath;

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
            catch (WebException ex) { MessageBox.Show($"Failed to download file {fileURL} : {ex.Message}"); }
            catch (Exception ex) { _log.Error(ex.Message); }
        }
        #endregion
        #region Logging
        protected ILog _log = LogManager.GetLogger(typeof(FormExcelMassAnalysis));
        #endregion
    }
    #region ExcelHelpers
    internal class ExcelHelpers
    {
        public static double ReadDouble(string name, Excel.Worksheet worksheet, string cellName)
        {
            try { return (double)worksheet.Range[cellName, cellName].Value; }
            catch (Exception ex) { throw new ExceptionCellReading(name, cellName, ex.Message); }
        }
        public static string ReadString(string name, Excel.Worksheet worksheet, string cellName)
        {
            try { return worksheet.Range[cellName, cellName].Value.ToString(); }
            catch (Exception ex) { throw new ExceptionCellReading(name, cellName, ex.Message); }
        }
        public static void FillComboWithColumnName(ComboBox cb)
        {
            cb.Items.Clear();
            char[] az = Enumerable.Range('A', 'Z' - 'A' + 1).Select(i => (char)i).ToArray();
            foreach (var c in az)
                cb.Items.Add(c.ToString());
        }
        public static string ColumnIndexToColumnLetter(int colIndex)
        {
            var div = colIndex;
            var colLetter = string.Empty;
            while (div > 0)
            {
                var mod = (div - 1) % 26;
                colLetter = (char)(65 + mod) + colLetter;
                div = (div - mod) / 26;
            }
            return colLetter;
        }
        public static int ColumnLetterToColumnIndex(string columnLetter, int max = 26)
        {
            string columnLetterUpper = columnLetter.ToUpper();
            int sum = 0;
            foreach (var t in columnLetterUpper)
            {
                sum *= 26;
                sum += t - 'A' + 1;
            }
            if (sum < 0) sum = 0;
            if (sum > max) sum = max;
            return sum;
        }
    }
    #endregion
    #region ExceptionCellReading
    internal class ExceptionCellReading : Exception
    {
        public ExceptionCellReading()
            : base()
        {
        }
        public ExceptionCellReading(string message)
            : base(message)
        {
        }
        public ExceptionCellReading(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        public ExceptionCellReading(string vName, string cellName, string sMessage)
            : base(sMessage)
        {
            VName = vName;
            CellName = cellName;
        }
        public string VName { get; set; }
        public string CellName { get; set; }
        public override string Message => $"{VName} expected in cell {CellName}";
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(Message);
            sb.Append(base.ToString());
            return sb.ToString();
        }
    }
    #endregion
}
