#region Using directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    #region BoxGeneric
    public class BoxGeneric : Drawable
    {
        #region Constructor
        public BoxGeneric(uint pickId, double length, double width, double height, BoxPosition boxPosition)
            : base(pickId)
        {
            if (!boxPosition.IsValid)
                throw new GraphicsException("Invalid BoxPosition: can not create box");
            Dimensions = new Vector3D(length, width, height);
            BoxPosition = boxPosition;
        }
        #endregion

        #region Override Drawable
        public override void Draw(Graphics3D graphics) { }
        #endregion

        #region Public properties
        public double Length => Dimensions.X;
        public double Width => Dimensions.Y;
        public double Height => Dimensions.Z;
        public Vector3D LengthAxis => HalfAxis.ToVector3D(BoxPosition.DirectionLength);
        public Vector3D WidthAxis => HalfAxis.ToVector3D(BoxPosition.DirectionWidth);
        public Vector3D HeightAxis => Vector3D.CrossProduct(LengthAxis, WidthAxis);
        public override Vector3D Center => BoxPosition.Center(Dimensions);
        #endregion

        #region Validity
        public bool IsValid => Dimensions.X > 0.0 && Dimensions.Y > 0.0 && Dimensions.Z > 0.0 && (LengthAxis != WidthAxis);
        #endregion

        #region XMin / XMax / YMin / YMax / ZMin
        public Vector3D PtMin
        {
            get
            {
                double xmin = double.MaxValue,
                    ymin = double.MaxValue,
                    zmin = double.MaxValue;
                foreach (Vector3D v in Points)
                {
                    xmin = Math.Min(v.X, xmin);
                    ymin = Math.Min(v.Y, ymin);
                    zmin = Math.Min(v.Z, zmin);
                }
                return new Vector3D(xmin, ymin, zmin);
            }
        }
        public Vector3D PtMax
        {
            get
            {
                double xmax = double.MinValue,
                    ymax = double.MinValue,
                    zmax = double.MinValue;
                foreach (Vector3D v in Points)
                {
                    xmax = Math.Max(v.X, xmax);
                    ymax = Math.Max(v.Y, ymax);
                    zmax = Math.Max(v.Z, zmax);
                }
                return new Vector3D(xmax, ymax, zmax);
            }
        }
        public BBox3D BBox
        {
            get
            {
                BBox3D bbox = new BBox3D();
                foreach (Vector3D v in Points)
                    bbox.Extend(v);
                return bbox;
            }
        }
        #endregion

        #region Points / Triangles
        public override Vector3D[] Points
        {
            get
            {
                Vector3D position = BoxPosition.Position;
                Vector3D lengthAxis = LengthAxis;
                Vector3D widthAxis = WidthAxis;
                Vector3D heightAxis = HeightAxis;
                Vector3D[] points = new Vector3D[8];
                points[0] = position;
                points[1] = position + Dimensions.X * lengthAxis;
                points[2] = position + Dimensions.X * lengthAxis + Dimensions.Y * widthAxis;
                points[3] = position + Dimensions.Y * widthAxis;

                points[4] = position + Dimensions.Z * heightAxis;
                points[5] = position + Dimensions.Z * heightAxis + Dimensions.X * lengthAxis;
                points[6] = position + Dimensions.Z * heightAxis + Dimensions.X * lengthAxis + Dimensions.Y * widthAxis;
                points[7] = position + Dimensions.Z * heightAxis + Dimensions.Y * widthAxis;

                return points;
            }
        }
        public Vector3D[] PointsSmallOffset
        {
            get
            {
                Vector3D lengthAxis = LengthAxis;
                Vector3D widthAxis = WidthAxis;
                Vector3D heightAxis = HeightAxis;
                Vector3D[] points = new Vector3D[8];
                const double offset = 3.0;
                Vector3D origin = new Vector3D(offset, offset, offset);
                points[0] = origin;
                points[1] = origin + (Dimensions.X - 2.0 * offset) * lengthAxis;
                points[2] = origin + (Dimensions.X - 2.0 * offset) * lengthAxis + (Dimensions.Y - 2.0 * offset) * widthAxis;
                points[3] = origin + (Dimensions.Y - 2.0 * offset) * widthAxis;

                points[4] = origin + (Dimensions.Z - 2.0 * offset) * heightAxis;
                points[5] = origin + (Dimensions.Z - 2.0 * offset) * heightAxis + (Dimensions.X - 2.0 * offset) * lengthAxis;
                points[6] = origin + (Dimensions.Z - 2.0 * offset) * heightAxis + (Dimensions.X - 2.0 * offset) * lengthAxis + (Dimensions.Y - 2.0 * offset) * widthAxis;
                points[7] = origin + (Dimensions.Z - 2.0 * offset) * heightAxis + (Dimensions.Y - 2.0 * offset) * widthAxis;

                return points;
            }
        }
        public Vector3D[] Normals
        {
            get
            {
                Vector3D[] normals = new Vector3D[6];
                normals[0] = -Vector3D.XAxis;
                normals[1] = Vector3D.XAxis;
                normals[2] = -Vector3D.YAxis;
                normals[3] = Vector3D.YAxis;
                normals[4] = -Vector3D.ZAxis;
                normals[5] = Vector3D.ZAxis;
                return normals;
            }
        }
        public Vector2D[] UVs
        {
            get
            {
                Vector2D[] uvs = new Vector2D[4];
                uvs[0] = new Vector2D(0.0, 0.0);
                uvs[1] = new Vector2D(1.0, 0.0);
                uvs[2] = new Vector2D(1.0, 1.0);
                uvs[3] = new Vector2D(0.0, 1.0);
                return uvs;
            }
        }
        public TriangleIndices[] TriangleIndices
        {
            get
            {
                TriangleIndices[] tri = new TriangleIndices[12];
                ulong n0 = (ulong)HalfAxis.ToHalfAxis(-LengthAxis);
                tri[0] = new TriangleIndices(0, 4, 3, n0, n0, n0, 1, 3, 0);
                tri[1] = new TriangleIndices(3, 4, 7, n0, n0, n0, 0, 3, 2);
                ulong n1 = (ulong)HalfAxis.ToHalfAxis(LengthAxis);
                tri[2] = new TriangleIndices(1, 2, 5, n1, n1, n1, 0, 1, 2);
                tri[3] = new TriangleIndices(5, 2, 6, n1, n1, n1, 1, 3, 2);
                ulong n2 = (ulong)HalfAxis.ToHalfAxis(-WidthAxis);
                tri[4] = new TriangleIndices(0, 1, 4, n2, n2, n2, 0, 1, 2);
                tri[5] = new TriangleIndices(4, 1, 5, n2, n2, n2, 2, 1, 3);
                ulong n3 = (ulong)HalfAxis.ToHalfAxis(WidthAxis);
                tri[6] = new TriangleIndices(7, 6, 2, n3, n3, n3, 3, 2, 0);
                tri[7] = new TriangleIndices(7, 2, 3, n3, n3, n3, 3, 0, 1);
                ulong n4 = (ulong)HalfAxis.ToHalfAxis(-HeightAxis);
                tri[8] = new TriangleIndices(0, 3, 1, n4, n4, n4, 2, 0, 3);
                tri[9] = new TriangleIndices(1, 3, 2, n4, n4, n4, 3, 0, 1);
                ulong n5 = (ulong)HalfAxis.ToHalfAxis(HeightAxis);
                tri[10] = new TriangleIndices(4, 5, 7, n5, n5, n5, 0, 1, 2);
                tri[11] = new TriangleIndices(7, 5, 6, n5, n5, n5, 2, 1, 3);
                return tri;
            }
        }
        public TriangleIndices[] TrianglesByFace(HalfAxis.HAxis axis)
        {
            TriangleIndices[] tri = new TriangleIndices[2];
            ulong n = (ulong)axis;
            switch (axis)
            {
                case HalfAxis.HAxis.AXIS_X_N:
                    tri[0] = new TriangleIndices(0, 4, 3, n, n, n, 1, 2, 0);
                    tri[1] = new TriangleIndices(3, 4, 7, n, n, n, 0, 2, 3);
                    break;
                case HalfAxis.HAxis.AXIS_X_P:
                    tri[0] = new TriangleIndices(1, 2, 5, n, n, n, 0, 1, 3);
                    tri[1] = new TriangleIndices(5, 2, 6, n, n, n, 3, 1, 2);
                    break;
                case HalfAxis.HAxis.AXIS_Y_N:
                    tri[0] = new TriangleIndices(0, 1, 4, n, n, n, 0, 1, 3);
                    tri[1] = new TriangleIndices(4, 1, 5, n, n, n, 3, 1, 2);
                    break;
                case HalfAxis.HAxis.AXIS_Y_P:
                    tri[0] = new TriangleIndices(7, 6, 2, n, n, n, 2, 3, 0);
                    tri[1] = new TriangleIndices(7, 2, 3, n, n, n, 2, 0, 1);
                    break;
                case HalfAxis.HAxis.AXIS_Z_N:
                    tri[0] = new TriangleIndices(0, 3, 1, n, n, n, 2, 0, 3);
                    tri[1] = new TriangleIndices(1, 3, 2, n, n, n, 3, 0, 1);
                    break;
                case HalfAxis.HAxis.AXIS_Z_P:
                    tri[0] = new TriangleIndices(4, 5, 7, n, n, n, 0, 1, 2);
                    tri[1] = new TriangleIndices(7, 5, 6, n, n, n, 2, 1, 3);
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
            return tri;
        }
        public Vector3D[] Oriented(Vector3D pt0, Vector3D pt1, Vector3D pt2, Vector3D pt3, Vector3D pt)
        {
            Vector3D crossProduct = Vector3D.CrossProduct(pt1 - pt0, pt2 - pt0);
            if (Vector3D.DotProduct(crossProduct, pt - pt0) < 0)
                return new Vector3D[] { pt0, pt1, pt2, pt3 };
            else
                return new Vector3D[] { pt3, pt2, pt1, pt0 };
        }
        #endregion

        #region Faces
        public virtual Color ColorFace(short i) => Color.Beige;
        public virtual Face[] Faces
        {
            get
            {
                Vector3D position = BoxPosition.Position;
                Vector3D lengthAxis = LengthAxis;
                Vector3D widthAxis = WidthAxis;
                Vector3D heightAxis = HeightAxis;

                var points = new Vector3D[8];
                points[0] = position;
                points[1] = position + Dimensions.X * lengthAxis;
                points[2] = position + Dimensions.X * lengthAxis + Dimensions.Y * widthAxis;
                points[3] = position + Dimensions.Y * widthAxis;

                points[4] = position + Dimensions.Z * heightAxis;
                points[5] = position + Dimensions.Z * heightAxis + Dimensions.X * lengthAxis;
                points[6] = position + Dimensions.Z * heightAxis + Dimensions.X * lengthAxis + Dimensions.Y * widthAxis;
                points[7] = position + Dimensions.Z * heightAxis + Dimensions.Y * widthAxis;

                var faces = new Face[6];
                faces[0] = new Face(PickId, new Vector3D[] { points[3], points[0], points[4], points[7] }, ColorFace(0), Color.Black, "BOX", false); // AXIS_X_N <-> left
                faces[1] = new Face(PickId, new Vector3D[] { points[1], points[2], points[6], points[5] }, ColorFace(1), Color.Black, "BOX", false); // AXIS_X_P <-> right
                faces[2] = new Face(PickId, new Vector3D[] { points[0], points[1], points[5], points[4] }, ColorFace(2), Color.Black, "BOX", false); // AXIS_Y_N <-> front
                faces[3] = new Face(PickId, new Vector3D[] { points[2], points[3], points[7], points[6] }, ColorFace(3), Color.Black, "BOX", false); // AXIS_Y_P <-> back
                faces[4] = new Face(PickId, new Vector3D[] { points[3], points[2], points[1], points[0] }, ColorFace(4), Color.Black, "BOX", false); // AXIS_Z_N <-> bottom
                faces[5] = new Face(PickId, new Vector3D[] { points[4], points[5], points[6], points[7] }, ColorFace(5), Color.Black, "BOX", false); // AXIS_Z_P <-> top
                return faces;
            }
        }
        public Face TopFace
        {
            get
            {
                Face[] faces = Faces;
                Face topFace = faces[0];
                foreach (var face in faces)
                    if (face.Center.Z > topFace.Center.Z)
                        topFace = face;
                return topFace;
            }
        }
        #endregion

        #region Point inside ? / Ray intersection
        public bool PointInside(Vector3D pt)
        {
            foreach (Face face in Faces)
            {
                if (face.PointRelativePosition(pt) != -1)
                    return false;
            }
            return true;
        }
        public bool RayIntersect(Ray ray, out Vector3D ptInter)
        {
            List<Vector3D> listIntersections = new List<Vector3D>();
            foreach (Face f in Faces)
            {
                if (f.RayIntersect(ray, out Vector3D pt))
                    listIntersections.Add(pt);
            }
            // instantiate intersection comparer
            RayIntersectionComparer comp = new RayIntersectionComparer(ray);
            // sort intersections
            listIntersections.Sort(comp);
            // save intersection point
            if (listIntersections.Count > 0)
                ptInter = listIntersections[0];
            else
                ptInter = Vector3D.Zero;
            // return true if an intersection was found
            return listIntersections.Count > 0;
        }
        #endregion

        #region Helpers
        public void ApplyElong(double d)
        {
            Dimensions += new Vector3D(d, d, d);
            BoxPosition = BoxPosition.Translate(-new Vector3D(-0.5 * d, -0.5 * d, -0.5 * d));
        }
        #endregion

        #region Object overrides
        public override string ToString() => $"BoxPosition={BoxPosition} # Dimensions={Dimensions}";
        #endregion

        #region Data members
        public Vector3D Dimensions { get; private set; } = Vector3D.Zero;
        public BoxPosition BoxPosition { get; set; }
        #endregion
    }
    #endregion

}
