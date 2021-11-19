#region Using directives
using System;
using System.Text;
using System.Globalization;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Exporters
{
    public class ExporterCSV_FMLogistic : ExporterRobot
    {
        #region Constructor
        public ExporterCSV_FMLogistic()
        {
            PositionCoordinateMode = CoordinateMode.CM_COG;
        }
        #endregion
        #region Override Exporter
        public override string Name => FormatName;
        public override string Extension => "csv";
        public override string Filter => "Comma Separated Values (*.csv) |*.csv";
        public override bool ShowSelectorCoordinateMode => false;
        public override bool UseRobotPreparation => true;
        public override System.Drawing.Bitmap BrandLogo => Properties.Resources.FM_Logistic;
        public override int MaxLayerIndexExporter(AnalysisLayered analysis) => Math.Min(analysis.SolutionLay.LayerCount, 2);

        public override void Export(AnalysisLayered analysis, NumberFormatInfo nfi, ref StringBuilder sb)
        {
            var sol = analysis.SolutionLay;
            var layers = sol.Layers;
            var pallet = analysis.Container as PalletProperties;

            int caseNumber = 1;

            // case dimension
            Vector3D caseDim = analysis.ContentDimensions;
            sb.AppendLine($"{caseNumber},{(int)caseDim.X},{(int)caseDim.Y},{(int)caseDim.Z}");
            // number of layers; number of drops
            int noDrops = sol.SolutionItems.Count;
            sb.AppendLine($"{sol.SolutionItems.Count},{noDrops}");
            // interlayers
            int iLayer = 0;
            int xInterlayer = (int)(pallet.Length / 2);
            int yInterlayer = (int)(pallet.Width / 2);
            foreach (var solItem in sol.SolutionItems)
            {
                sb.AppendLine($"{iLayer + 1},{xInterlayer},{yInterlayer},{(solItem.HasInterlayer ? 1 : 0)},{solItem.InterlayerIndex}");
                ++iLayer;
            }
            bool topInterlayer = analysis is AnalysisCasePallet analysisCasePallet && analysisCasePallet.HasTopInterlayer;
            sb.AppendLine($"{iLayer + 1},{xInterlayer},{yInterlayer},{(topInterlayer ? 1 : 0)},{1}");

            // 1 line per drop in the 2 first layer
            int iLine = 1;
            int actualLayer = 0;
            iLayer = 0;
            while (actualLayer < MaxLayerIndexExporter(analysis))
            {
                if (layers[iLayer] is Layer3DBox layer0)
                {
                    foreach (var bPos in layer0)
                    {
                        Vector3D vPos = ConvertPosition(bPos, caseDim);
                        int orientation = ConvertPositionAngleToPositionIndex(bPos);
                        int blockType = 1;

                        sb.AppendLine($"{iLine},{(int)vPos.X},{(int)vPos.Y},{(int)vPos.Z},{orientation},{caseNumber},{blockType}");
                        ++iLine;
                    }
                    ++actualLayer;
                }
                ++iLayer;
            }
        }
        public override void Export(RobotPreparation robotPreparation, NumberFormatInfo nfi, ref StringBuilder sb)
        {
            int caseNumber = 1;
 
            // case dimensions (X;Y;Z)
            var caseDim = robotPreparation.ContentDimensions;
            sb.AppendLine($"{caseNumber},{(int)caseDim.X},{(int)caseDim.Y},{(int)caseDim.Z}");
            // number of layers; number of drops
            sb.AppendLine($"{robotPreparation.LayerCount},{robotPreparation.DropCount}");
            // interlayers (layer index;X;Y;0/1;index interlayer)
            int iLayer = 1;
            Vector3D palletDim = robotPreparation.PalletDimensions;
            foreach (var indexInterlayer in robotPreparation.ListInterlayerIndexes)
                sb.AppendLine($"{iLayer++},{(int)(0.5f * palletDim.X)},{(int)(0.5f * palletDim.Y)},{(indexInterlayer != -1 ? 1 : 0)},{indexInterlayer}");

            // 1 line per block in the 2 first layer
            // get Layer 0
            int iLine = 1;
            for (int i = 0; i < 2; ++i)
            {
                RobotLayer robotLayer = robotPreparation.GetLayer(i);
                if (null != robotLayer)
                {
                    foreach (var drop in robotLayer.Drops)
                    {
                        BoxPosition boxPos = drop.BoxPositionMain;
                        int orientation = ConvertPositionAngleToPositionIndex(boxPos);
                        Vector3D vPos = ConvertPosition(drop.BoxPositionMain, drop.Dimensions);
                        int blockType = drop.PackDirection == RobotDrop.PackDir.LENGTH ? 1 : 0;

                        sb.AppendLine($"{iLine},{(int)vPos.X},{(int)vPos.Y},{(int)vPos.Z},{orientation},{drop.Number},{blockType}");
                    }
                }
            }
        }
        #endregion
        #region Static members
        public static string FormatName => "csv (FM Logistic)";
        #endregion
    }
}
