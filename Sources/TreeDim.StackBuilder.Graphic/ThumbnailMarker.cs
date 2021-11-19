#region Using directives
using System.Drawing;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    internal class ThumbnailMarker
    {
        public static void Annotate(System.Drawing.Graphics g, Size s, string text)
        {
            // do not annotate if text is empty
            if (string.IsNullOrEmpty(text)) return;
            // font
            Font tfont = new Font(FontName, FontSize);
            // string format
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Far };
            // measure string
            Size txtSize = g.MeasureString(text, tfont).ToSize();
            // draw back
            g.FillRectangle(new SolidBrush(BackColor), new Rectangle(s.Width - txtSize.Width - 2, s.Height - txtSize.Height - 2, txtSize.Width + 2, txtSize.Height + 2));
            // draw text string
            g.DrawString(text, tfont, new SolidBrush(FrontColor), new Point(s.Width - 3, s.Height - 3), sf);
        }
        public static int FontSize { get; set; } = 9;
        public static string FontName { get; set; } = "Arial";
        public static Color BackColor { get; set; } = Color.Black;
        public static Color FrontColor { get; set; } = Color.White;
    }        
}
