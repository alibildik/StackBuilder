#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Sharp3D.Math.Core;

using log4net;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    #region LayerDescCyl
    public class LayerDescCyl : LayerDesc
    {
        #region Constructor
        public LayerDescCyl(string patternName, bool swapped)
            : base(patternName, swapped)
        {
        }
        #endregion

        #region Object override
        public override string ToString()
        {
            return string.Format("{0} | {1}", PatternName, Swapped ? "t" : "f");
        }
        public static LayerDescCyl Parse(string value)
        {
            Regex r = new Regex(@"(?<name>.*)\|(?<swap>.*)", RegexOptions.Singleline);
            Match m = r.Match(value);
            if (m.Success)
            {
                string patternName = m.Result("${name}");
                bool swapped = string.Equals("t", m.Result("${swap}"), StringComparison.CurrentCultureIgnoreCase);
                return new LayerDescCyl(patternName, swapped);
            }
            else
                throw new Exception("Failed to parse LayerDesc");
        }
        #endregion
    }
    #endregion

    #region Layer2DCyl
    public class Layer2DCylImp : List<Vector2D>, ILayer2D
    {
        #region Constructor
        public Layer2DCylImp(double radius, double height, Vector2D dimContainer, bool swapped)
        {
            Radius = radius; LayerHeight = height;
            DimContainer = dimContainer;
            Swapped = swapped;
        }
        #endregion

        #region Public methods
        public bool IsValidPosition(Vector2D vPosition)
        {
            return (vPosition.X - Radius >= -1) && (vPosition.X + Radius <= DimContainer.X)
                && (vPosition.Y - Radius >= -1) && (vPosition.Y + Radius <= DimContainer.Y);
        }
        public int CountInHeight(double height)
        {
            return NoLayers(height) * Count;
        }
        public int PerPalletCount(double zHeight)
        {
            return NoLayers(zHeight) * Count;
        }
        public int NoLayers(double height)
        {
            return (int)Math.Floor(height / LayerHeight); 
        }
        public double Radius { get; }
        #endregion

        #region ILayer2D implementation
        public string PatternName { get; set; } = string.Empty;
        public bool Swapped { get; } = false;
        public string Name
        {
            get { return string.Format("{0}_{1}", PatternName, Swapped ? "t" : "f"); }
        }
        public double LayerHeight { get; }
        public double MaximumSpace { get { return 0.0; } }
        public double Length { get { return DimContainer.X; } }
        public double Width  { get { return DimContainer.Y; } }
        public string Tooltip(double height)
        {
            return string.Format("{0} * {1} = {2}\n {3}"
                    , Count
                    , NoLayers(height)
                    , CountInHeight(height)
                    , Name);
        }
        public void UpdateMaxSpace(double space, string patternName)
        { 
        }
        public BBox3D BBox
        {
            get
            {
                BBox3D bbox = BBox3D.Initial;
                foreach (Vector2D v in this)
                {
                    Vector3D pos = new Vector3D(v.X, v.Y, 0.0);

                    Vector3D[] pts = new Vector3D[8];
                    pts[0] = pos - Radius * Vector3D.XAxis - Radius * Vector3D.YAxis;
                    pts[1] = pos + Radius * Vector3D.XAxis - Radius * Vector3D.YAxis;
                    pts[2] = pos + Radius * Vector3D.XAxis + Radius * Vector3D.YAxis;
                    pts[3] = pos - Radius * Vector3D.XAxis + Radius * Vector3D.YAxis;
                    pts[4] = pos - Radius * Vector3D.XAxis - Radius * Vector3D.YAxis + LayerHeight * Vector3D.ZAxis;
                    pts[5] = pos + Radius * Vector3D.XAxis - Radius * Vector3D.YAxis + LayerHeight * Vector3D.ZAxis;
                    pts[6] = pos + Radius * Vector3D.XAxis + Radius * Vector3D.YAxis + LayerHeight * Vector3D.ZAxis;
                    pts[7] = pos - Radius * Vector3D.XAxis + Radius * Vector3D.YAxis + LayerHeight * Vector3D.ZAxis;

                    foreach (Vector3D pt in pts)
                        bbox.Extend(pt);
                }
                return bbox;
            }
        }
        public double AreaEfficiency => 100.0 * Count * Math.Pow(Radius, 2) * Math.PI / (DimContainer.X * DimContainer.Y);
        #endregion

        #region Public properties
        public LayerDesc LayerDescriptor => new LayerDescCyl(PatternName, Swapped);
        public Vector2D DimContainer { get; set; }
        #endregion

        #region Data members
        protected static ILog _log = LogManager.GetLogger(typeof(Layer2DBrickImp));
        #endregion
    }
    #endregion

    #region Comparers
    public class LayerCylComparerCount : IComparer<Layer2DCylImp>
    {
        #region Data members
        private double _height = 0;
        #endregion

        #region Constructor
        public LayerCylComparerCount(double height)
        {
            _height = height;
        }
        #endregion

        #region Implement IComparer
        public int Compare(Layer2DCylImp layer0, Layer2DCylImp layer1)
        {
            int layer0Count = layer0.CountInHeight(_height);
            int layer1Count = layer1.CountInHeight(_height);

            if (layer0Count < layer1Count)
                return 1;
            else if (layer0Count == layer1Count)
                return 0;
            else
                return -1;            
        }
        #endregion
    }
    #endregion
}
