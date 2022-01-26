#region Using directives
using System.Windows.Forms;
using System.Drawing;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics.Controls
{
    public partial class CCtrlListBoxConveyor : ListBox
    {
        #region Constructor
        public CCtrlListBoxConveyor()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
        }
        #endregion
        #region Public properties
        public PackableBrick Packable { get; set; }
        #endregion
        #region Override ListBox
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            bool selected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
            e.DrawBackground();
            e.DrawFocusRectangle();
            if (!DesignMode && Items.Count > 0 && e.Index != -1)
            {
                if (Items[e.Index] is ConveyorSetting item)
                {
                    var imageGenerator = new ImageGenConveyorSetting()
                    {
                        Size = new Size(e.Bounds.Width, e.Bounds.Height),
                        ShowBelt = false,
                        ShowFrameRef = false, 
                        BackgroundColor = selected ? Color.LightBlue : Color.White,
                    };
                    e.Graphics.DrawImage(
                        imageGenerator.GenerateImage(Packable, item.Number, item.Angle),
                        e.Bounds.Left,
                        e.Bounds.Top
                        );
                }
            }
            base.OnDrawItem(e);
        }
        #endregion
    }
}
