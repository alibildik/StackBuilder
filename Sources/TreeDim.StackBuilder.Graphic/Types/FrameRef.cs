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
    public class FrameRef : Drawable
    {
        #region Constructor
        public FrameRef(uint pickId) : base(pickId)
        { 
        }
        #endregion
        #region Public properties
        public double Length { get; set; } = 100.0;
        public Vector3D Position { get; set; }
        public HalfAxis.HAxis LengthAxis { get; set; } = HalfAxis.HAxis.AXIS_X_P;
        public HalfAxis.HAxis WidthAxis { get; set; } = HalfAxis.HAxis.AXIS_Y_P;
        #endregion
        #region Override Drawable
        public override Vector3D[] Points
        {
            get
            {
                var points = new Vector3D[4];
                points[0] = Position;
                points[1] = Position + 2*Length * HalfAxis.ToVector3D(LengthAxis);
                points[2] = Position + 2*Length * HalfAxis.ToVector3D(WidthAxis);
                points[3] = Position + 2*Length * Vector3D.CrossProduct(HalfAxis.ToVector3D(LengthAxis), HalfAxis.ToVector3D(WidthAxis));
                return points;
            }
        }
        public override void Draw(Graphics3D graphics)
        {
            var pts = Points;
            graphics.Draw(new Segment(pts[0], pts[1], Color.Red));
            graphics.Draw("X", pts[1], Color.Red, graphics.FontSize);
            graphics.Draw(new Segment(pts[0], pts[2], Color.Green));
            graphics.Draw("Y", pts[2], Color.Green, graphics.FontSize);
            graphics.Draw(new Segment(pts[0], pts[3], Color.Blue));
            graphics.Draw("Z", pts[3], Color.Blue, graphics.FontSize);
        }
        #endregion
    }
}
