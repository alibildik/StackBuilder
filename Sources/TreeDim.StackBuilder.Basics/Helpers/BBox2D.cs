#region Using directives
using System;

using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    public class BBox2D : ICloneable
    {
        #region Constructor
        public BBox2D() { }
        public BBox2D(double xmin, double ymin, double xmax, double ymax)
        {
            Extend(new Vector2D(xmin, ymin));
            Extend(new Vector2D(xmax, ymax));
        }
        public void Extend(Vector2D vec)
        {
            PtMin = new Vector2D(Math.Min(PtMin.X, vec.X), Math.Min(PtMin.Y, vec.Y));
            PtMax = new Vector2D(Math.Max(PtMax.X, vec.X), Math.Max(PtMax.Y, vec.Y));
        }
        public void Extend(BBox2D bbox)
        { Extend(bbox.PtMin); Extend(bbox.PtMax); }
        public BBox2D(BBox2D bbox) { Extend(bbox.PtMin); Extend(bbox.PtMax); }
        public BBox2D(Vector2D position, HalfAxis.HAxis lengthAxis, Vector2D dim)
        {
            Vector2D xAxis = Vector2D.XAxis, yAxis = Vector2D.YAxis;
            switch (lengthAxis)
            {
                case HalfAxis.HAxis.AXIS_X_P: xAxis = Vector2D.XAxis;  yAxis = Vector2D.YAxis;  break;
                case HalfAxis.HAxis.AXIS_Y_P: xAxis = Vector2D.YAxis;  yAxis = -Vector2D.XAxis; break;
                case HalfAxis.HAxis.AXIS_X_N: xAxis = -Vector2D.XAxis; yAxis = -Vector2D.YAxis; break;
                case HalfAxis.HAxis.AXIS_Y_N: xAxis = -Vector2D.YAxis; yAxis = Vector2D.XAxis; break;
                default: break;
            }

            var points = new Vector2D[4];
            points[0] = position;
            points[1] = position + dim.X * xAxis;
            points[2] = position + dim.X * xAxis + dim.Y * yAxis;
            points[3] = position + dim.Y * yAxis;

            foreach (var pt in points)
                Extend(pt);
        }
        #endregion
        #region ICloneable implementation
        public object Clone() => new BBox2D(this);
        #endregion
        #region Public properties
        public Vector2D PtMin { get; set; } = new Vector2D(double.MaxValue, double.MaxValue);
        public Vector2D PtMax { get; set; } = new Vector2D(double.MinValue, double.MinValue);
        public Vector2D[] Corners
        {
            get
            {
                Vector2D[] corners = new Vector2D[4];
                corners[0] = new Vector2D(PtMin.X, PtMin.Y);
                corners[1] = new Vector2D(PtMax.X, PtMin.Y);
                corners[2] = new Vector2D(PtMax.X, PtMax.Y);
                corners[3] = new Vector2D(PtMin.X, PtMax.Y);
                return corners;
            }
        }
        public Vector2D Dimensions => PtMax - PtMin;
        public static BBox2D Initial => new BBox2D();
        #endregion
        #region Override object
        public override int GetHashCode()
        {
            return PtMin.GetHashCode() ^ PtMax.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj is BBox2D v)
                return (PtMin.Equals(v.PtMin)) && (PtMax.Equals(v.PtMax));
            return false;
        }
        public override string ToString() => $"xmin = {PtMin.X}, ymin = {PtMin.Y}, xmax = {PtMax.X}, ymax = {PtMax.Y}";
        #endregion
    }
}
