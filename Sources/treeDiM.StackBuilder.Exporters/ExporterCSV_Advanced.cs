#region Using directives
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using log4net;
using Sharp3D.Math.Core;
using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Exporters
{
    public class ExporterCSV_Advanced : ExporterRobot
    {
        #region Static members
        public static string FormatName => "csv (Advanced)";
        #endregion
        #region Override ExporterRobot
        public override string Name => FormatName;
        public override string Filter => "Comma Separated Values (*.csv) | *.csv";
        public override string Extension => "csv";
        public override bool UseCoordinateSelector => false;
        public override bool UseAngleSelector => true;
        public override bool UseRobotPreparation => true;
        public override bool UseDockingOffsets => true;
        public override Bitmap BrandLogo => Properties.Resources.treeDiM;
        public override bool ShowOutput => true;
        public override bool UseDirectExport => true;
        public override void Export(RobotPreparation robotPreparation, NumberFormatInfo nfi, ref StringBuilder sb)
        {
            var analysis = robotPreparation.Analysis;
            var sol = analysis.SolutionLay;
            var pal = analysis.PalletProperties;
            var boxDim = analysis.Content.OuterDimensions;
            var weight = analysis.Content.Weight;
            var cog = analysis.Content.COG;
            var cogInterlayer = Vector3D.Zero;
            robotPreparation.GetLayers(out List<RobotLayer> robotLayers, out List<Pair<int, double>> interlayers, out int noCycles);
            string author = analysis.ParentDocument != null ? analysis.ParentDocument.Author : string.Empty;

            sb.AppendLine($"ExportVersion;{ExportVersion};");
            sb.AppendLine($"Date;{DateTime.Now.ToShortDateString()};");
            sb.AppendLine($"Unit;{UnitsManager.LengthUnitString}|{UnitsManager.MassUnitString};");
            sb.AppendLine($"Author;{author};");
            sb.AppendLine($"Analysis;{analysis.Name}");
            int itemID = 1;
            sb.AppendLine($"{itemID++};{pal.Name};{pal.Length};{pal.Width};{pal.Height};{pal.Weight};");
            sb.AppendLine($"{itemID++};{analysis.Content.Name};{boxDim.X};{boxDim.Y};{boxDim.Z};{analysis.Content.Weight};");
            foreach (var interlayer in analysis.Interlayers)
                sb.AppendLine($"{itemID++};{interlayer.Name};{interlayer.Length};{interlayer.Width};{interlayer.Thickness};{interlayer.Weight};");

            for (int iLayer = 0; iLayer < robotPreparation.NumberOfLayers; ++iLayer)
            {
                // interlayer
                if (-1 != interlayers[iLayer].first)
                {
                    var interlayerProp = analysis.Interlayers[interlayers[iLayer].first];
                    var vPos = new Vector3D(pal.Length / 2, pal.Width / 2, interlayers[iLayer].second);
                    int angle = 0;
                    double dockingX = 0.0, dockingY = 0.0;
                    sb.AppendLine($"Interlayer;{interlayers[iLayer].first+2};1;0;{vPos.X};{vPos.Y};{vPos.Z};{angle};{dockingX};{dockingY};{robotPreparation.DockingOffsets.Z};");
                }

                // layer drops
                if (iLayer < robotLayers.Count)
                {
                    var robotLayer = robotLayers[iLayer];
                    foreach (var robotDrop in robotLayer.Drops)
                    {
                        // place settings
                        Vector3D vPos = robotDrop.Center3D;
                        int angle = Modulo360(robotDrop.RawAngleSimple - robotDrop.ConveyorSetting.Angle);
                        double dockingX = DockingDistanceX(robotDrop, robotPreparation.DockingOffsets.X);
                        double dockingY = DockingDistanceY(robotDrop, robotPreparation.DockingOffsets.Y);
                        sb.AppendLine($"Box;2;{robotDrop.Number};{robotDrop.ConveyorSetting.Angle};{vPos.X};{vPos.Y};{vPos.Z};{angle};{dockingX};{dockingY};{robotPreparation.DockingOffsets.Z} ");
                    }
                }
            }
        }
        #endregion

            #region Helpers
        private static readonly string ExportVersion = "2.0";
        #endregion
        #region Private data members
        private static ILog _log = LogManager.GetLogger(typeof(ExporterCSV_Advanced));
        #endregion
    }
}
