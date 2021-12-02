#region Using directives
using System;
using Sharp3D.Math.Core;

using System.Collections.Generic;
using System.Linq;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    public static class EnumExtensions
    {
        public static bool IsOneOf(this Enum enumeration, params Enum[] enums)
        {
            return enums.Contains(enumeration);
        }
    }
    public class HalfAxis
    {
        #region Values
        public enum HAxis
        {
            AXIS_X_N // -X
          , AXIS_X_P // X
          , AXIS_Y_N // -Y
          , AXIS_Y_P // Y
          , AXIS_Z_N // -Z
          , AXIS_Z_P // Z
        }
        #endregion
        #region Static conversion methods
        public static HAxis Opposite(HAxis axis)
        {
            switch (axis)
            {
                case HAxis.AXIS_X_N: return HAxis.AXIS_X_P;
                case HAxis.AXIS_X_P: return HAxis.AXIS_X_N;
                case HAxis.AXIS_Y_N: return HAxis.AXIS_Y_P;
                case HAxis.AXIS_Y_P: return HAxis.AXIS_Y_N;
                case HAxis.AXIS_Z_N: return HAxis.AXIS_Z_P;
                case HAxis.AXIS_Z_P: return HAxis.AXIS_Z_N;
                default: return HAxis.AXIS_Z_P;
            }
        }
        public static Vector3D ToVector3D(HAxis axis)
        {
            switch (axis)
            {
                case HAxis.AXIS_X_N: return -Vector3D.XAxis;
                case HAxis.AXIS_X_P: return Vector3D.XAxis;
                case HAxis.AXIS_Y_N: return -Vector3D.YAxis;
                case HAxis.AXIS_Y_P: return Vector3D.YAxis;
                case HAxis.AXIS_Z_N: return -Vector3D.ZAxis;
                default:                return Vector3D.ZAxis;
            }
        }
        public static HAxis ToHalfAxis(Vector3D v)
        {
            const double eps = 1.0E-03;
            v.Normalize();
            if (Math.Abs(Vector3D.DotProduct(v, -Vector3D.XAxis) - 1) < eps)
                return HAxis.AXIS_X_N;
            else if (Math.Abs(Vector3D.DotProduct(v, Vector3D.XAxis) - 1) < eps)
                return HAxis.AXIS_X_P;
            else if (Math.Abs(Vector3D.DotProduct(v, -Vector3D.YAxis) - 1) < eps)
                return HAxis.AXIS_Y_N;
            else if (Math.Abs(Vector3D.DotProduct(v, Vector3D.YAxis) - 1) < eps)
                return HAxis.AXIS_Y_P;
            else if (Math.Abs(Vector3D.DotProduct(v, -Vector3D.ZAxis) - 1) < eps)
                return HAxis.AXIS_Z_N;
            else
                return HAxis.AXIS_Z_P;
        }
        public static HAxis CrossProduct(HAxis axis0, HAxis axis1)
        {
            return ToHalfAxis(Vector3D.CrossProduct(ToVector3D(axis0), ToVector3D(axis1)));
        }
        public static HAxis Transform(HAxis axis, Transform3D transform)
        {
            return ToHalfAxis(transform.transformRot(HalfAxis.ToVector3D(axis)));
        }
        public static HAxis ReflectionX(HAxis axis)
        {
            Vector3D v = ToVector3D(axis);
            return ToHalfAxis(new Vector3D(v.X, -v.Y, v.Z)); 
        }
        public static HAxis ReflectionY(HAxis axis)
        {
            Vector3D v = ToVector3D(axis);
            return ToHalfAxis(new Vector3D(-v.X, v.Y, v.Z));
        }
        public static string ToString(HAxis axis)
        {
            switch (axis)
            {
                case HAxis.AXIS_X_N: return "XN";
                case HAxis.AXIS_X_P: return "XP";
                case HAxis.AXIS_Y_N: return "YN";
                case HAxis.AXIS_Y_P: return "YP";
                case HAxis.AXIS_Z_N: return "ZN";
                case HAxis.AXIS_Z_P: return "ZP";
                default: return "ZP";
            }
        }
        public static string ToAbbrev(HAxis axis)
        { 
            switch (axis)
            {
                case HAxis.AXIS_X_N: return "X-";
                case HAxis.AXIS_X_P: return "X+";
                case HAxis.AXIS_Y_N: return "Y-";
                case HAxis.AXIS_Y_P: return "Y+";
                case HAxis.AXIS_Z_N: return "Z-";
                case HAxis.AXIS_Z_P: return "Z+";
                default: return "Z+";
            }
        }
        public static HAxis Parse(string sAxis)
        {
            if (string.Equals(sAxis, "XN", StringComparison.CurrentCultureIgnoreCase)) return HAxis.AXIS_X_N;
            else if (string.Equals(sAxis, "XP", StringComparison.CurrentCultureIgnoreCase)) return HAxis.AXIS_X_P;
            else if (string.Equals(sAxis, "YN", StringComparison.CurrentCultureIgnoreCase)) return HAxis.AXIS_Y_N;
            else if (string.Equals(sAxis, "YP", StringComparison.CurrentCultureIgnoreCase)) return HAxis.AXIS_Y_P;
            else if (string.Equals(sAxis, "ZN", StringComparison.CurrentCultureIgnoreCase)) return HAxis.AXIS_Z_N;
            else if (string.Equals(sAxis, "ZP", StringComparison.CurrentCultureIgnoreCase)) return HAxis.AXIS_Z_P;
            throw new Exception(string.Format("Invalid HalfAxis value {0}", sAxis));
        }
        public static HAxis ParseAbbrev(string sAxis)
        {
            if (string.Equals(sAxis, "X-", StringComparison.CurrentCultureIgnoreCase)) return HAxis.AXIS_X_N;
            else if (string.Equals(sAxis, "X+", StringComparison.CurrentCultureIgnoreCase)) return HAxis.AXIS_X_P;
            else if (string.Equals(sAxis, "Y-", StringComparison.CurrentCultureIgnoreCase)) return HAxis.AXIS_Y_N;
            else if (string.Equals(sAxis, "Y+", StringComparison.CurrentCultureIgnoreCase)) return HAxis.AXIS_Y_P;
            else if (string.Equals(sAxis, "Z-", StringComparison.CurrentCultureIgnoreCase)) return HAxis.AXIS_Z_N;
            else if (string.Equals(sAxis, "Z+", StringComparison.CurrentCultureIgnoreCase)) return HAxis.AXIS_Z_P;
            throw new Exception(string.Format("Invalid HalfAxis value {0}", sAxis));
        }
        public static HAxis[] Positives
        {
            get
            {
                return new HAxis[]
                {
                    HAxis.AXIS_X_P,
                    HAxis.AXIS_Y_P,
                    HAxis.AXIS_Z_P 
                }; 
            }
        }
        public static HAxis[] All
        {
            get
            {
                return new HAxis[]
                {
                    HAxis.AXIS_X_N,
                    HAxis.AXIS_X_P,
                    HAxis.AXIS_Y_N,
                    HAxis.AXIS_Y_P,
                    HAxis.AXIS_Z_N,
                    HAxis.AXIS_Z_P
                };
            }
        }
        public static int Direction(HAxis axis)
        {
            switch (axis)
            { 
                case HAxis.AXIS_X_N:
                case HAxis.AXIS_X_P:
                    return 0;
                case HAxis.AXIS_Y_N:
                case HAxis.AXIS_Y_P:
                    return 1;
                case HAxis.AXIS_Z_N:
                case HAxis.AXIS_Z_P:
                    return 2;
                default:
                    return -1;
            }
        }
        #endregion
    }

    public struct Orientation
    {
        #region Constructor
        public Orientation(HalfAxis.HAxis dir0, HalfAxis.HAxis dir1)
        { Dir0 = dir0; Dir1 = dir1; }
        #endregion
        #region Public properties
        public HalfAxis.HAxis Dir0 { get; set; }
        public HalfAxis.HAxis Dir1 { get; set; }
        public Transform3D Transformation
        {
            get
            {
                Matrix4D mat = Matrix4D.Identity;
                Vector3D x = HalfAxis.ToVector3D(Dir0);
                Vector3D y = HalfAxis.ToVector3D(Dir1);
                Vector3D z = Vector3D.CrossProduct(x, y);
                mat.M11 = x.X;
                mat.M21 = x.Y;
                mat.M31 = x.Z;
                mat.M12 = y.X;
                mat.M22 = y.Y;
                mat.M32 = y.Z;
                mat.M13 = z.X;
                mat.M23 = z.Y;
                mat.M33 = z.Z;
                return new Transform3D(mat);
            }
        }
        #endregion
        #region Static members
        public static Orientation Default = new Orientation(HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
        #endregion
    }
}
