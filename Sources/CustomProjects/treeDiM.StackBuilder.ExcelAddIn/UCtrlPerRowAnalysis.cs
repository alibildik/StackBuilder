#region Using directives
using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using Excel = Microsoft.Office.Interop.Excel;

using Sharp3D.Math.Core;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Engine;
using treeDiM.StackBuilder.Reporting;

using treeDiM.StackBuilder.ExcelAddIn.Properties;
#endregion

namespace treeDiM.StackBuilder.ExcelAddIn
{
    public partial class UCtrlPerRowAnalysis : UserControl
    {
        public UCtrlPerRowAnalysis()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            uCtrlMaxPalletHeight.Value = Settings.Default.LoadedPalletHeight;
            uCtrlOverhang.ValueX = Settings.Default.OverhangX;
            uCtrlOverhang.ValueY = Settings.Default.OverhangY;

            GenerateImage = Settings.Default.GenerateImageInRow;
            GenerateImageInFolder = Settings.Default.GenerateImageInFolder;
            GenerateReport = Settings.Default.GenerateReports;
            DirectoryPathImages = Settings.Default.ImageFolderPath;
            DirectoryPathReports = Settings.Default.ReportFolderPath;            

            OnGenerateImagesChanged(this, null);
            OnGenerateReportChanged(this, null);
        }
        private void SaveSettings()
        {
            Settings.Default.GenerateImageInRow = GenerateImage;
            Settings.Default.GenerateImageInFolder = GenerateImageInFolder;
            Settings.Default.GenerateReports = GenerateReport;
            Settings.Default.ImageFolderPath = DirectoryPathImages;
            Settings.Default.ReportFolderPath = DirectoryPathReports;
        }
        #region Event handlers
        private void OnCompute(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            SaveSettings();

            StringBuilder sbErrors = new StringBuilder();
            try
            {
                string startLetter = Settings.Default.ColumnLetterOutputStart;
                int colStartIndex = ExcelHelpers.ColumnLetterToColumnIndex(Settings.Default.ColumnLetterOutputStart);

                Excel.Worksheet xlSheet = Globals.StackBuilderAddIn.Application.ActiveSheet as Excel.Worksheet;
                Excel.Range range = xlSheet.UsedRange;
                int rowCount = range.Rows.Count;

                // get list of pallets
                List<PalletProperties> pallets = GetSelectedPallets();
                if (0 == pallets.Count)
                {
                    MessageBox.Show(Resources.ID_ERROR_NOPALLETSELECTED);
                    return;
                }
                int palletColStartIndex = colStartIndex - 1;

                // pallet loop
                foreach (PalletProperties palletProperties in pallets)
                {
                    int iOutputFieldCount = palletColStartIndex;
                    int iNoCols = 0;
                    // modify header
                    // count
                    Excel.Range countHeaderCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + 1];
                    countHeaderCell.Value = Resources.ID_RESULT_NOCASES;
                    ++iNoCols;
                    // byLayerCount
                    Excel.Range layerCountHeader = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + 1];
                    layerCountHeader.Value = Resources.ID_RESULT_LAYERCOUNT;
                    ++iNoCols;
                    // load weight
                    Excel.Range loadWeightHeaderCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + 1];
                    loadWeightHeaderCell.Value = Resources.ID_RESULT_LOADWEIGHT + " (" + UnitsManager.MassUnitString + ")";
                    ++iNoCols;
                    // total pallet weight
                    Excel.Range totalPalletWeightHeaderCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + 1];
                    totalPalletWeightHeaderCell.Value = Resources.ID_RESULT_TOTALPALLETWEIGHT + " (" + UnitsManager.MassUnitString + ")";
                    ++iNoCols;
                    // pallet dimensions
                    Excel.Range palletDimensionsHeaderCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + 1];
                    palletDimensionsHeaderCell.Value = $"{Resources.ID_RESULT_PALLETDIMENSIONS} ({UnitsManager.LengthUnitString}x{UnitsManager.LengthUnitString}x{UnitsManager.LengthUnitString})";
                    ++iNoCols;
                    // load dimensions
                    Excel.Range loadDimensionsHeaderCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + 1];
                    loadDimensionsHeaderCell.Value = $"{Resources.ID_RESULT_LOADDIMENSIONS} ({UnitsManager.LengthUnitString}x{UnitsManager.LengthUnitString}x{UnitsManager.LengthUnitString})";
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
                    double largestDimensionMinimum = LargestDimMin;

                    string colName = Settings.Default.ColumnLetterName;
                    string colDescription = Settings.Default.ColumnLetterDescription;
                    string colLength = Settings.Default.ColumnLetterLength;
                    string colWidth = Settings.Default.ColumnLetterWidth;
                    string colHeight = Settings.Default.ColumnLetterHeight;
                    string colWeight = Settings.Default.ColumnLetterWeight;

                    // loop through rows
                    for (var iRow = 2; iRow <= rowCount; ++iRow)
                    {
                        try
                        {
                            // get name
                            string name = (xlSheet.Range[colName + iRow, colName + iRow].Value).ToString();
                            // get description
                            string description = string.IsNullOrEmpty(colDescription) ? string.Empty : (xlSheet.Range[colDescription + iRow, colDescription + iRow].Value).ToString();
                            // get length
                            double length = xlSheet.Range[colLength + iRow, colLength + iRow].Value;
                            // get width
                            double width = xlSheet.Range[colWidth + iRow, colWidth + iRow].Value;
                            // get height
                            double height = xlSheet.Range[colHeight + iRow, colHeight + iRow].Value;

                            double maxDimension = Math.Max(Math.Max(length, width), height);
                            if (maxDimension < largestDimensionMinimum) continue;

                            // get weight
                            double? weight = null;
                            try { weight = xlSheet.Range[colWeight + iRow, colWeight + iRow].Value; } catch (Exception /*ex*/) { }
                            // compute stacking
                            int stackCount = 0, layerCount = 0, byLayerCount = 0;
                            double loadWeight = 0.0, totalPalletWeight = 0.0, stackEfficiency = 0.0;
                            double palletLength = 0.0, palletWidth = 0.0, palletHeight = 0.0;
                            double loadLength = 0.0, loadWidth = 0.0, loadHeight = 0.0;
                            string stackImagePath = string.Empty;
                            iOutputFieldCount = palletColStartIndex;
                            // generate result
                            GenerateResult(name, description
                                , length, width, height, weight
                                , palletProperties
                                , ref stackCount
                                , ref layerCount, ref byLayerCount
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
                            layerCountCell.Value = $"{layerCount} x {byLayerCount}";
                            // insert load weight
                            var loadWeightCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow];
                            loadWeightCell.Value = loadWeight;
                            // insert total weight
                            var totalWeightCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow];
                            totalWeightCell.Value = totalPalletWeight;
                            // insert pallet dimensions
                            var palletDimCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow];
                            palletDimCell.Value = $"{palletLength}x{palletWidth}x{palletHeight}";
                            // insert load dimensions
                            var loadDimCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow];
                            loadDimCell.Value = $"{loadLength}x{loadWidth}x{loadHeight}";
                            // efficiency
                            var efficiencyCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow];
                            efficiencyCell.Value = Math.Round(stackEfficiency, 2);
                            // insert image 
                            if (GenerateImage)
                            {
                                var imageCell = xlSheet.Range[
                                    ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount) + iRow,
                                    ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow];
                                xlSheet.Shapes.AddPicture(stackImagePath,
                                    Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue,
                                    imageCell.Left + 1, imageCell.Top + 1, imageCell.Width - 2, imageCell.Height - 2);
                            }
                        }
                        catch (OutOfMemoryException ex)
                        {
                            sbErrors.Append(ex.Message);
                        }
                        catch (EngineException ex)
                        {
                            sbErrors.Append(ex.Message);
                        }
                        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException /*ex*/)
                        {
                            iOutputFieldCount = palletColStartIndex;
                            var countCel = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow];
                            countCel.Value = string.Format($"ERROR : Invalid input data!");
                        }
                        catch (InvalidCastException ex)
                        {
                            throw ex;
                        }
                        catch (Exception ex)
                        {
                            throw ex; // rethrow
                        }
                    } // loop row
                    // increment palletColStartIndex
                    palletColStartIndex += iNoCols;

                } // loop pallet
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
        private void OnGenerateImagesChanged(object sender, EventArgs e)
        {
            tbFolderImages.Enabled = chkbGenerateImageInFolder.Checked;
            bnFolderImages.Enabled = chkbGenerateImageInFolder.Checked;
        }
        private void OnGenerateReportChanged(object sender, EventArgs e)
        {
            tbFolderReports.Enabled = chkbGenerateReportInFolder.Checked;
            bnFolderReports.Enabled = chkbGenerateReportInFolder.Checked;
        }
        private void OnEditPallets(object sender, EventArgs e)
        {
            try
            {
                // get the collection of work sheets
                Excel.Sheets sheets = Globals.StackBuilderAddIn.Application.Worksheets;
                Excel.Worksheet worksheet;

                // find the "Pallets" worksheet
                try
                {
                    worksheet = (Excel.Worksheet)sheets["Pallets"];
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    worksheet = (Excel.Worksheet)sheets.Add();
                    worksheet.Name = "Pallets";

                    // modify header
                    worksheet.Range["a" + 1, "a" + 1].Value = "Name";
                    worksheet.Range["b" + 1, "b" + 1].Value = "Description";
                    worksheet.Range["c" + 1, "c" + 1].Value = "Length";
                    worksheet.Range["d" + 1, "d" + 1].Value = "Width";
                    worksheet.Range["e" + 1, "e" + 1].Value = "Height";
                    worksheet.Range["f" + 1, "f" + 1].Value = "Weight";
                    worksheet.Range["g" + 1, "g" + 1].Value = "Form factor";

                    var headerRange = worksheet.Range["a" + 1, "g" + 1];
                    headerRange.Font.Bold = true;

                    // initialize pallet sheet
                    string[] palletTypes = PalletData.TypeNames;
                    int i = 2;
                    foreach (string typeName in palletTypes)
                    {
                        var palletData = PalletData.GetByName(typeName);
                        worksheet.Range["a" + i, "a" + i].Value = palletData.Name;
                        worksheet.Range["b" + i, "b" + i].Value = palletData.Description;
                        worksheet.Range["c" + i, "c" + i].Value = palletData.Length;
                        worksheet.Range["d" + i, "d" + i].Value = palletData.Width;
                        worksheet.Range["e" + i, "e" + i].Value = palletData.Height;
                        worksheet.Range["f" + i, "f" + i].Value = palletData.Weight;
                        worksheet.Range["g" + i, "g" + i].Value = palletData.Name;
                        ++i;
                    }

                    // fit column width
                    worksheet.Range["a" + 1, "g" + (i - 1)].Columns.AutoFit();
                    worksheet.Activate();
                }
                OnRefreshPallets(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private List<PalletProperties> GetSelectedPallets()
        {
            var pallets = new List<PalletProperties>();
            foreach (object o in lbPallets.CheckedItems)
            {
                if (o is PalletItem item) pallets.Add(item.PalletProp);
            }
            return pallets;
        }
        private void OnRefreshPallets(object sender, EventArgs e)
        {
            try
            {
                var sheets = Globals.StackBuilderAddIn.Application.Worksheets;
                var worksheet = (Excel.Worksheet)sheets["Pallets"];
                var rowCount = worksheet.UsedRange.Rows.Count;

                var listPallets = new List<PalletProperties>();
                var cumulatedErrorString = new StringBuilder();
                for (int i = 2; i <= rowCount; ++i)
                {
                    try
                    {
                        // try & get typeName
                        string typeName = "EUR";
                        try { typeName = worksheet.Range["g" + i, "g" + i].Value.ToString(); }
                        catch (Exception) { /* ignored */}

                        PalletProperties palletProp = new PalletProperties(null, typeName
                            , ExcelHelpers.ReadDouble("Pallet length", worksheet, "c" + i)
                            , ExcelHelpers.ReadDouble("Pallet width", worksheet, "d" + i)
                            , ExcelHelpers.ReadDouble("Pallet height", worksheet, "e" + i)
                            )
                        {
                            Weight = ExcelHelpers.ReadDouble("Weight", worksheet, "f" + i)
                        };
                        palletProp.ID.SetNameDesc(
                            ExcelHelpers.ReadString("Pallet name", worksheet, "a" + i),
                            ExcelHelpers.ReadString("Pallet description", worksheet, "b" + i)
                            );
                        listPallets.Add(palletProp);
                    }
                    catch (Exception ex)
                    {
                        cumulatedErrorString.Append(ex.Message);
                    }
                }
                // fill list box
                lbPallets.Items.Clear();
                lbPallets.SelectionMode = SelectionMode.One;
                foreach (PalletProperties palletProperties in listPallets)
                {
                    int index = lbPallets.Items.Add(new PalletItem(palletProperties));
                    lbPallets.SetItemChecked(index, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
        #region Computing results
        private void GenerateResult(
            string name, string description
            , double length, double width, double height
            , double? weight
            , PalletProperties palletProperties
            , ref int stackCount
            , ref int layerCount, ref int byLayerCount
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
                stackImagePath = Path.ChangeExtension(Path.Combine(DirectoryPathImages, name), "png");

            Graphics3DImage graphics = null;
            if (GenerateImage || GenerateImageInFolder)
            {
                graphics = new Graphics3DImage(new Size(ImageSize, ImageSize))
                {
                    FontSizeRatio = Settings.Default.FontSizeRatio,
                    CameraPosition = Graphics3D.Corner_0
                };
            }

            // compute analysis
            ConstraintSetCasePallet constraintSet = new ConstraintSetCasePallet();
            constraintSet.SetAllowedOrientations(new[] { !AllowOnlyZOrientation, !AllowOnlyZOrientation, true });
            constraintSet.SetMaxHeight(new OptDouble(true, PalletMaximumHeight));
            constraintSet.Overhang = Overhang;

            //if (palletProperties.AdmissibleLoadWeight > 0)
                //constraintSet.OptMaxWeight = new OptDouble(true, palletProperties.AdmissibleLoadWeight);

            SolverCasePallet solver = new SolverCasePallet(bProperties, palletProperties, constraintSet);
            List<AnalysisLayered> analyzes = solver.BuildAnalyses(false);
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
                    if (solutionLayered.Layers.Count > 0)
                        byLayerCount = solutionLayered.Layers[0].BoxCount;

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
                        var margins = new Margins();
                        //Reporter reporter = new ReporterMSWord(inputData, ref rnRoot, Settings.Default.ReportTemplatePath/*Reporter.TemplatePath*/, outputFilePath, margins);
                        Reporter.SetFontSizeRatios(0.015F, 0.05F);
                        Reporter reporter = new ReporterPDF(inputData, ref rnRoot, Settings.Default.ReportTemplatePath, outputFilePath);
                    }
                }
            }
            if (GenerateImage || GenerateImageInFolder)
            {
                var bmp = graphics.Bitmap;
                bmp.Save(stackImagePath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }
        private void OnFolderImages(object sender, EventArgs e)
        {
            folderBrowserDlgImages.SelectedPath = DirectoryPathImages;
            if (DialogResult.OK == folderBrowserDlgImages.ShowDialog())
                DirectoryPathImages = folderBrowserDlgImages.SelectedPath;
        }
        private void OnFolderReports(object sender, EventArgs e)
        {
            folderBrowserDlgReports.SelectedPath = DirectoryPathReports;
            if (DialogResult.OK == folderBrowserDlgReports.ShowDialog())
                DirectoryPathReports = folderBrowserDlgReports.SelectedPath;
        }
        #endregion

        #region Private properties (Pane settings)
        private bool GenerateImage { get { return chkbGenerateImageInRow.Checked; } set { chkbGenerateImageInRow.Checked = value; } }
        private bool GenerateImageInFolder { get { return chkbGenerateImageInFolder.Checked; } set { chkbGenerateImageInFolder.Checked = value; } }
        private bool GenerateReport { get => chkbGenerateReportInFolder.Checked; set => chkbGenerateReportInFolder.Checked = value; }
        private string DirectoryPathImages { get => tbFolderImages.Text; set => tbFolderImages.Text = value; }
        private string DirectoryPathReports { get => tbFolderReports.Text; set => tbFolderReports.Text = value; }
        private int ImageSize => Settings.Default.ImageSize;
        private bool AllowOnlyZOrientation => chkbOnlyZOrientation.Checked;
        private double PalletMaximumHeight => uCtrlMaxPalletHeight.Value;
        private int StackCountMax => Settings.Default.StackCountMax;
        private double LargestDimMin => Settings.Default.MinDimensions;
        private Vector2D Overhang => new Vector2D(uCtrlOverhang.ValueX, uCtrlOverhang.ValueY);
        #endregion

        #region Static member functions
        internal static string TaskPaneTitle => "Stackbuilder Per Row Analysis";
        #endregion
    }
}
