#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using System.Drawing;
using System.Drawing.Imaging;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public class Graphics2DImage : Graphics2D
    {
        #region Constructor
        public Graphics2DImage(Size size)
        {
            Bitmap = new Bitmap(size.Width, size.Height);
        }
        #endregion
        #region Graphics2D abstract method implementation
        public override Size Size => Bitmap.Size;
        public override System.Drawing.Graphics Graphics => System.Drawing.Graphics.FromImage(Bitmap);
        #endregion
        #region Public methods
        public void SaveAs(string filename) => Bitmap.Save(filename, ImageFormat.Bmp);
        #endregion
        #region Public properties
        public Bitmap Bitmap { get; }
        #endregion
    }

    public class Graphics2DForm : Graphics2D
    {
        #region Constructor
        public Graphics2DForm(Control ctrl, System.Drawing.Graphics g)
        {
            Ctrl = ctrl;
            Graph = g;
            Graph.Clear(Color.White);
        }
        #endregion

        #region Graphics2D abstract methods implementation
        public override Size Size => Ctrl.Size;
        public override System.Drawing.Graphics Graphics => Graph;
        #endregion
        #region Public properties
        public Control Ctrl { get; set; }
        public System.Drawing.Graphics Graph { get; set; }
        #endregion
    }
}
