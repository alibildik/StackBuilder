#region Using directives
using System.Text;
using System.Globalization;
using System.Drawing;
using System.Collections.Generic;
using System.IO;

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
        #region Override ExporterRobot
        public override string Name => FormatName;
        public override string Filter => "Comma Separated Values (*.csv) | *.csv";
        public override string Extension => "csv";
        public override bool UseCoordinateSelector => false;
        public override bool UseAngleSelector => true;
        public override bool UseRobotPreparation => true;
        public override bool UseDockingOffsets => true;
        public override Bitmap BrandLogo => Properties.Resources.ABB_France;
        public override bool ShowOutput => Properties.Settings.Default.ShowOutputABB;
        public override bool UseDirectExport => true;
        public override string Encrypt(string input)
        {
            string[] sKeyIn =
                {
                "aBCEefgIilnoPprstVxy",
                "[]0123456789.,",
                "[]0123456789.,"
            };
            string[] sKeyOut =
                {
                "#+FaG7HcI2JeKs1%zU5!",
                "#+FaG7HcI2JeKs",
                "%!c9LDwJuKoHi5"
            };
            // read string
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(input ?? ""));
            // string builder
            StringBuilder sb = new StringBuilder();
            // output
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                while (reader.Peek() > 0)
                {
                    var line = reader.ReadLine();
                    if (line.Contains("START;") || line.Contains("END;"))
                        sb.AppendLine(line);
                    else
                    {
                        var lineParts = line.Split(';');
                        string encryptedLine = string.Empty;
                        for (int i = 0; i < 3; ++i)
                        {
                            if (lineParts[i].Trim().Length > 0)
                                encryptedLine += EncryptWKey(lineParts[i], sKeyIn[i], sKeyOut[i]) + ";";
                        }
                        sb.AppendLine(encryptedLine);
                    }
                }
            }
            return sb.ToString();
        }
        private string EncryptWKey(string s, string sKeyIn, string sKeyOut)
        {
            System.Diagnostics.Debug.Assert(sKeyIn.Length == sKeyOut.Length);
            string sCopy = string.Empty;
            for (int i = 0; i < s.Length; ++i)
            {
                int index = sKeyIn.IndexOf(s[i]);
                sCopy += (index != -1) ? sKeyOut[index] : s[i];
            }
            return sCopy;
        }
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

            sb.AppendLine("START;");
            sb.AppendLine($"ExportVersion;{ExportVersion};");
            sb.AppendLine($"Config;[{sol.LayerCount},{sol.InterlayerCount},{sol.ItemCount},{noCycles}];");
            sb.AppendLine($"Pallet;[[{pal.Length},{pal.Width},{pal.Height}],{pal.Weight}];");

            for (int iLayer = 0; iLayer < robotPreparation.NumberOfLayers; ++iLayer)
            {
                // interlayer
                if (-1 != interlayers[iLayer].first)
                {
                    var interlayerProp = analysis.Interlayers[interlayers[iLayer].first];
                    var vPos = new Vector3D(pal.Length / 2, pal.Width / 2, interlayers[iLayer].second);
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
                if (iLayer < robotLayers.Count)
                {
                    var robotLayer = robotLayers[iLayer];
                    foreach (var robotDrop in robotLayer.Drops)
                    {
                        // item name
                        string itemName = "Box";
                        // item pick setting
                        string itemPickSettings = string.Format(
                            CultureInfo.InvariantCulture,
                            "[[{0},{1},{2}],{3},[{4},{5},{6}],{7},{8},{9}]",
                            boxDim.X, boxDim.Y, boxDim.Z, weight, cog.X, cog.Y, cog.Z,
                            robotDrop.ConveyorSetting.GripperAngle, robotDrop.ConveyorSetting.Angle, robotPreparation.Facing
                            );
                        // place settings
                        Vector3D vPos = robotDrop.Center3D;
                        int angle = Modulo360(robotDrop.RawAngleSimple - robotDrop.ConveyorSetting.Angle);
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
                else
                    _log.Error($"RobotLayer count {robotLayers.Count} not equal to solution layer count {robotPreparation.NumberOfLayers}");
            }

            // top interlayer?
            if (interlayers.Count == robotPreparation.NumberOfLayers + 1 && -1 != interlayers[robotPreparation.NumberOfLayers].first)
            {
                var interlayerProp = analysis.Interlayers[interlayers[robotPreparation.NumberOfLayers].first];
                var vPos = new Vector3D(pal.Length / 2, pal.Width / 2, interlayers[robotPreparation.NumberOfLayers].second);
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
            sb.AppendLine("END;");
        }
        #endregion
        #region Helpers
        private static readonly string ExportVersion = "1.0";
        #endregion
        #region Private data members
        private static ILog _log = LogManager.GetLogger(typeof(ExporterCSV_ABB_France));
        #endregion
    }
}
