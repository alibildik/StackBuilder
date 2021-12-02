#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Sharp3D.Math.Core;
using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public class Corner : Drawable
    {
        #region Data members
        private Vector3D _position;
        #endregion

        #region Constructor
        public Corner(uint pickId, PalletCornerProperties cornerProperties)
            : base(0)
        {
            PickId = pickId;
            W = cornerProperties.Width;
            Th = cornerProperties.Thickness;
            Height = cornerProperties.Length;
            Color = cornerProperties.Color;
        }
        #endregion

        #region Public properties
        public double Height { get; set; } = 0.0;
        public Vector3D Position
        {
            get { return _position; }
            set { _position = value; }
        }
        public HalfAxis.HAxis LengthAxis { get; set; } = HalfAxis.HAxis.AXIS_X_P;
        public HalfAxis.HAxis WidthAxis { get; set; } = HalfAxis.HAxis.AXIS_Y_P;
        #endregion

        #region Points / Face
        public void SetPosition(Vector3D position, HalfAxis.HAxis lengthAxis, HalfAxis.HAxis widthAxis)
        {
            _position = position;
            LengthAxis = lengthAxis;
            WidthAxis = widthAxis;
        }

        public override Vector3D[] Points
        {
            get
            {
                //
                //   5---4
                //   |   |
                //   |   |
                //   |   0 ----- 2
                //   |           |
                //   1 --------- 3
                //
                //
                //   11--10
                //   |   |
                //   |   |
                //   |   6 ----- 8
                //   |           |
                //   7 --------- 9
                //
                Vector3D[] points = new Vector3D[12];

                Vector3D LAxis = HalfAxis.ToVector3D(LengthAxis);
                Vector3D WAxis = HalfAxis.ToVector3D(WidthAxis);
                Vector3D HAxis = Vector3D.CrossProduct(LAxis, WAxis);
                // bottom
                points[0] = _position;
                points[1] = _position - Th * LAxis - Th * WAxis; 
                points[2] = _position + (W - Th) * LAxis;
                points[3] = _position + (W - Th) * LAxis - Th * WAxis;
                points[4] = _position + (W - Th) * WAxis;
                points[5] = _position - Th * LAxis + (W - Th) * WAxis;
                // top
                for (int i=0; i<6; ++i)
                    points[i+6] = points[i] + Height * HAxis;
                return points;
            }
        }

        public Face[] Faces
        {
            get
            {
                //     
                //    5---4
                //    |   |  
                //    |   |  
                //    |   0 ----- 2
                //    |           |  
                //    1 --------- 3
                // 
                //
                //    11--10
                //    |   |
                //    |   |
                //    |   6 ----- 8
                //    |           |
                //    7 --------- 9
                //
                int[,] indexes = {
                    // side 1
                    {1, 7, 11, 5}       //0
                    , {5, 11, 10, 4}    //1
                    , {4, 10, 6, 0}     //2    
                    , {6, 10, 11, 7}    //7
                    // side 2
                    , {3, 9, 7, 1}      //5
                    , {0, 6, 8, 2}      //3
                    , {2, 8, 9, 3}      //4
                    , {6, 7, 9, 8}      //6
                };

                Face[] faces = new Face[8];
                Vector3D[] points = Points;
                for (int i = 0; i < 8; ++i)
                    faces[i] = new Face(PickId,
                        new Vector3D[] { points[indexes[i, 0]], points[indexes[i, 1]], points[indexes[i, 2]], points[indexes[i, 3]] },
                        Color, Color.Black, "CORNER", true);
                return faces;
            }
        }

        public Vector3D NormalPart1
        {
            get
            {
                Vector3D[] points = Points;
                Vector3D norm = Vector3D.CrossProduct(points[7]-points[1], points[5]-points[1]);
                norm.Normalize();
                return norm;
            }
        }

        public Vector3D NormalPart2
        {
            get
            {
                Vector3D[] points = Points;
                Vector3D norm = Vector3D.CrossProduct(points[3]-points[1], points[7]-points[1]);
                norm.Normalize();
                return norm;
            }
        }

        public double W { get; set; } = 0.0;
        public double Th { get; set; } = 0.0;
        public Color Color { get; set; }
        #endregion

        #region Drawable override
        public override void Draw(Graphics3D graphics)
        {
            foreach (Face face in Faces)
                graphics.AddFace(face);
        }
        public override void DrawBegin(Graphics3D graphics)
        {
            // sanity check
            if (Height <= 1.0E-3) return;

            Face[] faces = Faces;
            if (Vector3D.DotProduct(faces[0].Normal, graphics.ViewDirection) >= 0.0)
            {
                for (int i = 0; i < 4; ++i)
                    graphics.AddFaceBackground(faces[i]);
            }
            if (Vector3D.DotProduct(faces[4].Normal, graphics.ViewDirection) >= 0.0)
            {
                for (int i = 4; i < 8; ++i)
                    graphics.AddFaceBackground(faces[i]); 
            }

            if (Math.Abs(Vector3D.DotProduct(faces[0].Normal, graphics.ViewDirection)) < 1.0E-3)
                graphics.AddFaceBackground(faces[3]);

            if (Math.Abs(Vector3D.DotProduct(faces[4].Normal, graphics.ViewDirection)) < 1.0E-3)
                graphics.AddFaceBackground(faces[7]);
        }
        public override void DrawEnd(Graphics3D graphics)
        {
            // sanity check
            if (Height <= 1.0E-3) return;

            Face[] faces = Faces;
            if (Vector3D.DotProduct(faces[0].Normal, graphics.ViewDirection) <= 0.0)
            {
                for (int i = 0; i < 4; ++i)
                    graphics.AddFace(faces[i]);
            }
            if (Vector3D.DotProduct(faces[4].Normal, graphics.ViewDirection) <= 0.0)
            {
                for (int i = 4; i < 8; ++i)
                    graphics.AddFace(faces[i]); 
            }
            if (Math.Abs(Vector3D.DotProduct(faces[0].Normal, graphics.ViewDirection)) < 1.0E-3)
                graphics.AddFace(faces[3]);

            if (Math.Abs(Vector3D.DotProduct(faces[4].Normal, graphics.ViewDirection)) < 1.0E-3)
                graphics.AddFace(faces[7]);
        }
        #endregion
    }
}
