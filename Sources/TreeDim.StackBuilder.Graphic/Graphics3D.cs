#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

using Sharp3D.Math.Core;
using log4net;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public abstract class Graphics3D
    {
        #region Enums
        enum PaintingAlgorithm  {
            ALGO_PAINTER,
            ALGO_BSPTREE
        }
        public enum FaceDir {
            FRONT
            , BACK
        }
        #endregion

        #region Data members
        /// <summary>
        /// Viewport
        /// </summary>
        private readonly float[] _viewport = new float[4] { -500.0f, -500.0f, 500.0f, 500.0f };
        /// <summary>
        /// Compute viewport automatically if enabled
        /// </summary>
        private bool AutoViewport { get; set; } = true;

        public List<Cyl> Cylinders { get; set; } = new List<Cyl>();
        public List<Segment> Segments { get; set; } = new List<Segment>();
        public List<Segment> SegmentsBackground { get; set; } = new List<Segment>();
        public List<BoxGeneric> Boxes { get; set; } = new List<BoxGeneric>();
        public List<Face> Faces { get; set; } = new List<Face>();
        public List<Triangle> Triangles { get; set; } = new List<Triangle>();
        public List<PalletLabel> PalletLabels { get; set; } = new List<PalletLabel>();
        /// <summary>
        /// image inst
        /// </summary>
        private List<ImageInst> ListImageInst { get; set; } = new List<ImageInst>();
        private List<ImageCached> ListImageCached { get; set; } = new List<ImageCached>();
        /// <summary>
        /// face in the background
        /// </summary>
        private List<Face> _facesBackground = new List<Face>();
        /// <summary>
        /// dimensions cube
        /// </summary>
        private List<DimensionCube> Dimensions { get; set; } = new List<DimensionCube>();
        private const double _cameraDistance = 100000.0;
        private static Vector3D XAxisPrev { get; set; } = Vector3D.Zero;

        public static readonly Vector3D Front = new Vector3D(_cameraDistance, 0.0, 0.0);
        public static readonly Vector3D Back = new Vector3D(-_cameraDistance, 0.0, 0.0);
        public static readonly Vector3D Left = new Vector3D(0.0, -_cameraDistance, 0.0);
        public static readonly Vector3D Right = new Vector3D(0.0, _cameraDistance, 0.0);
        public static readonly Vector3D Top = new Vector3D(0.0, 0.0, _cameraDistance);
        public static readonly Vector3D Corner_0 = new Vector3D(
                Math.Cos(45.0 * Math.PI / 180.0) * Math.Sqrt(2.0) * _cameraDistance
                , Math.Sin(45.0 * Math.PI / 180.0) * Math.Sqrt(2.0) * _cameraDistance
                , _cameraDistance);
        public static readonly Vector3D Corner_90 = new Vector3D(
             Math.Cos(135.0 * Math.PI / 180.0) * Math.Sqrt(2.0) * _cameraDistance
             , Math.Sin(135.0 * Math.PI / 180.0) * Math.Sqrt(2.0) * _cameraDistance
             , _cameraDistance);
        public static readonly Vector3D Corner_180 = new Vector3D(
             Math.Cos(225.0 * Math.PI / 180.0) * Math.Sqrt(2.0) * _cameraDistance
             , Math.Sin(225.0 * Math.PI / 180.0) * Math.Sqrt(2.0) * _cameraDistance
             , _cameraDistance);
        public static readonly Vector3D Corner_270 = new Vector3D(
             Math.Cos(315.0 * Math.PI / 180.0) * Math.Sqrt(2.0) * _cameraDistance
             , Math.Sin(315.0 * Math.PI / 180.0) * Math.Sqrt(2.0) * _cameraDistance
             , _cameraDistance);

        private static ILog _log = LogManager.GetLogger(typeof(Graphics3D));
        #endregion

        #region Constructors
        protected Graphics3D()
        {
            _viewport[0] = -500.0f;
            _viewport[1] = -500.0f;
            _viewport[2] = 500.0f;
            _viewport[3] = 500.0f;

            GridFontSize = 8;
        }
        #endregion

        #region Public properties
        public float FontSizeRatio { get; set; } = 0.015f;
        public float FontSize => null == Size ? 0.0f : FontSizeRatio * Size.Height;
        /// <summary>
        /// Background color
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.White;
        /// <summary>
        /// View point (position of the observer's eye)
        /// </summary>
        public Vector3D CameraPosition { get; set; } = new Vector3D(0.0, 0.0, -1000.0);
        /// <summary>
        /// Target point
        /// </summary>
        public Vector3D Target { get; set; } = Vector3D.Zero;
        /// <summary>
        /// Light direction
        /// </summary>
        public Vector3D VLight
        {
            get
            {
                Vector3D vLight = (CameraPosition + 5000 * Vector3D.ZAxis) - Target;
                vLight.Normalize();
                return vLight;
            }
        }
        /// <summary>
        /// View direction
        /// </summary>
        public Vector3D ViewDirection
        {
            get
            {
                Vector3D viewDir = Target - CameraPosition;
                viewDir.Normalize();
                return viewDir;
            }
        }
        public double MarginPercentage { get; set; } = 0.01;

        /// <summary>
        /// boolean to show box ids
        /// To be used when debugging
        /// </summary>
        public bool ShowBoxIds { get; set; } = false;
        /// <summary>
        /// boolean to show case testures
        /// </summary>
        public bool ShowTextures { get; set; } = true;
        /// <summary>
        ///  boolean to show or hide dimensions
        /// </summary>
        public bool ShowDimensions { get; set; } = true;
        /// <summary>
        /// boolean to show or hide facing
        /// </summary>
        public bool ShowFacing { get; set; } = true;
        /// <summary>
        /// gets or sets boxel order status
        /// If set to true the boxel order will be used when drawing box layers
        /// </summary>
        public bool UseBoxelOrderer { get; set; } = true;
        /// <summary>
        /// sort faces using face comparer (painter algo)
        /// </summary>
        public bool EnableFaceSorting { get; set; } = true;
        /// <summary>
        /// Offset
        /// </summary>
        public Point Offset => TransformPoint(Vector3D.Zero);
        #endregion

        #region Private properties
        internal int GridFontSize { get; set; }
        #endregion

        #region Helpers
        internal Point[] TransformPoint(Transform3D transform, Vector3D[] pts)
        { 
            Point[] points = new Point[pts.Length];
            int i = 0;
            foreach (Vector3D v in pts)
            {
                Vector3D vt = transform.transform(v);
                points[i++] = new Point((int)vt.X, (int)vt.Y);
            }
            return points;        
        }
        internal Point[] TransformPoint(Vector3D[] pts) => TransformPoint(GetCurrentTransformation(), pts);
        internal Point TransformPoint(Transform3D transform, Vector3D pt)
        { 
            Vector3D vt = transform.transform(pt);
            return new Point((int)vt.X, (int)vt.Y);        
        }
        internal Point TransformPoint(Vector3D pt) => TransformPoint(GetCurrentTransformation(), pt);

        public Transform3D GetWorldToEyeTransformation()
        {
            /*
            Orthographic transformation chain
            • Start with coordinates in object’s local coordinates
            • Transform into world coords (modeling transform, Mm)
            • Transform into eye coords (camera xf., Mcam = Fc–1)
            • Orthographic projection, Morth
            • Viewport transform, Mvp

            ps = Mvp*Morth*Mcam*Mm*po
            */
            Vector3D zaxis = CameraPosition - Target;
            zaxis.Normalize();
            Vector3D up = Vector3D.ZAxis;
            Vector3D xaxis = Vector3D.CrossProduct(up, zaxis);
            if (Vector3D.CrossProduct(up, zaxis).GetLengthSquared() < 0.0001)
            {
                xaxis = XAxisPrev.GetLength() > 0 ? XAxisPrev : -Vector3D.XAxis;
                xaxis.Z = 0.0;
            }
            xaxis.Normalize();
            XAxisPrev = xaxis;

            Vector3D yaxis = Vector3D.CrossProduct(zaxis, xaxis);
            Matrix4D Mcam = new Matrix4D(
                    xaxis.X, xaxis.Y, xaxis.Z, -Vector3D.DotProduct(CameraPosition - Target, xaxis),
                    yaxis.X, yaxis.Y, yaxis.Z, -Vector3D.DotProduct(CameraPosition - Target, yaxis),
                    -zaxis.X, -zaxis.Y, -zaxis.Z, -Vector3D.DotProduct(CameraPosition - Target, -zaxis),
                    0.0, 0.0, 0.0, 1.0
                );
            return new Transform3D(Mcam);
        }
        private Transform3D GetPerspectiveTransformation1(double fov, double near, double far)
        {
            // set the basic projection matrix
            double scale = 1.0 / Math.Tan(fov * 0.5 * Math.PI / 180.0);
            Matrix4D Mpers = new Matrix4D(
                scale, 0.0, 0.0, 0.0
                , 0.0, scale, 0.0, 0.0
                , 0.0, 0.0, -far / (far - near), -1.0
                , 0.0, 0.0, -far * near / (far - near), 0.0
                );
            return new Transform3D(Mpers);
        }

        Transform3D GetPerspectiveTransformation2(double persProjWidth, double persProjHeight, double zNear, double zFar, double fov)
        {
            double ar = persProjWidth / persProjHeight;
            double zRange = zNear - zFar;
            double tanHalfFOV = Math.Tan(fov * 0.5 * Math.PI / 180.0);

            Matrix4D m = new Matrix4D
            {
                M11 = 1.0 / (tanHalfFOV * ar),
                M12 = 0.0,
                M13 = 0.0,
                M14 = 0.0,

                M21 = 0.0,
                M22 = 1.0 / tanHalfFOV,
                M23 = 0.0,
                M24 = 0.0,

                M31 = 0.0,
                M32 = 0.0,
                M33 = (-zNear - zFar) / zRange,
                M34 = 2.0 * zFar * zNear / zRange,

                M41 = 0.0,
                M42 = 0.0,
                M43 = 1.0,
                M44 = 0.0
            };
            return new Transform3D(m);
        }

        private Transform3D GetOrthographicProjection(Vector3D vecMin, Vector3D vecMax)
        {
            double[] sizeMin = new double[3];
            sizeMin[0] = 1.0;
            sizeMin[1] = Size.Height;
            sizeMin[2] = 0.0;

            double[] sizeMax = new double[3];
            sizeMax[0] = Size.Width;
            sizeMax[1] = 1.0;
            sizeMax[2] = 1.0;
            return Transform3D.OrthographicProjection(vecMin, vecMax, sizeMin, sizeMax);
        }
        /// <summary>
        /// Background faces
        /// </summary>
        /// <param name="face">Face to be drawn before other faces</param>
        public void AddFaceBackground(Face face)
        {
            _facesBackground.Add(face);
        }
        /// <summary>
        /// add face
        /// </summary>
        /// <param name="face">Face item</param>
        public void AddFace(Face face) => Faces.Add(face);
        public void AddTriangle(Triangle triangle) => Triangles.Add(triangle);
        public void AddBox(BoxGeneric box)
        {
            if (!box.IsValid)
                throw new GraphicsException("Box is invalid and cannot be drawn!");
            Boxes.Add(box);
        }
        public void AddSegmentBackgound(Segment seg) => SegmentsBackground.Add(seg);
        public void AddSegment(Segment seg) => Segments.Add(seg);
        public void AddImage(uint pickId, SubContent content, Vector3D vDimensions, BoxPosition boxPosition)
        {
            ListImageInst.Add(new ImageInst(pickId, content, vDimensions, boxPosition));
        }

        public void AddCylinder(Cyl cylinder) => Cylinders.Add(cylinder);
        public void AddDimensions(DimensionCube dimensionCube)
        {
            Dimensions.Add(dimensionCube);
        }
        public void AddLabel(PalletLabel palletLabel)
        {
            PalletLabels.Add(palletLabel);
        }
        #endregion

        #region Abstract methods and properties
        abstract public Size Size { get; }
        abstract public System.Drawing.Graphics Graphics { get; }

        public Transform3D CurrentTransf { get; set; }
        #endregion

        #region Public methods
        /// <summary>
        /// SetViewport
        /// </summary>
        /// <param name="xmin">xmin -> bottom</param>
        /// <param name="ymin">ymin -> left</param>
        /// <param name="xmax">xmax -> right</param>
        /// <param name="ymax">ymax -> top</param>
        public void SetViewport(float xmin, float ymin, float xmax, float ymax)
        {
            _viewport[0] = xmin;
            _viewport[1] = ymin;
            _viewport[2] = xmax;
            _viewport[3] = ymax;
        }

        public void SetCameraPosition(double distance, double angleHorizDeg, double angleVertDeg)
        {
            double angleHorizRad = angleHorizDeg * Math.PI / 180.0;
            double angleVertRad = angleVertDeg * Math.PI / 180.0;
            CameraPosition = new Vector3D(
                    distance * Math.Cos(angleHorizRad) * Math.Cos(angleVertRad)
                    , distance * Math.Sin(angleHorizRad) * Math.Cos(angleVertRad)
                    , distance * Math.Sin(angleVertRad));
        }
        public static void Reset() { XAxisPrev = Vector3D.Zero; }
        /// <summary>
        /// Draw all entities stored in buffer
        /// </summary>
        public void Flush()
        {
            // initialize
            Vector3D vLight = CameraPosition - Target; vLight.Normalize();
            CurrentTransf = null;
            System.Drawing.Graphics g = Graphics;
            g.Clear(BackgroundColor);

            if (EnableFaceSorting)
            {
                // sort face list
                FaceComparison faceComparer = new FaceComparison(GetWorldToEyeTransformation());
                Faces.Sort(faceComparer);
            }
            // draw background segments
            foreach (Segment s in SegmentsBackground)
                Draw(s);
            // draw background faces
            foreach (Face face in _facesBackground)
                Draw(face, FaceDir.FRONT);
            // draw all faces using solid / transparency
            foreach (Face face in Faces)
                Draw(face, FaceDir.BACK);
            // draw triangles
            foreach (Triangle tr in Triangles)
                Draw(tr, FaceDir.FRONT);

            var boxesImage1 = new List<BoxGeneric>();
            foreach (var imageInst in ListImageInst)
            {
                var b = imageInst.ToBox();
                boxesImage1.Add(b);
            }
            // sort box list
            if (UseBoxelOrderer)
            {
                BoxelOrderer boxelOrderer = new BoxelOrderer(Boxes, ViewDirection);
                Boxes = boxelOrderer.GetSortedList();

                var boxelOrderer2 = new BoxelOrderer(boxesImage1, ViewDirection);
                boxesImage1 = boxelOrderer2.GetSortedList();

            }
            else
                Boxes.Sort(new DrawableComparerSimplifiedPainterAlgo(GetWorldToEyeTransformation()));

            // sort cylinder list
            Cylinders.Sort(new DrawableComparerSimplifiedPainterAlgo(GetWorldToEyeTransformation()));

            if (Cylinders.Count > 0)
            {
                // sort by Z
                var drawableList = new List<Drawable>();
                drawableList.AddRange(Boxes);
                drawableList.AddRange(Cylinders);
                drawableList.Sort(new DrawableComparerSimplifiedPainterAlgo(GetWorldToEyeTransformation()));

                var boxes = new List<BoxGeneric>();
                var cylinders = new List<Cyl>();
                var processingBox = drawableList[0] is Box;
                foreach (Drawable drawable in drawableList)
                {
                    if ((drawable is Box box) && processingBox)
                        boxes.Add(box);
                    else if (drawable is Cyl cyl && !processingBox)
                        cylinders.Add(cyl);
                    else
                    {
                        if (boxes.Count > 0)
                        {
                            var boxelOrderer = new BoxelOrderer(boxes, ViewDirection);
                            boxes = boxelOrderer.GetSortedList();
                            // draw boxes
                            foreach (var bb in boxes)
                                bb.Draw(this);
                            // clear
                            boxes.Clear();
                        }
                        if (cylinders.Count > 0)
                        {
                            cylinders.Sort(new DrawableComparerSimplifiedPainterAlgo(GetWorldToEyeTransformation()));
                            // draw cylinders
                            foreach (Cyl cc in cylinders)
                                cc.Draw(this);
                            // clear
                            cylinders.Clear();
                        }
                        if (drawable is Box b)
                        {
                            boxes.Add(b);
                            processingBox = true;
                        }
                        else if (drawable is Cyl c)
                        {
                            cylinders.Add(c);
                            processingBox = false;
                        }
                    }
                }
                // remaining boxes
                var boxelOrdererRem = new BoxelOrderer(boxes, ViewDirection);
                boxes = boxelOrdererRem.GetSortedList();
                // draw boxes
                foreach (var box in boxes)
                    box.Draw(this);
                // remaining cylinders
                cylinders.Sort(new DrawableComparerSimplifiedPainterAlgo(GetWorldToEyeTransformation()));
                // draw cylinders
                foreach (var cyl in cylinders)
                    cyl.Draw(this);
                // clear
                boxes.Clear();
            }
            else
            {
                // draw all boxes
                foreach (var box in Boxes)
                    box.Draw(this);
                // draw all triangles
                foreach (Triangle tr in Triangles)
                    Draw(tr, FaceDir.FRONT);
            }
            // images inst
            if (ListImageInst.Count > 0)
            {
                // --- sort image inst
                List<ImageInst> listImageInstSorted = new List<ImageInst>();
                foreach (Box b in boxesImage1)
                {
                    var imageInst = ListImageInst.Find(i => i.PickId == b.PickId);
                    listImageInstSorted.Add(new ImageInst(b.PickId, imageInst.Content, b.Dimensions, b.BoxPosition));
                }
                
                // draw image inst
                foreach (ImageInst im in listImageInstSorted)
                    Draw(im);
                ListImageInst.Clear();
            }
            // draw faces : end
            foreach (Face face in Faces)
                Draw(face, FaceDir.FRONT);
            // draw segment list (e.g. hatching)
            foreach (Segment seg in Segments)
                Draw(seg);
            foreach (var label in PalletLabels)
                Draw(label.Face, FaceDir.FRONT);
            // draw cotation cubes
            if (ShowDimensions)
            {
                foreach (var qc in Dimensions)
                    qc.Draw(this);
            }
        }

        public Transform3D GetCurrentTransformation()
        {
            if (null == CurrentTransf)
            {
                // get transformations
                Transform3D world2eye = GetWorldToEyeTransformation();
                Transform3D orthographicProj = GetOrthographicProjection(
                    new Vector3D(_viewport[0], _viewport[1], -10000.0)
                    , new Vector3D(_viewport[2], _viewport[3], 10000.0));

                // build automatic viewport
                if (AutoViewport)
                {
                    BBox3D bbox = new BBox3D();
                    bbox.Reset();
                    // boxes
                    foreach (var box in Boxes)
                       bbox.Extend( box.GetBBox(world2eye) );
                    // images
                    foreach (var img in ListImageInst)
                        bbox.Extend(img.GetBBox(world2eye));
                    // triangles
                    foreach (var tr in Triangles)
                        foreach (Vector3D pt in tr.Points)
                            bbox.Extend(world2eye.transform(pt));
                    // cylinders
                    foreach (var cyl in Cylinders)
                        bbox.Extend(cyl.GetBBox(world2eye));
                    // faces
                    foreach (Face face in Faces)
                        foreach (Vector3D pt in face.Points)
                            bbox.Extend( world2eye.transform(pt) );
                    // segments
                    foreach (var seg in Segments)
                        foreach (var pt in seg.Points)
                            bbox.Extend( world2eye.transform(pt) );
                    // cube dimensions
                    foreach (DimensionCube dimCube in Dimensions)
                        foreach (Vector3D pt in dimCube.DrawingPoints(this))
                           bbox.Extend( world2eye.transform(pt) );
                    // image inst
                    foreach (var imageInst in ListImageInst)
                        bbox.Extend(world2eye.transform(imageInst.PointBase));

                    if (bbox.IsValid)
                    {
                        Vector3D vecMin1 = bbox.PtMin, vecMax1 = bbox.PtMax;
                        // adjust width/height
                        if ((bbox.PtMax.Y - bbox.PtMin.Y) / Size.Height > (bbox.PtMax.X - bbox.PtMin.X) / Size.Width)
                        {
                            double actualWidth = (bbox.PtMax.Y - bbox.PtMin.Y) * Size.Width / Size.Height;
                            vecMin1.X = 0.5 * (bbox.PtMin.X + bbox.PtMax.X) - 0.5 * actualWidth;
                            vecMax1.X = 0.5 * (bbox.PtMin.X + bbox.PtMax.X) + 0.5 * actualWidth;
                        }
                        else
                        {
                            double actualHeight = (bbox.PtMax.X - bbox.PtMin.X) * Size.Height / Size.Width;
                            vecMin1.Y = 0.5 * (bbox.PtMin.Y + bbox.PtMax.Y) - 0.5 * actualHeight;
                            vecMax1.Y = 0.5 * (bbox.PtMin.Y + bbox.PtMax.Y) + 0.5 * actualHeight;
                        }
                        // set margins
                        double width = vecMax1.X - vecMin1.X;
                        vecMin1.X -= MarginPercentage * width;
                        vecMax1.X += MarginPercentage * width;
                        double height = vecMax1.Y - vecMin1.Y;
                        vecMin1.Y -= MarginPercentage * height;
                        vecMax1.Y += MarginPercentage * height;

                        orthographicProj = GetOrthographicProjection(vecMin1, vecMax1);
                    }
                    else
                        _log.Warn("Graphics3D.GetCurrentTransformation() -> BBox invalid : can not compute viewport automatically");
                }
                CurrentTransf = orthographicProj * world2eye;
            }
            return CurrentTransf;
        }
        public Transform3D GetCurrentTransformation(List<Vector3D> points)
        {
            if (null == CurrentTransf)
            {
                // get transformations
                var world2eye = GetWorldToEyeTransformation();
                var orthographicProj = GetOrthographicProjection(
                    new Vector3D(_viewport[0], _viewport[1], -10000.0)
                    , new Vector3D(_viewport[2], _viewport[3], 10000.0));

                // build automatic viewport
                if (AutoViewport)
                {
                    Vector3D vecMin = new Vector3D(double.MaxValue, double.MaxValue, double.MaxValue);
                    Vector3D vecMax = new Vector3D(double.MinValue, double.MinValue, double.MinValue);

                    foreach (Vector3D pt in points)
                    {
                        Vector3D ptT = world2eye.transform(pt);
                        vecMin.X = Math.Min(vecMin.X, ptT.X);
                        vecMin.Y = Math.Min(vecMin.Y, ptT.Y);
                        vecMin.Z = Math.Min(vecMin.Z, ptT.Z);
                        vecMax.X = Math.Max(vecMax.X, ptT.X);
                        vecMax.Y = Math.Max(vecMax.Y, ptT.Y);
                        vecMax.Z = Math.Max(vecMax.Z, ptT.Z);
                    }
                    Vector3D vecMin1 = vecMin, vecMax1 = vecMax;
                    // adjust width/height
                    if ((vecMax.Y - vecMin.Y) / Size.Height > (vecMax.X - vecMin.X) / Size.Width)
                    {
                        double actualWidth = (vecMax.Y - vecMin.Y) * Size.Width / Size.Height;
                        vecMin1.X = 0.5 * (vecMin.X + vecMax.X) - 0.5 * actualWidth;
                        vecMax1.X = 0.5 * (vecMin.X + vecMax.X) + 0.5 * actualWidth;
                    }
                    else
                    {
                        double actualHeight = (vecMax.X - vecMin.X) * Size.Height / Size.Width;
                        vecMin1.Y = 0.5 * (vecMin.Y + vecMax.Y) - 0.5 * actualHeight;
                        vecMax1.Y = 0.5 * (vecMin.Y + vecMax.Y) + 0.5 * actualHeight;
                    }
                    // set margins
                    double width = vecMax1.X - vecMin1.X;
                    vecMin1.X -= MarginPercentage * width;
                    vecMax1.X += MarginPercentage * width;
                    double height = vecMax1.Y - vecMin1.Y;
                    vecMin1.Y -= MarginPercentage * height;
                    vecMax1.Y += MarginPercentage * height;

                    orthographicProj = GetOrthographicProjection(vecMin1, vecMax1);
                }
                CurrentTransf = orthographicProj * world2eye;
            }
            return CurrentTransf;
        }


        #endregion

        #region Drawing methods
        /// <summary>
        /// Draw a line segment
        /// </summary>
        /// <param name="seg">Segment object to be drawn</param>
        internal void Draw(Segment seg)
        {
            System.Drawing.Graphics g = Graphics;
            Brush brush = new SolidBrush(seg.Color);
            Pen pen = new Pen(brush);
            Point[] pt = TransformPoint(seg.Points);
            g.DrawLine(pen, pt[0], pt[1]);
        }
        /// <summary>
        /// Draw a text string at a 3D location
        /// </summary>
        /// <param name="text">Text to draw</param>
        /// <param name="position">3D point on which to center the text string</param>
        /// <param name="color">Color of solid brush used to draw text</param>
        /// <param name="fontSize">Size of font used to draw text</param>
        internal void Draw(string text, Vector3D position, Color color, float fontSize)
        {
            System.Drawing.Graphics g = Graphics;
            Point pt = TransformPoint(position);
            Font font = new Font("Arial", fontSize > 1.0f ? fontSize : 1.0f);
            SizeF sizeF = g.MeasureString(text, font);
            g.DrawString(text
                , font
                , new SolidBrush(color)
                , new Point(pt.X - (int)(0.5f * sizeF.Width), pt.Y - (int)(0.5f * sizeF.Height))
                , StringFormat.GenericDefault);
        }

        internal void Draw(Triangle tr, FaceDir dir)
        {
            System.Drawing.Graphics g = Graphics;

            // test if triangle can actually be seen
            if ((Vector3D.DotProduct(tr.Normal, CameraPosition - Target) > 0.0 && dir == FaceDir.BACK)
                || (Vector3D.DotProduct(tr.Normal, CameraPosition - Target) < 0.0 && dir == FaceDir.FRONT))
                return;
            // compute triangle color
            double cosA = Math.Abs(Vector3D.DotProduct(tr.Normal, VLight));
            Color color = Color.FromArgb(
                tr.IsSolid ? 255 : (dir == FaceDir.FRONT ? 64 : 255)
                , (int)(tr.ColorFill.R * cosA)
                , (int)(tr.ColorFill.G * cosA)
                , (int)(tr.ColorFill.B * cosA));
            Brush brush = new SolidBrush(color);
            // draw filled triangle
            Point[] pt = TransformPoint(tr.Points);
            g.FillPolygon(brush, pt);
            // draw path
            Brush brush0 = new SolidBrush(tr.ColorPath);
            Pen pen0 = new Pen(brush0, 0);
            for (int i = 1; i < pt.Length; ++i)
            {
                if (tr.DrawPath[i-1])
                    g.DrawLine(pen0, pt[i - 1], pt[i]);
            }
            if (tr.DrawPath[2])
                g.DrawLine(pen0, pt[pt.Length - 1], pt[0]);
        }
        /// <summary>
        /// Draw a face
        /// </summary>
        /// <param name="face">Face object to be drawn</param>
        internal void Draw(Face face, FaceDir dir)
        {
            System.Drawing.Graphics g = Graphics;

            // test if face can actually be seen
            if ((Vector3D.DotProduct(face.Normal, ViewDirection) < 0.0 && dir == FaceDir.BACK)
                || (Vector3D.DotProduct(face.Normal, ViewDirection) > 0.0 && dir == FaceDir.FRONT))
                return;

            // compute face color
            double cosA = Math.Abs(Vector3D.DotProduct(face.Normal, VLight));
            Color color = Color.FromArgb(
                face.IsSolid ? 255 : (dir == FaceDir.FRONT ? 64 : 255)
                , (int)(face.ColorFill.R * cosA)
                , (int)(face.ColorFill.G * cosA)
                , (int)(face.ColorFill.B * cosA));
            Point[] pt = TransformPoint(face.Points);

            Brush brush = new SolidBrush(color);
            g.FillPolygon(brush, pt);
            // draw images
            foreach (Texture texture in face.Textures)
            {
                Point[] ptsImage = TransformPoint(face.PointsImage(texture));
                Point[] pts = new Point[3];
                pts[0] = ptsImage[2];
                pts[1] = ptsImage[3];
                pts[2] = ptsImage[1];
                g.DrawImage(texture.Bitmap, pts);
            }
            // draw path
            Brush brush0 = new SolidBrush(face.ColorPath);
            int ptCount = pt.Length;
            for (int i = 1; i < ptCount; ++i)
            {
                // there is a bug here!
                // -> a polygon that result from first split will lose all edges
                // when split a second time
                g.DrawLine(new Pen(brush0, 1.5f), pt[i - 1], pt[i]);
            }
            g.DrawLine(new Pen(brush0, 1.5f), pt[ptCount - 1], pt[0]);
        }
        internal void Draw(Face face, FaceDir dir, Color colorApply, bool transparent)
        {
            System.Drawing.Graphics g = Graphics;

            // test if face can actually be seen
            if ((Vector3D.DotProduct(face.Normal, ViewDirection) < 0.0 && dir == FaceDir.BACK)
                || (Vector3D.DotProduct(face.Normal, ViewDirection) > 0.0 && dir == FaceDir.FRONT))
                return;

            // compute face color
            double cosA = Math.Abs(Vector3D.DotProduct(face.Normal, VLight));
            var color = Color.FromArgb(
                transparent ? 64 : 255
                , (int)(colorApply.R * cosA)
                , (int)(colorApply.G * cosA)
                , (int)(colorApply.B * cosA));
            var pt = TransformPoint(face.Points);

            Brush brush = new SolidBrush(color);
            g.FillPolygon(brush, pt);
            // draw path
            float fThickness = transparent ? 2.0f : 1.0f;
            Brush brush0 = new SolidBrush(face.ColorPath);
            for (int i = 1; i < pt.Length; ++i)
            {
                // there is a bug here!
                // -> a polygon that result from first split will lose all edges
                // when split a second time
                g.DrawLine(new Pen(brush0, fThickness), pt[i - 1], pt[i]);
            }
            g.DrawLine(new Pen(brush0, fThickness), pt[pt.Length - 1], pt[0]);
        }
        internal void Draw(ImageInst img)
        {
            try
            {
                Point ptImg = TransformPoint(img.PointBase);

                Bitmap bmp = null;
                Point ptOffset = Point.Empty;
                GetCachedBitmap(img, ref bmp, ref ptOffset);

                if (ptImg.X > 0 || ptImg.Y > 0)
                {
                    System.Drawing.Graphics g = Graphics;
                    g.DrawImage(bmp, new Point(ptImg.X - ptOffset.X, ptImg.Y - ptOffset.Y));
                }
                else
                {
                    _log.Error("Invalid image position");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        internal void GetCachedBitmap(ImageInst img, ref Bitmap bmp, ref Point offset)
        {
            var imgCached = ListImageCached.Find(delegate(ImageCached imgc) { return imgc.Matches(img); });
            if (null == imgCached)
            {
                imgCached = new ImageCached(img.Content, img.AxisLength, img.AxisWidth);
                ListImageCached.Add(imgCached);
            }
            // *** get size in pixels
            int xmin = int.MaxValue, ymin = int.MaxValue, xmax = int.MinValue, ymax=int.MinValue;
            BBox3D bbox = imgCached.Content.Bbox;
            foreach (var vPt in bbox.Corners)
            {
                var pt = TransformPoint(GetCurrentTransformation() * imgCached.RelativeTransf, vPt);
                xmin = Math.Min(xmin, pt.X);
                xmax = Math.Max(xmax, pt.X);
                ymin = Math.Min(ymin, pt.Y);
                ymax = Math.Max(ymax, pt.Y);
            }
            Size s = new Size(xmax-xmin + 1, ymax-ymin + 1);
            // ***
            bmp = imgCached.Image(s, CameraPosition, Target, ref offset);
        }
        #endregion
    }
}
