#region Using directives
using System;
using System.Drawing;
using System.Windows.Forms;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public partial class Graphics3DConveyor : UserControl
    {
        public Graphics3DConveyor()
        {
            InitializeComponent();
            // double buffering
            SetDoubleBuffered();
            SetStyle(ControlStyles.Selectable, true);
            TabStop = false;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (null == Packable) return;


            double angleHorizRad = AngleHoriz * Math.PI / 180.0;
            double angleVertRad = AngleVert * Math.PI / 180.0;
            double cameraDistance = 100000.0;
            var graphics = new Graphics3DForm(this, e.Graphics)
            {
                ShowFacing = true,
                FontSizeRatio = 0.1f,
                Target = Vector3D.Zero,
                CameraPosition = new Vector3D(
                    cameraDistance * Math.Cos(angleHorizRad) * Math.Cos(angleVertRad)
                    , cameraDistance * Math.Sin(angleHorizRad) * Math.Cos(angleVertRad)
                    , cameraDistance * Math.Sin(angleVertRad))
            };

            // draw conveyor belt
            double offsetBelt = 100.0;
            double beltLength = 2 * offsetBelt + (MaxDropNumber + 1) * Packable.Length;
            double beltWidth = 2 * offsetBelt + Packable.Width;

            // belt
            var beltFace = new Face(
                0,
                new Vector3D[]
                {
                    new Vector3D(-offsetBelt, 0.5 * beltWidth, 0.0),
                    new Vector3D(-offsetBelt + beltLength, 0.5 * beltWidth, 0.0),
                    new Vector3D(-offsetBelt + beltLength, -0.5 * beltWidth, 0.0),
                    new Vector3D(-offsetBelt, -0.5 * beltWidth, 0.0)
                },
                "BELT",
                false
                )
            {
                ColorFill = Color.LightGray
            };
            graphics.AddFace(beltFace);
            // arrow
            var arrow = new Arrow(1)
            {
                Length = Packable.Length,
                Width = 0.2 * Packable.Length,
                ColorFill = Color.OrangeRed,
                ColorPath = Color.OrangeRed,
                Position = new Vector3D(beltLength - 1.25 * offsetBelt, 0.0, 0.0),
                LengthAxis = HalfAxis.HAxis.AXIS_X_N,
                WidthAxis = HalfAxis.HAxis.AXIS_Y_N
            };
            arrow.Draw(graphics);
            // frame referential
            var frameRef = new FrameRef(2)
            {
                Length = Packable.Length,
                Position = new Vector3D(-offsetBelt, -0.5 * beltWidth, 0.0),
                LengthAxis = HalfAxis.HAxis.AXIS_X_P,
                WidthAxis = HalfAxis.HAxis.AXIS_Y_P
            };
            graphics.AddFrameRef(frameRef);

            HalfAxis.HAxis axis0 = HalfAxis.HAxis.AXIS_X_P, axis1 = HalfAxis.HAxis.AXIS_Y_P;
            double xStep = 0.0;
            double xOffset = 0.0, yOffset = 0.0;
            switch (CaseAngle)
            {
                case 0:
                    axis0 = HalfAxis.HAxis.AXIS_X_P; axis1 = HalfAxis.HAxis.AXIS_Y_P;
                    xStep = Packable.Length;
                    xOffset = 0.0; yOffset = -0.5 * Packable.Width;
                    break;
                case 90:
                    axis0 = HalfAxis.HAxis.AXIS_Y_P; axis1 = HalfAxis.HAxis.AXIS_X_N;
                    xStep = Packable.Width;
                    xOffset = Packable.Width; yOffset = -0.5 * Packable.Length;
                    break;
                case 180:
                    axis0 = HalfAxis.HAxis.AXIS_X_N; axis1 = HalfAxis.HAxis.AXIS_Y_N;
                    xStep = Packable.Length;
                    xOffset = Packable.Length; yOffset = 0.5 * Packable.Width;
                    break;
                case 270:
                    axis0 = HalfAxis.HAxis.AXIS_Y_N; axis1 = HalfAxis.HAxis.AXIS_X_P;
                    xStep = Packable.Width;
                    xOffset = 0.0; yOffset = 0.5 * Packable.Length;
                    break;
                default: break;
            }

            for (int i = 0; i < MaxDropNumber; ++i)
            {
                graphics.AddBox(
                    new Box(0, Packable, new BoxPosition(new Vector3D(xOffset + (MaxDropNumber - i - 1) * xStep, yOffset, 0.0), axis0, axis1)));
            }
            graphics.Flush();
        }
        #region Double buffering
        private void SetDoubleBuffered()
        {
            System.Reflection.PropertyInfo aProp =
                typeof(Control).GetProperty(
                    "DoubleBuffered",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

            aProp.SetValue(this, true, null);
        }
        #endregion

        #region Data members
        public double AngleHoriz { get; set; } = 95;
        private double AngleVert { get; set; } = 45.0;
        public int MaxDropNumber { get; set; }
        public int CaseAngle { get; set; }
        public PackableBrick Packable { get; set; }
        #endregion
    }
}
