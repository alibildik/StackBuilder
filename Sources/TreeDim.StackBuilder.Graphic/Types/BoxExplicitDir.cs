#region Using directives
using System.Linq;
using System.Drawing;


using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public class BoxExplicitDir : Drawable
    {
        public BoxExplicitDir(uint pickId,
            double length, double width, double height,
            Vector3D position, Vector3D dirLength, Vector3D dirWidth,
            Color color)
            : base(pickId)
        {
            Length = length;
            Width = width;
            Height = height;
            Position = position;
            LengthAxis = dirLength;
            WidthAxis = dirWidth;
            Colors = Enumerable.Repeat(color, 6).ToArray();
        }
        public Vector3D Position { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public Vector3D LengthAxis { get; set; }
        public Vector3D WidthAxis { get; set; }
        public Vector3D HeightAxis => Vector3D.CrossProduct(LengthAxis, WidthAxis);
        public Color[] Colors { get; private set; }
        public Vector3D Dimensions => new Vector3D(Length, Width, Height);
        public void SetAllFacesColor(Color color) { Colors = Enumerable.Repeat(color, 6).ToArray(); }
        public void SetFaceColor(int iFace, Color color) => Colors[iFace] = color;
        public override void Draw(Graphics3D graphics) { }
        public override Vector3D[] Points
        {
            get
            {
                Vector3D position = Position;
                Vector3D[] points = new Vector3D[8];
                points[0] = position;
                points[1] = position + Dimensions.X * LengthAxis;
                points[2] = position + Dimensions.X * LengthAxis + Dimensions.Y * WidthAxis;
                points[3] = position + Dimensions.Y * WidthAxis;

                points[4] = position + Dimensions.Z * HeightAxis;
                points[5] = position + Dimensions.Z * HeightAxis + Dimensions.X * LengthAxis;
                points[6] = position + Dimensions.Z * HeightAxis + Dimensions.X * LengthAxis + Dimensions.Y * WidthAxis;
                points[7] = position + Dimensions.Z * HeightAxis + Dimensions.Y * WidthAxis;

                return points;
            }
        }
        public Triangle[] Triangles
        {
            get
            {
                Vector3D[] points = Points;
                return new Triangle[]
                {
                        // XN
                        //
                        // 7 ------- 4
                        // |         |
                        // 3 ------- 0
                        //
                        new Triangle(PickId, points[0], true, points[4], false, points[3], true, Colors[0], Color.Black),
                        new Triangle(PickId, points[3], false, points[4], true, points[7], true, Colors[0], Color.Black),
                        // XP
                        //
                        // 5 ------- 6
                        // |         |
                        // 1 ------- 2
                        //
                        new Triangle(PickId, points[1], true, points[2], false, points[5], true, Colors[1], Color.Black),
                        new Triangle(PickId, points[5], false, points[2], true, points[6], true, Colors[1], Color.Black),
                        // YN
                        //
                        // 4 -------- 5
                        // |          |
                        // 0 -------- 1
                        //
                        new Triangle(PickId, points[0], true, points[1], false, points[4], true, Colors[2], Color.Black),
                        new Triangle(PickId, points[4], false, points[1], true, points[5], true, Colors[2], Color.Black),
                        // YP
                        //
                        // 6 -------- 7
                        // |          | 
                        // 2 -------- 3
                        //
                        new Triangle(PickId, points[7], true, points[6], true, points[2], false, Colors[3], Color.Black),
                        new Triangle(PickId, points[7], false, points[2], true, points[3], true, Colors[3], Color.Black),
                        // ZN
                        //
                        // 3 -------- 2
                        // |          | 
                        // 0 -------- 1
                        //
                        new Triangle(PickId, points[0], true, points[3], false, points[1], true, Colors[4], Color.Black),
                        new Triangle(PickId, points[1], false, points[3], true, points[2], true, Colors[4], Color.Black),
                        // ZP
                        //
                        // 7-----------6
                        // |           |
                        // 4-----------5

                        new Triangle(PickId, points[4], true, points[5], true, points[6], false, Colors[5], Color.Black),
                        new Triangle(PickId, points[4], false, points[6], true, points[7], true, Colors[5], Color.Black)
                };
            }
        }
    }
}
