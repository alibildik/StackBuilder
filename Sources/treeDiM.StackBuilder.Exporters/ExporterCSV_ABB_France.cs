#region Using directives
using System;
using System.Text;
using System.Globalization;

using log4net;

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
        public override bool ShowSelectorCoordinateMode => false;
        public override bool UseRobotPreparation => true;
        public override System.Drawing.Bitmap BrandLogo => Properties.Resources.ABB_France;

        public override void Export(AnalysisLayered analysis, NumberFormatInfo nfi, ref StringBuilder sb)
        {
            try
            {
                var analysisCasePallet = analysis as AnalysisCasePallet;
                var sol = analysis.SolutionLay;
                var pal = analysisCasePallet.PalletProperties;
                sb.AppendLine("START;");
                sb.AppendLine($"ExportVersion;{ExportVersion};");
                sb.AppendLine($"Config;[{sol.LayerCount},{sol.InterlayerCount},{sol.ItemCount},{sol.ItemCount + sol.InterlayerCount}];");
                sb.AppendLine($"Pallet;[[{pal.Length},{pal.Width},{pal.Height}],{pal.Weight}];");

                foreach (var layer in sol.Layers)
                {

                    if (layer is InterlayerPos interlayerPos)
                    {
                        sb.AppendLine($"Interlayer;[[]];");
                    }
                    else if (layer is Layer3DBox layerBox)
                    { 
                        foreach (var bPos in layerBox)
                            sb.AppendLine($"Box;[[]];");
                    }
                }

                sb.AppendLine("END;");
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        public override void Export(RobotPreparation robotPreparation, NumberFormatInfo nfi, ref StringBuilder sb)
        {
            try
            {
                var analysis = robotPreparation.Analysis;
                var sol = analysis.SolutionLay;
                var pal = analysis.PalletProperties;
                sb.AppendLine("START;");
                sb.AppendLine($"ExportVersion;{ExportVersion};");
                sb.AppendLine($"Config;[{sol.LayerCount},{sol.InterlayerCount},{sol.ItemCount},{sol.ItemCount + sol.InterlayerCount}];");
                sb.AppendLine($"Pallet;[[{pal.Length},{pal.Width},{pal.Height}],{pal.Weight}];");

                for (int iLayer = 0; iLayer < robotPreparation.LayerTypes.Count; ++iLayer)
                {
                    sb.AppendLine($"Interlayer;[[]];");
                    var robotLayer = robotPreparation.GetLayer(0);
                    foreach (var robotDrop in robotLayer.Drops)
                    {
                        sb.AppendLine($"Box;[[]];");
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
        private static string ExportVersion = "1.0";
        #endregion

        #region Private data members
        private static ILog _log = LogManager.GetLogger(typeof(ExporterCSV_ABB_France));
        #endregion
    }
}
