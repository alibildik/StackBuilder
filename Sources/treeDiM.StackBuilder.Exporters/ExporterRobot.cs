﻿#region Using directives
using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Linq;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Exporters
{
    public abstract class ExporterRobot : Exporter
    {
        public virtual bool UseRobotPreparation { get; } = false;
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
            Export(robotPreparation, nfi, ref sb);
            // write to stream
            var writer = new StreamWriter(stream);
            writer.Write(sb.ToString());
            writer.Flush();
            stream.Position = 0;
        }
        public abstract void Export(RobotPreparation robotPreparation, NumberFormatInfo nfi, ref StringBuilder sb);

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
        public abstract void Export(AnalysisLayered analysis, NumberFormatInfo nfi, ref StringBuilder sb);
        public virtual int MaxLayerIndexExporter(AnalysisLayered analysis) => analysis.SolutionLay.LayerCount;

        public abstract System.Drawing.Bitmap BrandLogo { get; }

        public virtual bool ShowSelectorCoordinateMode { get; } = true;
        public CoordinateMode PositionCoordinateMode { get; set; } = CoordinateMode.CM_CORNER;
        public enum CoordinateMode { CM_CORNER, CM_COG };



        #region Helpers
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
                new ExporterCSV_ABB_France(),
                new ExporterCSV_FMLogistic(),
                new ExporterCSV_TechBSA()
            };
        #endregion
    }
}
