#region Using directives
using System;
using System.Collections.Generic;
using System.Xml;
using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    /// <summary>
    /// Box position
    /// </summary>
    [Serializable]
    public struct BoxPosition : ICloneable
    {
        #region Constructor
        public BoxPosition(Vector3D vPosition, HalfAxis.HAxis dirLength = HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis dirWidth = HalfAxis.HAxis.AXIS_Y_P)
        {
            if (dirLength == dirWidth)
                throw new Exception("Can not create BoxPosition"); 
            Position = vPosition;
            DirectionLength = dirLength;
            DirectionWidth = dirWidth;
            Index = null;
        }
        public BoxPosition(BoxPosition bpos)
        {
            Position = bpos.Position;
            DirectionLength = bpos.DirectionLength;
            DirectionWidth = bpos.DirectionWidth;
            Index = bpos.Index;
        }
        #endregion

        #region Public properties
        public int? Index { get; set; }
        public Vector3D Position { get; set; }
        public HalfAxis.HAxis DirectionLength { get; set; }
        public HalfAxis.HAxis DirectionWidth { get; set; }
        public HalfAxis.HAxis DirectionHeight
        {
            get
            {
                if (!IsValid)
                    throw new Exception("Invalid position -> Can not compute DirectionHeight.");
                return HalfAxis.ToHalfAxis(Vector3D.CrossProduct(HalfAxis.ToVector3D(DirectionLength), HalfAxis.ToVector3D(DirectionWidth)));
            }
        }
        public bool IsValid => DirectionLength != DirectionWidth;
        public BBox3D BBox(Vector3D dimensions)
        {
            BBox3D bbox = new BBox3D();
            foreach (Vector3D pt in Points(dimensions))
                bbox.Extend(pt);
            return bbox;
        }
        public Vector3D Center(Vector3D dimensions)
        {
            Vector3D vI = HalfAxis.ToVector3D(DirectionLength);
            Vector3D vJ = HalfAxis.ToVector3D(DirectionWidth);
            Vector3D vK = Vector3D.CrossProduct(vI, vJ);
            return Position + (0.5 * dimensions.X * vI) + (0.5 * dimensions.Y * vJ) + (0.5 * dimensions.Z * vK);
        }

        public double MaxDistance(Vector3D dimensions, Vector3D ptRef)
        {
            double maxDistance = double.MinValue;
            foreach (Vector3D pt in Points(dimensions))
            {
                double dist = (pt - ptRef).GetLength();
                maxDistance = Math.Max(maxDistance, dist);
            }
            return maxDistance;
        }
        public bool IsZOriented => DirectionHeight.IsOneOf(HalfAxis.HAxis.AXIS_Z_N, HalfAxis.HAxis.AXIS_Z_P);
        #endregion

        #region Static properties
        public static BoxPosition Zero => new BoxPosition(Vector3D.Zero, HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
        #endregion

        #region Translate / Rotate
        public BoxPosition Translate(Vector3D v)
        {
            return new BoxPosition(Position + v, DirectionLength, DirectionWidth);
        }
        public BoxPosition Translate(HalfAxis.HAxis axis, double value)
        {
            Vector3D v = Position;
            v += value * HalfAxis.ToVector3D(axis);
            return new BoxPosition(v, DirectionLength, DirectionWidth);
        }
        public BoxPosition RotateZ90(Vector3D dim)
        {
            Vector3D centerOfRotation = Center(dim);
            return Transform(this, Transform3D.Translation(centerOfRotation) * Transform3D.RotationZ(90.0) * Transform3D.Translation(-centerOfRotation));
        }
        /*
        public BoxPosition RotateZ180(Vector3D dim)
        {
            Vector3D centerOfRotation = Center(dim);
            return Transform(this, Transform3D.Translation(centerOfRotation) * Transform3D.RotationZ(180.0) * Transform3D.Translation(-centerOfRotation));
        }
        */
        public BoxPosition RotateZ180(Vector3D dim)
        {
            Vector3D v = Position;
            v += dim.X * HalfAxis.ToVector3D(DirectionLength) + dim.Y * HalfAxis.ToVector3D(DirectionWidth);
            return new BoxPosition(v, HalfAxis.Opposite(DirectionLength), HalfAxis.Opposite(DirectionWidth));
        }
        public void RotateThisZ180(Vector3D dim)
        {
            Position += dim.X * HalfAxis.ToVector3D(DirectionLength) + dim.Y * HalfAxis.ToVector3D(DirectionWidth);
            DirectionLength = HalfAxis.Opposite(DirectionLength);
            DirectionWidth = HalfAxis.Opposite(DirectionWidth);
        }
        #endregion

        #region Transformation
        public Transform3D Transformation
        {
            get
            {
                // build 4D matrix
                Vector3D vAxisLength = HalfAxis.ToVector3D(DirectionLength);
                Vector3D vAxisWidth = HalfAxis.ToVector3D(DirectionWidth);
                Vector3D vAxisHeight = Vector3D.CrossProduct(vAxisLength, vAxisWidth);
                Matrix4D mat = Matrix4D.Identity;
                mat.M11 = vAxisLength.X; mat.M12 = vAxisWidth.X; mat.M13 = vAxisHeight.X; mat.M14 = Position.X;
                mat.M21 = vAxisLength.Y; mat.M22 = vAxisWidth.Y; mat.M23 = vAxisHeight.Y; mat.M24 = Position.Y;
                mat.M31 = vAxisLength.Z; mat.M32 = vAxisWidth.Z; mat.M33 = vAxisHeight.Z; mat.M34 = Position.Z;
                return new Transform3D(mat);
            }
        }
        public static BoxPosition Transform(BoxPosition boxPosition, Transform3D transform)
        {
            if (!boxPosition.IsValid)
                throw new Exception("Invalid box position : can not transform");
            return new BoxPosition(
                transform.transform(boxPosition.Position)
                , HalfAxis.Transform(boxPosition.DirectionLength, transform)
                , HalfAxis.Transform(boxPosition.DirectionWidth, transform)
                );
        }
        public BoxPosition Transform(Transform3D transform)
        {
            return Transform(this, transform);
        }
        public BoxPosition Adjusted(Vector3D dimensions)
        {
            var boxPosTemp = new BoxPosition(Position, DirectionLength, DirectionWidth);
            // reverse if oriented to Z- (AXIS_Z_N)
            if (DirectionHeight == HalfAxis.HAxis.AXIS_Z_N)
            {
                if (DirectionLength == HalfAxis.HAxis.AXIS_X_P)
                {
                    boxPosTemp.DirectionWidth = HalfAxis.HAxis.AXIS_Y_P;
                    boxPosTemp.Position += new Vector3D(0.0, -dimensions.Y, -dimensions.Z);
                }
                else if (DirectionLength == HalfAxis.HAxis.AXIS_Y_P)
                {
                    boxPosTemp.DirectionWidth = HalfAxis.HAxis.AXIS_X_N;
                    boxPosTemp.Position += new Vector3D(dimensions.Y, 0.0, -dimensions.Z);
                }
                else if (DirectionLength == HalfAxis.HAxis.AXIS_X_N)
                {
                    boxPosTemp.DirectionLength = HalfAxis.HAxis.AXIS_X_P;
                    boxPosTemp.Position += new Vector3D(-dimensions.X, 0.0, -dimensions.Z);
                }
                else if (DirectionLength == HalfAxis.HAxis.AXIS_Y_N)
                {
                    boxPosTemp.DirectionWidth = HalfAxis.HAxis.AXIS_X_P;
                    boxPosTemp.Position += new Vector3D(-dimensions.Y, 0.0, -dimensions.Z);
                }
            }
            return boxPosTemp;
        }
        public bool IsPointInside(Vector3D dim, Vector3D pt)
        {
            var bbox = BBox(dim);
            var ptMin = bbox.PtMin;
            var ptMax = bbox.PtMax;
            return (ptMin.X <= pt.X) && (pt.X <= ptMax.X)
                && (ptMin.Y <= pt.Y) && (pt.Y <= ptMax.Y)
                && (ptMin.Z <= pt.Z) && (pt.Z <= ptMax.Z);
        }
        public Vector3D[] TopFacePoints(Vector3D dim)
        {
            Vector3D[] points = Points(dim);
            int[,] faceIndexes = {
                { 3, 0, 4, 7},
                { 1, 2, 6, 5},
                { 0, 1, 5, 4},
                { 2, 3, 7, 6},
                { 3, 2, 1, 0},
                { 4, 5, 6, 7}
            };
            Vector3D[] vFace = new Vector3D[4];
            for (int i = 0; i < 6; ++i)
            {
                
                for (int j = 0; j < 4; ++j)
                    vFace[j] = points[faceIndexes[i, j]];
                Vector3D faceNormal = Vector3D.CrossProduct(vFace[1] - vFace[0], vFace[2] - vFace[0]);
                faceNormal.Normalize();
                if (Math.Abs(Vector3D.DotProduct(faceNormal, Vector3D.ZAxis) - 1) < MathFunctions.EpsilonD)
                    return vFace;
            }
            throw new Exception("No top face found?");
        }
        public Vector3D[] Points(Vector3D dim)
        {
            Vector3D position = Position;
            Vector3D vI = HalfAxis.ToVector3D(DirectionLength);
            Vector3D vJ = HalfAxis.ToVector3D(DirectionWidth);
            Vector3D vK = Vector3D.CrossProduct(vI, vJ);

            var points = new Vector3D[8];
            points[0] = position;
            points[1] = position + dim[0] * vI;
            points[2] = position + dim[0] * vI + dim[1] * vJ;
            points[3] = position + dim[1] * vJ;

            points[4] = position + dim[2] * vK;
            points[5] = position + dim[2] * vK + dim[0] * vI;
            points[6] = position + dim[2] * vK + dim[0] * vI + dim[1] * vJ;
            points[7] = position + dim[2] * vK + dim[1] * vJ;

            return points;
        }
        public void MinMax2D(double boxLength, double boxWidth, out Vector2D vMin, out Vector2D vMax)
        {
            Vector3D[] pts = new Vector3D[4];
            pts[0] = new Vector3D(Position.X, Position.Y, 0.0);
            pts[1] = new Vector3D(Position.X, Position.Y, 0.0) + HalfAxis.ToVector3D(DirectionLength) * boxLength;
            pts[2] = new Vector3D(Position.X, Position.Y, 0.0) + HalfAxis.ToVector3D(DirectionWidth) * boxWidth;
            pts[3] = new Vector3D(Position.X, Position.Y, 0.0) + HalfAxis.ToVector3D(DirectionLength) * boxLength + HalfAxis.ToVector3D(DirectionWidth) * boxWidth;

            vMin = new Vector2D(double.MaxValue, double.MaxValue);
            vMax = new Vector2D(double.MinValue, double.MinValue);
            foreach (Vector3D v in pts)
            {
                vMin.X = Math.Min(v.X, vMin.X);
                vMin.Y = Math.Min(v.Y, vMin.Y);
                vMax.X = Math.Max(v.X, vMax.X);
                vMax.Y = Math.Max(v.Y, vMax.Y);
            }
        }
        public bool IntersectPlane(Vector3D dim, double absValue, int axis, ref List<Vector3D> ptsInter)
        {
            var bbox = BBox(dim);
            var ptMin = bbox.PtMin;
            var ptMax = bbox.PtMax;
            if (0 == axis && absValue >= ptMin.X && absValue <= ptMax.X)
            {
                ptsInter.Add(new Vector3D(absValue, ptMin.Y, ptMin.Z));
                ptsInter.Add(new Vector3D(absValue, ptMin.Y, ptMax.Z));
                ptsInter.Add(new Vector3D(absValue, ptMax.Y, ptMax.Z));
                ptsInter.Add(new Vector3D(absValue, ptMax.Y, ptMin.Z));
            }
            else if (1 == axis && absValue >= ptMin.Y && absValue <= ptMax.Y)
            {
                ptsInter.Add(new Vector3D(ptMin.X, absValue, ptMin.Z));
                ptsInter.Add(new Vector3D(ptMin.X, absValue, ptMax.Z));
                ptsInter.Add(new Vector3D(ptMax.X, absValue, ptMax.Z));
                ptsInter.Add(new Vector3D(ptMax.X, absValue, ptMin.Z));
            }
            else if (2 == axis && absValue >= ptMin.Z && absValue <= ptMax.Z)
            {
                ptsInter.Add(new Vector3D(ptMin.X, ptMin.Y, absValue));
                ptsInter.Add(new Vector3D(ptMin.X, ptMax.Y, absValue));
                ptsInter.Add(new Vector3D(ptMax.X, ptMax.Y, absValue));
                ptsInter.Add(new Vector3D(ptMax.X, ptMin.Y, absValue));
            }
            else
                return false;
            return true;
        }
        public void FlipFacingOutside(int facing, Vector3D dimBox, Vector2D dimContainer)
        {
            // sanity check
            if (!DirectionHeight.IsOneOf(HalfAxis.HAxis.AXIS_Z_N, HalfAxis.HAxis.AXIS_Z_P))
                return;

            Vector3D posFacing = Vector3D.Zero;
            HalfAxis.HAxis hAxis = HalfAxis.HAxis.AXIS_X_P;
            GetFacingPosition(dimBox, facing, false, ref posFacing, ref hAxis);
            Vector3D posFacingOpposite = Vector3D.Zero;
            HalfAxis.HAxis hAxisOpposite = HalfAxis.HAxis.AXIS_X_P;
            GetFacingPosition(dimBox, facing, true, ref posFacingOpposite, ref hAxisOpposite);

            if (hAxis == HalfAxis.HAxis.AXIS_X_P || hAxis == HalfAxis.HAxis.AXIS_X_N)
            {
                double distBorder = Math.Min(Math.Abs(posFacing.Y - 0.0), Math.Abs(dimContainer.Y - posFacing.Y));
                double distBorderOpposite = Math.Min(Math.Abs(posFacingOpposite.Y - 0.0), Math.Abs(dimContainer.Y - posFacingOpposite.Y));
                if (distBorderOpposite < distBorder) RotateThisZ180(dimBox);
            }
            else if (hAxis == HalfAxis.HAxis.AXIS_Y_P || hAxis == HalfAxis.HAxis.AXIS_Y_N)
            {
                double distBorder = Math.Min(Math.Abs(posFacing.X - 0.0), Math.Abs(dimContainer.X - posFacing.X));
                double distBorderOpposite = Math.Min(Math.Abs(posFacingOpposite.X - 0.0), Math.Abs(dimContainer.X - posFacingOpposite.X));
                if (distBorderOpposite < distBorder) RotateThisZ180(dimBox);
            }
        }
        private bool GetFacingPosition(Vector3D dims, int facing, bool opposite, ref Vector3D posFacing, ref HalfAxis.HAxis hAxis)
        {
            if (DirectionHeight != HalfAxis.HAxis.AXIS_Z_P && DirectionHeight != HalfAxis.HAxis.AXIS_Z_N)
                return false;
            posFacing = Position + dims.Z * Vector3D.ZAxis;
            if (!opposite)
            {
                switch (facing)
                {
                    case 1:
                        posFacing += 0.5 * dims.X * HalfAxis.ToVector3D(DirectionLength);
                        hAxis = DirectionLength;
                        break;
                    case 2:
                        posFacing += dims.X * HalfAxis.ToVector3D(DirectionLength) + 0.5 * dims.Y * HalfAxis.ToVector3D(DirectionWidth);
                        hAxis = DirectionWidth;
                        break;
                    case 3:
                        posFacing += 0.5 * dims.X * HalfAxis.ToVector3D(DirectionLength) + dims.Y * HalfAxis.ToVector3D(DirectionWidth);
                        hAxis = HalfAxis.Opposite(DirectionLength);
                        break;
                    case 4:
                        posFacing += 0.5 * dims.Y * HalfAxis.ToVector3D(DirectionWidth);
                        hAxis = HalfAxis.Opposite(DirectionWidth);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (facing)
                {
                    case 1:
                        posFacing += 0.5 * dims.X * HalfAxis.ToVector3D(DirectionLength) + dims.Y * HalfAxis.ToVector3D(DirectionWidth);
                        hAxis = HalfAxis.Opposite(DirectionLength);
                        break;
                    case 2:
                        posFacing += 0.5 * dims.Y * HalfAxis.ToVector3D(DirectionWidth);
                        hAxis = HalfAxis.Opposite(DirectionWidth);
                        break;
                    case 3:
                        posFacing += 0.5 * dims.X * HalfAxis.ToVector3D(DirectionLength);
                        hAxis = DirectionLength;
                        break;
                    case 4:
                        posFacing += dims.X * HalfAxis.ToVector3D(DirectionLength) + 0.5 * dims.Y * HalfAxis.ToVector3D(DirectionWidth);
                        hAxis = DirectionWidth;
                        break;
                    default:
                        break;
                }
            }
            return true;
        }
        #endregion

        #region Static members
        public static bool Intersect(BoxPosition p1, BoxPosition p2, double boxLength, double boxWidth)
        {
            p1.MinMax2D(boxLength, boxWidth, out Vector2D v1Min, out Vector2D v1Max);
            p2.MinMax2D(boxLength, boxWidth, out Vector2D v2Min, out Vector2D v2Max);
            if (v1Max.X <= v2Min.X || v2Max.X <= v1Min.X || v1Max.Y <= v2Min.Y || v2Max.Y <= v1Min.Y)
                return false;
            return true;
        }
        public static BoxPosition FromPosDimOrient(Vector3D pos, Vector3D dimensions, int orientation)
        {
            // 1 -> L W H
            // 2 -> W L H
            // 3 -> W H L
            // 4 -> H W L
            // 5 -> L H W
            // 6 -> H L W
            switch (orientation)
            {
                case 1: return new BoxPosition(pos, HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
                case 2: return new BoxPosition(pos + dimensions.Y * Vector3D.XAxis, HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N);
                case 3: return new BoxPosition(pos, HalfAxis.HAxis.AXIS_Z_P, HalfAxis.HAxis.AXIS_X_P);
                case 4: return new BoxPosition(pos + dimensions.Y * Vector3D.YAxis, HalfAxis.HAxis.AXIS_Z_P, HalfAxis.HAxis.AXIS_Y_N);
                case 5: return new BoxPosition(pos + dimensions.Z * Vector3D.YAxis, HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Z_P);
                case 6: return new BoxPosition(pos, HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_Z_P);
                default: throw new Exception($"FromPosDimOrient : cannot have orientation = {orientation}");
            }
        }
        /// <summary>
        /// This method will be used to build 
        /// </summary>
        /// <param name="pos">Position of left/front/down corner of box</param>
        /// <param name="dimOriented">Projections of box on axes X/Y/Z</param>
        /// <param name="dimOriginal">Box length, width, height</param>
        /// <returns></returns>
        public static BoxPosition FromPositionDimension(Vector3D pos, Vector3D dimOriented, Vector3D dimOriginal)
        {
            if (MostlyEqual(dimOriented[0], dimOriginal[0])) 
            {
                // L W H
                if (MostlyEqual(dimOriented[1], dimOriginal[1]))
                    return FromPosDimOrient(pos, dimOriginal, 1);
                // L H W
                else if (MostlyEqual(dimOriented[1], dimOriginal[2]))
                    return FromPosDimOrient(pos, dimOriginal, 5);
            }
            else if (MostlyEqual(dimOriented[0], dimOriginal[1]))
            {
                // W L H
                if (MostlyEqual(dimOriented[1], dimOriginal[0]))
                   return FromPosDimOrient(pos, dimOriginal, 2);
                // W H L
                else if (MostlyEqual(dimOriented[1], dimOriginal[2]))
                    return FromPosDimOrient(pos, dimOriginal, 3);
            }
            else if (MostlyEqual(dimOriented[0], dimOriginal[2]))
            {
                // H L W
                if (MostlyEqual(dimOriented[1], dimOriginal[0]))
                    return FromPosDimOrient(pos, dimOriginal, 6);
                // H W L
                else if (MostlyEqual(dimOriented[1], dimOriginal[1]))
                   return FromPosDimOrient(pos, dimOriginal, 4);
            }
            return Zero;
        }
        private static bool MostlyEqual(double val0, double val1) => Math.Abs(val1 - val0) < 1.0e-03;
        public static BoxPosition Parse(string s)
        {
            string[] sArray = s.Split('|');
            var v = Vector3D.Parse(sArray[0]);
            string sOrientation = sArray[1];
            sOrientation = sOrientation.Trim();
            sOrientation = sOrientation.TrimStart('(');
            sOrientation = sOrientation.TrimEnd(')');
            string[] vOrientation = sOrientation.Split(',');
            return new BoxPosition(v, HalfAxis.Parse(vOrientation[0]), HalfAxis.Parse(vOrientation[1]));
        }
        #endregion

        #region BoxPosition list
        public static bool SaveListToXMLFile(List<BoxPosition> boxPositions, string filename)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Save(filename);

                var listElt = xmlDoc.CreateElement("BoxPositions");
                xmlDoc.AppendChild(listElt);

                foreach (var bPos in boxPositions)
                {
                    // BoxPosition
                    XmlElement boxPositionElt = xmlDoc.CreateElement("BoxPosition");
                    listElt.AppendChild(boxPositionElt);
                    // Position
                    XmlAttribute positionAttribute = xmlDoc.CreateAttribute("Position");
                    positionAttribute.Value = bPos.Position.ToString();
                    boxPositionElt.Attributes.Append(positionAttribute);
                    // AxisLength
                    XmlAttribute axisLengthAttribute = xmlDoc.CreateAttribute("AxisLength");
                    axisLengthAttribute.Value = HalfAxis.ToString(bPos.DirectionLength);
                    boxPositionElt.Attributes.Append(axisLengthAttribute);
                    // AxisWidth
                    XmlAttribute axisWidthAttribute = xmlDoc.CreateAttribute("AxisWidth");
                    axisWidthAttribute.Value = HalfAxis.ToString(bPos.DirectionWidth);
                    boxPositionElt.Attributes.Append(axisWidthAttribute);
                }
            }
            catch (Exception /*ex*/)
            {
                return false;
            }
            return true;
        }
        public static bool LoadListFromXMLFile(out List<BoxPosition> boxPositions, string filename)
        {
            boxPositions = new List<BoxPosition>();
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filename);

                XmlElement eltBoxPositions = xmlDoc.DocumentElement;
                foreach (XmlNode nodeBP in eltBoxPositions.ChildNodes)
                {
                    if (nodeBP is XmlElement eltBoxPosition)
                    {
                        string sPosition = eltBoxPosition.Attributes["Position"].Value;
                        string sAxisLength = eltBoxPosition.Attributes["AxisLength"].Value;
                        string sAxisWidth = eltBoxPosition.Attributes["AxisWidth"].Value;
                        boxPositions.Add(new BoxPosition(Vector3D.Parse(sPosition), HalfAxis.Parse(sAxisLength), HalfAxis.Parse(sAxisWidth)));
                    }
                }
            }
            catch (Exception /*ex*/)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Object method overrides
        public override string ToString() => $"{Position} | ({HalfAxis.ToString(DirectionLength)},{HalfAxis.ToString(DirectionWidth)})";
        public object Clone() => new BoxPosition(Position, DirectionLength, DirectionWidth);
        public override bool Equals(object obj)
        {
            BoxPosition bPos = (BoxPosition)obj;
            return Position.Equals(bPos.Position)
                && bPos.DirectionLength.Equals(DirectionLength)
                && bPos.DirectionWidth.Equals(DirectionWidth); 
        }
        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ DirectionLength.GetHashCode() ^ DirectionWidth.GetHashCode();
        }
        public static bool operator ==(BoxPosition left, BoxPosition right) { return left.Equals(right); }
        public static bool operator !=(BoxPosition left, BoxPosition right) { return !(left == right); }
        #endregion

        public static bool HaveSameOrientation(BoxPosition bpos1, BoxPosition bpos2)
            => bpos1.DirectionLength == bpos2.DirectionLength && bpos1.DirectionWidth == bpos2.DirectionWidth;

    }

    public class BoxInteraction
    {
        public static bool BoxIsInside(BoxPosition bPos, Vector3D dim, Vector2D ptMin, Vector2D ptMax)
        {
            var bbox = bPos.BBox(dim);
            return ptMin.X <= bbox.PtMin.X && ptMin.Y <= bbox.PtMin.Y
                && bbox.PtMax.X <= ptMax.X && bbox.PtMax.Y <= ptMax.Y;
        }
        public static bool BoxCanMoveInside(BoxPosition bPos, Vector3D dim, Vector2D ptMin, Vector2D ptMax, HalfAxis.HAxis axis)
        {
            var bbox = bPos.BBox(dim);
            switch (axis)
            {
                case HalfAxis.HAxis.AXIS_X_N: return bbox.PtMin.X > ptMin.X;
                case HalfAxis.HAxis.AXIS_X_P: return bbox.PtMax.X < ptMax.X;
                case HalfAxis.HAxis.AXIS_Y_N: return bbox.PtMin.Y > ptMin.Y;
                case HalfAxis.HAxis.AXIS_Y_P: return bbox.PtMax.Y < ptMax.Y;
                default: return false;
            }
        }
        public static bool BoxesAreInside(List<BoxPosition> list, Vector3D dim, Vector2D ptMin, Vector2D ptMax)
        {
            foreach (var bpos in list)
            {
                if (!BoxIsInside(bpos, dim, ptMin, ptMax))
                    return false;
            }
            return true;
        }
        public static bool PointIsInside(BoxPosition bPos, Vector3D dim, Vector2D pt)
        {
            var bbox = bPos.BBox(dim);
            return pt.X >= bbox.PtMin.X && pt.X <= bbox.PtMax.X
                && pt.Y >= bbox.PtMin.Y && pt.Y <= bbox.PtMax.Y;
        }
        public static int SelectedPosition(List<BoxPosition> list, Vector3D dim, Vector2D pt)
        {
            for (int i = 0; i < list.Count; ++i)
                if (PointIsInside(list[i], dim, pt))
                    return i;
            return -1;
        }
        public static bool HaveIntersection(List<BoxPosition> list, Vector3D dim, int selectedIndex, BoxPosition bPosition)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (i != selectedIndex && HaveIntersection(list[i], dim, bPosition, dim))
                    return true;
            }
            return false;
        }
        public static bool HaveIntersection(BoxPosition bPos1, Vector3D dim1, BoxPosition bPos2, Vector3D dim2)
        {
            var bbox1 = bPos1.BBox(dim1);
            var bbox2 = bPos2.BBox(dim2);
            Vector3D pt1 = bbox1.PtMin, pt2 = bbox1.PtMax;
            Vector3D pt3 = bbox2.PtMin, pt4 = bbox2.PtMax;

            Vector3D pt5 = new Vector3D(Math.Max(pt1.X, pt3.X), Math.Max(pt1.Y, pt3.Y), Math.Max(pt1.Z, pt3.Z));
            Vector3D pt6 = new Vector3D(Math.Min(pt2.X, pt4.X), Math.Min(pt2.Y, pt4.Y), Math.Min(pt2.Z, pt4.Z));

            // no intersections ?
            if ((pt5.X - pt6.X > -EPSILON)
                || (pt5.Y - pt6.Y > -EPSILON)
                || (pt5.Z - pt6.Z > -EPSILON))
                return false;
            else
                return true;
        }
        public static bool MinDistance(List<BoxPosition> list, Vector3D dim, int selectedIndex, HalfAxis.HAxis axis, ref double distance)
        {
            if (selectedIndex < 0 || selectedIndex > list.Count - 1)
                return false;
            var bPos = list[selectedIndex];
            distance = double.MaxValue;
            bool found = false;
            for (int i = 0; i < list.Count; ++i)
            {
                if (i == selectedIndex)
                    continue;
                double dTemp = double.MaxValue;
                if (MinDistance(bPos, dim, list[i], dim, axis, ref dTemp))
                {
                    found = true;
                    distance = Math.Min(distance, dTemp);
                }
            }
            if (!found) distance = 0.0;
            return found;
        }
        public static bool MinDistance(
            BoxPosition bPos1, Vector3D dim1
            , BoxPosition bPos2, Vector3D dim2
            , HalfAxis.HAxis axis
            , ref double distance)
        {
            var bbox1 = bPos1.BBox(dim1);
            var bbox2 = bPos2.BBox(dim2);
            Vector3D pt1 = bbox1.PtMin, pt2 = bbox1.PtMax;
            Vector3D pt3 = bbox2.PtMin, pt4 = bbox2.PtMax;

            Vector3D pt5 = new Vector3D(Math.Max(pt1.X, pt3.X), Math.Max(pt1.Y, pt3.Y), Math.Max(pt1.Z, pt3.Z));
            Vector3D pt6 = new Vector3D(Math.Min(pt2.X, pt4.X), Math.Min(pt2.Y, pt4.Y), Math.Min(pt2.Z, pt4.Z));

            if (axis == HalfAxis.HAxis.AXIS_X_N || axis == HalfAxis.HAxis.AXIS_X_P)
            {
                if ((pt5.Y - pt6.Y > -EPSILON) || (pt5.Z - pt6.Z > -EPSILON))
                    return false;
                if (axis == HalfAxis.HAxis.AXIS_X_N)
                {
                    distance = pt1.X - pt4.X;
                    return distance >= -EPSILON;
                }
                else if (axis == HalfAxis.HAxis.AXIS_X_P)
                {
                    distance = pt3.X - pt2.X;
                    return distance >= -EPSILON;
                }
            }
            else if (axis == HalfAxis.HAxis.AXIS_Y_N || axis == HalfAxis.HAxis.AXIS_Y_P)
            {
                if ((pt5.X - pt6.X > -EPSILON) || (pt5.Z - pt6.Z > -EPSILON))
                    return false;
                if (axis == HalfAxis.HAxis.AXIS_Y_N)
                {
                    distance = pt1.Y - pt4.Y;
                    return distance >= -EPSILON;
                }
                else if (axis == HalfAxis.HAxis.AXIS_Y_P)
                {
                    distance = pt3.Y - pt2.Y;
                    return distance >= -EPSILON;
                }
            }
            else if (axis == HalfAxis.HAxis.AXIS_Z_N || axis == HalfAxis.HAxis.AXIS_Z_P)
            {
                if ((pt5.X - pt6.X > -EPSILON) || (pt5.Y - pt6.Z > -EPSILON))
                    return false;
                if (axis == HalfAxis.HAxis.AXIS_Z_N)
                {
                    distance = pt1.Z - pt4.Z;
                    return distance >= -EPSILON;
                }
                else if (axis == HalfAxis.HAxis.AXIS_Z_P)
                {
                    distance = pt3.Z - pt2.Z;
                    return distance >= -EPSILON;
                }
            }
            distance = 0.0;
            return false;
        }
        static readonly double EPSILON = 1.0E-03;
    }
}
