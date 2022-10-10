#region Using directives
using System.Drawing;

using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public class DimensionCube
    {
        #region Constructors
        public DimensionCube(Vector3D dimensions)
        {
            Dimensions[0] = dimensions.X; Dimensions[1] = dimensions.Y; Dimensions[2] = dimensions.Z;
            BuildPoints();
        }
        public DimensionCube(double length, double width, double height)
        { 
            Dimensions[0] = length; Dimensions[1] = width; Dimensions[2] = height;
            BuildPoints();
        }
        public DimensionCube(double[] dim)
        {
            for (int i = 0; i < 3; ++i) Dimensions[i] = dim[i];
            BuildPoints();
        }
        public DimensionCube(Vector3D position, double length, double width, double height, Color color, bool above)
        {
            Position = position;
            Dimensions[0] = length; Dimensions[1] = width; Dimensions[2] = height;
            Color = color;
            Above = above;
            BuildPoints();
        }
        public DimensionCube(Basics.BBox3D bbox, Color color, bool above)
        {
            Position = bbox.PtMin;
            Dimensions[0] = bbox.Length; Dimensions[1] = bbox.Width; Dimensions[2] = bbox.Height;
            Color = color;
            Above = above;
            BuildPoints();
        }
        #endregion

        #region Public properties
        public bool[] ShowArrow { get; set; } = new bool[] { true, true, true };
        public Vector3D Position { get; set; }
        public Color Color { get; set; } = Color.Black;
        public bool Above { get; set; } = false;
        #endregion

        #region Drawing
        public void Draw(Graphics3D graphics)
        {
            ArrowPoints(graphics, out int[] arrow0, out int[] arrow1, out int[] arrow2);

            DrawArrow(arrow0, graphics);
            DrawArrow(arrow1, graphics);
            DrawArrow(arrow2, graphics);
        }

        public Vector3D[] DrawingPoints(Graphics3D graphics)
        {
            ArrowPoints(graphics, out int[] arrow0, out int[] arrow1, out int[] arrow2);

            const double exageration = 1.2;

            Vector3D[] pts = new Vector3D[6];
            pts[0] = Points[arrow0[0]] + (Points[arrow0[0]] - Points[arrow0[2]]) * OffsetPerc * exageration;
            pts[1] = Points[arrow0[1]] + (Points[arrow0[1]] - Points[arrow0[3]]) * OffsetPerc * exageration;
            pts[2] = Points[arrow1[0]] + (Points[arrow1[0]] - Points[arrow1[2]]) * OffsetPerc * exageration;
            pts[3] = Points[arrow1[1]] + (Points[arrow1[1]] - Points[arrow1[3]]) * OffsetPerc * exageration;
            pts[4] = Points[arrow2[0]] + (Points[arrow2[0]] - Points[arrow2[2]]) * OffsetPerc * exageration;
            pts[5] = Points[arrow2[1]] + (Points[arrow2[1]] - Points[arrow2[3]]) * OffsetPerc * exageration;
            return pts;
        }
        #endregion

        #region Transformation
        public static DimensionCube Transform(DimensionCube dimCube, Transform3D transform)
        {
            Vector3D pos = transform.transform(dimCube.Position);
            Vector3D dim = transform.transformRot(new Vector3D(dimCube.Dimensions) );
            if (dim.X < 0) { pos.X += dim.X; dim.X = -dim.X; }
            if (dim.Y < 0) { pos.Y += dim.Y; dim.Y = -dim.Y; }
            if (dim.Z < 0) { pos.Z += dim.Z; dim.Z = -dim.Z; }
            return new DimensionCube(pos, dim.X, dim.Y, dim.Z, dimCube.Color, dimCube.Above);
        }
        #endregion

        #region Helpers
        private void BuildPoints()
        {
            Points[0] = Position;
            Points[1] = Position + Dimensions[0] * Vector3D.XAxis;
            Points[2] = Position + Dimensions[0] * Vector3D.XAxis + Dimensions[1] * Vector3D.YAxis;
            Points[3] = Position + Dimensions[1] * Vector3D.YAxis;
            Points[4] = Position + Dimensions[2] * Vector3D.ZAxis;
            Points[5] = Position + Dimensions[0] * Vector3D.XAxis + Dimensions[2] * Vector3D.ZAxis;
            Points[6] = Position + Dimensions[0] * Vector3D.XAxis + Dimensions[1] * Vector3D.YAxis + Dimensions[2] * Vector3D.ZAxis;
            Points[7] = Position + Dimensions[1] * Vector3D.YAxis + Dimensions[2] * Vector3D.ZAxis; 
        }

        private void ArrowPoints(Graphics3D graphics, out int[] arrow0, out int[] arrow1, out int[] arrow2)
        {
            Vector3D viewDir = graphics.ViewDirection;
            arrow0 = null;
            arrow1 = null;
            arrow2 = null;

            if (!Above)
            {
                if (viewDir.X < 0.0 && viewDir.Y >= 0.0)
                {
                    arrow0 = new int[] { 0, 4, 1, 5 };
                    arrow1 = new int[] { 0, 1, 3, 2 };
                    arrow2 = new int[] { 1, 2, 0, 3 };
                }
                else if (viewDir.X < 0.0 && viewDir.Y < 0.0)
                {
                    arrow0 = new int[] { 1, 5, 2, 6 };
                    arrow1 = new int[] { 1, 2, 0, 3 };
                    arrow2 = new int[] { 2, 3, 1, 0 };
                }
                else if (viewDir.X >= 0.0 && viewDir.Y < 0.0)
                {
                    arrow0 = new int[] { 2, 6, 3, 7 };
                    arrow1 = new int[] { 2, 3, 1, 0 };
                    arrow2 = new int[] { 3, 0, 2, 1 };
                }
                else if (viewDir.X >= 0.0 && viewDir.Y >= 0.0)
                {
                    arrow0 = new int[] { 3, 7, 0, 4 };
                    arrow1 = new int[] { 3, 0, 2, 1 };
                    arrow2 = new int[] { 0, 1, 3, 2 };
                }
            }
            else
            {
                if (viewDir.X < 0.0 && viewDir.Y >= 0.0)
                {
                    arrow0 = new int[] { 2, 6, 3, 7 };
                    arrow1 = new int[] { 7, 6, 4, 5 };
                    arrow2 = new int[] { 4, 7, 5, 6 };
                }
                else if (viewDir.X < 0.0 && viewDir.Y < 0.0)
                {
                    arrow0 = new int[] { 3, 7, 0, 4 };
                    arrow1 = new int[] { 4, 7, 5, 6 };
                    arrow2 = new int[] { 4, 5, 7, 6 };
                }
                else if (viewDir.X >= 0.0 && viewDir.Y < 0.0)
                {
                    arrow0 = new int[] { 4, 0, 5, 1 };
                    arrow1 = new int[] { 4, 5, 7, 6 };
                    arrow2 = new int[] { 6, 5, 7, 4 };
                }
                else if (viewDir.X >= 0.0 && viewDir.Y >= 0.0)
                {
                    arrow0 = new int[] { 1, 5, 2, 6 };
                    arrow1 = new int[] { 5, 6, 4, 7 };
                    arrow2 = new int[] { 7, 6, 4, 5 };
                }
            }
        }

        private void DrawArrow(int[] arrow, Graphics3D graphics)
        {
            Vector3D pt0 = Points[arrow[0]];
            Vector3D pt0_ = pt0 + (pt0 - Points[arrow[2]]) * OffsetPerc;
            Vector3D pt00_ = pt0 + (pt0 - Points[arrow[2]]) * OffsetPerc * 1.1;

            Vector3D pt1 = Points[arrow[1]];
            Vector3D pt1_ = pt1 + (pt1 - Points[arrow[3]]) * OffsetPerc;
            Vector3D pt11_ = pt1 + (pt1 - Points[arrow[3]]) * OffsetPerc * 1.1;

            if ((pt1 - pt0).GetLengthSquared() < 1.0E-03)
                return;

            string text = string.Format("{0:0.#}", (pt1-pt0).GetLength());
            graphics.Draw(text, 0.5 * (pt1_ + pt0_), Color, graphics.FontSize);
            graphics.Draw(new Segment(pt0_, pt0_ + (pt1 - pt0) * (2.0 / 5.0), Color));
            graphics.Draw(new Segment(pt0_ + (pt1 - pt0) * (3.0 / 5.0), pt1_, Color));
            graphics.Draw(new Segment(pt0, pt00_, Color));
            graphics.Draw(new Segment(pt1, pt11_, Color));
        }
        #endregion

        #region Data members
        private double[] Dimensions = new double[3];
        private double OffsetPerc { get; set; } = 0.2;
        private Vector3D[] Points = new Vector3D[8];
        #endregion
    }
}
