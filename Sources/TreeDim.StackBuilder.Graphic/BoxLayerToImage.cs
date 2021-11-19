#region Using directives
using System.Collections.Generic;
using System.Drawing;

using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public static class BoxLayerToImage
    {
        #region Enums
        public enum EGraphMode { GRAPH_2D, GRAPH_3D };
        #endregion
        #region Drawing
        public static Bitmap Draw(IEnumerable<Box> boxes, Vector2D dimensions, Size size, EGraphMode eMode, string text)
        {
            if (EGraphMode.GRAPH_2D == eMode)
            {
                var graphics = new Graphics2DImage(size);
                using (var viewer = new ViewerLayeredBoxes(dimensions))
                    viewer.Draw(graphics, boxes, false, text);
                return graphics.Bitmap;
            }
            else
            {
                var graphics = new Graphics3DImage(size) { MarginPercentage = 0.05 };
                using (var viewer = new ViewerLayeredBoxes(dimensions))
                    viewer.Draw(graphics, boxes, false, text);
                return graphics.Bitmap;
            }
        }
        #endregion
    }
}
