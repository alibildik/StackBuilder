#region Using directives
using System.Text;
using System.IO;
using System.Globalization;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Exporters
{
    public class ExporterCSV : ExporterRobot
    {
        #region Static members
        public static string FormatName => "csv (default)";
        #endregion

        public ExporterCSV() {}
        public override string Name => FormatName;
        public override string Extension => "csv";
        public override string Filter => "Comma Separated Values (*.csv)|*.csv";
        public override System.Drawing.Bitmap BrandLogo => Properties.Resources.treeDiM;
        public override void Export(AnalysisLayered analysis, NumberFormatInfo nfi, ref StringBuilder sb)
        {
            SolutionLayered sol = analysis.SolutionLay;
            var layers = sol.Layers;
            foreach (ILayer layer in layers)
            {
                if (layer is Layer3DBox layerBox)
                {
                    layerBox.Sort(analysis.Content, Layer3DBox.SortType.DIST_MAXCORNER);
                    foreach (BoxPosition bPosition in layerBox)
                    {
                        Vector3D pos = ConvertPosition(bPosition, analysis.ContentDimensions);
                        sb.AppendLine(
                            $"1;" +
                            $"{pos.X.ToString("0,0.0", nfi)};" +
                            $"{pos.Y.ToString("0,0.0", nfi)};" +
                            $"{pos.Z.ToString("0,0.0", nfi)};" +
                            $"{bPosition.DirectionLength};" +
                            $"{bPosition.DirectionWidth}");
                    }
                }
                else if (layer is Layer3DCyl layerCyl)
                {
                    layerCyl.Sort(analysis.Content, Layer3DCyl.SortType.DIST_CENTER);
                    foreach (Vector3D vPos in layerCyl)
                    {
                        sb.AppendLine(
                            $"1;" +
                            $"{vPos.X.ToString("0,0.0", nfi)};" +
                            $"{vPos.Y.ToString("0,0.0", nfi)};" +
                            $"{vPos.Z.ToString("0,0.0", nfi)};"
                            );
                    }
                }
                else if (layer is InterlayerPos interlayerPos)
                {
                    var interlayerProp = sol.Interlayers[interlayerPos.TypeId];
                    var bPosition = new BoxPosition(new Vector3D(
                            0.5 * (analysis.ContainerDimensions.X - interlayerProp.Length)
                            , 0.5 * (analysis.ContainerDimensions.Y - interlayerProp.Width)
                            , interlayerPos.ZLow),
                            HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
                    Vector3D pos = ConvertPosition(bPosition, interlayerProp.Dimensions);
                    sb.AppendLine(
                        $"{interlayerPos.TypeId + 1};" +
                        $"{pos.X.ToString("0,0.0", nfi)};" +
                        $"{pos.Y.ToString("0,0.0", nfi)};" +
                        $"{pos.Z.ToString("0,0.0", nfi)};"
                        );
                }
            }
        }

        public override void Export(RobotPreparation robotPreparation, NumberFormatInfo nfi, ref StringBuilder sb)
        {
        }
    }
}
