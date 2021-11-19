#region Using directives
using System;
using System.Drawing;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public class ViewerILayer2D : IDisposable
    {
        #region Data members
        public static int FontSize { get; set; } = 9;
        public ILayer2D Layer { get; set; }
        #endregion

        #region Constructor
        public ViewerILayer2D(ILayer2D layer) { Layer = layer; }
        #endregion

        #region Public methods
        public void Draw(Graphics2D graphics, Packable packable, double height, bool selected, bool annotate)
        {
            graphics.NumberOfViews = 1;
            graphics.Clear(selected ? Color.LightBlue : Color.White);
            graphics.SetViewport(0.0f, 0.0f, (float)Layer.Length, (float)Layer.Width);
            graphics.SetCurrentView(0);
            graphics.DrawRectangle(Vector2D.Zero, new Vector2D(Layer.Length, Layer.Width), Color.Black);

            // draw layer (brick)
            if (Layer is Layer2DBrick layer2D)
            {
                uint pickId = 0;
                foreach (var bPosition in layer2D.Positions)
                {
                    Box b = null;
                    if (packable is PackProperties)
                        b = new Pack(pickId++, packable as PackProperties, bPosition);
                    else if (packable is PackableBrick)
                        b = new Box(pickId++, packable as PackableBrick, bPosition);
                    if (null != b)
                        b.Draw(graphics);
                }
            }
            // draw layer (cylinder)
            else if (Layer is Layer2DCylImp)
            {
                Layer2DCylImp layer2DCyl = Layer as Layer2DCylImp;
                uint pickId = 0;
                foreach (Vector2D pos in layer2DCyl)
                {
                    Cylinder c = new Cylinder(pickId++, packable as CylinderProperties, new CylPosition(new Vector3D(pos.X, pos.Y, 0.0), HalfAxis.HAxis.AXIS_Z_P));
                    c.Draw(graphics);
                }
            }
            // draw axes
            bool showAxis = false;
            if (showAxis)
            {
                // draw axis X
                graphics.DrawLine(Vector2D.Zero, new Vector2D(Layer.Length, 0.0), Color.Red);
                // draw axis Y
                graphics.DrawLine(Vector2D.Zero, new Vector2D(0.0, Layer.Width), Color.Green);
            }
            // annotate thumbnail
            if (annotate)
                ThumbnailMarker.Annotate(graphics.Graphics, graphics.Size, $"{Layer.Count}*{Layer.NoLayers(height)}={Layer.CountInHeight(height)}");
        }
        public void Draw(Graphics3D graphics, Packable packable, double height, bool selected, bool annotate)
        {
            graphics.BackgroundColor = selected ? Color.LightBlue : Color.White;
            graphics.CameraPosition = Graphics3D.Corner_0;

            // draw layer (brick)
            if (Layer is Layer2DBrick layer2D)
            {
                uint pickId = 0;
                foreach (var bPosition in layer2D.Positions)
                {
                    if (packable is PackProperties)
                        graphics.AddBox(new Pack(pickId++, packable as PackProperties, bPosition));
                    else if (packable is PackableBrick)
                        graphics.AddBox(new Box(pickId++, packable as PackableBrick, bPosition));
                }
            }
            // draw layer (cylinder)
            else if (Layer is Layer2DCylImp)
            {
                Layer2DCylImp layer2DCyl = Layer as Layer2DCylImp;
                uint pickId = 0;
                foreach (Vector2D pos in layer2DCyl)
                {
                    Cyl cyl = null;
                    if (packable is CylinderProperties cylProperties)
                        cyl = new Cylinder(pickId++, cylProperties, new CylPosition(new Vector3D(pos.X, pos.Y, 0.0), HalfAxis.HAxis.AXIS_Z_P));
                    else if (packable is BottleProperties bottleProperties)
                        cyl = new Bottle(pickId++, bottleProperties, new CylPosition(new Vector3D(pos.X, pos.Y, 0.0), HalfAxis.HAxis.AXIS_Z_P));
                    graphics.AddCylinder(cyl);
                }
            }
            graphics.Flush();
            // annotate thumbnail
            if (annotate)
                ThumbnailMarker.Annotate(graphics.Graphics, graphics.Size, $"{Layer.Count}*{Layer.NoLayers(height)}={Layer.CountInHeight(height)}");
        }
        #endregion

        #region IDisposable implementation
        public void Dispose() {}
        #endregion
    }
}
