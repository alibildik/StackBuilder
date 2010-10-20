﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using Sharp3D.Math.Core;
using System.Drawing;
using TreeDim.StackBuilder.Basics;

using System.Diagnostics;
#endregion

namespace TreeDim.StackBuilder.Graphics
{
    #region Box
    public class Box : Drawable
    {
        #region Data members
        private uint _pickId = 0;
        private double[] _dim = new double[3];
        private Vector3D _position = Vector3D.Zero;
        private Vector3D _lengthAxis = Vector3D.XAxis;
        private Vector3D _widthAxis = Vector3D.YAxis;
        private Color[] _colors;
        private List<Texture>[] _textureLists = new List<Texture>[6];
        /// <summary>
        /// Bundle properties
        /// </summary>
        private bool _isBundle = false;
        private int _noFlats = 0;
        #endregion

        #region Constructor
        public Box(uint pickId, double length, double width, double height)
        {
            _pickId = pickId;
            _dim[0] = length;
            _dim[1] = width;
            _dim[2] = height;

            _colors = new Color[6];
            _colors[0] = Color.Red;
            _colors[1] = Color.Red;
            _colors[2] = Color.Green;
            _colors[3] = Color.Green;
            _colors[4] = Color.Blue;
            _colors[5] = Color.Blue;

            for (int i=0; i<6; ++i)
                _textureLists[i] = null;
        }
        public Box(uint pickId, double length, double width, double height, double x, double y, double z)
        {
            _pickId = pickId;
            _dim[0] = length;
            _dim[1] = width;
            _dim[2] = height;

            _colors = new Color[6];
            _colors[0] = Color.Red;
            _colors[1] = Color.Red;
            _colors[2] = Color.Green;
            _colors[3] = Color.Green;
            _colors[4] = Color.Blue;
            _colors[5] = Color.Blue;

            for (int i = 0; i < 6; ++i)
                _textureLists[i] = null;

            _position.X = x;
            _position.Y = y;
            _position.Z = z;        
        }
        public Box(uint pickId, BProperties bProperties)
        {
            _pickId = pickId;
            _dim[0] = bProperties.Length;
            _dim[1] = bProperties.Width;
            _dim[2] = bProperties.Height;

            _colors = bProperties.Colors;

            // IsBundle ?
            _isBundle = bProperties.IsBundle;
        }
        public Box(uint pickId, BProperties bProperties, BoxPosition bPosition)
        {
            if (!bPosition.IsValid)
                throw new GraphicsException("Invalid BoxPosition: can not create box");
            _pickId = pickId;
            _dim[0] = bProperties.Length;
            _dim[1] = bProperties.Width;
            _dim[2] = bProperties.Height;

            _colors = bProperties.Colors;

            // set position
            Position = bPosition.Position;
            // set direction length
            switch (bPosition.DirectionLength)
            {
                case HalfAxis.HAxis.AXIS_X_N: LengthAxis = -Vector3D.XAxis; break;
                case HalfAxis.HAxis.AXIS_X_P: LengthAxis = Vector3D.XAxis; break;
                case HalfAxis.HAxis.AXIS_Y_N: LengthAxis = -Vector3D.YAxis; break;
                case HalfAxis.HAxis.AXIS_Y_P: LengthAxis = Vector3D.YAxis; break;
                case HalfAxis.HAxis.AXIS_Z_N: LengthAxis = -Vector3D.ZAxis; break;
                case HalfAxis.HAxis.AXIS_Z_P: LengthAxis = Vector3D.ZAxis; break;
                default:
                    Debug.Assert(false);
                    break;
            }
            // set direction width
            switch (bPosition.DirectionWidth)
            {
                case HalfAxis.HAxis.AXIS_X_N: WidthAxis = -Vector3D.XAxis; break;
                case HalfAxis.HAxis.AXIS_X_P: WidthAxis = Vector3D.XAxis; break;
                case HalfAxis.HAxis.AXIS_Y_N: WidthAxis = -Vector3D.YAxis; break;
                case HalfAxis.HAxis.AXIS_Y_P: WidthAxis = Vector3D.YAxis; break;
                case HalfAxis.HAxis.AXIS_Z_N: WidthAxis = -Vector3D.ZAxis; break;
                case HalfAxis.HAxis.AXIS_Z_P: WidthAxis = Vector3D.ZAxis; break;
                default:
                    Debug.Assert(false);
                    break;
            }
            // IsBundle ?
            _noFlats = 3;
            _isBundle = bProperties.IsBundle;
        }

        public Box(uint pickId, InterlayerProperties interlayerProperties)
        {
            _pickId = pickId;
            _dim[0] = interlayerProperties.Length;
            _dim[1] = interlayerProperties.Width;
            _dim[2] = interlayerProperties.Thickness;
            _colors = new Color[6];
            for (int i = 0; i < 6; ++i)
                _colors[i] = interlayerProperties.Color;
        }

        public Box(uint pickId, InterlayerProperties interlayerProperties, BoxPosition bPosition)
        {
            _pickId = pickId;
            _dim[0] = interlayerProperties.Length;
            _dim[1] = interlayerProperties.Width;
            _dim[2] = interlayerProperties.Thickness;
            _colors = new Color[6];
            for (int i = 0; i < 6; ++i)
                _colors[i] = interlayerProperties.Color;

            // set position
            Position = bPosition.Position;
            // set direction length
            switch (bPosition.DirectionLength)
            {
                case HalfAxis.HAxis.AXIS_X_N: LengthAxis = -Vector3D.XAxis; break;
                case HalfAxis.HAxis.AXIS_X_P: LengthAxis = Vector3D.XAxis; break;
                case HalfAxis.HAxis.AXIS_Y_N: LengthAxis = -Vector3D.YAxis; break;
                case HalfAxis.HAxis.AXIS_Y_P: LengthAxis = Vector3D.YAxis; break;
                case HalfAxis.HAxis.AXIS_Z_N: LengthAxis = -Vector3D.ZAxis; break;
                case HalfAxis.HAxis.AXIS_Z_P: LengthAxis = Vector3D.ZAxis; break;
                default:
                    Debug.Assert(false);
                    break;
            }
            // set direction width
            switch (bPosition.DirectionWidth)
            {
                case HalfAxis.HAxis.AXIS_X_N: WidthAxis = -Vector3D.XAxis; break;
                case HalfAxis.HAxis.AXIS_X_P: WidthAxis = Vector3D.XAxis; break;
                case HalfAxis.HAxis.AXIS_Y_N: WidthAxis = -Vector3D.YAxis; break;
                case HalfAxis.HAxis.AXIS_Y_P: WidthAxis = Vector3D.YAxis; break;
                case HalfAxis.HAxis.AXIS_Z_N: WidthAxis = -Vector3D.ZAxis; break;
                case HalfAxis.HAxis.AXIS_Z_P: WidthAxis = Vector3D.ZAxis; break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        public Box(uint pickId, BundleProperties bundleProperties)
        {
            _pickId = pickId;
            _isBundle = true;
            _dim[0] = bundleProperties.Length;
            _dim[1] = bundleProperties.Width;
            _dim[2] = bundleProperties.Height;
            _colors = new Color[6]; 
            for (int i = 0; i < 6; ++i)
                _colors[i] = bundleProperties.Color;
            _noFlats = bundleProperties.NoFlats;
            // IsBundle ?
            _isBundle = bundleProperties.IsBundle;
        }
        #endregion

        #region Public properties
        public uint PickId
        {
            get { return _pickId; }
        }
        public Vector3D Position
        {
            get { return _position; }
            set { _position = value; }
        }
        public double Length
        {
            get { return _dim[0]; }
        }
        public double Width
        {
            get { return _dim[1]; }
        }
        public double Height
        {
            get { return _dim[2]; }
        }

        public Vector3D LengthAxis
        {
            get { return _lengthAxis; }
            set { _lengthAxis = value; }
        }
        public Vector3D WidthAxis
        {
            get { return _widthAxis; }
            set { _widthAxis = value; }
        }

        public Color[] Colors
        {
            get { return _colors; }
        }

        public bool IsBundle
        {
            get { return _isBundle; }
            set { _isBundle = value; }
        }
        public int BundleFlats
        {
            get { return _noFlats; }
        }

        public Face TopFace
        {
            get
            {
                Face[] faces = Faces;
                Face topFace = faces[0];
                foreach (Face face in faces)
                    if (face.Center.Z > topFace.Center.Z)
                        topFace = face;
                return topFace;
            }
        }

        public Vector3D[] Oriented(Vector3D pt0, Vector3D pt1, Vector3D pt2, Vector3D pt3, Vector3D pt)
        {
            Vector3D crossProduct = Vector3D.CrossProduct(pt1 - pt0, pt2 - pt0);
            if (Vector3D.DotProduct(crossProduct, pt - pt0) < 0)
                return new Vector3D[] { pt0, pt1, pt2, pt3 };
            else
                return new Vector3D[] { pt3, pt2, pt1, pt0 };
        }



        public Vector3D Center
        {
            get
            {
                return _position
                    + 0.5 * _dim[0] * _lengthAxis
                    + 0.5 * _dim[1] * _widthAxis
                    + 0.5 * _dim[2] * Vector3D.CrossProduct(_lengthAxis, _widthAxis);
            }
        }

        #endregion

        #region XMin / XMax / YMin / YMax / ZMin
        public double XMin
        {
            get
            {
                double xmin = double.MaxValue;
                foreach (Vector3D v in this.Points)
                {
                    if (v.X < xmin)
                        xmin = v.X;
                }
                return xmin;
            }
        }
        public double XMax
        {
            get
            {
                double xmax = double.MinValue;
                foreach (Vector3D v in this.Points)
                {
                    if (v.X > xmax)
                        xmax = v.X;
                }
                return xmax;
            }
        }
        public double YMin
        {
            get
            {
                double ymin = double.MaxValue;
                foreach (Vector3D v in this.Points)
                {
                    if (v.Y < ymin)
                        ymin = v.Y;
                }
                return ymin;
            }
        }
        public double YMax
        {
            get
            {
                double ymax = double.MinValue;
                foreach (Vector3D v in this.Points)
                {
                    if (v.Y > ymax)
                        ymax = v.Y;
                }
                return ymax;
            }
        }

        public double ZMin
        {
            get
            {
                double zmin = double.MaxValue;
                foreach (Vector3D v in this.Points)
                    zmin = Math.Min(v.Z, zmin);
                return zmin;
            }
        }
        #endregion

        #region Points / Faces
        public Vector3D[] Points
        {
            get
            {
                Vector3D heightAxis = Vector3D.CrossProduct(_lengthAxis, _widthAxis);
                Vector3D[] points = new Vector3D[8];
                points[0] = _position;
                points[1] = _position + _dim[0] * _lengthAxis;
                points[2] = _position + _dim[0] * _lengthAxis + _dim[1] * _widthAxis;
                points[3] = _position + _dim[1] * _widthAxis;

                points[4] = _position + _dim[2] * heightAxis;
                points[5] = _position + _dim[2] * heightAxis + _dim[0] * _lengthAxis;
                points[6] = _position + _dim[2] * heightAxis + _dim[0] * _lengthAxis + _dim[1] * _widthAxis;
                points[7] = _position + _dim[2] * heightAxis + _dim[1] * _widthAxis;

                return points;
            }
        }
        public Face[] Faces
        {
            get
            {
                Vector3D heightAxis = Vector3D.CrossProduct(_lengthAxis, _widthAxis);
                Vector3D[] points = new Vector3D[8];
                points[0] = _position;
                points[1] = _position + _dim[0] * _lengthAxis;
                points[2] = _position + _dim[0] * _lengthAxis + _dim[1] * _widthAxis;
                points[3] = _position + _dim[1] * _widthAxis;

                points[4] = _position + _dim[2] * heightAxis;
                points[5] = _position + _dim[2] * heightAxis + _dim[0] * _lengthAxis ;
                points[6] = _position + _dim[2] * heightAxis + _dim[0] * _lengthAxis + _dim[1] * _widthAxis ;
                points[7] = _position + _dim[2] * heightAxis + _dim[1] * _widthAxis ;

                Face[] faces = new Face[6];
                faces[0] = new Face(_pickId, new Vector3D[] { points[3], points[0], points[4], points[7] }); // AXIS_X_N
                faces[1] = new Face(_pickId, new Vector3D[] { points[1], points[2], points[6], points[5] }); // AXIS_X_P
                faces[2] = new Face(_pickId, new Vector3D[] { points[0], points[1], points[5], points[4] }); // AXIS_Y_N
                faces[3] = new Face(_pickId, new Vector3D[] { points[2], points[3], points[7], points[6] }); // AXIS_Y_P
                faces[4] = new Face(_pickId, new Vector3D[] { points[3], points[2], points[1], points[0] }); // AXIS_Z_N
                faces[5] = new Face(_pickId, new Vector3D[] { points[4], points[5], points[6], points[7] }); // AXIS_Z_P

                int i = 0;
                foreach (Face face in faces)
                {
                    face.ColorFill = _colors[i];
                    face.Textures = _textureLists[i];
                    ++i;
                }

                return faces;
            }
        }
        #endregion

        #region Validity
        public bool IsValid
        {
            get
            {
                return _dim[0] > 0.0 && _dim[1] > 0.0 && _dim[2] > 0.0 && (_lengthAxis != _widthAxis);
            }
        }
        #endregion

        #region Public methods
        public override void Draw(Graphics3D graphics)
        {
            foreach (Face face in Faces)
                graphics.AddFace(face);
        }

        public void SetAllFacesColor(Color color)
        {
            for (int i = 0; i < 6; ++i)
                _colors[i] = color;        
        }

        public void SetFaceColor(HalfAxis.HAxis iFace, Color color)
        {
            _colors[(int)iFace] = color;
        }

        public void SetFaceTextures(HalfAxis.HAxis iFace, List<Texture> textures)
        {
            _textureLists[(int)iFace] = textures;
        }

        public bool PointInside(Vector3D pt)
        {
            foreach (Face face in Faces)
            {
                if (face.PointRelativePosition(pt) != -1)
                    return false;
            }
            return true;            
        }
        public bool PointBehind(Vector3D pt, Vector3D viewDir)
        {
            const double eps = 1.0E-06;
            foreach (Face f in Faces)
            {
                // is face visible ?
                if (!f.IsVisible(viewDir))
                    continue;
                // if face is top/bottom face -> skip
                if (Math.Abs(Vector3D.DotProduct(Vector3D.ZAxis, f.Normal) - 1.0) < eps)
                    continue;
                if (Math.Abs(Vector3D.DotProduct(-Vector3D.ZAxis, f.Normal) - 1.0) < eps)
                    continue;
                // is behind face
                if (!f.PointIsBehind(pt, viewDir))
                    return false;
            }
            return true;
        }

        public bool PointInFront(Vector3D pt, Vector3D viewDir)
        {
            const double eps = 1.0E-06;
            foreach (Face f in Faces)
            {
                // is face visible ?
                if (!f.IsVisible(viewDir))
                    continue;
                // if face is top/bottom face -> skip
                if (Math.Abs(Vector3D.DotProduct(Vector3D.ZAxis, f.Normal) - 1.0) < eps)
                    continue;
                if (Math.Abs(Vector3D.DotProduct(-Vector3D.ZAxis, f.Normal) - 1.0) < eps)
                    continue;
                // is behind face
                if (!f.PointIsInFront(pt, viewDir))
                    return false;
            }
            return true;       
        }

        /// <summary>
        /// Box b is behind this,
        /// if and if only at least one point is behind both visible faces
        /// </summary>
        /// <param name="b"></param>
        /// <param name="viewDir"></param>
        /// <returns></returns>
        public bool BoxBehind(Box b, Vector3D viewDir)
        {
            const double eps = 0.00001;
            foreach (Vector3D pt in b.Points)
            {
                int iVisibleFaces = 0;
                int iCountBehind = 0;
                foreach (Face f in Faces)
                {
                    if (!f.IsVisible(viewDir)) continue;
                    // if face is top/bottom face -> skip
                    if (Math.Abs(Vector3D.DotProduct(Vector3D.ZAxis, f.Normal) - 1.0) < eps)
                        continue;
                    if (Math.Abs(Vector3D.DotProduct(-Vector3D.ZAxis, f.Normal) - 1.0) < eps)
                        continue;
                    ++iVisibleFaces;
                    if (f.PointIsBehind(pt, viewDir))
                        ++iCountBehind;
                }
                if (iVisibleFaces == iCountBehind)
                    return true;
            }
            return false;
        }
        // --- Definition:
        // Box b is in front of this,
        // if and if only at all its points are in front of both visible faces
        // ---
        public bool BoxInFront(Box b, Vector3D viewDir)
        {
            foreach (Face f in Faces)
            {
                if (!f.IsVisible(viewDir)) continue;
                const double eps = 0.00001;
                // if face is top/bottom face -> skip
                if (Math.Abs(Vector3D.DotProduct(Vector3D.ZAxis, f.Normal) - 1.0) < eps)
                    continue;
                if (Math.Abs(Vector3D.DotProduct(-Vector3D.ZAxis, f.Normal) - 1.0) < eps)
                    continue; 

                int iCountFront = 0;
                foreach (Vector3D pt in b.Points)
                {
                    if (f.PointIsInFront(pt, viewDir))
                        ++iCountFront;
                }
                if (iCountFront == 8)
                    return true;
            }
            return false;
        }
        #endregion
    }
    #endregion


}
