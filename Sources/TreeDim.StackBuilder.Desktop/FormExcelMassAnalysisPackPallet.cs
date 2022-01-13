#region Using directives
using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Linq;


using Excel = Microsoft.Office.Interop.Excel;
using Sharp3D.Math.Core;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Engine;
using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormExcelMassAnalysisPackPallet : FormExcelMassAnalysis
    {
        #region Constructor
        public FormExcelMassAnalysisPackPallet()
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

            UpdateStatus(this, null);
        }
        #endregion
        #region Override FormExcelMassAnalysis
        protected override string SampleFileURL => Settings.Default.MassExcelTestSheetURLPackPallet;
        protected override void InitializeInputFields()
        {
            base.InitializeInputFields();

            FillPalletList(lbPallets);

            ComboBox[] comboBoxes = new ComboBox[]
            {
                cbName,
                cbDescription,
                cbLength,
                cbWidth,
                cbHeight,
                cbWeight,
                cbNumber,
                cbMaxLength,
                cbMaxWidth,
                cbMaxHeight,
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
            cbNumber.SelectedIndex = 6;
            cbMaxLength.SelectedIndex = 7;
            cbMaxWidth.SelectedIndex = 8;
            cbMaxHeight.SelectedIndex = 9;
            cbOutputStart.SelectedIndex = 10;
            // ---
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
            cbNumber.SelectedIndex = ExcelHelpers.ColumnLetterToColumnIndex(Settings.Default.MassExcelColLetterNumber) - 1;
            cbMaxLength.SelectedIndex = ExcelHelpers.ColumnLetterToColumnIndex(Settings.Default.MassExcelColLetterMaxLength) - 1;
            cbMaxWidth.SelectedIndex = ExcelHelpers.ColumnLetterToColumnIndex(Settings.Default.MassExcelColLetterMaxWidth) - 1;
            cbMaxHeight.SelectedIndex = ExcelHelpers.ColumnLetterToColumnIndex(Settings.Default.MassExcelColLetterMaxHeight) - 1;
            cbOutputStart.SelectedIndex = ExcelHelpers.ColumnLetterToColumnIndex(Settings.Default.MassExcelColLetterOutputStart) - 1;
        }
        protected override void SaveSettings()
        {
            Settings.Default.MassExcelColLetterName = ColumnLetterName;
            Settings.Default.MassExcelColLetterDescription = ColumnLetterDescription;
            Settings.Default.MassExcelColLetterLength = ColumnLetterLength;
            Settings.Default.MassExcelColLetterWidth = ColumnLetterWidth;
            Settings.Default.MassExcelColLetterHeight = ColumnLetterHeight;
            Settings.Default.MassExcelColLetterWeight = ColumnLetterWeight;
            Settings.Default.MassExcelColLetterNumber = ColumnLetterNumber;
            Settings.Default.MassExcelColLetterMaxLength = ColumnLetterMaxLength;
            Settings.Default.MassExcelColLetterMaxWidth = ColumnLetterMaxWidth;
            Settings.Default.MassExcelColLetterMaxHeight = ColumnLetterMaxHeight;
            Settings.Default.MassExcelImageSize = ImageSize;
        }
        #endregion
        #region Computation
        protected override string GetStatusMessage()
        {
            if (SelectedPallets(lbPallets).Count < 1)
                return Resources.ID_ERROR_NOPALLETSELECTED;
            else
                return base.GetStatusMessage();
        }
        protected override void Compute(Excel.Worksheet xlSheet, StringBuilder sbErrors)
        {
            string colName = ColumnLetterName;
            string colDescription = ColumnLetterDescription;
            string colLength = ColumnLetterLength;
            string colWidth = ColumnLetterWidth;
            string colHeight = ColumnLetterHeight;
            string colWeight = ColumnLetterWeight;
            string colNumber = ColumnLetterNumber;
            string colMaxLength = ColumnLetterMaxLength;
            string colMaxWidth = ColumnLetterMaxWidth;
            string colMaxHeight = ColumnLetterMaxHeight;

            Excel.Range range = xlSheet.UsedRange;
            int rowCount = range.Rows.Count;
            int colStartIndex = ExcelHelpers.ColumnLetterToColumnIndex(ColumnLetterOutputStart);
            int palletColStartIndex = colStartIndex;
            // pallet loop
            var pallets = SelectedPallets(lbPallets);
            foreach (var pallet in pallets)
            {
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
                // loadweight
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
                    Excel.Range imagePackHeaderCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + 1];
                    imagePackHeaderCell.Value = Resources.ID_RESULT_PACKIMAGE;
                    imagePackHeaderCell.ColumnWidth = 24;
                    ++iNoCols;

                    Excel.Range imageHeaderCell = xlSheet.Range[ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount) + 1];
                    imageHeaderCell.Value = Resources.ID_RESULT_IMAGE;
                    imageHeaderCell.ColumnWidth = 24;
                    ++iNoCols;

                    Excel.Range dataRange = xlSheet.Range[
                        "a" + 2,
                        ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount) + rowCount
                        ];
                    dataRange.RowHeight = 128;
                    Excel.Range imageRange = xlSheet.Range[
                        ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount) + 2,
                        ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount)+rowCount
                        ];
                }


                // ### header : end

                // set bold font for all header row
                // modify range for images

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
                        string name = (xlSheet.Range[colName+iRow, colName+iRow].Value).ToString();
                        string description = string.IsNullOrEmpty(colDescription) ? string.Empty : (xlSheet.Range[colDescription + iRow, colDescription + iRow]).ToString();
                        double length = (double)xlSheet.Range[colLength + iRow, colLength + iRow].Value;
                        double width = (double)xlSheet.Range[colWidth + iRow, colWidth + iRow].Value;
                        double height = (double)xlSheet.Range[colHeight + iRow, colHeight + iRow].Value;
                        int number = (int)(double)xlSheet.Range[colNumber + iRow, colNumber + iRow].Value;
                        double maxLength = (double)xlSheet.Range[colMaxLength + iRow, colMaxLength + iRow].Value;
                        double maxWidth = (double)xlSheet.Range[colMaxWidth + iRow, colMaxWidth + iRow].Value;
                        double maxHeight = (double)xlSheet.Range[colMaxHeight + iRow, colMaxHeight + iRow].Value;

                        double? weight = null;
                        if (!string.IsNullOrEmpty(colWeight)
                            && null != xlSheet.Range[colWeight + iRow, colWeight + iRow].Value)
                            weight = (double)xlSheet.Range[colWeight + iRow, colWeight + iRow].Value;
                        int stackCount = 0, layerCount = 0;
                        int iTIcount = 0;
                        string sTiHi = string.Empty;
                        double loadLength = 0.0, loadWidth = 0.0, loadHeight = 0.0;
                        double loadWeight = 0.0, totalPalletWeight = 0.0;
                        double palletLength = 0.0, palletWidth = 0.0, palletHeight = 0.0;
                        double stackEfficiency = 0.0;
                        string stackImagePath = string.Empty;
                        string packImagePath = string.Empty;

                        // generate result
                        GenerateResult(name, description
                            , length, width, height, weight
                            , number
                            , WrapperThickness, NoWalls
                            , maxLength, maxWidth, maxHeight
                            , pallet
                            , ref stackCount
                            , ref layerCount, ref iTIcount, ref sTiHi
                            , ref loadWeight, ref totalPalletWeight
                            , ref palletLength, ref palletWidth, ref palletHeight
                            , ref loadLength, ref loadWidth, ref loadHeight 
                            , ref stackEfficiency
                            , ref packImagePath
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
                            var imagePackCell = xlSheet.Range[
                                ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount) + iRow,
                                ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow
                                ];
                            xlSheet.Shapes.AddPicture(
                                packImagePath,
                                Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue,
                                (float)Convert.ToDecimal(imagePackCell.Left) + 1.0f,
                                (float)Convert.ToDecimal(imagePackCell.Top) + 1.0f,
                                (float)Convert.ToDecimal(imagePackCell.Width) - 2.0f,
                                (float)Convert.ToDecimal(imagePackCell.Height) - 2.0f
                                );

                            // pallet image
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
                palletColStartIndex += iNoCols;
            } // loop pallets
        }
        private void GenerateResult(
            string name, string description
            , double length, double width, double height, double? weight
            , int number
            , double wrapperThickness
            , int[] noWalls
            , double maxLength, double maxWidth, double maxHeight
            , PalletProperties pallet
            , ref int stackCount
            , ref int layerCount, ref int TICount, ref string sTiHi
            , ref double loadWeight, ref double totalWeight
            , ref double palletLength, ref double palletWidth, ref double palletHeight
            , ref double loadLength, ref double loadWidth, ref double loadHeight
            , ref double stackEfficiency
            , ref string packImagePath
            , ref string stackImagePath
            )
        {
            stackCount = 0;

            // generate box
            var bProperties = new BoxProperties(null, length, width, height);
            bProperties.ID.SetNameDesc(name, description);
            if (weight.HasValue) bProperties.SetWeight(weight.Value);
            bProperties.SetColor(Color.Turquoise);
            // constraint set
            ConstraintSetCasePallet constraintSet = new ConstraintSetCasePallet();
            constraintSet.SetAllowedOrientations(new[] { false, false, true });
            constraintSet.SetMaxHeight(new OptDouble(true, MaximumPalletHeight));
            constraintSet.Overhang = Overhang;
            // Param set optim pack
            ParamSetPackOptim paramSetOptimPack = new ParamSetPackOptim(number, Vector3D.Zero, new Vector3D(maxLength, maxWidth, maxHeight), false,
                    true, Color.LightGray, NoWalls, WrapperThickness, 0.0,
                    PackWrapper.WType.WT_POLYETHILENE, false, Color.Chocolate, new int[] { 2, 2, 2 }, 0.0, 0.0, 0.0
                    );

            var packOptimizer = new PackOptimizer(
                   bProperties, pallet, constraintSet,
                   paramSetOptimPack
                   );
            List<AnalysisLayered> analyses = packOptimizer.BuildAnalyses(false);


            
            if (analyses.Any())
            {
                var analysis = analyses[0];
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

                if (GenerateImage && stackCount <= StackCountMax)
                {
                    // generate image path
                    stackImagePath = Path.Combine(Path.ChangeExtension(Path.GetTempFileName(), "png"));
                    packImagePath = Path.Combine(Path.ChangeExtension(Path.GetTempFileName(), "png"));

                    // pack
                    var graphicsPack = new Graphics3DImage(new Size(ImageSize, ImageSize))
                    {
                        FontSizeRatio = 0.02F,
                        CameraPosition = Graphics3D.Corner_0
                    };
                    if (analysis.Content is PackProperties packProperties)
                    {
                        var pack = new Pack(0, packProperties);
                        graphicsPack.AddBox(pack);
                        graphicsPack.AddDimensions(
                            new DimensionCube(Vector3D.Zero, pack.Length, pack.Width, pack.Height, Color.Black, true)
                            );
                        if (null != packProperties.Wrap && packProperties.Wrap.Transparent)
                            graphicsPack.AddDimensions(
                                new DimensionCube(
                                    packProperties.InnerOffset
                                    , packProperties.InnerLength, packProperties.InnerWidth, packProperties.InnerHeight, Color.Red, false)
                                );
                        graphicsPack.Flush();
                        var bmpPack = graphicsPack.Bitmap;
                        bmpPack.Save(packImagePath, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    // pallet
                    var graphics = new Graphics3DImage(new Size(ImageSize, ImageSize))
                    {
                        FontSizeRatio = 0.02F,
                        CameraPosition = Graphics3D.Corner_0
                    };
                    var sv = new ViewerSolution(analysis.SolutionLay);
                    sv.Draw(graphics, Transform3D.Identity);
                    graphics.Flush();

                    var bmp = graphics.Bitmap;
                    bmp.Save(stackImagePath, System.Drawing.Imaging.ImageFormat.Png);
                }
            }

        }
        #endregion
        #region Column letter
        private string ColumnLetterName => cbName.SelectedItem.ToString();
        private string ColumnLetterDescription => cbDescription.SelectedItem.ToString();
        private string ColumnLetterLength => cbLength.SelectedItem.ToString();
        private string ColumnLetterWidth => cbWidth.SelectedItem.ToString();
        private string ColumnLetterHeight => cbHeight.SelectedItem.ToString();
        private string ColumnLetterWeight => cbWeight.SelectedItem.ToString();
        private string ColumnLetterNumber => cbNumber.SelectedItem.ToString();
        private string ColumnLetterMaxLength => cbMaxLength.SelectedItem.ToString();
        private string ColumnLetterMaxWidth => cbMaxWidth.SelectedItem.ToString();
        private string ColumnLetterMaxHeight => cbMaxHeight.SelectedItem.ToString();
        private string ColumnLetterOutputStart => cbOutputStart.SelectedItem.ToString();
        #endregion
        #region Private properties
        private int ImageSize { get => (int)nudImageSize.Value; set => nudImageSize.Value = value; }
        private bool GenerateImage => true;
        private double WrapperThickness => uCtrlWrapperThickness.Value;
        private int[] NoWalls => new int[3] { uCtrlNumberOfWalls.NoX, uCtrlNumberOfWalls.NoY, uCtrlNumberOfWalls.NoZ };
        private double MaximumPalletHeight
        { 
            get => uCtrlMaxPalletHeight.Value;
            set => uCtrlMaxPalletHeight.Value = value;
        }
        private int StackCountMax => Settings.Default.MassExcelStackCountMax;
        private Vector2D Overhang
        {
            get => uCtrlOverhang.Value;
            set => uCtrlOverhang.Value = value;
        }
        #endregion
    }
}
