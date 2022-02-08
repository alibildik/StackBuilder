#region Using directives
using System;
using System.Drawing;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    internal class ImageGen
    { 
        public Size Size { get; set; }
        public double AngleHoriz { get; set; } = 95.0;
        public double AngleVert { get; set; } = 45.0;
        public double CameraDistance { get; set; } = 100000;
        public Vector3D CameraPosition
        {
            get
            {
                double angleHorizRad = AngleHoriz * Math.PI / 180.0;
                double angleVertRad = AngleVert * Math.PI / 180.0;
                return new Vector3D(
                        CameraDistance * Math.Cos(angleHorizRad) * Math.Cos(angleVertRad)
                        , CameraDistance * Math.Sin(angleHorizRad) * Math.Cos(angleVertRad)
                        , CameraDistance * Math.Sin(angleVertRad));
            }
        }
        public Color BackgroundColor { get; set; } = Color.White;
    }
    internal class ImageGenConveyorSetting : ImageGen
    {
        public Image GenerateImage(PackableBrick packable, int number, int angle, int gripperAngle)
        {
            var graphics = new Graphics3DImage(Size)
            {
                CameraPosition = CameraPosition,
                ShowFacing = true,
                Target = Vector3D.Zero,
                BackgroundColor = BackgroundColor
            };
            Draw(graphics, packable, number, angle, gripperAngle);
            return graphics.Bitmap;
        }
        public void Draw(Graphics3D graphics, PackableBrick packable, int number, int angle, int gripperAngle)
        {
            // draw conveyor belt
            double offsetBelt = 100.0;
            double beltLength = 2 * offsetBelt + (number + 1) * packable.Length;
            double beltWidth = 2 * offsetBelt + packable.Width;

            // belt + arrow
            if (ShowBelt)
            {

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

                var arrow = new Arrow(1)
                {
                    Length = packable.Length,
                    Width = 0.2 * packable.Length,
                    ColorFill = Color.OrangeRed,
                    ColorPath = Color.OrangeRed,
                    Position = new Vector3D(beltLength - 1.25 * offsetBelt, 0.0, 0.0),
                    LengthAxis = HalfAxis.HAxis.AXIS_X_N,
                    WidthAxis = HalfAxis.HAxis.AXIS_Y_N
                };
                arrow.Draw(graphics);
            }
            // frame referential
            if (ShowFrameRef)
            {
                var frameRef = new FrameRef(2)
                {
                    Length = packable.Length,
                    Position = new Vector3D(-offsetBelt, -0.5 * beltWidth, 0.0),
                    LengthAxis = HalfAxis.HAxis.AXIS_X_P,
                    WidthAxis = HalfAxis.HAxis.AXIS_Y_P
                };
                graphics.AddFrameRef(frameRef);
            }

            HalfAxis.HAxis axis0 = HalfAxis.HAxis.AXIS_X_P, axis1 = HalfAxis.HAxis.AXIS_Y_P;
            double xStep = 0.0;
            double xOffset = 0.0, yOffset = 0.0;
            ConvertAngle(angle, packable, ref axis0, ref axis1, ref xStep, ref xOffset, ref yOffset);

            for (int i = 0; i < number; i++)
                graphics.AddBox(
                    new Box(0, packable, new BoxPosition(new Vector3D(xOffset + (number - i - 1) * xStep, yOffset, 0.0), axis0, axis1))
                    );
            graphics.Flush();

            string csGripperAngle = Properties.Resources.ID_GRIPPERANGLE;
            ThumbnailMarker.Annotate(graphics.Graphics, graphics.Size, $"{csGripperAngle}={gripperAngle}");
        }
        public void ConvertAngle(double angle, PackableBrick packable,
            ref HalfAxis.HAxis axis0, ref HalfAxis.HAxis axis1, ref double xStep, ref double xOffset, ref double yOffset)
        {
            switch (angle)
            {
                case 180:
                    axis0 = HalfAxis.HAxis.AXIS_X_P; axis1 = HalfAxis.HAxis.AXIS_Y_P;
                    xStep = packable.Length;
                    xOffset = 0.0; yOffset = -0.5 * packable.Width;
                    break;
                case 270:
                    axis0 = HalfAxis.HAxis.AXIS_Y_P; axis1 = HalfAxis.HAxis.AXIS_X_N;
                    xStep = packable.Width;
                    xOffset = packable.Width; yOffset = -0.5 * packable.Length;
                    break;
                case 0:
                    axis0 = HalfAxis.HAxis.AXIS_X_N; axis1 = HalfAxis.HAxis.AXIS_Y_N;
                    xStep = packable.Length;
                    xOffset = packable.Length; yOffset = 0.5 * packable.Width;
                    break;
                case 90:
                    axis0 = HalfAxis.HAxis.AXIS_Y_N; axis1 = HalfAxis.HAxis.AXIS_X_P;
                    xStep = packable.Width;
                    xOffset = 0.0; yOffset = 0.5 * packable.Length;
                    break;
                default: break;
            }
        }
        public bool ShowBelt { get; set; } = true;
        public bool ShowFrameRef { get; set; } = true;
    }
}
