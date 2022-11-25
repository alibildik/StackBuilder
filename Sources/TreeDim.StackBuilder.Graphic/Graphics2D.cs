#region Data members
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public abstract class Graphics2D
    {
        #region Abstract methods and properties
        abstract public System.Drawing.Graphics Graphics { get; }
        abstract public Size Size { get; }
        #endregion
        #region Public methods
        /// <summary>
        /// Background color
        /// </summary>
        public void Clear(Color color)
        {
            ColorBackground = color;
            Graphics.Clear(ColorBackground);
        }
        /// <summary>
        /// SetViewport
        /// </summary>
        /// <param name="xmin">xmin -> bottom</param>
        /// <param name="ymin">ymin -> left</param>
        /// <param name="xmax">xmax -> right</param>
        /// <param name="ymax">ymax -> top</param>
        public void SetViewport(float xmin, float ymin, float xmax, float ymax)
        {
            if (ViewportRatio > AspectRatio)
            {   // pallet length
                float marginX = MarginRatio * (xmax - xmin) / NumberOfViews;
                Viewport[0] = -marginX;
                Viewport[1] = -marginX / AspectRatio;
                Viewport[2] = (xmax - xmin) / NumberOfViews + marginX;
                Viewport[3] = ((xmax - xmin) / NumberOfViews + marginX) / AspectRatio;
            }
            else
            {   // pallet width
                float margin = 0.1f * (xmax - xmin) / NumberOfViews;
                Viewport[0] = -margin * AspectRatio;
                Viewport[1] = -margin;
                Viewport[2] = (ymax - ymin + margin) * AspectRatio;
                Viewport[3] = ymax - ymin + margin;
            }

            Viewport[0] = xmin;
            Viewport[1] = ymin;
            Viewport[2] = xmax;
            Viewport[3] = ymax;
        }
        public void SetViewport(Vector2D ptMin, Vector2D ptMax) => SetViewport((float)ptMin.X, (float)ptMin.Y, (float)ptMax.X, (float)ptMax.Y);
        public void SetCurrentView(uint iIndexView)
        {
            IIndexView = iIndexView;
        }
        #endregion
        #region Drawing methods
        public void DrawBoxSelected(Box box)
        {
            Brush brushPath = new SolidBrush(Color.Red);
            Pen penPath = new Pen(brushPath);

            // get points
            Point[] pt = TransformPoint(box.TopFace.Points);
            Graphics.DrawPolygon(penPath, pt);
        }
        public void DrawRectangle(Vector2D vMin, Vector2D vMax, Color penColor)
        {
            Point[] pt = TransformPoint(new Vector2D[] { vMin, vMax });
            System.Drawing.Graphics g = Graphics;
            Pen penRect = new Pen(penColor);
            g.DrawRectangle(penRect, Math.Min(pt[0].X, pt[1].X), Math.Min(pt[0].Y, pt[1].Y), Math.Abs(pt[1].X - pt[0].X), Math.Abs(pt[1].Y - pt[0].Y));
        }
        public void DrawLine(Vector2D v1, Vector2D v2, Color penColor, float penWidth = 1.0f)
        {
            Point[] pt = TransformPoint(new Vector2D[] { v1, v2 });
            System.Drawing.Graphics g = Graphics;
            g.DrawLines(new Pen(penColor, penWidth), pt);
        }
        public void DrawContour(Vector2D[] v, Color penColor, float penWidth = 1.0f)
        {
            Point[] pt = TransformPoint(v);
            System.Drawing.Graphics g = Graphics;
            g.DrawLines(new Pen(penColor, penWidth), pt);        
        }
        public void DrawArrow(Vector2D v, int iDir, int length, int baseDistance, int radius, Color color, out Rectangle rectButton)
        {
            Pen pen = new Pen(color, 5)
            {
                StartCap = LineCap.Flat,
                EndCap = LineCap.ArrowAnchor
            };
            Point[] pt = TransformPoint(new Vector2D[] { v });
            Point pt0 = pt[0], pt1 = pt[0];
            switch (iDir)
            {
                case 0: pt0.Offset(baseDistance, 0); pt1.Offset(length, 0); break;
                case 1: pt0.Offset(0, -baseDistance); pt1.Offset(0, -length); break;
                case 2: pt0.Offset(-baseDistance, 0); pt1.Offset(-length, 0); break;
                case 3: pt0.Offset(0, baseDistance); pt1.Offset(0, length); break;
                default: break;
            }
            Graphics.DrawLine(pen, pt0, pt1);

            Point ptMiddle = new Point((pt0.X + pt1.X) / 2 - radius, (pt0.Y + pt1.Y) / 2 - radius);
            rectButton = new Rectangle(ptMiddle, new Size(2 * radius, 2 * radius));

            Brush brushSolid = new SolidBrush(color);
            Graphics.FillEllipse(brushSolid, rectButton);
        }
        public void DrawArcArrow(Vector2D v, int length, int radius, Color color, out Rectangle rectButton)
        {
            Pen pen = new Pen(color, 5)
            {
                StartCap = LineCap.Flat,
                EndCap = LineCap.ArrowAnchor
            };
            Point[] pt = TransformPoint(new Vector2D[] { v });

            Graphics.DrawArc(pen, pt[0].X - length, pt[0].Y - length, 2 * length, 2 * length, -10.0F, -80.0F);

            Point ptMiddle = new Point(pt[0].X + (int)(length * Math.Cos(0.25 * Math.PI)) - radius, pt[0].Y - (int)(length * Math.Sin(0.25 * Math.PI)) - radius);
            rectButton = new Rectangle(ptMiddle, new Size(2 * radius, 2 * radius));

            Brush brushSolid = new SolidBrush(color);
            Graphics.FillEllipse(brushSolid, rectButton);
        }
        public void DrawArcArrow180(Vector2D v, int length, int radius, Color color, out Rectangle rectButton)
        {
            Pen pen = new Pen(color, 5)
            {
                StartCap = LineCap.Flat,
                EndCap = LineCap.ArrowAnchor
            };
            Point[] pt = TransformPoint(new Vector2D[] { v });

            Graphics.DrawArc(pen, pt[0].X - length, pt[0].Y - length, 2 * length, 2 * length, -10.0F, -170.0F);

            Point ptMiddle = new Point(pt[0].X + (int)(length * Math.Cos(0.25 * Math.PI)) - radius, pt[0].Y - (int)(length * Math.Sin(0.25 * Math.PI)) - radius);
            rectButton = new Rectangle(ptMiddle, new Size(2 * radius, 2 * radius));

            Brush brushSolid = new SolidBrush(color);
            Graphics.FillEllipse(brushSolid, rectButton);
        }

        public void DrawText(string sText, int size, Vector2D v, Color color, TexpPos textPos = TexpPos.TEXT_CENTER)
        {
            Point[] pt = TransformPoint(new Vector2D[] { v });
            Point pt0 = pt[0];
            Font font = new Font("Arial", size);
            SizeF sizeString = Graphics.MeasureString(sText, font);
            switch (textPos)
            {
                case TexpPos.TEXT_CENTER:
                    pt0.X -= (int)sizeString.Width / 2;
                    pt0.Y -= (int)sizeString.Height / 2;
                    break;
                case TexpPos.TEXT_BOTTOMRIGHT:
                    pt0.X -= (int)sizeString.Width;
                    pt0.Y -= (int)sizeString.Height;
                    break;
                default: break;
            }

            Graphics.DrawString(sText, font, new SolidBrush(color), pt0);
        }
        public void DrawText(string sText, int size)
        {
            Graphics.DrawString(sText, new Font("Arial", size), new SolidBrush(Color.Red), new PointF(5, Size.Height - 20));
        }
        #endregion
        #region Public properties
        public uint NumberOfViews { get; set; } = 1;
        public float[] Viewport = new float[4];
        public uint IIndexView { get; set; } = 0;
        public Color ColorBackground { get; private set; } = Color.White;
        #endregion
        #region Private helpers
        private float AspectRatio => Size.Width / ((float)Size.Height * NumberOfViews);
        private float ViewportRatio => (Viewport[2] - Viewport[0]) / (Viewport[3] - Viewport[1]);
        private float MarginX => MarginRatio * (Viewport[2] - Viewport[0]);
        private float MarginY => MarginRatio * (Viewport[3] - Viewport[1]);
        public float MarginRatio { get; set; } = 0.1f;
        public Vector2D ReverseTransform(Point pt)
        {
            double VPSpanX = Viewport[2] - Viewport[0] + 2 * MarginX;
            double VPSpanY = Viewport[3] - Viewport[1] + 2 * MarginY;
            double VRatio = ViewportRatio / AspectRatio;

            if (VRatio >= 1)
            {
                return new Vector2D(
                    Viewport[0] - MarginX + VPSpanX * (NumberOfViews * ((double)pt.X / Size.Width) - IIndexView),
                    Viewport[3] + MarginY - pt.Y  * VRatio * VPSpanY / Size.Height
                    );
            }
            else
            {
                return new Vector2D(
                    Viewport[0] - MarginX + (VPSpanX/VRatio) * ((pt.X/(double)Size.Width) * NumberOfViews - IIndexView),
                    Viewport[3] + MarginY - (pt.Y / (double)Size.Height) * VPSpanY
                    );
            }
        }
        private Point[] TransformPoint(Vector2D[] points2d)
        {
            double VPSpanX = Viewport[2] - Viewport[0] + 2 * MarginX;
            double VPSpanY = Viewport[3] - Viewport[1] + 2 * MarginY;
            double VRatio = ViewportRatio / AspectRatio;

            Point[] points = new Point[points2d.Length];
            int i = 0;

            foreach (Vector2D v in points2d)
            {
                if (VRatio >= 1)
                {
                    points[i].X = (int) (
                        (float)(Size.Width / NumberOfViews) * (IIndexView + (v.X - Viewport[0] + MarginX) / VPSpanX)
                        );
                    points[i].Y = (int) (
                        (Size.Height / VRatio) * (Viewport[3] - v.Y + MarginY) / VPSpanY
                        );
                }
                else
                {
                    points[i].X = (int) (
                        (float)(Size.Width / NumberOfViews) * (IIndexView + VRatio * (v.X - Viewport[0] + MarginX) / VPSpanX)
                        );
                    points[i].Y = (int) (
                        Size.Height * (Viewport[3] - v.Y + MarginY) / VPSpanY
                        );
                }
                ++i;
            }
            return points;
        }
        public Point[] TransformPoint(Vector3D[] points3d)
        {
            double VPSpanX = Viewport[2] - Viewport[0] + 2 * MarginX;
            double VPSpanY = Viewport[3] - Viewport[1] + 2 * MarginY;
            double VRatio = ViewportRatio / AspectRatio;

            Point[] points = new Point[points3d.Length];
            int i = 0;
            foreach (Vector3D v in points3d)
            {
                if (ViewportRatio > AspectRatio)
                {
                    points[i].X = (int)(
                        (float)(Size.Width / NumberOfViews) * (IIndexView + (v.X - Viewport[0] + MarginX) / VPSpanX)
                        );
                    points[i].Y = (int)(
                        (Size.Height / VRatio) * (Viewport[3] - v.Y + MarginY) / VPSpanY
                        );
                }
                else
                {
                    points[i].X = (int)(
                        (float)(Size.Width / NumberOfViews) * (IIndexView + VRatio * (v.X - Viewport[0] + MarginX) / VPSpanX)
                        );
                    points[i].Y = (int)(
                        Size.Height * (Viewport[3] - v.Y + MarginY) / VPSpanY
                        );
                }
                ++i;
            }
            return points;
        }
        #endregion
        #region Data members
        public int SelectedItem { get; set; } = -1;
        #endregion
        #region Enums
        public enum TexpPos { TEXT_CENTER, TEXT_BOTTOMLEFT, TEXT_BOTTOMRIGHT, TEXT_TOPLEFT, TEXT_TOPRIGHT };
        #endregion

    }
}
