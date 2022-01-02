#region Using directives
using System;
using System.Collections.Generic;
using System.Text;

using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    #region InvalidBBoxException
    public class InvalidBBoxException : Exception
    {
        #region Constructor
        public InvalidBBoxException(string message)
            : base(message)
        {
        }
        #endregion
    }
    #endregion

    #region BBox3D
    public class BBox3D : ICloneable
    {
        #region Constructor
        public BBox3D()
        {
        }
        public BBox3D(double xmin, double ymin, double zmin, double xmax, double ymax, double zmax)
        {
            Extend(new Vector3D(xmin, ymin, zmin));
            Extend(new Vector3D(xmax, ymax, zmax));
        }
        public BBox3D(Vector3D pt0, Vector3D pt1)
        {
            Extend(pt0);
            Extend(pt1);
        }
        public BBox3D(BBox3D box)
        {
            PtMin = new Vector3D(box.PtMin);
            PtMax = new Vector3D(box.PtMax);
        }
        public BBox3D(BoxPosition bPos, Vector3D dim)
        {
            Vector3D lengthAxis = HalfAxis.ToVector3D(bPos.DirectionLength);
            Vector3D widthAxis = HalfAxis.ToVector3D(bPos.DirectionWidth);
            Vector3D heightAxis = HalfAxis.ToVector3D(bPos.DirectionHeight);

            var points = new Vector3D[8];
            points[0] = bPos.Position;
            points[1] = bPos.Position + dim[0] * lengthAxis;
            points[2] = bPos.Position + dim[0] * lengthAxis + dim[1] * widthAxis;
            points[3] = bPos.Position + dim[1] * widthAxis;

            points[4] = bPos.Position + dim[2] * heightAxis;
            points[5] = bPos.Position + dim[2] * heightAxis + dim[0] * lengthAxis;
            points[6] = bPos.Position + dim[2] * heightAxis + dim[0] * lengthAxis + dim[1] * widthAxis;
            points[7] = bPos.Position + dim[2] * heightAxis + dim[1] * widthAxis;

            foreach (var pt in points)
                Extend(pt);
        }
        public BBox3D(BoxPosition bPos, Vector3D dim, Transform3D transf)
        {
            Vector3D lengthAxis = HalfAxis.ToVector3D(bPos.DirectionLength);
            Vector3D widthAxis = HalfAxis.ToVector3D(bPos.DirectionWidth);
            Vector3D heightAxis = HalfAxis.ToVector3D(bPos.DirectionHeight);

            var points = new Vector3D[8];
            points[0] = bPos.Position;
            points[1] = bPos.Position + dim[0] * lengthAxis;
            points[2] = bPos.Position + dim[0] * lengthAxis + dim[1] * widthAxis;
            points[3] = bPos.Position + dim[1] * widthAxis;

            points[4] = bPos.Position + dim[2] * heightAxis;
            points[5] = bPos.Position + dim[2] * heightAxis + dim[0] * lengthAxis;
            points[6] = bPos.Position + dim[2] * heightAxis + dim[0] * lengthAxis + dim[1] * widthAxis;
            points[7] = bPos.Position + dim[2] * heightAxis + dim[1] * widthAxis;

            foreach (var pt in points)
                Extend(transf.transform(pt));
        }
        #endregion

        #region Constants
        public static BBox3D Initial => new BBox3D();
        #endregion

        #region Public methods
        public bool IsValid => (PtMin.X <= PtMax.X) && (PtMin.Y <= PtMax.Y) && (PtMin.Z <= PtMax.Z);
        public void Reset()
        {
            PtMin = new Vector3D(double.MaxValue, double.MaxValue, double.MaxValue);
            PtMax = new Vector3D(double.MinValue, double.MinValue, double.MinValue);
        }
        public void Extend(Vector3D vec)
        {
            PtMin = new Vector3D(Math.Min(PtMin.X, vec.X), Math.Min(PtMin.Y, vec.Y), Math.Min(PtMin.Z, vec.Z));
            PtMax = new Vector3D( Math.Max(PtMax.X, vec.X), Math.Max(PtMax.Y, vec.Y), Math.Max(PtMax.Z, vec.Z));
        }
        public void Extend(BBox3D box)
        {
            Extend(box.PtMin);
            Extend(box.PtMax);
        }
        public void Inflate(double margin)
        {
            if (!IsValid)
                throw new InvalidBBoxException("Box is invalid: can not set margin");
            PtMin -= new Vector3D(margin, margin, margin);
            PtMax += new Vector3D(margin, margin, margin);
        }
        public BBox2D ToBBox2D() => new BBox2D(PtMin.X, PtMin.Y, PtMax.X, PtMax.Y);
        #endregion

        #region Public properties
        public double Length { get { return PtMax.X - PtMin.X; } }
        public double Width { get { return PtMax.Y - PtMin.Y; } }
        public double Height { get { return PtMax.Z - PtMin.Z; } }
        public Vector3D PtMin { get; set; } = new Vector3D(double.MaxValue, double.MaxValue, double.MaxValue);
        public Vector3D PtMax { get; set; } = new Vector3D(double.MinValue, double.MinValue, double.MinValue);
        public Vector3D[] Corners
        {
            get
            {
                Vector3D[] corners = new Vector3D[8];
                corners[0] = new Vector3D(PtMin.X, PtMin.Y, PtMin.Z);
                corners[1] = new Vector3D(PtMax.X, PtMin.Y, PtMin.Z);
                corners[2] = new Vector3D(PtMax.X, PtMax.Y, PtMin.Z);
                corners[3] = new Vector3D(PtMin.X, PtMax.Y, PtMin.Z);
                corners[4] = new Vector3D(PtMin.X, PtMin.Y, PtMax.Z);
                corners[5] = new Vector3D(PtMax.X, PtMin.Y, PtMax.Z);
                corners[6] = new Vector3D(PtMax.X, PtMax.Y, PtMax.Z);
                corners[7] = new Vector3D(PtMin.X, PtMax.Y, PtMax.Z);
                return corners;
            }
        }
        public double[] Dimensions => new double[] { PtMax.X - PtMin.X, PtMax.Y - PtMin.Y, PtMax.Z - PtMin.Z };
        public Vector3D DimensionsVec => PtMax - PtMin;
        public Vector3D Center => 0.5 * (PtMin + PtMax);
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Creates an exact copy of this <see cref="Box2D"/> object.
        /// </summary>
        /// <returns>The <see cref="Box2D"/> object this method creates, cast as an object.</returns>
        object ICloneable.Clone()
        {
            return new BBox3D(this);
        }
        /// <summary>
        /// Creates an exact copy of this <see cref="Box2D"/> object.
        /// </summary>
        /// <returns>The <see cref="Box2D"/> object this method creates.</returns>
        public BBox3D Clone()
        {
            return new BBox3D(this);
        }
        #endregion

        #region System.Object overrides
        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return PtMin.GetHashCode() ^ PtMax.GetHashCode();
        }
        /// <summary>
        /// Returns a value indicating whether this instance is equal to
        /// the specified object.
        /// </summary>
        /// <param name="obj">An object to compare to this instance.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Box2D"/> and has the same values as this instance; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is BBox3D v)
            {
                return (PtMin.Equals(v.PtMin)) && (PtMax.Equals(v.PtMax));
            }
            return false;
        }
        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"xmin = {PtMin.X}, ymin = {PtMin.Y}, zmin = {PtMin.Z}, xmax = {PtMax.X}, ymax = {PtMax.Y}, zmax = {PtMax.Z}";        
        #endregion
    }
    #endregion
}
