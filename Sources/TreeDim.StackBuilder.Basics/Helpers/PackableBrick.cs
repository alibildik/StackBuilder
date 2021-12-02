using System;
using System.Text;
using Sharp3D.Math.Core;

namespace treeDiM.StackBuilder.Basics
{
    public abstract class PackableBrick : Packable
    {
        protected PackableBrick(Document doc)
            : base(doc)
        {
        }

        public override double Volume { get { return Length * Width * Height; } }
        public abstract double Length { get; }
        public abstract double Width { get; }
        public abstract double Height { get; }
        public override Vector3D OuterDimensions => new Vector3D(Length, Width, Height);
        public virtual bool OrientationAllowed(HalfAxis.HAxis axis) { return true; }
        public override bool IsBrick => true;
        public Vector3D Bulge
        {
            get => _bulge;
            set
            {
                if (value.X < 0 || value.Y < 0 || value.Z < 0)
                    throw new Exception($"Invalid bulge value {value}");
                _bulge = value;
            }
        }
        public bool HasBulge => _bulge.GetLengthSquared() > EPSILON;

        public double Dim(int index)
        {
            switch (index)
            {
                case 0: return Length;
                case 1: return Width;
                case 2: return Height;
                default: throw new Exception("Invalid index...");
            }
        }

        public double Dimension(HalfAxis.HAxis axis)
        {
            switch (axis)
            {
                case HalfAxis.HAxis.AXIS_X_N:
                case HalfAxis.HAxis.AXIS_X_P:
                    return Length;
                case HalfAxis.HAxis.AXIS_Y_N:
                case HalfAxis.HAxis.AXIS_Y_P:
                    return Width;
                case HalfAxis.HAxis.AXIS_Z_N:
                case HalfAxis.HAxis.AXIS_Z_P:
                    return Height;
                default:
                    return 0.0;
            }
        }

        public double[] Dimensions => new double[] { Length, Width, Height };
        // Facing properties
        //
        //  ------- 2 -------
        //  |               |
        //  3               1
        //  |               |
        //  ------- 0 -------
        //
        //  -1 => no facing shown
        //   0 => front
        //   1 => right
        //   2 => back
        //   3 => left
        public int Facing { get; set; } = -1;
        public int FacingAngle
        {
            get
            {
                switch (Facing)
                {
                    case 0: return 0;
                    case 1: return 90;
                    case 2: return 180;
                    case 3: return 270;
                    default: return 0;
                }
            }
        }
        public override string ToString()
        {
            StringBuilder sBuilder = new StringBuilder();
            sBuilder.Append(base.ToString());
            sBuilder.AppendFormat("Packable => Length = {0}      Width = {1}     Height = {2}"
                , Length, Width, Height
                );
            return sBuilder.ToString();
        }

        #region Non-Public Members
        protected Vector3D _bulge = Vector3D.Zero;
        protected static readonly double EPSILON = 1.0E-03;
        #endregion

    }
}