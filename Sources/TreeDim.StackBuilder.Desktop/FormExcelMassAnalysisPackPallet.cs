#region Using directives
using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;


using Excel = Microsoft.Office.Interop.Excel;
using Sharp3D.Math.Core;

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
            cbDescription.SelectedIndex = 2;
            cbLength.SelectedIndex = 3;
            cbWidth.SelectedIndex = 4;
            cbHeight.SelectedIndex = 5;
            cbWeight.SelectedIndex = 6;
            cbNumber.SelectedIndex = 7;
            cbMaxLength.SelectedIndex = 8;
            cbMaxWidth.SelectedIndex = 9;
            cbMaxHeight.SelectedIndex = 10;
            cbOutputStart.SelectedIndex = 11;
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
                // set bold font for all header row
                // modify range for images

                for (var iRow = 2; iRow <= rowCount; ++iRow)
                {
                    try
                    {
                        string name = (xlSheet.Range[colName+iRow, colName+iRow].Value).ToString();
                        string description = string.IsNullOrEmpty(colDescription) ? string.Empty : (xlSheet.Range[colDescription + iRow, colDescription + iRow]).ToString();
                        double length = (double)xlSheet.Range[colLength + iRow, colLength + iRow].Value;
                        double width = (double)xlSheet.Range[colWidth + iRow, colWidth + iRow].Value;
                        double height = (double)xlSheet.Range[colHeight + iRow, colHeight + iRow].Value;
                        double weight = (double)xlSheet.Range[colWeight + iRow, colWeight + iRow].Value;
                        int number = (int)xlSheet.Range[colNumber + iRow, colNumber + iRow].Value;
                        double maxLength = (double)xlSheet.Range[colMaxLength + iRow, colMaxLength + iRow].Value;
                        double maxWidth = (double)xlSheet.Range[colMaxWidth + iRow, colMaxWidth + iRow].Value;
                        double maxHeight = (double)xlSheet.Range[colMaxHeight + iRow, colMaxHeight + iRow].Value;
                        int stackCount = 0;
                        double stackEfficiency = 0.0;
                        string stackImagePath = string.Empty;
                        // generate result
                        GenerateResult(name, description
                            , length, width, height, weight
                            , number
                            , WrapperThickness, NoWalls
                            , maxLength, maxWidth, maxHeight
                            , ref stackCount
                            , ref stackEfficiency
                            , ref stackImagePath);


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
            , ref int stackCount, ref double stackEfficiency
            , ref string stackImagePath
            )
        {
            stackCount = 0;

            // generate box
            var bProperties = new BoxProperties(null, length, width, height);
            bProperties.ID.SetNameDesc(name, description);
            if (weight.HasValue) bProperties.SetWeight(weight.Value);
            bProperties.SetColor(Color.Turquoise);

            Graphics3DImage graphics = null;
            if (GenerateImage)
            {
                graphics = new Graphics3DImage(new Size(ImageSize, ImageSize))
                {
                    FontSizeRatio = 0.01F,
                    CameraPosition = Graphics3D.Corner_0
                };
            }
            List<AnalysisLayered> analyzes = new List<AnalysisLayered>();
            if (analyzes.Count > 0)
            {
                var analysis = analyzes[0];
                if (GenerateImage)
                {
                    var sv = new ViewerSolution(analysis.SolutionLay);
                    sv.Draw(graphics, Transform3D.Identity);
                    graphics.Flush();
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
        private string ColumnLetterOutputStart => cbOutputStart.ToString();
        #endregion
        #region Private properties
        private int ImageSize { get => (int)nudImageSize.Value; set => nudImageSize.Value = value; }
        private bool GenerateImage => true;
        private double WrapperThickness => uCtrlWrapperThickness.Value;
        private int[] NoWalls => new int[3] { uCtrlNumberOfWalls.NoX, uCtrlNumberOfWalls.NoY, uCtrlNumberOfWalls.NoZ };
        #endregion
    }
}
