#region Using directives
using System;
using System.Collections.Generic;

using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    public abstract class Layer2DBrick : ILayer2D
    {
        #region Constructor
        protected Layer2DBrick(Vector3D dimBox, Vector3D bulge, Vector2D dimContainer, string name, HalfAxis.HAxis axisOrtho)
        {
            Name = name;
            PatternName = name;
            DimBox = dimBox;
            Bulge = bulge;
            DimContainer = dimContainer;
            AxisOrtho = axisOrtho;
        }
        #endregion
        #region ILayer2D implementation
        public virtual string PatternName { get; private set; } = string.Empty;
        public virtual string Name { get; private set; }
        public bool Swapped { get; protected set; } = false;
        public double LayerHeight => BoxHeight;
        public int Count => _listBoxPosition.Count;
        public double Length => DimContainer.X;
        public double Width => DimContainer.Y;
        public virtual LayerDesc LayerDescriptor => null;
        public virtual double MaximumSpace { get => 0.0; }
        public void Clear() { _listBoxPosition.Clear(); }
        public int CountInHeight(double height) => NoLayers(height) * Count;
        public int NoLayers(double height) => (int)Math.Floor(height / LayerHeight);
        public virtual string Tooltip(double height) => $"{Count} * {NoLayers(height)} = {CountInHeight(height)} | {HalfAxis.ToString(AxisOrtho)}";
        public virtual void UpdateMaxSpace(double space, string patternName) { }
        public BBox3D BBox
        {
            get
            {
                BBox3D bb = BBox3D.Initial;
                foreach (BoxPosition bp in _listBoxPosition)
                    bb.Extend(bp.BBox(DimBox));
                return bb;
            }
        }
        #endregion
        #region Layer2DBrick specific
        public HalfAxis.HAxis AxisOrtho { get; private set; } = HalfAxis.HAxis.AXIS_Z_P;
        public HalfAxis.HAxis VerticalAxisProp => VerticalAxis(AxisOrtho);
        public bool IsZOriented => VerticalAxisProp.IsOneOf(HalfAxis.HAxis.AXIS_Z_P, HalfAxis.HAxis.AXIS_Z_N);
        public int VerticalDirection => HalfAxis.Direction(VerticalAxisProp);
        public virtual double BoxLength
        {
            get
            {
                switch (AxisOrtho)
                {
                    case HalfAxis.HAxis.AXIS_X_N: return DimBoxTotal.Z;
                    case HalfAxis.HAxis.AXIS_X_P: return DimBoxTotal.Z;
                    case HalfAxis.HAxis.AXIS_Y_N: return DimBoxTotal.X;
                    case HalfAxis.HAxis.AXIS_Y_P: return DimBoxTotal.Y;
                    case HalfAxis.HAxis.AXIS_Z_N: return DimBoxTotal.Y;
                    case HalfAxis.HAxis.AXIS_Z_P: return DimBoxTotal.X;
                    default: throw new Exception("Invalid ortho axis");
                }
            }
        }
        public virtual double BoxWidth
        {
            get
            {
                switch (AxisOrtho)
                {
                    case HalfAxis.HAxis.AXIS_X_N: return DimBoxTotal.Y;
                    case HalfAxis.HAxis.AXIS_X_P: return DimBoxTotal.X;
                    case HalfAxis.HAxis.AXIS_Y_N: return DimBoxTotal.Z;
                    case HalfAxis.HAxis.AXIS_Y_P: return DimBoxTotal.Z;
                    case HalfAxis.HAxis.AXIS_Z_N: return DimBoxTotal.X;
                    case HalfAxis.HAxis.AXIS_Z_P: return DimBoxTotal.Y;
                    default: throw new Exception("Invalid ortho axis");
                }
            }
        }
        public double BoxHeight
        {
            get
            {
                switch (AxisOrtho)
                {
                    case HalfAxis.HAxis.AXIS_X_N: return DimBoxTotal.X;
                    case HalfAxis.HAxis.AXIS_X_P: return DimBoxTotal.Y;
                    case HalfAxis.HAxis.AXIS_Y_N: return DimBoxTotal.Y;
                    case HalfAxis.HAxis.AXIS_Y_P: return DimBoxTotal.X;
                    case HalfAxis.HAxis.AXIS_Z_N: return DimBoxTotal.Z;
                    case HalfAxis.HAxis.AXIS_Z_P: return DimBoxTotal.Z;
                    default: throw new Exception("Invalid ortho axis");
                }
            }
        }
        public HalfAxis.HAxis LengthAxis
        {
            get
            {
                switch (AxisOrtho)
                {
                    case HalfAxis.HAxis.AXIS_X_N: return HalfAxis.HAxis.AXIS_Z_P;
                    case HalfAxis.HAxis.AXIS_X_P: return HalfAxis.HAxis.AXIS_Y_P;
                    case HalfAxis.HAxis.AXIS_Y_N: return HalfAxis.HAxis.AXIS_X_P;
                    case HalfAxis.HAxis.AXIS_Y_P: return HalfAxis.HAxis.AXIS_Z_P;
                    case HalfAxis.HAxis.AXIS_Z_N: return HalfAxis.HAxis.AXIS_Y_P;
                    case HalfAxis.HAxis.AXIS_Z_P: return HalfAxis.HAxis.AXIS_X_P;
                    default: throw new Exception("Invalid ortho axis");
                }
            }
        }
        public HalfAxis.HAxis WidthAxis
        {
            get
            {
                switch (AxisOrtho)
                {
                    case HalfAxis.HAxis.AXIS_X_N: return HalfAxis.HAxis.AXIS_Y_P;
                    case HalfAxis.HAxis.AXIS_X_P: return HalfAxis.HAxis.AXIS_Z_P;
                    case HalfAxis.HAxis.AXIS_Y_N: return HalfAxis.HAxis.AXIS_Z_P;
                    case HalfAxis.HAxis.AXIS_Y_P: return HalfAxis.HAxis.AXIS_X_P;
                    case HalfAxis.HAxis.AXIS_Z_N: return HalfAxis.HAxis.AXIS_X_P;
                    case HalfAxis.HAxis.AXIS_Z_P: return HalfAxis.HAxis.AXIS_Y_P;
                    default: throw new Exception("Invalid ortho axis");
                }
            }
        }

        public Vector3D DimBox { get; private set; }
        public Vector3D Bulge { get; private set; }
        public Vector3D DimBoxTotal => DimBox + Bulge;
        public Vector2D DimContainer { get; private set; }
        protected void Add(BoxPosition position) => _listBoxPosition.Add(position);
        protected void AddRange(List<BoxPosition> positions) => _listBoxPosition.AddRange(positions);
        public List<BoxPosition> Positions => _listBoxPosition;
        #endregion

        #region Object override
        public override string ToString() => $"{PatternName}_{(Swapped ? "1" : "0")}";
        #endregion

        #region Static methods
        public static HalfAxis.HAxis VerticalAxis(HalfAxis.HAxis axisOrtho)
        {
            switch (axisOrtho)
            {
                case HalfAxis.HAxis.AXIS_X_N: return HalfAxis.HAxis.AXIS_X_N;
                case HalfAxis.HAxis.AXIS_X_P: return HalfAxis.HAxis.AXIS_Y_P;
                case HalfAxis.HAxis.AXIS_Y_N: return HalfAxis.HAxis.AXIS_Y_N;
                case HalfAxis.HAxis.AXIS_Y_P: return HalfAxis.HAxis.AXIS_X_P;
                case HalfAxis.HAxis.AXIS_Z_N: return HalfAxis.HAxis.AXIS_Z_N;
                case HalfAxis.HAxis.AXIS_Z_P: return HalfAxis.HAxis.AXIS_Z_P;
                default: throw new Exception("Invalid ortho axis");
            }        
        }
        #endregion
        #region Data members
        protected List<BoxPosition> _listBoxPosition = new List<BoxPosition>();
        #endregion
    }

    public abstract class Layer2DBrickIndexed : ILayer2D
    {
        #region Constructor
        protected Layer2DBrickIndexed(Vector3D dimBox, Vector2D dimContainer, string name, HalfAxis.HAxis axisOrtho)
        {
            Name = name;
            PatternName = name;
            DimBox = dimBox;
            DimContainer = dimContainer;
            AxisOrtho = axisOrtho;
        }
        #endregion

        #region ILayer2D implementation
        public virtual string PatternName { get; private set; } = string.Empty;
        public virtual string Name { get; private set; }
        public bool Swapped { get; protected set; } = false;
        public double LayerHeight => BoxHeight;
        public int Count => _listBoxPosition.Count;
        public double Length => DimContainer.X;
        public double Width => DimContainer.Y;
        public virtual LayerDesc LayerDescriptor => null;
        public virtual double MaximumSpace { get => 0.0; }
        public void Clear() { _listBoxPosition.Clear(); }
        public int CountInHeight(double height) => NoLayers(height) * Count;
        public int NoLayers(double height) => (int)Math.Floor(height / LayerHeight);
        public virtual string Tooltip(double height) => $"{Count} * {NoLayers(height)} = {CountInHeight(height)} | {HalfAxis.ToString(AxisOrtho)}";
        public virtual void UpdateMaxSpace(double space, string patternName) { }
        public BBox3D BBox
        {
            get
            {
                BBox3D bb = BBox3D.Initial;
                foreach (BoxPositionIndexed bp in _listBoxPosition)
                    bb.Extend(bp.BPos.BBox(DimBox));
                return bb;
            }
        }
        #endregion
        public HalfAxis.HAxis AxisOrtho { get; private set; } = HalfAxis.HAxis.AXIS_Z_P;
        public virtual double BoxLength
        {
            get
            {
                switch (AxisOrtho)
                {
                    case HalfAxis.HAxis.AXIS_X_N: return DimBoxTotal.Z;
                    case HalfAxis.HAxis.AXIS_X_P: return DimBoxTotal.Z;
                    case HalfAxis.HAxis.AXIS_Y_N: return DimBoxTotal.X;
                    case HalfAxis.HAxis.AXIS_Y_P: return DimBoxTotal.Y;
                    case HalfAxis.HAxis.AXIS_Z_N: return DimBoxTotal.Y;
                    case HalfAxis.HAxis.AXIS_Z_P: return DimBoxTotal.X;
                    default: throw new Exception("Invalid ortho axis");
                }
            }
        }
        public virtual double BoxWidth
        {
            get
            {
                switch (AxisOrtho)
                {
                    case HalfAxis.HAxis.AXIS_X_N: return DimBoxTotal.Y;
                    case HalfAxis.HAxis.AXIS_X_P: return DimBoxTotal.X;
                    case HalfAxis.HAxis.AXIS_Y_N: return DimBoxTotal.Z;
                    case HalfAxis.HAxis.AXIS_Y_P: return DimBoxTotal.Z;
                    case HalfAxis.HAxis.AXIS_Z_N: return DimBoxTotal.X;
                    case HalfAxis.HAxis.AXIS_Z_P: return DimBoxTotal.Y;
                    default: throw new Exception("Invalid ortho axis");
                }
            }
        }
        public double BoxHeight
        {
            get
            {
                switch (AxisOrtho)
                {
                    case HalfAxis.HAxis.AXIS_X_N: return DimBoxTotal.X;
                    case HalfAxis.HAxis.AXIS_X_P: return DimBoxTotal.Y;
                    case HalfAxis.HAxis.AXIS_Y_N: return DimBoxTotal.Y;
                    case HalfAxis.HAxis.AXIS_Y_P: return DimBoxTotal.X;
                    case HalfAxis.HAxis.AXIS_Z_N: return DimBoxTotal.Z;
                    case HalfAxis.HAxis.AXIS_Z_P: return DimBoxTotal.Z;
                    default: throw new Exception("Invalid ortho axis");
                }
            }
        }
        public HalfAxis.HAxis LengthAxis
        {
            get
            {
                switch (AxisOrtho)
                {
                    case HalfAxis.HAxis.AXIS_X_N: return HalfAxis.HAxis.AXIS_Z_P;
                    case HalfAxis.HAxis.AXIS_X_P: return HalfAxis.HAxis.AXIS_Y_P;
                    case HalfAxis.HAxis.AXIS_Y_N: return HalfAxis.HAxis.AXIS_X_P;
                    case HalfAxis.HAxis.AXIS_Y_P: return HalfAxis.HAxis.AXIS_Z_P;
                    case HalfAxis.HAxis.AXIS_Z_N: return HalfAxis.HAxis.AXIS_Y_P;
                    case HalfAxis.HAxis.AXIS_Z_P: return HalfAxis.HAxis.AXIS_X_P;
                    default: throw new Exception("Invalid ortho axis");
                }
            }
        }
        public HalfAxis.HAxis WidthAxis
        {
            get
            {
                switch (AxisOrtho)
                {
                    case HalfAxis.HAxis.AXIS_X_N: return HalfAxis.HAxis.AXIS_Y_P;
                    case HalfAxis.HAxis.AXIS_X_P: return HalfAxis.HAxis.AXIS_Z_P;
                    case HalfAxis.HAxis.AXIS_Y_N: return HalfAxis.HAxis.AXIS_Z_P;
                    case HalfAxis.HAxis.AXIS_Y_P: return HalfAxis.HAxis.AXIS_X_P;
                    case HalfAxis.HAxis.AXIS_Z_N: return HalfAxis.HAxis.AXIS_X_P;
                    case HalfAxis.HAxis.AXIS_Z_P: return HalfAxis.HAxis.AXIS_Y_P;
                    default: throw new Exception("Invalid ortho axis");
                }
            }
        }
        public Vector3D DimBox { get; private set; }
        public Vector3D Bulge { get; set; }
        public Vector3D DimBoxTotal => DimBox + Bulge;
        public Vector2D DimContainer { get; private set; }
        protected void Add(BoxPositionIndexed position) => _listBoxPosition.Add(position);
        protected void AddRange(List<BoxPositionIndexed> positions) => _listBoxPosition.AddRange(positions);
        public List<BoxPositionIndexed> Positions => _listBoxPosition;

        #region Data members
        protected List<BoxPositionIndexed> _listBoxPosition = new List<BoxPositionIndexed>();
        #endregion
    }
}
