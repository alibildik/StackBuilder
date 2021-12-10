#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public class Arrow : Drawable
    {
        public Arrow(uint pickId) : base(pickId)
        {        
        }
        public double Length { get; set; }
        public double Width { get; set; }
        public Vector3D Position { get; set; }
        public HalfAxis.HAxis LengthAxis { get; set; } = HalfAxis.HAxis.AXIS_X_P;
        public HalfAxis.HAxis WidthAxis { get; set; } = HalfAxis.HAxis.AXIS_Y_P;
        public Color ColorFill { get; set; }
        public Color ColorPath { get; set; }
        public override Vector3D[] Points
        {
            get
            {
                //               
                //              4-     
                //              | | 
                // 6------------5  -|    
                // |                 -3  
                // 0------------1  -|
                //              | |
                //              2-     
                Vector3D LAxis = HalfAxis.ToVector3D(LengthAxis);
                Vector3D WAxis = HalfAxis.ToVector3D(WidthAxis);

                Vector3D[] points = new Vector3D[7];
                points[0] = Position - 0.5 * Width * WAxis;
                points[1] = Position - 0.5 * Width * WAxis + (Length - 2 * Width) * LAxis;
                points[2] = Position - Width * WAxis       + (Length - 2 * Width) * LAxis;
                points[3] = Position + Length * LAxis;
                points[4] = Position + Width * WAxis       + (Length - 2 * Width) * LAxis;
                points[5] = Position + 0.5 * Width * WAxis + (Length - 2 * Width) * LAxis;
                points[6] = Position + 0.5 * Width * WAxis;

                return points;
            }
        }

        public Face[] Faces
        {
            get
            {
                Face[] faces = new Face[1];
                Vector3D[] points = Points;
                faces[0] = new Face(
                    PickId,
                    new Vector3D[] { points[0], points[1], points[5], points[6] },
                    ColorFill, ColorPath, "ARROW", true);
                return faces;
            }
        }
        public Triangle[] Triangles
        {
            get
            {
                Triangle[] triangles = new Triangle[1];
                Vector3D[] points = Points;
                triangles[0] = new Triangle(
                    PickId,
                    new Vector3D[] { points[2], points[3], points[4] },
                    new bool[] { true, true, true }
                    )
                    {
                        ColorFill = ColorFill,
                        ColorPath = ColorPath
                    };
                return triangles;
            }
        }
        public override void Draw(Graphics3D graphics)
        {
            foreach (var face in Faces)
                graphics.AddFace(face);
            foreach (var tr in Triangles)
                graphics.AddTriangle(tr);
        }
        public override void DrawBegin(Graphics3D graphics)
        {
            if (Vector3D.DotProduct(Faces[0].Normal, graphics.ViewDirection) >= 0.0)
                graphics.AddFace(Faces[0]);
            if (Vector3D.DotProduct(Triangles[0].Normal, graphics.ViewDirection) >= 0.0)
                graphics.AddTriangle(Triangles[0]);
        }
        public override void DrawEnd(Graphics3D graphics)
        {
            if (Vector3D.DotProduct(Faces[0].Normal, graphics.ViewDirection) <= 0.0)
                graphics.AddFace(Faces[0]);
            if (Vector3D.DotProduct(Triangles[0].Normal, graphics.ViewDirection) <= 0.0)
                graphics.AddTriangle(Triangles[0]);
        }
    }
}
