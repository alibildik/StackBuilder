#region Using directives
using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Linq;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Exporters.Properties;
#endregion

namespace treeDiM.StackBuilder.Exporters
{
    public abstract class ExporterRobot : Exporter
    {
        public virtual void Export(RobotPreparation robotPreparation, ref Stream stream)
        {
            // sanity check
            if (null == robotPreparation) return;
            // instantiate new string builder
            var sb = new StringBuilder();
            // number format information
            NumberFormatInfo nfi = new NumberFormatInfo {
                NumberDecimalSeparator = ".",
                NumberGroupSeparator = "",
                NumberDecimalDigits = 1
            };
            // actual export
            if (robotPreparation.IsValid)
                Export(robotPreparation, nfi, ref sb);
            else
                sb.AppendLine(Resources.ID_UNSUPPORTEDANALYSIS);
            // write to stream
            var writer = new StreamWriter(stream);
            writer.Write(sb.ToString());
            writer.Flush();
            stream.Position = 0;
        }
        public virtual void Export(RobotPreparation robotPreparation, NumberFormatInfo nfi, ref StringBuilder sb)
        {
            throw new NotImplementedException($"Prepared analysis export not implemented with {Name}.");
        }
        public override void Export(AnalysisLayered analysis, ref Stream stream)
        {
            // instantiate new string builder
            var sb = new StringBuilder();
            // number format
            NumberFormatInfo nfi = new NumberFormatInfo {
                NumberDecimalSeparator = ".",
                NumberGroupSeparator = "",
                NumberDecimalDigits = 1
            };
            // actual export
            Export(analysis, nfi, ref sb);
            // write to stream
            var writer = new StreamWriter(stream);
            writer.Write(sb.ToString());
            writer.Flush();
            stream.Position = 0;
        }
        public virtual void Export(AnalysisLayered analysis, NumberFormatInfo nfi, ref StringBuilder sb)
        {
            throw new NotImplementedException($"Direct analysis export not implemented with {Name}.");
        }
        public virtual int MaxLayerIndexExporter(AnalysisLayered analysis) => analysis.SolutionLay.LayerCount;
        public abstract System.Drawing.Bitmap BrandLogo { get; }
        public virtual bool UseRobotPreparation { get; } = false;
        public virtual bool UseCoordinateSelector { get; } = false;
        public virtual bool UseAngleSelector { get; } = false;
        public virtual bool UseDockingOffsets { get; } = false;
        public virtual bool HasFormatDefinition { get; } = false;
        public virtual string FormatDefinition { get; } = string.Empty;
        public CoordinateMode PositionCoordinateMode { get; set; } = CoordinateMode.CM_CORNER;
        public enum CoordinateMode { CM_CORNER, CM_COG };

        #region Helpers
        protected int Modulo360(int angle)
        {
            int angleNew = angle % 360;
            while (angleNew < 0) angleNew += 360;
            return angleNew;
        }
        protected Vector3D ConvertPosition(BoxPosition bp, Vector3D boxDim)
        {
            switch (PositionCoordinateMode)
            {
                case CoordinateMode.CM_COG:
                    return bp.Center(boxDim);
                default:
                    return bp.Position;
            }
        }
        protected int ConvertPositionAngleToPositionIndex(BoxPosition bp)
        {
            if (bp.DirectionHeight != HalfAxis.HAxis.AXIS_Z_P)
                throw new ExceptionUnexpectedOrientation(bp, this);

            switch (bp.DirectionLength)
            {
                case HalfAxis.HAxis.AXIS_X_P: return 1;
                case HalfAxis.HAxis.AXIS_Y_P: return 2;
                case HalfAxis.HAxis.AXIS_X_N: return 3;
                case HalfAxis.HAxis.AXIS_Y_N: return 4;
                default: throw new ExceptionUnexpectedOrientation(bp, this);
            }
        }
        protected static double DockingDistanceX(RobotDrop robotDrop, double dockingDistance)
        {
            if (robotDrop.HasNeighbour(RobotDrop.NeighbourDir.LEFT, dockingDistance))
                return dockingDistance;
            else if (robotDrop.HasNeighbour(RobotDrop.NeighbourDir.RIGHT, dockingDistance))
                return -dockingDistance;
            else
                return 0.0;
        }
        protected static double DockingDistanceY(RobotDrop robotDrop, double dockingDistance)
        {
            if (robotDrop.HasNeighbour(RobotDrop.NeighbourDir.BOTTOM, dockingDistance))
                return dockingDistance;
            else if (robotDrop.HasNeighbour(RobotDrop.NeighbourDir.TOP, dockingDistance))
                return -dockingDistance;
            else
                return 0.0;
        }
        #endregion

        #region Static properties and methods
        public static void SetDefault(string robotBrand)
        {
            Properties.Settings.Default.RobotBrand = robotBrand;
            Properties.Settings.Default.Save();
        }
        public static string DefaultName => Properties.Settings.Default.RobotBrand;

        public static ExporterRobot[] GetRobotExporters()
        {
            // default RobotExporter?
            var selectedExporter = GetByName(DefaultName);
            if (null == selectedExporter)
                return RobotExporters;
            else
                return new ExporterRobot[] { selectedExporter };
        }
        public static ExporterRobot GetByName(string name)
        {
            try { return RobotExporters.Single(r => string.Equals(r.Name, name, StringComparison.CurrentCultureIgnoreCase)); }
            catch (Exception) { return null; }
        }
        public static ExporterRobot[] RobotExporters =>
            new ExporterRobot[]
            {
                new ExporterXML(),
                new ExporterCSV(),
                new ExporterCSV_Angle(),
                new ExporterCSV_ABB_France(),
                new ExporterCSV_FMLogistic(),
                new ExporterCSV_TechBSA()
            };
        #endregion
    }
}
