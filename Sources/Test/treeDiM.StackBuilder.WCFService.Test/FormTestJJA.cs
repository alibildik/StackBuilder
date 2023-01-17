#region Using directives
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

using log4net;
using WeifenLuo.WinFormsUI.Docking;

using treeDiM.StackBuilder.WCFService.Test.Properties;
using JJA.InputData;
using System.Xml.Serialization;

using treeDiM.StackBuilder.WCFService.Test.SB_SR;
using System.Diagnostics;
#endregion

namespace treeDiM.StackBuilder.WCFService.Test
{
    public partial class FormTestJJA : DockContent
    {
        #region Constructor
        public FormTestJJA()
        {
            InitializeComponent();
        }
        #endregion
        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _log.Info("Loading form FormTestJJA");
            tbFilePath.Text = DefaultInputFilePath;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            DefaultInputFilePath = tbFilePath.Text;
            Settings.Default.Save();
        }
        #endregion
        #region Grid
        private string ConveyMode(DCSBConveyMode convMode)
        {
            switch (convMode)
            {
                case DCSBConveyMode.Manual: return "Manual";
                default: return string.Empty;
            }
        }
        #region Helpers
        private string Conveyability(DCSBConveyability conv)
        {
            switch (conv)
            {
                case DCSBConveyability.Conveyable: return "Conveyable";
                case DCSBConveyability.NonConveyableDimensions: return "Non conveyable (dimensions)";
                case DCSBConveyability.NonConveyableWeight: return "Non conveyable (weight)";
                default: return string.Empty;
            }
        }
        private string Stability(DCSBStabilityEnum stab)
        {
            switch (stab)
            {
                case DCSBStabilityEnum.Stable: return "stable";
                case DCSBStabilityEnum.Unstable: return "unstable";
                default: return string.Empty;
            }
        }
        private string ConveyFace(DCSBOrientationName orientation)
        {
            switch (orientation)
            {
                case DCSBOrientationName.BottomTop: return "BottomTop";
                case DCSBOrientationName.LeftRight: return "LeftRight";
                case DCSBOrientationName.FrontBack: return "FrontBack";
                default: return string.Empty;

            }
        }
        #endregion
        private void FillGridPallet1()
        {
            try
            {
                gridPallets.Rows.Clear();
                // viewColumnHeader
                SourceGrid.Cells.Views.ColumnHeader viewColumnHeader = new SourceGrid.Cells.Views.ColumnHeader()
                {
                    Background = new DevAge.Drawing.VisualElements.ColumnHeader()
                    {
                        BackColor = Color.LightGray,
                        Border = DevAge.Drawing.RectangleBorder.NoBorder
                    },
                    ForeColor = Color.Black,
                    Font = new Font("Arial", 10, FontStyle.Regular),
                    TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter
                };
                viewColumnHeader.ElementSort.SortStyle = DevAge.Drawing.HeaderSortStyle.None;
                // viewNormal
                var viewNormal = new CellBackColorAlternate(Color.LightBlue, Color.White);
                // ***
                // set first row
                gridPallets.BorderStyle = BorderStyle.FixedSingle;
                gridPallets.ColumnsCount = 4;
                gridPallets.FixedRows = 1;

                // header
                int iCol = 0;
                gridPallets.Rows.Insert(0);
                gridPallets[0, iCol] = new SourceGrid.Cells.ColumnHeader(string.Empty)
                {
                    AutomaticSortEnabled = false,
                    View = viewColumnHeader
                };
                for (int configId = 0; configId < 3; ++configId)
                    gridPallets[0, ++iCol] = new SourceGrid.Cells.ColumnHeader($"{OrientationToString(ColumnIndexToOrientation(configId))}")
                    {
                        AutomaticSortEnabled = false,
                        View = viewColumnHeader
                    };
                // pallet row
                // content
                int iIndex = 0;
                int indexStart = 0;

                // loop on cases
                for (int iCase = 0; iCase < Cases.Count; ++iCase)
                {
                    var crate = Cases[iCase];
                    var dimensions = new DCSBDim3D() { M0 = crate.dimensions[0], M1 = crate.dimensions[1], M2 = crate.dimensions[2] };
                    try
                    {
                        using (var client = new StackBuilderClient())
                        {
                            var configs = client.JJA_GetCaseConfigs(
                                dimensions,
                                crate.weight,
                                crate.pcb,
                                new DCCompFormat()
                                {
                                    Format = OutFormat.IMAGE,
                                    Size = new DCCompSize() { CX = 100, CY = 100 },
                                    ShowCotations = true,
                                    FontSizeRatio = 0.05f
                                }
                                );

                            indexStart = iIndex;
                            for (int configId = 0; configId < 3; ++configId)
                            {
                                iIndex = indexStart;

                                var config = configs[configId];
                                int colIndex = OrientationToColIndex(config.Orientation);
                                string sDim = $"{config.Length} x {config.Width} x {config.Height}";

                                // dimensions
                                ++iIndex;
                                if (configId == 0) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Dimensions") { View = viewNormal }; }
                                gridPallets[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(sDim) { View = viewNormal };
                                // volume
                                ++iIndex;
                                if (configId == 0) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Volume") { View = viewNormal }; }
                                gridPallets[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(config.Volume) { View = viewNormal };
                                // weight
                                ++iIndex;
                                if (configId == 0) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Weight case") { View = viewNormal }; }
                                gridPallets[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(config.Weight) { View = viewNormal };
                                // pcb
                                ++iIndex;
                                if (configId == 0) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Pcb") { View = viewNormal }; }
                                gridPallets[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(config.Pcb) { View = viewNormal };
                                // area
                                ++iIndex;
                                if (configId == 0) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Area bottom/top") { View = viewNormal }; }
                                gridPallets[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(config.AreaBottomTop) { View = viewNormal };
                                ++iIndex;
                                if (configId == 0) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Area front/back") { View = viewNormal }; }
                                gridPallets[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(config.AreaFrontBack) { View = viewNormal };
                                ++iIndex;
                                if (configId == 0) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Area left/right") { View = viewNormal }; }
                                gridPallets[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(config.AreaLeftRight) { View = viewNormal };
                                // stable
                                ++iIndex;
                                if (configId == 0) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Stable") { View = viewNormal }; }
                                gridPallets[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(Stability(config.Stable)) { View = viewNormal };
                                // conveyability
                                ++iIndex;
                                if (configId == 0) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Conveyability") { View = viewNormal }; }
                                gridPallets[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(Conveyability(config.Conveyability)) { View = viewNormal };
                                // convey mode
                                ++iIndex;
                                if (configId == 0) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Convey mode") { View = viewNormal }; }
                                gridPallets[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(ConveyMode(config.ConveyMode)) { View = viewNormal };
                                // convey face
                                ++iIndex;
                                if (configId == 0) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Convey face") { View = viewNormal }; }
                                gridPallets[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(config.ConveyFace) { View = viewNormal };
                                // image
                                ++iIndex;
                                if (configId == 0) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Image case") { View = viewNormal }; }
                                gridPallets[iIndex, colIndex + 1] = new SourceGrid.Cells.Image(ByteArrayToImage(config.Image.Bytes)) { View = viewNormal };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"Error -> Crate {iCase} : {ex.Message}");
                    }
                    for (int iPallet = 0; iPallet < Pallets.Count; ++iPallet)
                    {
                        var pallet = Pallets[iPallet];

                        bool firstSuccessfull = true;
                        // insert row
                        gridPallets.Rows.Insert(++iIndex);
                        iCol = 0;
                        // name
                        gridPallets[iIndex, 0] = new SourceGrid.Cells.RowHeader($"{crate.name} on {pallet.name}")
                        {
                            View = viewColumnHeader,
                            ColumnSpan = 4,
                            Tag = $"{crate.name} on {pallet.name}"
                        };
                        indexStart = iIndex;
                        for (int iOrientation = 0; iOrientation < 3; ++iOrientation)
                        {
                            iIndex = indexStart;
                            using (var client = new StackBuilderClient())
                            {
                                try
                                {
                                    var loadResult = client.JJA_GetLoadResultSinglePallet(
                                        new DCSBDim3D() { M0 = crate.dimensions[0], M1 = crate.dimensions[1], M2 = crate.dimensions[2] },
                                        crate.weight,
                                        crate.pcb,
                                        new DCSBPalletWHeight()
                                        {
                                            ID = iCase,
                                            Name = pallet.name,
                                            Dimensions = new DCSBDim3D()
                                            {
                                                M0 = pallet.dimensions[0],
                                                M1 = pallet.dimensions[1],
                                                M2 = pallet.dimensions[2]
                                            },
                                            Color = pallet.color,
                                            Weight = pallet.weight,
                                            PalletType = pallet.type,
                                            MaxPalletHeight = pallet.maxPalletHeight,
                                            MaxPalletLoad = pallet.maxLoadWeight
                                        },
                                        ColumnIndexToOrientation(iOrientation),
                                        new DCCompFormat()
                                        {
                                            Format = OutFormat.IMAGE,
                                            ShowCotations = true,
                                            Size = new DCCompSize() { CX = 250, CY = 250 },
                                            FontSizeRatio = 0.03f
                                        }
                                        );
                                    if (loadResult.Status.Status == DCSBStatusEnu.Success)
                                    {
                                        // case count per layer
                                        ++iIndex;
                                        if (firstSuccessfull) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Number of case per layer") { View = viewNormal }; }
                                        gridPallets[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(loadResult.Result.NumberPerLayer) { View = viewNormal };
                                        // no of layers
                                        ++iIndex;
                                        if (firstSuccessfull) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Number of layers") { View = viewNormal }; }
                                        gridPallets[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(loadResult.Result.NumberOfLayers) { View = viewNormal };
                                        // pallet map phrase
                                        ++iIndex;
                                        if (firstSuccessfull) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Pallet map phrase") { View = viewNormal }; }
                                        gridPallets[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(loadResult.Result.PalletMapPhrase) { View = viewNormal };
                                        // no of case/pallet
                                        ++iIndex;
                                        if (firstSuccessfull) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Number of case per pallet") { View = viewNormal }; }
                                        gridPallets[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(loadResult.Result.UpalCase) { View = viewNormal };
                                        // no of items/pallet
                                        ++iIndex;
                                        if (firstSuccessfull) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Number of items per pallet") { View = viewNormal }; }
                                        gridPallets[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(loadResult.Result.UpalItem) { View = viewNormal };
                                        // iso base
                                        ++iIndex;
                                        if (firstSuccessfull) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Area efficiency (1st layer)") { View = viewNormal }; }
                                        gridPallets[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(loadResult.Result.IsoBasePercentage.ToString("0.##")) { View = viewNormal };
                                        // iso pallet
                                        ++iIndex;
                                        if (firstSuccessfull) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Pallet volume efficiency (%)") { View = viewNormal }; }
                                        gridPallets[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(loadResult.Result.IsoVolPercentage.ToString("0.##")) { View = viewNormal };
                                        // load weight
                                        ++iIndex;
                                        if (firstSuccessfull) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Load weight (kg)") { View = viewNormal }; }
                                        gridPallets[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(loadResult.Result.LoadWeight) { View = viewNormal };
                                        // total weight
                                        ++iIndex;
                                        if (firstSuccessfull) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Total weight (kg)") { View = viewNormal }; }
                                        gridPallets[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(loadResult.Result.TotalWeight) { View = viewNormal };
                                        // OK / NOK
                                        ++iIndex;
                                        if (firstSuccessfull) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Max load validity") { View = viewNormal }; }
                                        gridPallets[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(loadResult.Result.MaxLoadValidity ? "OK" : "NOK") { View = viewNormal };
                                        // image
                                        ++iIndex;
                                        if (firstSuccessfull) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Image") { View = viewNormal }; }
                                        gridPallets[iIndex, iOrientation + 1] = new SourceGrid.Cells.Image(ByteArrayToImage(loadResult.OutFile.Bytes));
                                        // suggestion pallet length
                                        ++iIndex;
                                        if (firstSuccessfull) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Increase pallet length") { View = viewNormal }; }
                                        DCSBSuggestIncreasePalletXY suggestionPalletLength = loadResult.SuggestPalletLength;
                                        if (null != suggestionPalletLength)
                                        {
                                            gridPallets[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(
                                                $"length: from {suggestionPalletLength.PalletDimFrom.M0} to {suggestionPalletLength.PalletDimTo.M0}\n" +
                                                $"per layer count: from {suggestionPalletLength.PerLayerCountFrom} to {suggestionPalletLength.PerLayerCountTo}\n" +
                                                $" case count : from {suggestionPalletLength.CaseCountFrom} to {suggestionPalletLength.CaseCountTo}");
                                        }
                                        // suggestion pallet width
                                        ++iIndex;
                                        if (firstSuccessfull) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Increase pallet width") { View = viewNormal }; }
                                        DCSBSuggestIncreasePalletXY suggestionPalletWidth = loadResult.SuggestPalletWidth;
                                        if (null != suggestionPalletWidth)
                                        {
                                            gridPallets[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(
                                                $"width: from {suggestionPalletWidth.PalletDimFrom.M1} to {suggestionPalletWidth.PalletDimTo.M1}\n" +
                                                $"per layer count: from {suggestionPalletWidth.PerLayerCountFrom} to {suggestionPalletWidth.PerLayerCountTo}\n" +
                                                $" case count : from {suggestionPalletWidth.CaseCountFrom} to {suggestionPalletWidth.CaseCountTo}");
                                        }
                                        // suggestion pallet dim
                                        ++iIndex;
                                        if (firstSuccessfull) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Increase pallet dim") { View = viewNormal }; }
                                        DCSBSuggestIncreasePalletXY suggestionPalletDim = loadResult.SuggestPalletDim;
                                        if (null != suggestionPalletDim)
                                        {
                                            gridPallets[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(
                                                $"dims: from ({suggestionPalletWidth.PalletDimFrom.M0}, {suggestionPalletDim.PalletDimFrom.M1}) to ({suggestionPalletWidth.PalletDimTo.M0}, {suggestionPalletWidth.PalletDimTo.M1})\n" +
                                                $"per layer count: from {suggestionPalletDim.PerLayerCountFrom} to {suggestionPalletDim.PerLayerCountTo}\n" +
                                                $" case count : from {suggestionPalletDim.CaseCountFrom} to {suggestionPalletDim.CaseCountTo}");
                                        }
                                        // suggestion pallet height
                                        ++iIndex;
                                        if (firstSuccessfull) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Increase pallet height") { View = viewNormal }; }
                                        DCSBSuggestIncreasePalletZ suggestionPalletHeight = loadResult.SuggestPalletHeight;
                                        if (null != suggestionPalletHeight)
                                        {
                                            gridPallets[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(
                                                $"height: from {suggestionPalletHeight.HeightFrom} to {suggestionPalletHeight.HeightTo}\n" +
                                                $" layer count: from  {suggestionPalletHeight.LayerCountFrom} to {suggestionPalletHeight.LayerCountTo}\n" +
                                                $" case count : from {suggestionPalletHeight.CaseCountFrom} to {suggestionPalletHeight.CaseCountTo}");
                                        }
                                        // suggestion box dim1
                                        ++iIndex;
                                        if (firstSuccessfull) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Decrease box dim1") { View = viewNormal }; }
                                        DCSBSuggestDecreaseCaseXY suggestionCaseDim1 = loadResult.SuggestCaseDim1;
                                        gridPallets[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(
                                            $"dim: from ({suggestionCaseDim1.CaseDimFrom.M0}, {suggestionCaseDim1.CaseDimFrom.M1}, {suggestionCaseDim1.CaseDimFrom.M2}) to ({suggestionCaseDim1.CaseDimTo.M0}, {suggestionCaseDim1.CaseDimTo.M1}, {suggestionCaseDim1.CaseDimTo.M2})\n" +
                                            $" per layer count: from  {suggestionCaseDim1.PerLayerCountFrom} to {suggestionCaseDim1.PerLayerCountTo}\n" +
                                            $" case count : from {suggestionCaseDim1.CaseCountFrom} to {suggestionCaseDim1.CaseCountTo}");
                                        // suggestion box dim2
                                        ++iIndex;
                                        if (firstSuccessfull) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Decrease box dim2") { View = viewNormal }; }
                                        DCSBSuggestDecreaseCaseXY suggestionCaseDim2 = loadResult.SuggestCaseDim2;
                                        gridPallets[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(
                                            $"dim: from ({suggestionCaseDim2.CaseDimFrom.M0}, {suggestionCaseDim2.CaseDimFrom.M1}, {suggestionCaseDim2.CaseDimFrom.M2}) to ({suggestionCaseDim2.CaseDimTo.M0}, {suggestionCaseDim2.CaseDimTo.M1}, {suggestionCaseDim2.CaseDimTo.M2})\n" +
                                            $" per layer count: from  {suggestionCaseDim2.PerLayerCountFrom} to {suggestionCaseDim2.PerLayerCountTo}\n" +
                                            $" case count : from {suggestionCaseDim2.CaseCountFrom} to {suggestionCaseDim2.CaseCountTo}");

                                        firstSuccessfull = false;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _log.Error($"Error => {iOrientation} - {ex.Message}");
                                }
                            }
                        } // using
                    }
                }
                gridPallets.AutoSizeCells();
                gridPallets.Columns.StretchToFit();
                gridPallets.AutoStretchColumnsToFitWidth = true;
                gridPallets.Invalidate();
            }
            catch (Exception ex)
            {
                _log.Warn(ex.ToString());
            }
        }

        private void FillGridPallet2()
        {
            gridPallets.Rows.Clear();
            // *** viewColumnHeader
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
            // *** viewColumnHeader
            // *** viewNormal
            var viewNormal = new CellBackColorAlternate(Color.LightBlue, Color.White);
            // ***
            // set first row
            gridPallets.BorderStyle = BorderStyle.FixedSingle;
            gridPallets.ColumnsCount = 2;
            gridPallets.FixedRows = 1;

            // *** header : begin
            int iCol = 0;
            gridPallets.Rows.Insert(0);
            gridPallets[0, iCol] = new SourceGrid.Cells.ColumnHeader("")
            {
                AutomaticSortEnabled = false,
                View = viewColumnHeader
            };
            // *** header : end

            // build pallet array
            var dcsbPallets = new DCSBPalletWHeight[Pallets.Count];
            for (int i = 0; i < Pallets.Count; i++)
            {
                var pallet = Pallets[i];
                dcsbPallets[i] = new DCSBPalletWHeight()
                {
                    Name = pallet.name,
                    Color = pallet.color,
                    Dimensions = new DCSBDim3D() { M0 = pallet.dimensions[0], M1 = pallet.dimensions[1], M2 = pallet.dimensions[2] },
                    Weight = 20.0,
                    MaxPalletHeight = pallet.maxPalletHeight,
                    MaxPalletLoad = pallet.maxLoadWeight
                };
            }
            int iIndex = 0;
            // *** proceed with each crate
            for (int iCase = 0; iCase < Cases.Count; ++iCase)
            {
                var crate = Cases[iCase];
                // *** call WS
                using (StackBuilderClient client = new StackBuilderClient())
                {
                    var loadResults = client.JJA_GetMultiPalletResults(
                        new DCSBDim3D()
                        {
                            M0 = crate.dimensions[0],
                            M1 = crate.dimensions[1],
                            M2 = crate.dimensions[2]
                        },
                        crate.weight,
                        crate.pcb,
                        dcsbPallets);

                    foreach (var loadResult in loadResults)
                    {
                        if (null == loadResult)
                            continue;
                        if (loadResult.Status.Status != DCSBStatusEnu.Success)
                            continue;

                        string sOrientation = OrientationToString(loadResult.Orientation);

                        // name
                        gridPallets.Rows.Insert(++iIndex);
                        gridPallets[iIndex, 0] = new SourceGrid.Cells.RowHeader($"{crate.name} on {loadResult.Pallet.Name} ({sOrientation})")
                        {
                            View = viewColumnHeader,
                            ColumnSpan = 2,
                            Tag = $"{crate.name} on {loadResult.Pallet.Name} ({sOrientation})"
                        };
                        // config id
                        gridPallets.Rows.Insert(++iIndex);
                        gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Orientation") { View = viewNormal };
                        gridPallets[iIndex, 1] = new SourceGrid.Cells.Cell(loadResult.Orientation) { View = viewNormal };
                        // number of case per layer
                        gridPallets.Rows.Insert(++iIndex);
                        gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Number of case per layer") { View = viewNormal };
                        gridPallets[iIndex, 1] = new SourceGrid.Cells.Cell(loadResult.NumberPerLayer) { View = viewNormal };
                        // no of layers
                        gridPallets.Rows.Insert(++iIndex);
                        gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Number of layers") { View = viewNormal };
                        gridPallets[iIndex, 1] = new SourceGrid.Cells.Cell(loadResult.NumberOfLayers) { View = viewNormal };
                        // pallet map phrase
                        gridPallets.Rows.Insert(++iIndex);
                        gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Pallet map phrase") { View = viewNormal };
                        gridPallets[iIndex, 1] = new SourceGrid.Cells.Cell(loadResult.PalletMapPhrase) { View = viewNormal };
                        // no of case/pallet
                        gridPallets.Rows.Insert(++iIndex);
                        gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Number of cases per pallet") { View = viewNormal };
                        gridPallets[iIndex, 1] = new SourceGrid.Cells.Cell(loadResult.UpalCase) { View = viewNormal };
                        // no of items/pallet
                        gridPallets.Rows.Insert(++iIndex);
                        gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Number of items per pallet") { View = viewNormal };
                        gridPallets[iIndex, 1] = new SourceGrid.Cells.Cell(loadResult.UpalItem) { View = viewNormal };
                        // iso base
                        gridPallets.Rows.Insert(++iIndex);
                        gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Area efficiency (1st layer)") { View = viewNormal };
                        gridPallets[iIndex, 1] = new SourceGrid.Cells.Cell(loadResult.IsoBasePercentage) { View = viewNormal };
                        // iso pallet
                        gridPallets.Rows.Insert(++iIndex);
                        gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Pallet volume efficiency (%)") { View = viewNormal };
                        gridPallets[iIndex, 1] = new SourceGrid.Cells.Cell(loadResult.IsoVolPercentage) { View = viewNormal };
                    }
                }
                // *** call WS
            }
            // ***
            gridPallets.AutoSizeCells();
            gridPallets.Columns.StretchToFit();
            gridPallets.AutoStretchColumnsToFitWidth = true;
            gridPallets.Invalidate();
        }

        private void FillGridContainer1()
        {
            try
            {
                gridContainers.Rows.Clear();
                // viewColumnHeader
                SourceGrid.Cells.Views.ColumnHeader viewColumnHeader = new SourceGrid.Cells.Views.ColumnHeader()
                {
                    Background = new DevAge.Drawing.VisualElements.ColumnHeader()
                    {
                        BackColor = Color.LightGray,
                        Border = DevAge.Drawing.RectangleBorder.NoBorder
                    },
                    ForeColor = Color.Black,
                    Font = new Font("Arial", 10, FontStyle.Regular),
                    TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter
                };
                viewColumnHeader.ElementSort.SortStyle = DevAge.Drawing.HeaderSortStyle.None;
                // viewNormal
                var viewNormal = new CellBackColorAlternate(Color.LightBlue, Color.White);
                // ***
                // set first row
                gridContainers.BorderStyle = BorderStyle.FixedSingle;
                gridContainers.ColumnsCount = 4;
                gridContainers.FixedRows = 1;

                // header
                int iCol = 0;
                gridContainers.Rows.Insert(0);
                gridContainers[0, iCol] = new SourceGrid.Cells.ColumnHeader(string.Empty)
                {
                    AutomaticSortEnabled = false,
                    View = viewColumnHeader
                };
                for (int orientation = 0; orientation < 3; ++orientation)
                    gridContainers[0, ++iCol] = new SourceGrid.Cells.ColumnHeader($"{OrientationToString(ColumnIndexToOrientation(orientation))}")
                    {
                        AutomaticSortEnabled = false,
                        View = viewColumnHeader
                    };
                // pallet row
                // content
                int iIndex = 0;
                int indexStart = 0;

                // loop on cases
                for (int iCase = 0; iCase < Cases.Count; ++iCase)
                {
                    var crate = Cases[iCase];
                    var dimensions = new DCSBDim3D() { M0 = crate.dimensions[0], M1 = crate.dimensions[1], M2 = crate.dimensions[2] };

                    using (var client = new StackBuilderClient())
                    {
                        var configs = client.JJA_GetCaseConfigs(
                            dimensions,
                            crate.weight,
                            crate.pcb,
                            new DCCompFormat()
                            {
                                Format = OutFormat.IMAGE,
                                Size = new DCCompSize() { CX = 100, CY = 100 },
                                ShowCotations = true,
                                FontSizeRatio = 0.05f
                            }
                            );

                        indexStart = iIndex;
                        for (int configId = 0; configId < 3; ++configId)
                        {
                            iIndex = indexStart;
                            var config = configs[configId];
                            string sDim = $"{config.Length} x {config.Width} x {config.Height}";

                            int colIndex = OrientationToColIndex(config.Orientation);

                            // dimensions
                            ++iIndex;
                            if (configId == 0) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Dimensions") { View = viewNormal }; }
                            gridContainers[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(sDim) { View = viewNormal };
                            // volume
                            ++iIndex;
                            if (configId == 0) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Volume") { View = viewNormal }; }
                            gridContainers[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(config.Volume) { View = viewNormal };
                            // weight
                            ++iIndex;
                            if (configId == 0) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Weight case") { View = viewNormal }; }
                            gridContainers[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(config.Weight) { View = viewNormal };
                            // pcb
                            ++iIndex;
                            if (configId == 0) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Pcb") { View = viewNormal }; }
                            gridContainers[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(config.Pcb) { View = viewNormal };
                            // area
                            ++iIndex;
                            if (configId == 0) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Area bottom/top") { View = viewNormal }; }
                            gridContainers[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(config.AreaBottomTop) { View = viewNormal };
                            ++iIndex;
                            if (configId == 0) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Area front/back") { View = viewNormal }; }
                            gridContainers[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(config.AreaFrontBack) { View = viewNormal };
                            ++iIndex;
                            if (configId == 0) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Area left/right") { View = viewNormal }; }
                            gridContainers[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(config.AreaLeftRight) { View = viewNormal };
                            // stable
                            ++iIndex;
                            if (configId == 0) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Stable") { View = viewNormal }; }
                            gridContainers[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(Stability(config.Stable)) { View = viewNormal };
                            // conveyability
                            ++iIndex;
                            if (configId == 0) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Conveyability") { View = viewNormal }; }
                            gridContainers[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(Conveyability(config.Conveyability)) { View = viewNormal };
                            // convey mode
                            ++iIndex;
                            if (configId == 0) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Convey mode") { View = viewNormal }; }
                            gridContainers[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(ConveyMode(config.ConveyMode)) { View = viewNormal };
                            // convey face
                            ++iIndex;
                            if (configId == 0) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Convey face") { View = viewNormal }; }
                            gridContainers[iIndex, colIndex + 1] = new SourceGrid.Cells.Cell(config.ConveyFace) { View = viewNormal };
                            // image
                            ++iIndex;
                            if (configId == 0) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Image case") { View = viewNormal }; }
                            gridContainers[iIndex, colIndex + 1] = new SourceGrid.Cells.Image(ByteArrayToImage(config.Image.Bytes)) { View = viewNormal };
                        }
                    }
                    for (int iContainer = 0; iContainer < Containers.Count; ++iContainer)
                    {
                        var container = Containers[iContainer];
                        indexStart = iIndex;
                        bool firstSuccessfull = true;
                        // insert row
                        gridContainers.Rows.Insert(++iIndex);
                        iCol = 0;
                        gridContainers[iIndex, 0] = new SourceGrid.Cells.RowHeader($"{crate.name} on {container.name}")
                        {
                            View = viewColumnHeader,
                            ColumnSpan = 4,
                            Tag = $"{crate.name} on {container.name}"
                        };
 
                        for (int iOrientation = 0; iOrientation < 3; ++iOrientation)
                        {
                            iIndex = indexStart;
                            using (var client = new StackBuilderClient())
                            {
                                var loadResult = client.JJA_GetLoadResultSingleContainer(
                                    new DCSBDim3D() { M0 = crate.dimensions[0], M1 = crate.dimensions[1], M2 = crate.dimensions[2] },
                                    crate.weight,
                                    crate.pcb,
                                    new DCSBContainer()
                                    {
                                        ID = iCase,
                                        Name = container.name,
                                        Dimensions = new DCSBDim3D()
                                        {
                                            M0 = container.dimensions[0],
                                            M1 = container.dimensions[1],
                                            M2 = container.dimensions[2]
                                        },
                                        Color = container.color,
                                        MaxLoadWeight = container.maxLoadWeight
                                    },
                                    ColumnIndexToOrientation(iOrientation),
                                    new DCCompFormat()
                                    {
                                        Format = OutFormat.IMAGE,
                                        ShowCotations = true,
                                        Size = new DCCompSize() { CX = 250, CY = 250 },
                                        FontSizeRatio = 0.03f
                                    }
                                    );

                                if (loadResult.Status.Status == DCSBStatusEnu.Success)
                                {
                                    // case count per layer
                                    ++iIndex;
                                    if (firstSuccessfull) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Number of case per layer") { View = viewNormal }; }
                                    gridContainers[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(loadResult.Result.NumberPerLayer) { View = viewNormal };
                                    // no of layers
                                    ++iIndex;
                                    if (firstSuccessfull) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Number of layers") { View = viewNormal }; }
                                    gridContainers[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(loadResult.Result.NumberOfLayers) { View = viewNormal };
                                    // no of case/pallet
                                    ++iIndex;
                                    if (firstSuccessfull) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Number of case per container") { View = viewNormal }; }
                                    gridContainers[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(loadResult.Result.UpalCase) { View = viewNormal };
                                    // no of items/pallet
                                    ++iIndex;
                                    if (firstSuccessfull) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Number of items per container") { View = viewNormal }; }
                                    gridContainers[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(loadResult.Result.UpalItem) { View = viewNormal };
                                    // iso base
                                    ++iIndex;
                                    if (firstSuccessfull) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Area efficiency (1st layer)") { View = viewNormal }; }
                                    gridContainers[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(loadResult.Result.IsoBasePercentage.ToString("0.##")) { View = viewNormal };
                                    // iso pallet
                                    ++iIndex;
                                    if (firstSuccessfull) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Container volume efficiency (%)") { View = viewNormal }; }
                                    gridContainers[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(loadResult.Result.IsoVolPercentage.ToString("0.##")) { View = viewNormal };
                                    // load weight
                                    ++iIndex;
                                    if (firstSuccessfull) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Load weight (kg)") { View = viewNormal }; }
                                    gridContainers[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(loadResult.Result.LoadWeight) { View = viewNormal };
                                    // total weight
                                    ++iIndex;
                                    if (firstSuccessfull) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Total weight (kg)") { View = viewNormal }; }
                                    gridContainers[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(loadResult.Result.TotalWeight) { View = viewNormal };
                                    // OK / NOK
                                    ++iIndex;
                                    if (firstSuccessfull) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Max load validity") { View = viewNormal }; }
                                    gridContainers[iIndex, iOrientation + 1] = new SourceGrid.Cells.Cell(loadResult.Result.MaxLoadValidity ? "OK" : "NOK") { View = viewNormal };
                                    // image
                                    ++iIndex;
                                    if (firstSuccessfull) { gridContainers.Rows.Insert(iIndex); gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Image") { View = viewNormal }; }
                                    gridContainers[iIndex, iOrientation + 1] = new SourceGrid.Cells.Image(ByteArrayToImage(loadResult.OutFile.Bytes));

                                    firstSuccessfull = false;
                                }
                            }
                        } // using
                    }
                }
                gridContainers.AutoSizeCells();
                gridContainers.Columns.StretchToFit();
                gridContainers.AutoStretchColumnsToFitWidth = true;
                gridContainers.Invalidate();
            }
            catch (Exception ex)
            {
                _log.Warn(ex.ToString());
            }
        }
        private void FillGridContainer2()
        {
            try
            {
                gridContainers.Rows.Clear();
                // *** viewColumnHeader
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
                // *** viewColumnHeader
                // *** viewNormal
                var viewNormal = new CellBackColorAlternate(Color.LightBlue, Color.White);
                // ***
                // set first row
                gridContainers.BorderStyle = BorderStyle.FixedSingle;
                gridContainers.ColumnsCount = 2;
                gridContainers.FixedRows = 1;

                // *** header : begin
                int iCol = 0;
                gridContainers.Rows.Insert(0);
                gridContainers[0, iCol] = new SourceGrid.Cells.ColumnHeader("")
                {
                    AutomaticSortEnabled = false,
                    View = viewColumnHeader
                };
                // *** header : end

                // build pallet array
                var dcsbContainers = new DCSBContainer[Containers.Count];
                for (int i = 0; i < Containers.Count; i++)
                {
                    var container = Containers[i];
                    dcsbContainers[i] = new DCSBContainer()
                    {
                        Name = container.name,
                        Color = container.color,
                        Dimensions = new DCSBDim3D() { M0 = container.dimensions[0], M1 = container.dimensions[1], M2 = container.dimensions[2] },
                        MaxLoadWeight = container.maxLoadWeight,
                    };
                }
                int iIndex = 0;
                // *** proceed with each crate
                for (int iCase = 0; iCase < Cases.Count; ++iCase)
                {
                    var crate = Cases[iCase];

                    using (StackBuilderClient client = new StackBuilderClient())
                    {
                        var loadResults = client.JJA_GetMultiContainerResults(
                            new DCSBDim3D()
                            {
                                M0 = crate.dimensions[0],
                                M1 = crate.dimensions[1],
                                M2 = crate.dimensions[2]
                            }, crate.weight, crate.pcb, dcsbContainers);

                        foreach (var loadResult in loadResults)
                        {
                            if (null == loadResult)
                                continue;
                            if (loadResult.Status.Status != DCSBStatusEnu.Success)
                                continue;

                            string sOrientation = OrientationToString(loadResult.Orientation);
                            // name
                            gridContainers.Rows.Insert(++iIndex);
                            gridContainers[iIndex, 0] = new SourceGrid.Cells.RowHeader($"{crate.name} on {loadResult.Container.Name} ({sOrientation})")
                            {
                                View = viewColumnHeader,
                                ColumnSpan = 2,
                                Tag = $"{crate.name} on {loadResult.Container.Name} ({sOrientation})"
                            };
                            // number of case per layer
                            gridContainers.Rows.Insert(++iIndex);
                            gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Number of case per layer") { View = viewNormal };
                            gridContainers[iIndex, 1] = new SourceGrid.Cells.Cell(loadResult.NumberPerLayer) { View = viewNormal };
                            // no of layers
                            gridContainers.Rows.Insert(++iIndex);
                            gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Number of layers") { View = viewNormal };
                            gridContainers[iIndex, 1] = new SourceGrid.Cells.Cell(loadResult.NumberOfLayers) { View = viewNormal };
                            // no of items/pallet
                            gridContainers.Rows.Insert(++iIndex);
                            gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Number of items per container") { View = viewNormal };
                            gridContainers[iIndex, 1] = new SourceGrid.Cells.Cell(loadResult.UpalItem) { View = viewNormal };
                            // iso base
                            gridContainers.Rows.Insert(++iIndex);
                            gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Area efficiency (1st layer)") { View = viewNormal };
                            gridContainers[iIndex, 1] = new SourceGrid.Cells.Cell(loadResult.IsoBasePercentage.ToString("0.##")) { View = viewNormal };
                            // iso pallet
                            gridContainers.Rows.Insert(++iIndex);
                            gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Container volume efficiency (%)") { View = viewNormal };
                            gridContainers[iIndex, 1] = new SourceGrid.Cells.Cell(loadResult.IsoVolPercentage.ToString("0.##")) { View = viewNormal };
                            // load weight
                            gridContainers.Rows.Insert(++iIndex);
                            gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Load weight (kg)") { View = viewNormal };
                            gridContainers[iIndex, 1] = new SourceGrid.Cells.Cell(loadResult.LoadWeight) { View = viewNormal };
                            // total weight
                            gridContainers.Rows.Insert(++iIndex);
                            gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Total weight (kg)") { View = viewNormal };
                            gridContainers[iIndex, 1] = new SourceGrid.Cells.Cell(loadResult.TotalWeight) { View = viewNormal };
                            // OK / NOK
                            gridContainers.Rows.Insert(++iIndex);
                            gridContainers[iIndex, 0] = new SourceGrid.Cells.Cell($"Max load validity") { View = viewNormal };
                            gridContainers[iIndex, 1] = new SourceGrid.Cells.Cell(loadResult.MaxLoadValidity ? "OK" : "NOK") { View = viewNormal };
                        }
                    }
                }
                gridContainers.AutoSizeCells();
                gridContainers.Columns.StretchToFit();
                gridContainers.AutoStretchColumnsToFitWidth = true;
                gridContainers.Invalidate();
            }
            catch (Exception ex)
            {
                _log.Warn(ex.ToString());
            }
        }
        public Image ByteArrayToImage(byte[] byteArrayIn)
        {
            Image img = null;
            try
            {
                MemoryStream ms = new MemoryStream(byteArrayIn, 0, byteArrayIn.Length);
                ms.Write(byteArrayIn, 0, byteArrayIn.Length);
                img = Image.FromStream(ms, true);
            }
            catch { }
            return img;
        }
        #endregion
        #region Private properties
        public static string DefaultInputFilePath
        {
            get => Settings.Default.JJA_InputFilePath;
            set => Settings.Default.JJA_InputFilePath = value;
        }
        #endregion
        #region Handlers
        private void OnSelectInputDataFile(object sender, EventArgs e)
        {
            if (DialogResult.OK == openFileDialog.ShowDialog())
            {
                tbFilePath.Text = openFileDialog.FileName;
            }
        }
        private void OnLoadMultipleCalls(object sender, EventArgs e)
        {
            _log.Info("Calling FillGridContent1()...");
            LoadInputData();

            if (tabCtrlPalletContainer.SelectedIndex == 0)
                FillGridPallet1();
            else if (tabCtrlPalletContainer.SelectedIndex == 1)
                FillGridContainer1();
        }
        private void OnLoadSingleCall(object sender, EventArgs e)
        {
            _log.Info("Calling FillGridContent2()...");
            LoadInputData();

            if (tabCtrlPalletContainer.SelectedIndex == 0)
                FillGridPallet2();
            else if (tabCtrlPalletContainer.SelectedIndex == 1)
                FillGridContainer2();
        }

        private void OnFileChanged(object sender, EventArgs e)
        {
            bool fileExists = File.Exists(tbFilePath.Text);
            bnEditInputFile.Enabled = fileExists;
            bnLoad1.Enabled = fileExists;
            bnLoad2.Enabled = fileExists;
        }
        private void OnEditFile(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("notepad.exe", tbFilePath.Text);
        }
        #endregion
        #region Loading data
        private bool LoadInputData()
        {
            Containers.Clear();
            Pallets.Clear();
            Cases.Clear();

            bool result = false;
            string filePath = tbFilePath.Text;
            if (File.Exists(filePath))
            {
                using (var fStream = new FileStream(filePath, FileMode.Open))
                {
                    result = DeserializeInput(fStream, ref Containers, ref Pallets, ref Cases);
                }
            }
            return result;
        }
        private bool DeserializeInput(FileStream fStream, ref List<inputContainer> containers, ref List<inputPallet> pallets, ref List<inputCase> cases)
        {
            // Construct an instance of the XmlSerializer with the type  of object that is being deserialized.  
            XmlSerializer inputSerializer = new XmlSerializer(typeof(input));
            // Call the Deserialize method and cast to the object type.  
            var inputRoot = (input)inputSerializer.Deserialize(fStream);
            foreach (var inputCont in inputRoot.containers)
            { containers.Add(inputCont); }
            foreach (var inputPallet in inputRoot.pallets)
            { pallets.Add(inputPallet); }
            foreach (var inputCase in inputRoot.cases)
            { cases.Add(inputCase); }
            return containers.Count > 0 && pallets.Count > 0 && cases.Count > 0;
        }
        #endregion
        #region Helpers
        private DCSBOrientation ColumnIndexToOrientation(int iOrient)
        { 
            Debug.Assert(iOrient >= 0 && iOrient < 3);
            DCSBOrientation[] Orientations = { DCSBOrientation.BottomTop, DCSBOrientation.FrontBack, DCSBOrientation.LeftRight };
            return Orientations[iOrient];
        }
        private int OrientationToColIndex(DCSBOrientation orientation)
        { 
            switch (orientation)
            {
                case DCSBOrientation.BottomTop: return 0;
                case DCSBOrientation.FrontBack: return 1;
                case DCSBOrientation.LeftRight: return 2;
                default: Debug.Assert(false); return -1;
            }
        }
        private string OrientationToString(DCSBOrientation dcsbOrientation)
        {
            switch (dcsbOrientation)
            {
                case DCSBOrientation.FrontBack: return "Front/Back";
                case DCSBOrientation.LeftRight: return "Left/Right";
                case DCSBOrientation.BottomTop: return "Bottom/Top";
                default: Debug.Assert(false); return string.Empty;
            }
        }
        #endregion
        #region Data members
        private List<inputPallet> Pallets = new List<inputPallet>();
        private List<inputContainer> Containers = new List<inputContainer>();
        private List<inputCase> Cases = new List<inputCase>();
        protected ILog _log = LogManager.GetLogger(typeof(FormTestJJA));
        #endregion
    }
}
