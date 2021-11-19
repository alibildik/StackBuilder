#region Using directives
using System;
using System.Text;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;

using log4net;
using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Exporters
{
    public class ExporterCSV_TechBSA : ExporterRobot
    {
        #region Static members
        public static string FormatName => "csv (TechnologyBSA)";
        #endregion
        #region TechBSA specific
        private double HeightOffset(AnalysisLayered analysisLayered) => -(analysisLayered.Container as PalletProperties).Height;
        public int LayerDesignMode { get; set; } = 0;
        #endregion
        #region Override Exporter
        public override string Name => FormatName;
        public override string Filter => "Comma Separated Values (*.csv) |*.csv";
        public override string Extension => "csv";
        public override bool ShowSelectorCoordinateMode => false;
        public override System.Drawing.Bitmap BrandLogo => Properties.Resources.BSA_Technology;
        public override void Export(AnalysisLayered analysis, NumberFormatInfo nfi, ref StringBuilder sb)
        {
            if (analysis.SolutionLay.ItemCount > MaximumNumberOfCases)
                throw new ExceptionTooManyItems(Name, analysis.Solution.ItemCount, MaximumNumberOfCases);

            // height offset
            double heightOffset = HeightOffset(analysis);
            Vector3D positionOffset = heightOffset * Vector3D.ZAxis;

            // initialize file
            sb.AppendLine("Parameter; Field; DataType; Value");
            sb.AppendLine("Program:StackBuilder;$Type; STRING; PvParameter");

            // ### CASES ###
            // case counter
            uint iCaseCount = 0;
            var sol = analysis.SolutionLay;
            var layers = sol.Layers;
            foreach (var layer in layers)
            {
                if (layer is Layer3DBoxIndexed layerBox)
                {
                    var sortedLayer3DBox = layerBox.Sort(analysis.Content);
                    foreach (var bpi in sortedLayer3DBox)
                    {
                        var bPosition = bpi.BPos;
                        var pos = ConvertPosition(bPosition, analysis.ContentDimensions) + positionOffset;
                        int angle = 0;
                        switch (bPosition.DirectionLength)
                        {
                            case HalfAxis.HAxis.AXIS_X_P: angle = 0; break;
                            case HalfAxis.HAxis.AXIS_Y_P: angle = 90; break;
                            case HalfAxis.HAxis.AXIS_X_N: angle = 180; break;
                            case HalfAxis.HAxis.AXIS_Y_N: angle = 270; break;
                            default: break;
                        }
                        if (iCaseCount < MaximumNumberOfCases)
                        {
                            sb.AppendLine($"Program:StackBuilder;Program:StackBuilder.Box[{iCaseCount}].C;INT;{angle}");
                            sb.AppendLine($"Program:StackBuilder;Program:StackBuilder.Box[{iCaseCount}].X;REAL;{pos.X.ToString("0,0.0", nfi)}");
                            sb.AppendLine($"Program:StackBuilder;Program:StackBuilder.Box[{iCaseCount}].Y;REAL;{pos.Y.ToString("0,0.0", nfi)}");
                            sb.AppendLine($"Program:StackBuilder;Program:StackBuilder.Box[{iCaseCount}].Z;REAL;{pos.Z.ToString("0,0.0", nfi)}");
                            sb.AppendLine($"Program:StackBuilder;Program:StackBuilder.Box[{iCaseCount++}].I;INT;{bpi.Index}");
                        }
                    }
                }
            }
            // up to MaximumNumberOfCases
            while (iCaseCount < MaximumNumberOfCases)
            {
                sb.AppendLine($"Program:StackBuilder;Program:StackBuilder.Box[{iCaseCount}].C;INT;0");
                sb.AppendLine($"Program:StackBuilder;Program:StackBuilder.Box[{iCaseCount}].X;REAL;0");
                sb.AppendLine($"Program:StackBuilder;Program:StackBuilder.Box[{iCaseCount}].Y;REAL;0");
                sb.AppendLine($"Program:StackBuilder;Program:StackBuilder.Box[{iCaseCount++}].Z;REAL;0");
            }
            // ### INTERLAYERS ###
            int iLayerCount = 0;
            foreach (var solItem in sol.SolutionItems)
            {
                if (iLayerCount < MaximumNumberOfInterlayers)
                    sb.AppendLine($"Program:StackBuilder;Program:StackBuilder.InterlayerOnOff[{iLayerCount++}];BOOL;{Bool2string(solItem.HasInterlayer)}");
            }
            // pallet cap
            var hasPalletCap = false;
            if (analysis is AnalysisCasePallet analysisCasePallet)
                hasPalletCap = analysisCasePallet.HasPalletCap;
            if (iLayerCount < MaximumNumberOfInterlayers)
                sb.AppendLine($"Program:StackBuilder;Program:StackBuilder.InterlayerOnOff[{iLayerCount++}];BOOL;{Bool2string(hasPalletCap)}");
            // up to MaximumNumberOfInterlayers
            while (iLayerCount < MaximumNumberOfInterlayers)
                sb.AppendLine($"Program:StackBuilder;Program:StackBuilder.InterlayerOnOff[{iLayerCount++}];BOOL;{Bool2string(false)}");


            // ### BOX OUTER DIMENSIONS
            var dimensions = analysis.Content.OuterDimensions;
            double weight = analysis.Content.Weight;

            sb.AppendLine($"Program:StackBuilder;Program:StackBuilder.BoxDimension.L;REAL;{dimensions.X.ToString("0,0.0", nfi)}");
            sb.AppendLine($"Program:StackBuilder;Program:StackBuilder.BoxDimension.P;REAL;{dimensions.Y.ToString("0,0.0", nfi)}");
            sb.AppendLine($"Program:StackBuilder;Program:StackBuilder.BoxDimension.H;REAL;{dimensions.Z.ToString("0,0.0", nfi)}");
            sb.AppendLine($"Program:StackBuilder;Program:StackBuilder.BoxDimension.W;REAL;{weight.ToString("0,0.0", nfi)}");
            if (analysis.Container is PalletProperties palletProperties)
            {
                sb.AppendLine(
                    $"Program:StackBuilder;Program:StackBuilder.PalletDimension.L;REAL;{palletProperties.Length.ToString("0,0.0", nfi)}");
                sb.AppendLine(
                    $"Program:StackBuilder;Program:StackBuilder.PalletDimension.P;REAL;{palletProperties.Width.ToString("0,0.0", nfi)}");
                sb.AppendLine(
                    $"Program:StackBuilder;Program:StackBuilder.PalletDimension.H;REAL;{(palletProperties.Height+heightOffset).ToString("0,0.0", nfi)}");
                sb.AppendLine(
                    $"Program:StackBuilder;Program:StackBuilder.PalletDimension.W;REAL;{palletProperties.Weight.ToString("0,0.0", nfi)}");
            }

            int numberOfLayers = analysis.ConstraintSet.OptMaxLayerNumber.Value;
            sb.AppendLine($"Program:StackBuilder;Program:StackBuilder.NumberOfLayers;INT;{numberOfLayers}");
            bool layersMirrorX = false;
            if (sol.ItemCount > 1)
                layersMirrorX = sol.SolutionItems[0].SymetryX != sol.SolutionItems[1].SymetryX;
            sb.AppendLine($"Program:StackBuilder;Program:StackBuilder.LayersMirrorXOnOff;BOOL;{Bool2string(layersMirrorX)}");
            bool layersMirrorY = false;
            if (sol.ItemCount > 1)
                layersMirrorY = sol.SolutionItems[0].SymetryY != sol.SolutionItems[1].SymetryY;
            sb.AppendLine($"Program:StackBuilder;Program:StackBuilder.LayersMirrorYOnOff;BOOL;{Bool2string(layersMirrorY)}");
            sb.AppendLine($"Program:StackBuilder;Program:StackBuilder.TotalWeight;REAL;{sol.Weight.ToString("0,0.0", nfi)}");
            sb.AppendLine($"Program:StackBuilder;Program:StackBuilder.LayerDesignMode;INT;{LayerDesignMode}");
        }
        #endregion

        #region Import methods
        public static void Import(Stream csvStream,
            ref List<BoxPositionIndexed> boxPositions,
            ref Vector3D dimCase, ref double weightCase,
            ref Vector3D dimPallet, ref double weightPallet,
            ref int numberOfLayers,
            ref bool layersMirrorX, ref bool layersMirrorY,
            ref List<bool> interlayers,
            ref int layerDesignMode)
        {
            using (TextFieldParser csvParser = new TextFieldParser(csvStream))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { ";" });
                csvParser.HasFieldsEnclosedInQuotes = false;

                // Skip the row with the column names
                csvParser.ReadLine();
                csvParser.ReadLine();

                double zMin = double.MaxValue;
                numberOfLayers = 0;

                while (!csvParser.EndOfData)
                {
                    // Read current line fields, pointer moves to the next line.
                    string[] fields = csvParser.ReadFields();

                    string f1 = fields[1];
                    if (f1.Contains("Program:StackBuilder.Box") && f1.EndsWith(".C"))
                    {
                        try
                        {
                            int angle = int.Parse(fields[3]);
                            fields = csvParser.ReadFields();
                            double x = double.Parse(fields[3], NumberFormatInfo.InvariantInfo);
                            fields = csvParser.ReadFields();
                            double y = double.Parse(fields[3], NumberFormatInfo.InvariantInfo);
                            fields = csvParser.ReadFields();
                            double z = double.Parse(fields[3], NumberFormatInfo.InvariantInfo);
                            fields = csvParser.ReadFields();
                            int index = int.Parse(fields[3], NumberFormatInfo.InvariantInfo);
                            if (angle == 0 && x == 0 && y == 0 && z == 0 && index == 0)
                                continue;
                            if (z < zMin) zMin = z;

                            HalfAxis.HAxis axisL = HalfAxis.HAxis.AXIS_X_P;
                            HalfAxis.HAxis axisW = HalfAxis.HAxis.AXIS_Y_P;
                            switch (angle)
                            {
                                case 0:
                                    axisL = HalfAxis.HAxis.AXIS_X_P;
                                    axisW = HalfAxis.HAxis.AXIS_Y_P;
                                    break;
                                case 90:
                                    axisL = HalfAxis.HAxis.AXIS_Y_P;
                                    axisW = HalfAxis.HAxis.AXIS_X_N;
                                    break;
                                case 180:
                                    axisL = HalfAxis.HAxis.AXIS_X_N;
                                    axisW = HalfAxis.HAxis.AXIS_Y_N;
                                    break;
                                case 270:
                                    axisL = HalfAxis.HAxis.AXIS_Y_N;
                                    axisW = HalfAxis.HAxis.AXIS_X_P;
                                    break;
                                default:
                                    break;
                            }
                            if (Math.Abs(z - zMin) < 1.0E-06)
                                boxPositions.Add(new BoxPositionIndexed(new Vector3D(x, y, z), axisL, axisW, index));
                        }
                        catch (Exception ex)
                        {
                            _log.Error(ex.ToString());
                        }
                    }
                    else if (f1.Contains("Program:StackBuilder.InterlayerOnOff"))
                    {
                        interlayers.Add(string.Equals(fields[3], "TRUE", StringComparison.InvariantCultureIgnoreCase));
                    }
                    else if (f1.Contains("Program:StackBuilder.BoxDimension.L"))
                    {
                        dimCase.X = double.Parse(fields[3], NumberFormatInfo.InvariantInfo);
                        fields = csvParser.ReadFields();
                        dimCase.Y = double.Parse(fields[3], NumberFormatInfo.InvariantInfo);
                        fields = csvParser.ReadFields();
                        dimCase.Z = double.Parse(fields[3], NumberFormatInfo.InvariantInfo);
                        fields = csvParser.ReadFields();
                        weightCase = double.Parse(fields[3], NumberFormatInfo.InvariantInfo);
                    }
                    else if (f1.Contains("Program:StackBuilder.PalletDimension"))
                    {
                        dimPallet.X = double.Parse(fields[3], NumberFormatInfo.InvariantInfo);
                        fields = csvParser.ReadFields();
                        dimPallet.Y = double.Parse(fields[3], NumberFormatInfo.InvariantInfo);
                        fields = csvParser.ReadFields();
                        dimPallet.Z = double.Parse(fields[3], NumberFormatInfo.InvariantInfo);
                        fields = csvParser.ReadFields();
                        weightPallet = double.Parse(fields[3], NumberFormatInfo.InvariantInfo);

                    }
                    else if (f1.Contains("Program:StackBuilder.NumberOfLayers"))
                    {
                        numberOfLayers = int.Parse(fields[3], NumberFormatInfo.InvariantInfo);
                    }
                    else if (f1.Contains("Program:StackBuilder.LayersMirrorXOnOff"))
                    {
                        layersMirrorX = String2Bool(fields[3]);
                    }
                    else if (f1.Contains("Program:StackBuilder.LayersMirrorYOnOff"))
                    {
                        layersMirrorY = String2Bool(fields[3]);
                    }
                    else if (f1.Contains("Program:StackBuilder.LayerDesignMode"))
                    {
                        layerDesignMode = int.Parse(fields[3]);
                    }
                }
            }

            // reset position
            for (int i = 0; i < boxPositions.Count; ++i)
            {
                var bpos = boxPositions[i];
                HalfAxis.HAxis axisLength = bpos.BPos.DirectionLength;
                HalfAxis.HAxis axisWidth = bpos.BPos.DirectionWidth;
                Vector3D vI = HalfAxis.ToVector3D(axisLength);
                Vector3D vJ = HalfAxis.ToVector3D(axisWidth);
                Vector3D vK = Vector3D.CrossProduct(vI, vJ);
                var v = bpos.BPos.Position - 0.5 * dimCase.X * vI - 0.5 * dimCase.Y * vJ - 0.5 * dimCase.Z * vK - dimPallet.Z * Vector3D.ZAxis;
                int index = bpos.Index;
                boxPositions[i] = new BoxPositionIndexed(v, axisLength, axisWidth, index);
            }
        }
        public override void Export(RobotPreparation robotPreparation, NumberFormatInfo nfi, ref StringBuilder sb)
        {
        }
        #endregion

        #region Specific properties
        private int MaximumNumberOfCases => 400;
        private int MaximumNumberOfInterlayers => 40;
        #endregion

        #region Helpers
        private static string Bool2string(bool b) => b ? "TRUE" : "FALSE";
        private static bool String2Bool(string s) =>
            string.Equals(s, "TRUE", StringComparison.CurrentCultureIgnoreCase);


        private static ILog _log = LogManager.GetLogger(typeof(ExporterCSV_TechBSA));
        #endregion
    }
}
