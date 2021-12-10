#region Using directives
using System;
using System.Text;
using System.Globalization;
using System.Drawing;
using System.Collections.Generic;

using log4net;
using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Exporters
{
    public class ExporterCSV_ABB_France : ExporterRobot
    {
        #region Static members
        public static string FormatName => "csv (ABB France)";
        #endregion

        #region Override Exporter
        public override string Name => FormatName;
        public override string Filter => "Comma Separated Values (*.csv) | *.csv";
        public override string Extension => "csv";
        public override bool UseCoordinateSelector => false;
        public override bool UseAngleSelector => true;
        public override bool UseRobotPreparation => true;
        public override bool UseDockingOffsets => true;
        public override Bitmap BrandLogo => Properties.Resources.ABB_France;
        public override void Export(AnalysisLayered analysis, NumberFormatInfo nfi, ref StringBuilder sb)
        {
            throw new NotImplementedException($"Direct analysis export not implemented with {FormatName}.");
        }
        public override void Export(RobotPreparation robotPreparation, NumberFormatInfo nfi, ref StringBuilder sb)
        {
            try
            {
                var analysis = robotPreparation.Analysis;
                var sol = analysis.SolutionLay;
                var pal = analysis.PalletProperties;
                var boxDim = analysis.Content.OuterDimensions;
                var weight = analysis.Content.Weight;
                var cog = analysis.Content.COG;
                var cogInterlayer = Vector3D.Zero;

                sb.AppendLine("START;");
                sb.AppendLine($"ExportVersion;{ExportVersion};");
                sb.AppendLine($"Config;[{sol.LayerCount},{sol.InterlayerCount},{sol.ItemCount},{robotPreparation.NumberOfPlaceCycles}];");
                sb.AppendLine($"Pallet;[[{pal.Length},{pal.Width},{pal.Height}],{pal.Weight}];");

                robotPreparation.GetLayers(out List<RobotLayer> robotLayers, out List<Pair<int, double>> interlayers);
                for (int iLayer = 0; iLayer < robotPreparation.NumberOfLayers; ++iLayer)
                {
                    // interlayer
                    if (-1 != interlayers[iLayer].first)
                    {
                        var interlayerProp = analysis.Interlayers[interlayers[iLayer].first];
                        var vPos = new Vector3D(pal.Length/2, pal.Width/2, interlayers[iLayer].second);
                        // item name
                        string itemName = "Interlayer";
                        // item pick setting
                        string itemPickSettings = string.Format(
                            CultureInfo.InvariantCulture,
                            "[[{0},{1},{2}],{3},[{4},{5},{6}],0,0,0]",
                            interlayerProp.Length, interlayerProp.Width, interlayerProp.Thickness, interlayerProp.Weight, cogInterlayer.X, cogInterlayer.Y, cogInterlayer.Z);
                        // place settings
                        int number = 1;
                        int angle = 0;
                        double dockingX = 0.0;
                        double dockingY = 0.0;
                        string placeSettings = string.Format(
                            CultureInfo.InvariantCulture,
                            "[{0},[{1},{2},{3}],{0},[{4},{5},{6}]]",
                            number, vPos.X, vPos.Y, vPos.Z, angle, dockingX, dockingY, robotPreparation.DockingOffsets.Z);
                        // append interlayer line
                        sb.AppendLine($"{itemName};{itemPickSettings};{placeSettings};");
                    }
                    // layer drops
                    var robotLayer = robotLayers[iLayer];
                    foreach (var robotDrop in robotLayer.Drops)
                    {
                        // item name
                        string itemName = "Box";
                        // item pick setting
                        string itemPickSettings = string.Format(
                            CultureInfo.InvariantCulture,
                            "[[{0},{1},{2}],{3},[{4},{5},{6}],{7},{8},{9}]",
                            boxDim.X, boxDim.Y, boxDim.Z, weight, cog.X, cog.Y, cog.Z, robotPreparation.AngleGrabber, robotPreparation.AngleItem, robotPreparation.Facing
                            );
                        // place settings
                        Vector3D vPos = robotDrop.Center3D;
                        int angle = Modulo360(robotDrop.RawAngle + robotPreparation.AngleGrabber - robotPreparation.AngleItem);
                        double dockingX = DockingDistanceX(robotDrop, robotPreparation.DockingOffsets.X);
                        double dockingY = DockingDistanceY(robotDrop, robotPreparation.DockingOffsets.Y);
                        string placeSettings = string.Format(
                            CultureInfo.InvariantCulture,
                            "[{0},[{1},{2},{3}],{4},[{5},{6},{7}]]",
                            robotDrop.Number, vPos.X, vPos.Y, robotDrop.TopHeight, angle, dockingX, dockingY, robotPreparation.DockingOffsets.Z);
                        // append drop line
                        sb.AppendLine($"{itemName};{itemPickSettings};{placeSettings};");
                    }
                }
                sb.AppendLine("END;");
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        #endregion
        #region Helpers
        private static readonly string ExportVersion = "1.0";
        private int Modulo360(int angle)
        {
            int angleNew = angle % 360;
            while (angleNew < 0) angleNew += 360;
            return angleNew;
        }
        private double DockingDistanceX(RobotDrop robotDrop, double dockingDistance)
        {
            if (robotDrop.HasNeighbour(RobotDrop.NeighbourDir.LEFT, dockingDistance))
                return dockingDistance;
            else if (robotDrop.HasNeighbour(RobotDrop.NeighbourDir.RIGHT, dockingDistance))
                return -dockingDistance;
            else
                return 0.0;
        }
        private double DockingDistanceY(RobotDrop robotDrop, double dockingDistance)
        {
            if (robotDrop.HasNeighbour(RobotDrop.NeighbourDir.BOTTOM, dockingDistance))
                return dockingDistance;
            else if (robotDrop.HasNeighbour(RobotDrop.NeighbourDir.TOP, dockingDistance))
                return -dockingDistance;
            else
                return 0.0;
        }
        #endregion

        #region Private data members
        private static ILog _log = LogManager.GetLogger(typeof(ExporterCSV_ABB_France));
        #endregion
    }
}
