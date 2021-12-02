#region Using directives
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public class Graphics3DImage : Graphics3D
    {
        #region Constructor
        public Graphics3DImage(Size size)
        {
            Bitmap = new Bitmap(size.Width, size.Height);        
        }
        public Graphics3DImage(Size size, double angle, double cameraDistance)
        {
            Bitmap = new Bitmap(size.Width, size.Height);
            CameraPosition = new Vector3D(
                    Math.Cos(angle * Math.PI / 180.0) * Math.Sqrt(2.0) * cameraDistance
                    , Math.Sin(angle * Math.PI / 180.0) * Math.Sqrt(2.0) * cameraDistance
                    , cameraDistance);
            Target = Vector3D.Zero;
            SetViewport(-500.0f, -500.0f, 500.0f, 500.0f);
        }
        #endregion

        #region Graphics3D abstract method implementation
        public override Size Size => Bitmap.Size;
        public override System.Drawing.Graphics Graphics => System.Drawing.Graphics.FromImage(Bitmap);
        #endregion

        #region Public methods
        public void SaveAs(string filename)
        {
            ImageFormat format = ImageFormat.Bmp;
            Bitmap.Save(filename, format);
        }
        #endregion

        #region Public properties
        public Bitmap Bitmap { get; private set; }
        #endregion
    }

    public class Graphics3DForm : Graphics3D
    {
        #region Constructor
        public Graphics3DForm(Control ctrl, System.Drawing.Graphics g)
        {
            Ctrl = ctrl;
            _g = g;
        }
        #endregion

        #region Graphics3D abstract method implementation
        public override Size Size => Ctrl.Size;
        public override System.Drawing.Graphics Graphics => _g;
        #endregion

        #region Data members
        private System.Drawing.Graphics _g;
        private Control Ctrl { get; set; }
        #endregion
    }
}
