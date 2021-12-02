#region Using directives
using System;
using System.Drawing;
using System.Windows.Forms;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics.Controls
{
    #region UCtrlConveyorSettings
    public partial class UCtrlConveyorSettings : UserControl
    {
        #region Constructor
        public UCtrlConveyorSettings()
        {
            InitializeComponent();
        }
        #endregion
        #region Event handlers
        private void OnPaint(object sender, PaintEventArgs e)
        {
            if (null == BoxProperties) return;
            ConveryorToPicture.Draw(BoxProperties, AngleCase, MaxDropNumber, pbConveyor);
        }
        private void OnSettingsChanged(object sender, EventArgs e)
        {
            pbConveyor.Invalidate();
        }
        #endregion
        #region Public properties
        public PackableBrick BoxProperties
        {
            get => _packable;
            set
            {
                _packable = value;
                if (null == _packable)
                    return;
                pbConveyor.Invalidate();
            }
        }
        public int AngleCase
        {
            get => (int)nudCaseAngle.Value;
            set => nudCaseAngle.Value = value;
        }
        public int MaxDropNumber
        {
            get => (int)nudMaxNumber.Value;
            set => nudMaxNumber.Value = value;
        }
        public int AngleGrabber
        {
            get => (int)nudGrabberAngle.Value;
            set => nudGrabberAngle.Value = value;
        }
        #endregion
        #region Data members
        private PackableBrick _packable;
        #endregion
    }
    #endregion
    #region ConveryorToPicture
    internal class ConveryorToPicture
    {
        public static void Draw(PackableBrick packable, int caseAngle, int maxDropNumber, PictureBox pb)
        {
            var graphics = new Graphics3DImage(pb.Size, 275.0, 10000.0)
            {
                ShowFacing = true,
                FontSizeRatio = 0.1f
            };

            // draw conveyor belt
            double offsetBelt = 100.0;
            double beltLength = 2 * offsetBelt + (maxDropNumber + 1) * packable.Length;
            double beltWidth = 2 * offsetBelt + packable.Width;

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

            HalfAxis.HAxis axis0 = HalfAxis.HAxis.AXIS_X_P, axis1 = HalfAxis.HAxis.AXIS_Y_P;
            double xStep = 0.0;
            double xOffset = 0.0, yOffset = 0.0;
            switch (caseAngle)
            {
                case 0:
                    axis0 = HalfAxis.HAxis.AXIS_X_P; axis1 = HalfAxis.HAxis.AXIS_Y_P;
                    xStep = packable.Length;
                    xOffset = 0.0; yOffset = -0.5 * packable.Width;
                    break;
                case 90:
                    axis0 = HalfAxis.HAxis.AXIS_Y_P; axis1 = HalfAxis.HAxis.AXIS_X_N;
                    xStep = packable.Width;
                    xOffset = packable.Width; yOffset = -0.5 * packable.Length;
                    break;
                case 180:
                    axis0 = HalfAxis.HAxis.AXIS_X_N; axis1 = HalfAxis.HAxis.AXIS_Y_N;
                    xStep = packable.Length;
                    xOffset = packable.Length; yOffset = 0.5 * packable.Width;
                    break;
                case 270:
                    axis0 = HalfAxis.HAxis.AXIS_Y_N; axis1 = HalfAxis.HAxis.AXIS_X_P;
                    xStep = packable.Width;
                    xOffset = 0.0; yOffset = 0.5 * packable.Length;
                    break;
                default: break;
            }

            double x = xOffset;
            for (int i = 0; i < maxDropNumber; ++i)
            {
                graphics.AddBox(
                    new Box(0, packable, new BoxPosition(new Vector3D(x, yOffset, 0.0), axis0, axis1)));
                x += xStep;
            }

            // draw arrow
            var arrow = new Arrow(1)
            {
                Length = packable.Length,
                Width = 0.2 * packable.Length,
                ColorFill = Color.OrangeRed,
                ColorPath = Color.OrangeRed,
                Position = new Vector3D(beltLength - packable.Length - 1.25 * offsetBelt, 0.0, 0.0)
            };
            arrow.DrawEnd(graphics);

            var frameRef = new FrameRef(2)
            {
                Length = packable.Length,
                Position = new Vector3D(0.0, -packable.Length, 0.0),
                LengthAxis = HalfAxis.HAxis.AXIS_X_P,
                WidthAxis = HalfAxis.HAxis.AXIS_Y_P
            };
            graphics.AddFrameRef(frameRef);

            graphics.Flush();
            pb.Image = graphics.Bitmap;
        }
    }
    #endregion
}
