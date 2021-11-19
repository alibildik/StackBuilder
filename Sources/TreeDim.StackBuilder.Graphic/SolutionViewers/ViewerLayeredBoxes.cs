#region Using directives
using System;
using System.Collections.Generic;
using System.Drawing;

using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    internal class ViewerLayeredBoxes : IDisposable
    {
        public ViewerLayeredBoxes(Vector2D dim)
        {
            Dimensions = dim;
        }
        public void Draw(Graphics2D graphics, IEnumerable<Box> boxes, bool selected, string text)
        {
            graphics.NumberOfViews = 1;
            graphics.Clear(selected ? Color.LightBlue : Color.White);
            graphics.SetViewport(0.0f, 0.0f, (float)Dimensions.X, (float)Dimensions.Y);
            graphics.SetCurrentView(0);
            graphics.DrawRectangle(Vector2D.Zero, Dimensions, Color.Black);

            foreach (var b in boxes)
                b.Draw(graphics);

            // annotate thumbnail
            ThumbnailMarker.Annotate(graphics.Graphics, graphics.Size, text);
        }
        public void Draw(Graphics3D graphics, IEnumerable<Box> boxes, bool selected, string text)
        {
            graphics.BackgroundColor = selected ? Color.LightBlue : Color.White;
            graphics.CameraPosition = Graphics3D.Corner_0;
            // draw boxes
            foreach (var box in boxes)
                graphics.AddBox(box);
            graphics.Flush();
            ThumbnailMarker.Annotate(graphics.Graphics, graphics.Size, text);
        }

        public void Dispose() {}

        public Vector2D Dimensions { get; set; }
    }
}
