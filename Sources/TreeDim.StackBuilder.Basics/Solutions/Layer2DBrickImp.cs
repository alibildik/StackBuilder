#region Using directives
using System;
using System.Text.RegularExpressions;

using Sharp3D.Math.Core;

using log4net;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    #region LayerDescBox
    public class LayerDescBox : LayerDesc
    {
        #region Constructor
        public LayerDescBox(string patternName, HalfAxis.HAxis axis, bool swapped)
            : base(patternName, swapped)
        {
            AxisOrtho = axis;
        }
        #endregion
        #region Public properties
        public HalfAxis.HAxis AxisOrtho { get; }
        #endregion
        #region Object override
        public override string ToString() => $"{PatternName}|{HalfAxis.ToString(AxisOrtho)}|{SwappedStr}";
        public override bool Equals(object obj)
        {
            if (obj is LayerDescBox layerDesc)
                return PatternName == layerDesc.PatternName && Swapped == layerDesc.Swapped && AxisOrtho == layerDesc.AxisOrtho;
            else
                return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ AxisOrtho.GetHashCode();
        }
        #endregion
        #region Static methods
        public static LayerDesc Parse(string value)
        {
            Regex r = new Regex(@"(?<name>.*)\|(?<axis>.*)\|(?<swap>.*)", RegexOptions.Singleline);
            Match m = r.Match(value);
            if (m.Success)
            {
                string patternName = m.Result("${name}");
                HalfAxis.HAxis axis = HalfAxis.Parse( m.Result("${axis}"));
                bool swapped = string.Equals("t", m.Result("${swap}"), StringComparison.CurrentCultureIgnoreCase);
                return new LayerDescBox(patternName, axis, swapped);
            }
            else
                throw new Exception($"Failed to parse LayerDesc : {value}");
        }
        #endregion
    }
    #endregion
    #region Layer2DBrickDef
    public class Layer2DBrickImp : Layer2DBrick
    {
        #region Constructor
        public Layer2DBrickImp(Vector3D dimBox, Vector3D bulge, Vector2D dimContainer, string patternName, HalfAxis.HAxis axisOrtho, bool swapped)
            : base(dimBox, bulge, dimContainer, patternName, axisOrtho)
        {
            Swapped = swapped;
        }
        #endregion

        #region Public properties
        public double ForcedSpace { get; set; } = 0.0;
        #endregion

        #region Public methods
        public void AddPosition(Vector2D vPosition, HalfAxis.HAxis lengthAxis, HalfAxis.HAxis widthAxis)
        {
            // build 4D matrix
            Vector3D vAxisLength = HalfAxis.ToVector3D(lengthAxis);
            Vector3D vAxisWidth = HalfAxis.ToVector3D(widthAxis);
            Vector3D vAxisHeight = Vector3D.CrossProduct(vAxisLength, vAxisWidth);
            Matrix4D mat = Matrix4D.Identity;
            mat.M11 = vAxisLength.X;
            mat.M12 = vAxisLength.Y;
            mat.M13 = vAxisLength.Z;
            mat.M21 = vAxisWidth.X;
            mat.M22 = vAxisWidth.Y;
            mat.M23 = vAxisWidth.Z;
            mat.M31 = vAxisHeight.X;
            mat.M32 = vAxisHeight.Y;
            mat.M33 = vAxisHeight.Z;
            mat.M41 = 0.0;
            mat.M42 = 0.0;
            mat.M43 = 0.0;
            mat.M44 = 1.0;
            Transform3D localTransf = new Transform3D(mat);
            Transform3D localTransfInv = localTransf.Inverse();
            Transform3D originTranslation = Transform3D.Translation(localTransfInv.transform(VecTransf) - new Vector3D(0.5 * ForcedSpace, 0.5 * ForcedSpace, 0.0));

            var layerPos = new BoxPosition(
                originTranslation.transform(new Vector3D(vPosition.X, vPosition.Y, 0.0) + 0.5 * ForcedSpace * vAxisLength + 0.5 * ForcedSpace * vAxisWidth)
                , HalfAxis.ToHalfAxis(localTransfInv.transform(HalfAxis.ToVector3D(LengthAxis)))
                , HalfAxis.ToHalfAxis(localTransfInv.transform(HalfAxis.ToVector3D(WidthAxis)))
                );

            // spacing ?
            layerPos.Position += new Vector3D(0.5 * ForcedSpace, 0.5 * ForcedSpace, 0.0);
            var bposAdj = layerPos.Adjusted(DimBoxTotal);
            // bulge ?
            Vector3D vecBulge = Vector3D.Zero;
            if (Bulge.X > 0)
                vecBulge += 0.5 * Bulge.X * HalfAxis.ToVector3D(bposAdj.DirectionLength);
            if (Bulge.Y > 0)
                vecBulge += 0.5 * Bulge.Y * HalfAxis.ToVector3D(bposAdj.DirectionWidth);
            if (Bulge.Z > 0)
                vecBulge += 0.5 * Bulge.Z * HalfAxis.ToVector3D(bposAdj.DirectionHeight);
            bposAdj.Position += vecBulge;

            // add position
            Add(bposAdj);
        }
        public bool IsValidPosition(Vector2D vPosition, HalfAxis.HAxis lengthAxis, HalfAxis.HAxis widthAxis)
        {
            // check layerPos
            // get 4 points
            Vector3D[] pts = new Vector3D[4];
            pts[0] = new Vector3D(vPosition.X, vPosition.Y, 0.0);
            pts[1] = new Vector3D(vPosition.X, vPosition.Y, 0.0) + HalfAxis.ToVector3D(lengthAxis) * BoxLength;
            pts[2] = new Vector3D(vPosition.X, vPosition.Y, 0.0) + HalfAxis.ToVector3D(widthAxis) * BoxWidth;
            pts[3] = new Vector3D(vPosition.X, vPosition.Y, 0.0) + HalfAxis.ToVector3D(lengthAxis) * BoxLength + HalfAxis.ToVector3D(widthAxis) * BoxWidth;

            foreach (Vector3D pt in pts)
            {
                if (pt.X < (0.0 - _epsilon) || pt.X > (DimContainer.X + _epsilon) || pt.Y < (0.0 - _epsilon) || pt.Y > (DimContainer.Y + _epsilon))
                    return false;
            }
            return true;
        }
        #endregion

        #region ILayer2D implementation
        public string SwappedString => Swapped ? "t" : "f";
        public override string Name => $"{PatternName}_{HalfAxis.ToString(AxisOrtho)}_{SwappedString}";
        public override string Tooltip(double height)
            => $"{Count} * {NoLayers(height)} = {CountInHeight(height)}\n {HalfAxis.ToString(AxisOrtho)} | {PatternName} | {SwappedString}";
        #endregion

        #region Maximum space
        public override double MaximumSpace
        {
            get
            {
                System.Diagnostics.Debug.Assert(!double.IsNaN(_maximumSpace) && !double.IsInfinity(_maximumSpace));
                return _maximumSpace;
            }
        }
        public override void UpdateMaxSpace(double space, string patternName)
        {
            if (space < 0.0 - _epsilon)
            {
                _log.Info(string.Format("{0} : Negative space value? ({1})", patternName, space));
                return;
            }
            else if (double.IsNaN(space))
            {
                _log.Info(string.Format("{0} : Invalid space value! (NaN (NotANumber))", patternName));
                return;
            }
            else if (double.IsInfinity(space))
            {
                _log.Info(string.Format("{0} : Invalid space value! (Infinity)", patternName));
                return;
            }
            else
                _maximumSpace = Math.Max(space, _maximumSpace);
        }
        #endregion

        #region Public properties
        public override double BoxLength
        {
            get
            {
                switch (AxisOrtho)
                {
                    case HalfAxis.HAxis.AXIS_X_N: return DimBoxTotal.Z + ForcedSpace;
                    case HalfAxis.HAxis.AXIS_X_P: return DimBoxTotal.Z + ForcedSpace;
                    case HalfAxis.HAxis.AXIS_Y_N: return DimBoxTotal.X + ForcedSpace;
                    case HalfAxis.HAxis.AXIS_Y_P: return DimBoxTotal.Y + ForcedSpace;
                    case HalfAxis.HAxis.AXIS_Z_N: return DimBoxTotal.Y + ForcedSpace;
                    case HalfAxis.HAxis.AXIS_Z_P: return DimBoxTotal.X + ForcedSpace;
                    default: throw new Exception("Invalid ortho axis");
                }
            }
        }
        public override double BoxWidth
        {
            get
            {
                switch (AxisOrtho)
                {
                    case HalfAxis.HAxis.AXIS_X_N: return DimBoxTotal.Y + ForcedSpace;
                    case HalfAxis.HAxis.AXIS_X_P: return DimBoxTotal.X + ForcedSpace;
                    case HalfAxis.HAxis.AXIS_Y_N: return DimBoxTotal.Z + ForcedSpace;
                    case HalfAxis.HAxis.AXIS_Y_P: return DimBoxTotal.Z + ForcedSpace;
                    case HalfAxis.HAxis.AXIS_Z_N: return DimBoxTotal.X + ForcedSpace;
                    case HalfAxis.HAxis.AXIS_Z_P: return DimBoxTotal.Y + ForcedSpace;
                    default: throw new Exception("Invalid ortho axis");
                }
            }
        }
        public Vector3D VecTransf
        {
            get
            {
                switch (AxisOrtho)
                {
                    case HalfAxis.HAxis.AXIS_X_N: return new Vector3D(DimBoxTotal.Z, 0.0, 0.0);
                    case HalfAxis.HAxis.AXIS_X_P: return new Vector3D(0.0, 0.0, 0.0); ;
                    case HalfAxis.HAxis.AXIS_Y_N: return new Vector3D(0.0, DimBoxTotal.Z, 0.0);
                    case HalfAxis.HAxis.AXIS_Y_P: return Vector3D.Zero;
                    case HalfAxis.HAxis.AXIS_Z_N: return new Vector3D(0.0, 0.0, DimBoxTotal.Z);
                    case HalfAxis.HAxis.AXIS_Z_P: return Vector3D.Zero;
                    default: throw new Exception("Invalid ortho axis");
                }
            }
        }
        public int BoxCount => Count;
        public override LayerDesc LayerDescriptor => new LayerDescBox(PatternName, AxisOrtho, Swapped);
        #endregion

        #region Generate Layer2DEdited
        public Layer2DBrickExp GenerateLayer2DEdited()
        {
            var layer = new Layer2DBrickExp(DimBox, Bulge, DimContainer, $"{Name}_edit_{_indexEdit++}", AxisOrtho);
            foreach (var lp in Positions)
                layer.AddPosition(new BoxPosition(lp));
            return layer;
        }
        #endregion

        #region Data members
        private double _maximumSpace = 0.0;
        private static readonly double _epsilon = 1.0e-03;
        private static int _indexEdit = 0;
        protected static ILog _log = LogManager.GetLogger(typeof(Layer2DBrickImp));
        #endregion
    }
    #endregion
}
