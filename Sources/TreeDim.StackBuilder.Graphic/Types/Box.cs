#region Using directives
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Sharp3D.Math.Core;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    #region Box
    /// <summary>
    /// This class used to draw any brick with side parallel to axes and normal oriented to exterior
    /// Use method Graphics.Draw(Box box) to draw as ordered boxel
    /// </summary>
    public class Box : BoxGeneric
    {
        #region Constructors
        public Box(uint pickId, double length, double width, double height)
            : base(pickId, length, width, height, BoxPosition.Zero)
        {
            // colors & texturezs
            Colors = new Color[6];
            Colors[0] = Color.Red;
            Colors[1] = Color.Red;
            Colors[2] = Color.Green;
            Colors[3] = Color.Green;
            Colors[4] = Color.Blue;
            Colors[5] = Color.Blue;

            for (int i = 0; i < 6; ++i)
                TextureLists[i] = null;
        }
        public Box(uint pickId, double length, double width, double height, BoxPosition boxPosition)
            : base(pickId, length, width, height, boxPosition)
        {
            // colors & textures
            Colors = new Color[6];
            Colors[0] = Color.Red;
            Colors[1] = Color.Red;
            Colors[2] = Color.Green;
            Colors[3] = Color.Green;
            Colors[4] = Color.Blue;
            Colors[5] = Color.Blue;

            for (int i = 0; i < 6; ++i)
                TextureLists[i] = null;

        }
        public Box(uint pickId, PackableBrick packable)
            : base(pickId, packable.Length, packable.Width, packable.Height, BoxPosition.Zero)
        {
            // colors & textures
            Colors = Enumerable.Repeat(Color.Chocolate, 6).ToArray();

            if (!(packable is LoadedPallet))
            {
                var bProperties = PackableToBProperties(packable);
                if (null != bProperties)
                {
                    Colors = bProperties.Colors;
                    // IsBundle ?
                    IsBundle = bProperties.IsBundle;
                    if (IsBundle)
                        BundleFlats = (bProperties as BundleProperties).NoFlats;
                    // textures
                    if (bProperties is BoxProperties boxProperties)
                    {
                        List<Pair<HalfAxis.HAxis, Texture>> textures = boxProperties.TextureList;
                        foreach (Pair<HalfAxis.HAxis, Texture> tex in textures)
                        {
                            int iIndex = (int)tex.first;
                            if (null == TextureLists[iIndex])
                                TextureLists[iIndex] = new List<Texture>();
                            TextureLists[iIndex].Add(tex.second);
                        }
                        // tape
                        TapeWidth = boxProperties.TapeWidth;
                        TapeColor = boxProperties.TapeColor;
                    }
                }
                if (packable is PackableBrickNamed packableBrickNamed)
                    StrapperList = new List<PalletStrapper>(packableBrickNamed.StrapperSet.Strappers);
            }
        }
        public Box(uint pickId, PalletCapProperties capProperties, Vector3D position)
            : base(pickId, capProperties.Length, capProperties.Width, capProperties.Height, new BoxPosition(position, HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P))
        {
            // colors & textures
            Colors = new Color[6];
            SetAllFacesColor(capProperties.Color);
        }
        public Box(uint pickId, PackableBrick packable, BoxPosition bPosition)
            : base(pickId, packable.Length, packable.Width, packable.Height, bPosition)
        {
            // colors
            Colors = Enumerable.Repeat(Color.Chocolate, 6).ToArray();
            if (packable is PackProperties)
            {
            }
            else
            {
                BProperties bProperties = PackableToBProperties(packable);
                if (null == bProperties)
                    throw new Exception(string.Format("Type {0} cannot be handled by Box constructor", packable.GetType().ToString()));

                Colors = bProperties.Colors;
                IsBundle = bProperties.IsBundle;
                // is box ?
                if (bProperties is BoxProperties boxProperties)
                {
                    List<Pair<HalfAxis.HAxis, Texture>> textures = boxProperties.TextureList;
                    foreach (Pair<HalfAxis.HAxis, Texture> tex in textures)
                    {
                        int iIndex = (int)tex.first;
                        if (null == TextureLists[iIndex])
                            TextureLists[iIndex] = new List<Texture>();
                        TextureLists[iIndex].Add(tex.second);
                    }
                    TapeWidth = boxProperties.TapeWidth;
                    TapeColor = boxProperties.TapeColor;
                }
                // is bundle ?
                else if (bProperties.IsBundle)
                {
                    if (bProperties is BundleProperties bundleProp)
                        BundleFlats = bundleProp.NoFlats;
                }
            }
            if (packable is PackableBrickNamed packableBrickNamed)
                StrapperList = new List<PalletStrapper>(packableBrickNamed.StrapperSet.Strappers);
        }
        public Box(uint pickId, InterlayerProperties interlayerProperties)
            : base(pickId, interlayerProperties.Length, interlayerProperties.Width, interlayerProperties.Thickness, BoxPosition.Zero)
        {
            // colors
            Colors = Enumerable.Repeat(interlayerProperties.Color, 6).ToArray();
        }
        public Box(uint pickId, InterlayerProperties interlayerProperties, BoxPosition bPosition)
            : base(pickId, interlayerProperties.Length, interlayerProperties.Width, interlayerProperties.Thickness, BoxPosition.Zero)
        {
            // colors
            Colors = Enumerable.Repeat(interlayerProperties.Color, 6).ToArray();
            // set position
            BoxPosition = bPosition;
        }
        public Box(uint pickId, BundleProperties bundleProperties)
            : base(pickId, bundleProperties.Length, bundleProperties.Width, bundleProperties.Height, BoxPosition.Zero)
        {
            // colors
            Colors = Enumerable.Repeat(bundleProperties.Color, 6).ToArray();
            // specific
            BundleFlats = bundleProperties.NoFlats;
            IsBundle = bundleProperties.IsBundle;
        }
        #endregion
        #region Public properties
        public List<Texture>[] TextureLists { get; } = new List<Texture>[6];
        public Vector3D Position => BoxPosition.Position;

        public Color[] Colors { get; private set; }
        public bool IsBundle { get; set; } = false;
        public int BundleFlats { get; } = 0;

        public bool ShowTape => TapeWidth.Activated;
        public OptDouble TapeWidth { get; set; }
        public Color TapeColor { get; set; }
        public bool FacingMark { get; set; } = false;
        public bool ShowOrientationMark { get; set; } = false;
        public Color OrientationMarkColor { get; set; } = Color.White;
        public Vector3D[] TapePoints
        {
            get
            {
                Vector3D lengthAxis = LengthAxis;
                Vector3D widthAxis = WidthAxis;
                Vector3D vZ = Dimensions.Z * HeightAxis;
                var points = new Vector3D[4];
                points[0] = Position + 0.0 * lengthAxis + 0.5 * (Dimensions.Y - TapeWidth) * widthAxis + vZ;
                points[1] = Position + Dimensions.X * lengthAxis + 0.5 * (Dimensions.Y - TapeWidth) * widthAxis + vZ;
                points[2] = Position + Dimensions.X * lengthAxis + 0.5 * (Dimensions.Y + TapeWidth) * widthAxis + vZ;
                points[3] = Position + 0.0 * lengthAxis + 0.5 * (Dimensions.Y + TapeWidth) * widthAxis + vZ;
                return points;
            }
        }
        public Vector3D[] FacingPointsTop
        {
            get
            {
                Vector3D lengthAxis = LengthAxis;
                Vector3D widthAxis = WidthAxis;
                Vector3D vZ = Dimensions.Z * HeightAxis;
                
                double facing = UnitsManager.ConvertLengthFrom(Properties.Settings.Default.FacingDimension, UnitsManager.UnitSystem.UNIT_METRIC1);
                var points = new Vector3D[4];
                points[0] = Position + 0.5 * (Dimensions.X - facing) * lengthAxis + vZ;
                points[1] = Position + 0.5 * (Dimensions.X + facing) * lengthAxis + vZ;
                points[2] = Position + 0.5 * (Dimensions.X + facing) * lengthAxis + facing * widthAxis + vZ;
                points[3] = Position + 0.5 * (Dimensions.X - facing) * lengthAxis + facing * widthAxis + vZ;
                return points;
            }
        }
        public Vector3D[] FacingPointsRight
        {
            get
            {
                Vector3D lengthAxis = LengthAxis;
                Vector3D heightAxis = HeightAxis;
                double facing = UnitsManager.ConvertLengthFrom(Properties.Settings.Default.FacingDimension, UnitsManager.UnitSystem.UNIT_METRIC1);
                var points = new Vector3D[4];
                points[0] = Position + 0.5 * (Dimensions.X - facing) * lengthAxis + (Dimensions.Z - facing) * heightAxis;
                points[1] = Position + 0.5 * (Dimensions.X + facing) * lengthAxis + (Dimensions.Z - facing) * heightAxis;
                points[2] = Position + 0.5 * (Dimensions.X + facing) * lengthAxis + Dimensions.Z * heightAxis;
                points[3] = Position + 0.5 * (Dimensions.X - facing) * lengthAxis + Dimensions.Z * heightAxis;
                return points;
            }
        }

        public Face[] StrapperFaces
        {
            get
            {
                List<Face> faces = new List<Face>();
                foreach (var s in StrapperList)
                {
                    var points = new Vector3D[4];
                    var axis = Vector3D.Zero;

                    switch (s.Axis)
                    {
                        case 0:
                            axis = LengthAxis;
                            points[3] = new Vector3D(s.Abscissa, 0.0, 0.0);
                            points[2] = new Vector3D(s.Abscissa, Width, 0.0);
                            points[1] = new Vector3D(s.Abscissa, Width, Height);
                            points[0] = new Vector3D(s.Abscissa, 0.0, Height);
                            break;
                        case 1:
                            axis = WidthAxis;
                            points[0] = new Vector3D(0.0, s.Abscissa, 0.0);
                            points[1] = new Vector3D(Length, s.Abscissa, 0.0);
                            points[2] = new Vector3D(Length, s.Abscissa, Height);
                            points[3] = new Vector3D(0.0, s.Abscissa, Height);
                            break;
                        case 2:
                            axis = HeightAxis;
                            points[3] = new Vector3D(0.0, 0.0, s.Abscissa);
                            points[2] = new Vector3D(Length, 0.0, s.Abscissa);
                            points[1] = new Vector3D(Length, Width, s.Abscissa);
                            points[0] = new Vector3D(0.0, Width, s.Abscissa);
                            break;
                        default:
                            break;
                    }

                    for (int i = 0; i < 4; ++i)
                    {
                        var pt0 = Position + points[i].X * LengthAxis + points[i].Y * WidthAxis + points[i].Z * HeightAxis;
                        int i1 = i + 1 < 4 ? i + 1 : 0;
                        var pt1 = Position + points[i1].X * LengthAxis + points[i1].Y * WidthAxis + points[i1].Z * HeightAxis;
                        var vertices = new Vector3D[4];
                        vertices[0] = pt0 - 0.5 * s.Width * axis;
                        vertices[1] = pt0 + 0.5 * s.Width * axis;
                        vertices[2] = pt1 + 0.5 * s.Width * axis;
                        vertices[3] = pt1 - 0.5 * s.Width * axis;
                        faces.Add(new Face(0, vertices, s.Color, Color.Black, "BOX"));
                    }
                }
                return faces.ToArray();
            }
        }
        public List<PalletStrapper> StrapperList { get; } = new List<PalletStrapper>();
        #endregion
        #region Triangles
        public Triangle[] Triangles
        {
            get
            {
                Vector3D[] points = Points;
                if (ShowTape)
                {
                    Vector3D[] tapePoints = TapePoints;
                    return new Triangle[]
                    {
                        // XN
                        //
                        // 7 ------- 4
                        // |         |
                        // 3 ------- 0
                        //
                        new Triangle(PickId, points[0], true, points[4], false, points[3], true, Colors[0]),
                        new Triangle(PickId, points[3], false, points[4], true, points[7], true, Colors[0]),
                        // XP
                        //
                        // 5 ------- 6
                        // |         |
                        // 1 ------- 2
                        //
                        new Triangle(PickId, points[1], true, points[2], false, points[5], true, Colors[1]),
                        new Triangle(PickId, points[5], false, points[2], true, points[6], true, Colors[1]),
                        // YN
                        //
                        // 4 -------- 5
                        // |          |
                        // 0 -------- 1
                        //
                        new Triangle(PickId, points[0], true, points[1], false, points[4], true, Colors[2]),
                        new Triangle(PickId, points[4], false, points[1], true, points[5], true, Colors[2]),
                        // YP
                        //
                        // 6 -------- 7
                        // |          | 
                        // 2 -------- 3
                        //
                        new Triangle(PickId, points[7], true, points[6], true, points[2], false, Colors[3]),
                        new Triangle(PickId, points[7], false, points[2], true, points[3], true, Colors[3]),
                        // ZN
                        //
                        // 3 -------- 2
                        // |          | 
                        // 0 -------- 1
                        //
                        new Triangle(PickId, points[0], true, points[3], false, points[1], true, Colors[4]),
                        new Triangle(PickId, points[1], false, points[3], true, points[2], true, Colors[4]),
                        // ZP
                        //
                        // 7-----------6
                        // |           |
                        // t3          t2
                        // |           |
                        // t0          t1
                        // |           |
                        // 4-----------5

                        new Triangle(PickId, points[4], true, points[5], false, tapePoints[0], true, Colors[5]),
                        new Triangle(PickId, tapePoints[0], false, points[5], true, tapePoints[1], true, Colors[5]),
                        new Triangle(PickId, tapePoints[0], true, tapePoints[1], true, tapePoints[2], false, TapeColor),
                        new Triangle(PickId, tapePoints[0], false, tapePoints[2], true, tapePoints[3], true, TapeColor),
                        new Triangle(PickId, tapePoints[3], true, tapePoints[2], true, points[6], false, Colors[5]),
                        new Triangle(PickId, tapePoints[3], false, points[6], true, points[7], true, Colors[5])
                    };
                }
                else
                    return new Triangle[]
                    {
                        new Triangle(PickId, points[0], true, points[4], false, points[3], true, Colors[0]),
                        new Triangle(PickId, points[3], false, points[4], true, points[7], true, Colors[0]),
                        new Triangle(PickId, points[1], true, points[2], false, points[5], true, Colors[1]),
                        new Triangle(PickId, points[5], false, points[2], true, points[6], true, Colors[1]),
                        new Triangle(PickId, points[0], true, points[1], false, points[4], true, Colors[2]),
                        new Triangle(PickId, points[4], false, points[1], true, points[5], true, Colors[2]),
                        new Triangle(PickId, points[7], true, points[6], true, points[2], false, Colors[3]),
                        new Triangle(PickId, points[7], false, points[2], true, points[3], true, Colors[3]),
                        new Triangle(PickId, points[0], true, points[3], false, points[1], true, Colors[4]),
                        new Triangle(PickId, points[1], false, points[3], true, points[2], true, Colors[4]),
                        new Triangle(PickId, points[4], true, points[5], false, points[7], true, Colors[5]),
                        new Triangle(PickId, points[7], false, points[5], true, points[6], true, Colors[5])
                    };
            }
        }

        /// <summary>
        /// PointsImage
        /// </summary>
        /// <param name="texture">Texture (bitmap + Vector2D (position) + Vector2D(size) + double(angle))</param>
        /// <returns>array of Vector3D points</returns>
        public Vector3D[] PointsImage(int faceId, Texture texture)
        {
            Vector3D position = Position;
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

            Vector3D vecI = Vector3D.XAxis, vecJ = Vector3D.YAxis, vecO = Vector3D.Zero;
            switch (faceId)
            {
                case 0: vecI = points[0] - points[3]; vecJ = points[7] - points[3]; vecO = points[3]; break;
                case 1: vecI = points[2] - points[1]; vecJ = points[5] - points[1]; vecO = points[1]; break;
                case 2: vecI = points[1] - points[0]; vecJ = points[4] - points[0]; vecO = points[0]; break;
                case 3: vecI = points[3] - points[2]; vecJ = points[6] - points[2]; vecO = points[2]; break;
                case 4: vecI = points[0] - points[1]; vecJ = points[2] - points[1]; vecO = points[1]; break;
                case 5: vecI = points[5] - points[4]; vecJ = points[7] - points[4]; vecO = points[4]; break;
                default: break;
            }
            vecI.Normalize();
            vecJ.Normalize();

            var pts = new Vector3D[4];
            double cosAngle = Math.Cos(texture.Angle * Math.PI / 180.0);
            double sinAngle = Math.Sin(texture.Angle * Math.PI / 180.0);

            pts[0] = vecO + texture.Position.X * vecI + texture.Position.Y * vecJ;
            pts[1] = vecO + (texture.Position.X + texture.Size.X * cosAngle) * vecI + (texture.Position.Y + texture.Size.X * sinAngle) * vecJ;
            pts[2] = vecO + (texture.Position.X + texture.Size.X * cosAngle - texture.Size.Y * sinAngle) * vecI + (texture.Position.Y + texture.Size.X * sinAngle + texture.Size.Y * cosAngle) * vecJ;
            pts[3] = vecO + (texture.Position.X - texture.Size.Y * sinAngle) * vecI + (texture.Position.Y + texture.Size.Y * cosAngle) * vecJ;
            return pts;
        }
        #endregion
        #region Override faces
        public override Color ColorFace(short i) => Colors[i];
        public override Face[] Faces
        {
            get
            {
                var faces = base.Faces;
                int i = 0;
                foreach (var face in faces)
                {
                    face.Textures = TextureLists[i++];
                }
                return faces;
            }
        }
        #endregion
        #region Overrides Drawable
        public override void Draw(Graphics2D graphics)
        {
            System.Drawing.Graphics g = graphics.Graphics;
            Pen penPath = new Pen(new SolidBrush(TopFace.ColorPath));

            // get points
            Point[] ptsFace = graphics.TransformPoint(TopFace.Points);

            // draw solid face
            Brush brushSolid = new SolidBrush(TopFace.ColorFill);
            g.FillPolygon(brushSolid, ptsFace);
            g.DrawPolygon(penPath, ptsFace);
            // draw box tape
            if (TapeWidth.Activated)
            {
                // instantiate brush
                Brush brushTape = new SolidBrush(TapeColor);
                // fill polygon
                Point[] ptsTape = graphics.TransformPoint(TapePoints);
                g.FillPolygon(brushTape, ptsTape);
                g.DrawPolygon(penPath, ptsTape);
            }
            // orientation mark
            if (ShowOrientationMark)
            {
                Pen penPathOrientationMark = new Pen(new SolidBrush(OrientationMarkColor), 5.0F);
                Vector3D pt0 = TopFace.Points[0];
                Vector3D pt1 = TopFace.Points[1];
                double offset = 0.1 * (pt1 - pt0).GetLength();
                Point[] ptsOrientMark = graphics.TransformPoint(new Vector3D[] { pt0 + new Vector3D(offset, offset, 0.0), pt1 + new Vector3D(-offset, offset, 0.0) });
                g.DrawLine(penPathOrientationMark, ptsOrientMark[0], ptsOrientMark[1]);
            }
        }
        public override void Draw(Graphics3D graphics)
        {
            System.Drawing.Graphics g = graphics.Graphics;
            var viewDir = graphics.ViewDirection;

            Face[] faces = Faces;
            for (int i = 0; i < 6; ++i)
            {
                // Face
                Face face = faces[i];
                // visible ?
                if (!faces[i].IsVisible(viewDir)) continue;
                // color
                faces[i].ColorFill = Colors[i];
                // points
                Vector3D[] points3D = faces[i].Points;
                Point[] pt = graphics.TransformPoint(points3D);
                //  draw solid face
                Brush brush = new SolidBrush(faces[i].ColorGraph(graphics));
                g.FillPolygon(brush, pt);
                // draw textures
                if (null != face.Textures && graphics.ShowTextures)
                    foreach (Texture texture in face.Textures)
                    {
                        Point[] ptsImage = graphics.TransformPoint(PointsImage(i, texture));
                        Point[] pts = new Point[3];
                        pts[0] = ptsImage[3];
                        pts[1] = ptsImage[2];
                        pts[2] = ptsImage[0];
                        g.DrawImage(texture.Bitmap, pts);
                    }
                // draw path
                Brush brushPath = new SolidBrush(faces[i].ColorPath);
                Pen penPathThick = new Pen(brushPath, IsBundle ? 2.0f : 1.5f);
                int ptCount = pt.Length;
                for (int j = 1; j < ptCount; ++j)
                    g.DrawLine(penPathThick, pt[j - 1], pt[j]);
                g.DrawLine(penPathThick, pt[ptCount - 1], pt[0]);
                // draw bundle lines
                if (IsBundle && i < 4)
                {
                    Pen penPathThin = new Pen(brushPath, 1.5f);
                    int noSlice = Math.Min(BundleFlats, 4);
                    for (int iSlice = 0; iSlice < noSlice - 1; ++iSlice)
                    {
                        Vector3D[] ptSlice = new Vector3D[2];
                        ptSlice[0] = points3D[0] + (iSlice + 1) / (double)noSlice * (points3D[3] - points3D[0]);
                        ptSlice[1] = points3D[1] + (iSlice + 1) / (double)noSlice * (points3D[2] - points3D[1]);

                        Point[] pt2D = graphics.TransformPoint(ptSlice);
                        g.DrawLine(penPathThin, pt2D[0], pt2D[1]);
                    }
                }
            }
            Pen penBlack = new Pen(new SolidBrush(Color.Black), 1.5f);
            // draw box tape
            if (ShowTape && faces[5].IsVisible(viewDir))
            {
                // instantiate brush
                Brush brushTape = new SolidBrush(faces[5].ColorGraph(graphics, TapeColor));
                // get tape points
                Point[] pts = graphics.TransformPoint(TapePoints);
                // fill polygon
                g.FillPolygon(brushTape, pts);
                // draw path
                for (int j = 1; j < pts.Length; ++j)
                    g.DrawLine(penBlack, pts[j - 1], pts[j]);
                g.DrawLine(penBlack, pts[pts.Length - 1], pts[0]);
            }
            if (ShowTape && graphics.ShowFacing)
            {
                if (faces[5].IsVisible(viewDir))
                    g.FillPolygon(new SolidBrush(faces[5].ColorGraph(graphics, Color.Red)), graphics.TransformPoint(FacingPointsTop));
                if (faces[2].IsVisible(viewDir))
                    g.FillPolygon(new SolidBrush(faces[2].ColorGraph(graphics, Color.Red)), graphics.TransformPoint(FacingPointsRight));
            }
            // draw strappers
            foreach (var sf in StrapperFaces)
            {
                if (sf.IsVisible(viewDir))
                {
                    // get color
                    double cosA = Math.Abs(Vector3D.DotProduct(sf.Normal, graphics.VLight));
                    Color color = Color.FromArgb((int)(sf.ColorFill.R * cosA), (int)(sf.ColorFill.G * cosA), (int)(sf.ColorFill.B * cosA));
                    // instantiate brush
                    Brush brushStrapper = new SolidBrush(color);
                    // get face points
                    Point[] pts = graphics.TransformPoint(sf.Points);
                    // fill polygon
                    g.FillPolygon(brushStrapper, pts);
                    // draw path
                    int ptCount = pts.Length;
                    for (int j = 1; j < ptCount; ++j)
                        g.DrawLine(penBlack, pts[j - 1], pts[j]);
                    g.DrawLine(penBlack, pts[ptCount - 1], pts[0]);
                }
            }
            if (graphics.ShowBoxIds)
            {
                // draw box id
                Point ptId = graphics.TransformPoint(TopFace.Center);
                g.DrawString(
                    PickId.ToString()
                    , new Font("Arial", graphics.GridFontSize)
                    , Brushes.Black
                    , new Rectangle(ptId.X - 15, ptId.Y - 10, 30, 20)
                    , StringFormat.GenericDefault);
            }
        }
        public override void DrawWireframe(Graphics3D graphics)
        {
        }
        #endregion
        #region Colors / Texture
        public void SetAllFacesColor(Color color)
        {
            Colors = Enumerable.Repeat(color, 6).ToArray();
        }
        public void SetFaceColor(HalfAxis.HAxis iFace, Color color)
        {
            Colors[(int)iFace] = color;
        }
        public void SetFaceTextures(HalfAxis.HAxis iFace, List<Texture> textures)
        {
            TextureLists[(int)iFace] = textures;
        }
        #endregion
        #region Private helpers
        private BProperties PackableToBProperties(Packable packable)
        {
            if (packable is BProperties)
                return packable as BProperties;
            else if (packable is LoadedCase)
            {
                LoadedCase loadedCase = packable as LoadedCase;
                return PackableToBProperties(loadedCase.Container);
            }
            else if (packable is LoadedPallet)
            {
                LoadedPallet loadedPallet = packable as LoadedPallet;
                Vector3D dim = loadedPallet.OuterDimensions;
                BoxProperties boxProperties = new BoxProperties(loadedPallet.ParentDocument, dim.X, dim.Y, dim.Z);
                boxProperties.SetColor(Color.Chocolate);
                return boxProperties;
            }
            else
                return null;
        }
        #endregion
    }
    #endregion

    #region TriangleIndices
    public class TriangleIndices
    {
        #region Constructor
        public TriangleIndices(ulong v0, ulong v1, ulong v2, ulong n0, ulong n1, ulong n2, ulong uv0, ulong uv1, ulong uv2)
        {
            _vertex[0] = v0; _vertex[1] = v1; _vertex[2] = v2;
            _normal[0] = n0; _normal[1] = n1; _normal[2] = n2;
            _UV[0] = uv0; _UV[1] = uv1; _UV[2] = uv2;
        }
        #endregion
        #region Convert to string
        public string ConvertToString(ulong iTriangleIndex)
        => $"{_vertex[0] + iTriangleIndex * 8} {_normal[0]} {_UV[0]}"+
           $"{_vertex[1] + iTriangleIndex * 8} {_normal[1]} {_UV[1]}"+
           $"{_vertex[2] + iTriangleIndex * 8} {_normal[2]} {_UV[2]}";
        #endregion
        #region Data members
        public ulong[] _vertex = new ulong[3];
        public ulong[] _normal = new ulong[3];
        public ulong[] _UV = new ulong[3];
        #endregion
    }
    #endregion
}
