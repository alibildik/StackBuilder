using SourceGrid;
using System.Drawing;
using System;

namespace treeDiM.StackBuilder.WCFService.Test
{
    internal class CellBackColorAlternate : SourceGrid.Cells.Views.Cell
    {
        public CellBackColorAlternate(Color firstColor, Color secondColor)
        {
            FirstBackground = new DevAge.Drawing.VisualElements.BackgroundSolid(firstColor);
            SecondBackground = new DevAge.Drawing.VisualElements.BackgroundSolid(secondColor);
        }
        public DevAge.Drawing.VisualElements.IVisualElement FirstBackground { get; set; }
        public DevAge.Drawing.VisualElements.IVisualElement SecondBackground { get; set; }

        protected override void PrepareView(CellContext context)
        {
            base.PrepareView(context);

            if (Math.IEEERemainder(context.Position.Row, 2) == 0)
                Background = FirstBackground;
            else
                Background = SecondBackground;
        }

        public static CellBackColorAlternate ViewAliceBlueWhite
        {
            get
            {
                // CellBackColorAlternate view
                DevAge.Drawing.BorderLine border = new DevAge.Drawing.BorderLine(Color.LightBlue, 1);
                DevAge.Drawing.RectangleBorder cellBorder = new DevAge.Drawing.RectangleBorder(border, border);
                return new CellBackColorAlternate(Color.AliceBlue, Color.White) { Border = cellBorder };
            }
        }
    }
}