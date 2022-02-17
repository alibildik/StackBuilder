#region Using directives
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;

using log4net;
using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Exporters
{
    public class ExporterCSV_Angle : ExporterRobot
    {
        #region Static members
        public static string FormatName => "csv (with angle)";
        private static readonly string ExportVersion = "1.0";
        #endregion
        #region Override ExporterRobot
        public override Bitmap BrandLogo => Properties.Resources.treeDiM;
        public override string Name => FormatName;
        public override string Extension => "csv";
        public override string Filter => "Comma Separated Values (*.csv)|*.csv";
        public override bool UseDockingOffsets => true;
        public override bool UseRobotPreparation => true;
        #endregion
        #region Export method
        public override void Export(RobotPreparation robotPreparation, NumberFormatInfo nfi, ref StringBuilder sb)
        {
            var analysis = robotPreparation.Analysis;
            var sol = analysis.SolutionLay;
            var pal = analysis.PalletProperties;
            var boxDim = analysis.Content.OuterDimensions;
            var weight = analysis.Content.Weight;
            int indexBox = 0;

            sb.AppendLine($"{ExportVersion};");
            sb.AppendLine($"{pal.Length.ToString("0,0.0", nfi)};{pal.Width.ToString("0,0.0", nfi)};{pal.Height.ToString("0,0.0", nfi)};{pal.Weight.ToString("0,0.0", nfi)};");
            sb.AppendLine($"{analysis.Interlayers.Count + 1}");
            sb.AppendLine($"{indexBox};{boxDim.X.ToString("0,0.0", nfi)};{boxDim.Y.ToString("0,0.0", nfi)};{boxDim.Z.ToString("0,0.0", nfi)};{weight.ToString("0,0.0", nfi)};");
            int indexInterlayer = 1;
            foreach (var interlayer in analysis.Interlayers)
            {
                var interlayerDim = interlayer.Dimensions;
                sb.AppendLine($"{indexInterlayer++};{interlayerDim.X.ToString("0,0.0", nfi)};{interlayerDim.Y.ToString("0,0.0", nfi)};{interlayerDim.Z.ToString("0,0.0", nfi)};");
            }
            sb.AppendLine($"{sol.LayerCount};{sol.ItemCount};{sol.InterlayerCount}");
            robotPreparation.GetLayers(out List<RobotLayer> robotLayers, out List<Pair<int, double>> interlayers);

            for (int iLayer = 0; iLayer < robotPreparation.NumberOfLayers; ++iLayer)
            {
                // interlayer
                if (-1 != interlayers[iLayer].first)
                {
                    var vPos = new Vector3D(pal.Length / 2, pal.Width / 2, interlayers[iLayer].second);
                    sb.AppendLine(string.Format(nfi, "{0};{1};{2};{3};0",
                        interlayers[iLayer].first, vPos.X, vPos.Y, vPos.Z));
                }

                var robotLayer = robotLayers[iLayer];
                foreach (var robotDrop in robotLayer.Drops)
                {
                    Vector3D vPos = robotDrop.Center3D;

                    int idBox = 0;
                    sb.AppendLine(string.Format(nfi, "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}"
                        , idBox
                        , robotDrop.Number
                        , (int)robotDrop.PackDirection
                        , vPos.X, vPos.Y, vPos.Z
                        , Modulo360(robotDrop.RawAngleSimple)
                        , DockingDistanceX(robotDrop, robotPreparation.DockingOffsets.X)
                        , DockingDistanceY(robotDrop, robotPreparation.DockingOffsets.Y)
                        , robotPreparation.DockingOffsets.Z
                        ));
                }
            }
        }
        #endregion
    }
}
