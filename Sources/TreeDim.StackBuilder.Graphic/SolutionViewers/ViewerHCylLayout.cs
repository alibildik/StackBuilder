#region Using directives
using System;
using System.Drawing;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public class ViewerHCylLayout : IDisposable
    {
        public ViewerHCylLayout(HCylLayout cylLayout)
        {
            Layout = cylLayout;
        }
        public void Draw(Graphics3D graphics, CylinderProperties cylProperties, double height, bool selected, bool annotate)
        {
            graphics.BackgroundColor = selected ? Color.LightBlue : Color.White;
            graphics.CameraPosition = Graphics3D.Corner_0;

            uint pickId = 0;
            foreach (var cp in Layout.Positions)
            {   graphics.AddCylinder(new Cylinder(pickId++, cylProperties, cp)) ;  }
            graphics.Flush();

            if (annotate)
                ThumbnailMarker.Annotate(graphics.Graphics, graphics.Size, $"{Layout.Positions.Count}");
        }

        #region Implement IDisposable
        public void Dispose()
        {
        }
        #endregion

        #region Data members
        private HCylLayout Layout { get; set; }
        #endregion
    }
}
